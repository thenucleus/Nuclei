//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Builds proxy objects for the <see cref="RemoteCommandHub"/>.
    /// </summary>
    internal sealed class CommandProxyBuilder
    {
        /// <summary>
        /// The generator that will create the proxy objects.
        /// </summary>
        private readonly ProxyGenerator m_Generator = new ProxyGenerator();

        /// <summary>
        /// The ID of the local endpoint.
        /// </summary>
        private readonly EndpointId m_Local;

        /// <summary>
        /// The function which sends the message to the owning endpoint and returns a task that will,
        /// eventually, hold the return message.
        /// </summary>
        private readonly Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> m_SendWithResponse;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandProxyBuilder"/> class.
        /// </summary>
        /// <param name="localEndpoint">The ID number of the local endpoint.</param>
        /// <param name="sendWithResponse">
        ///     The function that sends out a message to the given endpoint and returns a task that will, eventually, hold the return message.
        /// </param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendWithResponse"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public CommandProxyBuilder(
            EndpointId localEndpoint,
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sendWithResponse,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => localEndpoint);
                Lokad.Enforce.Argument(() => sendWithResponse);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Local = localEndpoint;
            m_SendWithResponse = sendWithResponse;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The interface of the command set for which a proxy must be made.</typeparam>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public T ProxyConnectingTo<T>(EndpointId endpoint) where T : ICommandSet
        {
            object result = ProxyConnectingTo(endpoint, typeof(T));
            return (T)result;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <param name="interfaceType">The interface of the command set for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public object ProxyConnectingTo(EndpointId endpoint, Type interfaceType)
        {
            {
                Lokad.Enforce.Argument(() => interfaceType);
                Lokad.Enforce.With<ArgumentException>(
                    typeof(ICommandSet).IsAssignableFrom(interfaceType), 
                    Resources.Exceptions_Messages_ACommandSetTypeMustDeriveFromICommandSet);

                Lokad.Enforce.Argument(() => endpoint);
            }

            // We assume that the interface lives up to the demands we placed on it, i.e.:
            // - Derives from ICommandSet
            // - Has only methods, no properties and no events, other than those defined by
            //   ICommandSet
            // - Every method either returns nothing (void) or returns a Task<T> object.
            // All these checks should have been done when the interface was registered
            // at the remote endpoint.
            var selfReference = new ProxySelfReferenceInterceptor();
            var methodWithoutResult = new CommandSetMethodWithoutResultInterceptor(
                methodInvocation =>
                {
                    var msg = new CommandInvokedMessage(m_Local, methodInvocation);
                    return m_SendWithResponse(endpoint, msg);
                },
                m_Diagnostics);
            var methodWithResult = new CommandSetMethodWithResultInterceptor(
                methodInvocation =>
                {
                    var msg = new CommandInvokedMessage(m_Local, methodInvocation);
                    return m_SendWithResponse(endpoint, msg);
                },
                m_Diagnostics);

            var options = new ProxyGenerationOptions
                {
                    Selector = new CommandSetInterceptorSelector(),
                    BaseTypeForInterfaceProxy = typeof(CommandSetProxy),
                };

            try
            {
                var proxy = m_Generator.CreateInterfaceProxyWithoutTarget(
                        interfaceType,
                        options,
                        new IInterceptor[] { selfReference, methodWithoutResult, methodWithResult });

                return proxy;
            }
            catch (GeneratorException e)
            {
                throw new UnableToGenerateProxyException(
                    Resources.Exceptions_Messages_UnableToGenerateProxy, 
                    e);
            }
        }
    }
}
