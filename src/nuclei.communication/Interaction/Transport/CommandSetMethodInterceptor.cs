//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Defines the base class for <see cref="IInterceptor"/> instances that intercept specific types of
    /// methods on an <see cref="ICommandSet"/> interface.
    /// </summary>
    internal abstract class CommandSetMethodInterceptor : IInterceptor
    {
        private static string MethodToText(MethodInfo method)
        {
            return method.ToString();
        }

        /// <summary>
        /// The function which sends the <see cref="CommandInvokedMessage"/> to the owning endpoint.
        /// </summary>
        private readonly SendCommandData m_TransmitCommandInvocation;

        /// <summary>
        /// The object that stores the configuration for the application.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// The object that provides the diagnostics for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSetMethodInterceptor"/> class.
        /// </summary>
        /// <param name="transmitCommandInvocation">
        ///     The function used to send the information about the method invocation to the owning endpoint.
        /// </param>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <param name="systemDiagnostics">The function that is used to log messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="transmitCommandInvocation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        protected CommandSetMethodInterceptor(
            SendCommandData transmitCommandInvocation,
            IConfiguration configuration,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => transmitCommandInvocation);
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_TransmitCommandInvocation = transmitCommandInvocation;
            m_Configuration = configuration;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Called when a method or property call is intercepted.
        /// </summary>
        /// <param name="invocation">Information about the call that was intercepted.</param>
        public void Intercept(IInvocation invocation)
        {
            m_Diagnostics.Log(
                LevelToLog.Trace,
                CommunicationConstants.DefaultLogTextPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Invoking {0}",
                    MethodToText(invocation.Method)));

            Task<ICommunicationMessage> result;
            try
            {
                var tuple = ToParameterData(invocation.Method, invocation.Arguments);
                result = m_TransmitCommandInvocation(
                    new CommandInvokedData(
                        CommandId.Create(invocation.Method),
                        tuple.Item3),
                    tuple.Item1,
                    tuple.Item2);
            }
            catch (EndpointNotContactableException e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Tried to invoke {0}, but failed to contact the remote endpoint.",
                        MethodToText(invocation.Method)));

                throw new CommandInvocationFailedException(Resources.Exceptions_Messages_CommandInvocationFailed, e);
            }
            catch (FailedToSendMessageException e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Tried to invoke {0}, but failed to send the message.",
                        MethodToText(invocation.Method)));

                throw new CommandInvocationFailedException(Resources.Exceptions_Messages_CommandInvocationFailed, e);
            }

            invocation.ReturnValue = ExtractMethodReturnFrom(invocation, result);
        }

        /// <summary>
        /// Translates a <see cref="MethodInfo"/> and the related parameter values into a serializable form.
        /// </summary>
        /// <param name="method">The method information that needs to be serialized.</param>
        /// <param name="parameters">
        ///     The collection of parameter values with which the method should be called. Note that the parameter
        ///     values should be in the same order as they are given by the <c>MethodInfo.GetParameters()</c> method.
        /// </param>
        /// <returns>
        ///     An object that stores the method invocation information in a serializable format.
        /// </returns>
        private Tuple<int, TimeSpan, CommandParameterValueMap[]> ToParameterData(MethodBase method, object[] parameters)
        {
            var retryCount = CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending;
            var timeout = m_Configuration.HasValueFor(CommunicationConfigurationKeys.WaitForResponseTimeoutInMilliSeconds)
                ? TimeSpan.FromMilliseconds(
                    m_Configuration.Value<int>(CommunicationConfigurationKeys.WaitForResponseTimeoutInMilliSeconds))
                : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultWaitForResponseTimeoutInMilliSeconds);

            var methodParameters = method.GetParameters();
            Debug.Assert(methodParameters.Length == parameters.Length, "There are a different number of parameters than there are parameter values.");

            var namedParameters = new List<CommandParameterValueMap>();
            for (int i = 0; i < methodParameters.Length; i++)
            {
                var methodParameter = methodParameters[i];

                var attributes = methodParameter.GetCustomAttributes(true);
                if (attributes.Length > 0)
                {
                    var parameterUsageAttribute = attributes.FirstOrDefault(
                        o => InteractionExtensions.KnownCommandSetParameterAttributes.Contains(o.GetType()))
                        as CommandProxyParameterUsageAttribute;
                    if ((parameterUsageAttribute == null) || (parameterUsageAttribute.AllowedParameterType != methodParameter.ParameterType))
                    {
                        throw new NonMappedCommandParameterException();
                    }

                    if (parameterUsageAttribute is InvocationRetryCountAttribute)
                    {
                        retryCount = (int)parameters[i];
                        continue;
                    }

                    if (parameterUsageAttribute is InvocationTimeoutAttribute)
                    {
                        timeout = TimeSpan.FromMilliseconds((int)parameters[i]);
                        continue;
                    }

                    throw new NonMappedCommandParameterException();
                }

                namedParameters.Add(
                    new CommandParameterValueMap(
                        new CommandParameterDefinition(
                            methodParameter.ParameterType,
                            methodParameter.Name,
                            CommandParameterOrigin.FromCommand),
                        parameters[i]));
            }

            return new Tuple<int, TimeSpan, CommandParameterValueMap[]>(retryCount, timeout, namedParameters.ToArray());
        }

        /// <summary>
        /// Extracts the return value of the method from the response message.
        /// </summary>
        /// <param name="invocation">The information about the method invocation.</param>
        /// <param name="result">The task that will eventually return the response message.</param>
        /// <returns>The <see cref="Task"/> that will complete when the response message is received.</returns>
        protected abstract Task ExtractMethodReturnFrom(IInvocation invocation, Task<ICommunicationMessage> result);
    }
}