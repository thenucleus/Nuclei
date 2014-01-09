//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.ServiceModel;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the methods for processing messages from the network.
    /// </summary>
    /// <design>
    /// This class is meant to be able to handle many messages being send at the same time, 
    /// however there should only be one instance of this class so that we can create it
    /// ourselves when we want to.
    /// </design>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    internal sealed class MessageReceivingEndpoint : IMessagePipe
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivingEndpoint"/> class.
        /// </summary>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public MessageReceivingEndpoint(SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Accepts the messages.
        /// </summary>
        /// <param name="message">The message.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't really want the channel to die just because the other side didn't behave properly.")]
        public void AcceptMessage(ICommunicationMessage message)
        {
            try
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Received message of type {0}.",
                        message.GetType()));

                RaiseOnNewMessage(message);
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception occurred during the handling of a message of type {0}. Exception was: {1}",
                        message.GetType(),
                        e));
            }
        }

        /// <summary>
        /// An event raised when a new message is available in the pipe.
        /// </summary>
        public event EventHandler<MessageEventArgs> OnNewMessage;

        private void RaiseOnNewMessage(ICommunicationMessage message)
        {
            var local = OnNewMessage;
            if (local != null)
            {
                local(this, new MessageEventArgs(message));
            }
        }
    }
}
