//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Interaction.Transport.V1.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="CommandInvokedMessage"/>.
    /// </summary>
    internal sealed class CommandInvokedProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// An internal class that holds the method used to process <see cref="Task{T}"/> objects at runtime.
        /// </summary>
        /// <remarks>
        /// The method is wrapped in a class so that we can assign it to a variable typed as <c>dynamic</c>.
        /// By using that kind of typing we don't need to specify the exact method signature, which we don't
        /// know at compile time due to the missing information about the generic type.
        /// </remarks>
        private sealed class TaskReturn
        {
            /// <summary>
            /// The object that provides the diagnostics methods for the system.
            /// </summary>
            private readonly SystemDiagnostics m_Diagnostics;

            /// <summary>
            /// Initializes a new instance of the <see cref="TaskReturn"/> class.
            /// </summary>
            /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
            public TaskReturn(SystemDiagnostics systemDiagnostics)
            {
                {
                    Debug.Assert(systemDiagnostics != null, "The diagnostics object should not be null.");
                }

                m_Diagnostics = systemDiagnostics;
            }

            /// <summary>
            /// Provides a typed way of creating a return message based on the outcome of the invocation of a given command. This method 
            /// will only be invoked through reflection.
            /// </summary>
            /// <param name="local">The endpoint ID of the local endpoint.</param>
            /// <param name="originalMsg">The message that was send to invoke a given command.</param>
            /// <param name="returnValue">The task that will, eventually, return the desired result.</param>
            /// <returns>The communication message that should be send if the task finishes successfully.</returns>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
                Justification = "Will not make this method static so that the signature is consistent with HandleTypedTaskReturnValue<T>.")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "This code is called via reflection.")]
            public ICommunicationMessage HandleTaskReturnValue(EndpointId local, ICommunicationMessage originalMsg, Task returnValue)
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    "Processing Task return value from command.");

                if (returnValue.IsCanceled || returnValue.IsFaulted)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The task has failed. Exception is: {0}",
                            returnValue.Exception));

                    return new FailureMessage(local, originalMsg.Id);
                }

                return new SuccessMessage(local, originalMsg.Id);
            }

            /// <summary>
            /// Provides a typed way of creating a return message based on the outcome of the invocation of a given command. This method 
            /// will only be invoked through reflection.
            /// </summary>
            /// <typeparam name="T">The type of the return value.</typeparam>
            /// <param name="local">The endpoint ID of the local endpoint.</param>
            /// <param name="originalMsg">The message that was send to invoke a given command.</param>
            /// <param name="returnValue">The task that will, eventually, return the desired result.</param>
            /// <returns>The communication message that should be send if the task finishes successfully.</returns>
            [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic",
                Justification = "Cannot make this method static because then we cannot use 'dynamic' anymore to get to it.")]
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
                Justification = "This code is called via reflection.")]
            public ICommunicationMessage HandleTaskReturnValue<T>(EndpointId local, ICommunicationMessage originalMsg, Task<T> returnValue)
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Processing Task<T> return value from command. T is {0}",
                        typeof(T)));

                if (returnValue.IsCanceled || returnValue.IsFaulted)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Error,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "The task has failed. Exception is: {0}",
                            returnValue.Exception));

                    return new FailureMessage(local, originalMsg.Id);
                }

                return new CommandInvokedResponseMessage(local, originalMsg.Id, returnValue.Result);
            }
        }

        /// <summary>
        /// The collection that holds all the registered commands.
        /// </summary>
        private readonly ICommandCollection m_Commands;

        /// <summary>
        /// The action that is used to send a message to a remote endpoint.
        /// </summary>
        private readonly Action<EndpointId, ICommunicationMessage> m_SendMessage;

        /// <summary>
        /// The endpoint ID of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Current;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedProcessAction"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="sendMessage">The action that is used to send messages.</param>
        /// <param name="availableCommands">The collection that holds all the registered commands.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="availableCommands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedProcessAction(
            EndpointId localEndpoint,
            Action<EndpointId, ICommunicationMessage> sendMessage,
            ICommandCollection availableCommands,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => localEndpoint);
                Lokad.Enforce.Argument(() => sendMessage);
                Lokad.Enforce.Argument(() => availableCommands);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Current = localEndpoint;
            m_SendMessage = sendMessage;
            m_Commands = availableCommands;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(CommandInvokedMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Letting the exception escape will just kill the channel then we won't know what happened, so we log and move on.")]
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as CommandInvokedMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            var invocation = msg.Invocation;
            m_Diagnostics.Log(
                LevelToLog.Trace,
                CommunicationConstants.DefaultLogTextPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Received request to execute command: {0}.{1}",
                    invocation.Type.FullName,
                    invocation.MemberName));

            Task result = null;
            try
            {
                var type = ProxyExtensions.ToType(invocation.Type);
                var commandSet = m_Commands.CommandsFor(type);
                if (commandSet == null)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Command invokation was requested for {0}.{1} from {2} but this command was not registered.",
                            invocation.Type.FullName,
                            invocation.MemberName,
                            msg.Sender));

                    var failureResult = new FailureMessage(m_Current, msg.Id);
                    m_SendMessage(msg.Sender, failureResult);
                    return;
                }

                var parameterTypes = from pair in invocation.Parameters
                                     select ProxyExtensions.ToType(pair.Item1);
                var method = type.GetMethod(invocation.MemberName, parameterTypes.ToArray());
                
                var parameterValues = from pair in invocation.Parameters
                                      select pair.Item2;
                result = method.Invoke(commandSet, parameterValues.ToArray()) as Task;
                
                Debug.Assert(result != null, "The command return result was not a Task or Task<T>.");
                result.ContinueWith(
                    t => ProcessTaskReturnResult(msg, t),
                    TaskContinuationOptions.ExecuteSynchronously);
            }
            catch (Exception e)
            {
                HandleCommandExecutionFailure(msg, e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "There is no point crashing the current app without being able to notify the other side of the channel.")]
        private void ProcessTaskReturnResult(CommandInvokedMessage msg, Task result)
        {
            try
            {
                ICommunicationMessage returnMsg = null;
                if (result == null)
                {
                    returnMsg = new FailureMessage(m_Current, msg.Id);
                }
                else
                {
                    // The result is either Task or Task<T> (or a continuation task of some form of 
                    // ContinuationTaskFromTask, ContinuationResultTaskFromResultTask etc. etc.)
                    // In order to select the right method on the TaskReturn object we would need to know at compile time
                    // which type we get. Fortunately through the use of the 'dynamic' keyword we can let the 
                    // runtime deal with the selection of the right method. By using 'dynamic' the generic parameters 
                    // for the method are determined by the input parameters, not by the declared ones.
                    dynamic taskBuilder = new TaskReturn(m_Diagnostics);

                    // Call the desired method, making sure that we force the runtime to use the runtime type of the result
                    // variable, not the compile time one.
                    returnMsg = (ICommunicationMessage)taskBuilder.HandleTaskReturnValue(m_Current, msg, (dynamic)result);
                }

                m_SendMessage(msg.Sender, returnMsg);
            }
            catch (Exception e)
            {
                HandleCommandExecutionFailure(msg, e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "There is no point crashing the current app without being able to notify the other side of the channel.")]
        private void HandleCommandExecutionFailure(CommandInvokedMessage msg, Exception e)
        {
            try
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while invoking command {0}.{1}. Exception is: {2}",
                        msg.Invocation.Type.FullName,
                        msg.Invocation.MemberName,
                        e));
                m_SendMessage(msg.Sender, new FailureMessage(m_Current, msg.Id));
            }
            catch (Exception errorSendingException)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Error while trying to send process failure. Exception is: {0}",
                        errorSendingException));
            }
        }
    }
}
