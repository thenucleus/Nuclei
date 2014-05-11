//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.ServiceModel;
using System.Threading.Tasks;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Protocol.V1
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
    internal sealed class MessageReceivingEndpoint : IMessagePipe, IMessageReceivingEndpoint
    {
        /// <summary>
        /// The collection that contains the converters which convert between <see cref="ICommunicationMessage"/> objects
        /// and <see cref="IStoreV1CommunicationData"/> objects.
        /// </summary>
        private readonly Dictionary<Type, IConvertCommunicationMessages> m_Converters
            = new Dictionary<Type, IConvertCommunicationMessages>();

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivingEndpoint"/> class.
        /// </summary>
        /// <param name="messageConverters">The collection that contains all the message converters.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageConverters"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public MessageReceivingEndpoint(
            IEnumerable<IConvertCommunicationMessages> messageConverters,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => messageConverters);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Diagnostics = systemDiagnostics;
            foreach (var converter in messageConverters)
            {
                m_Converters.Add(converter.DataTypeToTranslate, converter);
            }
        }

        /// <summary>
        /// Accepts the messages.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>An object indicating that the data was received successfully.</returns>
        public MessageReceptionConfirmation AcceptMessage(IStoreV1CommunicationData message)
        {
            Task.Factory.StartNew(ProcessMessage, message);
            return new MessageReceptionConfirmation
                {
                    WasDataReceived = true,
                };
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't really want the channel to die just because the other side didn't behave properly.")]
        private void ProcessMessage(object obj)
        {
            var message = obj as IStoreV1CommunicationData;
            Debug.Assert(message != null, "The data should be an IStoreV1CommunicationData instance.");

            try
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Received message of type {0}.",
                        message.GetType()));

                var translatedMessage = TranslateMessage(message);
                RaiseOnNewMessage(translatedMessage);
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

        private ICommunicationMessage TranslateMessage(IStoreV1CommunicationData message)
        {
            if (!m_Converters.ContainsKey(message.GetType()))
            {
                return new UnknownMessageTypeMessage(message.Sender, message.InResponseTo);
            }

            var converter = m_Converters[message.GetType()];
            return converter.ToMessage(message);
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
