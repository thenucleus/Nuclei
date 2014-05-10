//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Interaction.Transport.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="CommandInvokedMessage"/>.
    /// </summary>
    internal sealed class CommandInvokedProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The collection that holds all the registered commands.
        /// </summary>
        private readonly ICommandCollection m_Commands;

        /// <summary>
        /// The action that is used to send a message to a remote endpoint.
        /// </summary>
        private readonly SendMessage m_SendMessage;

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
            SendMessage sendMessage,
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
            [DebuggerStepThrough]
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
                    "Received request to execute command: {0}",
                    invocation.Command));

            try
            {
                var id = invocation.Command;
                CommandDefinition commandSet;
                try
                {
                    commandSet = m_Commands.CommandToInvoke(id);
                }
                catch (UnknownCommandException)
                {
                    m_Diagnostics.Log(
                        LevelToLog.Trace,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Command invokation was requested for {0} from {1} but this command was not registered.",
                            id,
                            msg.Sender));

                    var failureResult = new FailureMessage(m_Current, msg.Id);
                    m_SendMessage(msg.Sender, failureResult, CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);
                    return;
                }

                var result = commandSet.Invoke(message.Sender, message.Id, invocation.Parameters);

                ICommunicationMessage responseMessage;
                if (commandSet.HasReturnValue)
                {
                    responseMessage = new CommandInvokedResponseMessage(m_Current, msg.Id, result);
                }
                else
                {
                    responseMessage = new SuccessMessage(m_Current, msg.Id);
                }

                m_SendMessage(msg.Sender, responseMessage, CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);
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
                        "Error while invoking command {0}. Exception is: {1}",
                        msg.Invocation.Command,
                        e));
                m_SendMessage(msg.Sender, new FailureMessage(m_Current, msg.Id), CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);
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
