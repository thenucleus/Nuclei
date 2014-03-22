//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Defines the sending end of a WCF message channel which can resurrect the channel if it faults.
    /// </summary>
    /// <source>
    /// Original idea obtained from http://kentb.blogspot.com/2010/01/wcf-channels-faulting-and-dependency.html
    /// </source>
    internal sealed class RestoringMessageSendingEndpoint : IMessageSendingEndpoint
    {
        /// <summary>
        /// The lock object used for getting locks on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection that contains the converters which convert between <see cref="ICommunicationMessage"/> objects
        /// and <see cref="IStoreV1CommunicationData"/> objects.
        /// </summary>
        private readonly Dictionary<Type, IConvertCommunicationMessages> m_Converters
            = new Dictionary<Type, IConvertCommunicationMessages>();

        /// <summary>
        /// The factory which creates new WCF channels.
        /// </summary>
        private readonly ChannelFactory<IMessageReceivingEndpointProxy> m_Factory;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The service on the other side of the channel.
        /// </summary>
        private IMessageReceivingEndpoint m_Service;

        /// <summary>
        /// The channel that handles the connections.
        /// </summary>
        private IChannel m_Channel;

        /// <summary>
        /// Indicates if the current endpoint has been disposed.
        /// </summary>
        private volatile bool m_IsDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RestoringMessageSendingEndpoint"/> class.
        /// </summary>
        /// <param name="address">The address of the remote endpoint.</param>
        /// <param name="template">The template that is used to create the binding used to connect to the remote endpoint.</param>
        /// <param name="dataContractResolver">The <see cref="DataContractResolver"/> that is used for the endpoint.</param>
        /// <param name="messageConverters">The collection that contains all the message converters.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostic methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="address"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="template"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataContractResolver"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageConverters"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public RestoringMessageSendingEndpoint(
            Uri address,
            IProtocolChannelTemplate template,
            ProtocolDataContractResolver dataContractResolver,
            IEnumerable<IConvertCommunicationMessages> messageConverters,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => address);
                Lokad.Enforce.Argument(() => template);
                Lokad.Enforce.Argument(() => dataContractResolver);
                Lokad.Enforce.Argument(() => messageConverters);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            var endpoint = new EndpointAddress(address);
            var binding = template.GenerateMessageBinding();
            m_Factory = new ChannelFactory<IMessageReceivingEndpointProxy>(binding, endpoint);
            foreach (var operation in m_Factory.Endpoint.Contract.Operations)
            {
                var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior == null)
                {
                    behavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(behavior);
                }

                behavior.DataContractResolver = dataContractResolver;
            }

            m_Diagnostics = systemDiagnostics;

            foreach (var converter in messageConverters)
            {
                m_Converters.Add(converter.MessageTypeToTranslate, converter);
            }
        }

        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="message">The message to be send.</param>
        public void Send(ICommunicationMessage message)
        {
            var v1Message = TranslateMessage(message);
            SendMessage(v1Message);
        }

        private IStoreV1CommunicationData TranslateMessage(ICommunicationMessage message)
        {
            if (!m_Converters.ContainsKey(message.GetType()))
            {
                throw new UnknownMessageTypeException();
            }

            var converter = m_Converters[message.GetType()];
            return converter.FromMessage(message);
        }

        private void SendMessage(IStoreV1CommunicationData message)
        {
            EnsureChannelIsAvailable();

            try
            {
                var service = m_Service;
                if (!m_IsDisposed)
                {
                    m_Diagnostics.Log(
                           LevelToLog.Trace,
                           CommunicationConstants.DefaultLogTextPrefix,
                           string.Format(
                               CultureInfo.InvariantCulture,
                               "Sending message [{0}] of type {1}.",
                               message.Id,
                               message.GetType()));

                    service.AcceptMessage(message);
                }
            }
            catch (FaultException e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception occurred during the sending of message of type {0}. Exception was: {1}",
                        message.GetType(),
                        e));

                if (e.InnerException != null)
                {
                    throw new FailedToSendMessageException(Resources.Exceptions_Messages_FailedToSendMessage, e.InnerException);
                }

                // There is no point in keeping the original call stack. The original
                // exception orginates on the other side of the channel. There is no
                // useful stack trace to keep!
                throw new FailedToSendMessageException();
            }
            catch (CommunicationException e)
            {
                // Either the connection was aborted or faulted (although it shouldn't be)
                // or something else nasty went wrong.
                throw new FailedToSendMessageException(Resources.Exceptions_Messages_FailedToSendMessage, e);
            }
        }

        private void EnsureChannelIsAvailable()
        {
            if (ShouldCreateChannel)
            {
                lock (m_Lock)
                {
                    if (ShouldCreateChannel)
                    {
                        if (m_Channel != null)
                        {
                            // The channel is probably faulted so terminate it.
                            m_Diagnostics.Log(
                                LevelToLog.Info,
                                CommunicationConstants.DefaultLogTextPrefix,
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Channel for endpoint at {0} has faulted. Aborting channel.",
                                    m_Factory.Endpoint.Address.Uri));
                            m_Channel.Abort();
                        }

                        m_Diagnostics.Log(
                            LevelToLog.Info,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Creating channel for endpoint at {0}.",
                                m_Factory.Endpoint.Address.Uri));
                        m_Service = m_Factory.CreateChannel();
                        m_Channel = (IChannel)m_Service;
                    }
                }
            }
        }

        private bool ShouldCreateChannel
        {
            [DebuggerStepThrough]
            get
            {
                return (!m_IsDisposed) && ((m_Channel == null) || (m_Channel.State == CommunicationState.Faulted));
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (m_IsDisposed)
            {
                // We've already disposed of the channel. Job done.
                return;
            }

            m_IsDisposed = true;

            IChannel local;
            lock (m_Lock)
            {
                local = m_Channel;
                m_Channel = null;
            }

            if (local != null && local.State != CommunicationState.Faulted)
            {
                try
                {
                    local.Close();
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Disposed of channel for {0}",
                            m_Factory.Endpoint.Address.Uri));
                }
                catch (CommunicationObjectFaultedException e)
                {
                    // The channel is faulted but there is nothing
                    // we can do about that so just ignore it.
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            m_Factory.Endpoint.Address.Uri,
                            e));
                }
                catch (ProtocolException e)
                {
                    // Apparently the channel was still in use, but we don't want to 
                    // use it anymore so just ignore it
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            m_Factory.Endpoint.Address.Uri,
                            e));
                }
                catch (CommunicationObjectAbortedException e)
                {
                    // The channel is now faulted but there is nothing
                    // we can do about that so just ignore it.
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            m_Factory.Endpoint.Address.Uri,
                            e));
                }
                catch (TimeoutException e)
                {
                    // The default close timeout elapsed before we were 
                    // finished closing the channel. So the channel
                    // is aborted. Nothing we can do, just ignore it.
                    m_Diagnostics.Log(
                        LevelToLog.Debug,
                        CommunicationConstants.DefaultLogTextPrefix,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Channel for {0} failed to close normally. Exception was: {1}",
                            m_Factory.Endpoint.Address.Uri,
                            e));
                }
            }
        }
    }
}
