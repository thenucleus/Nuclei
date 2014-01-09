//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Implements <see cref="ISendingEndpoint"/> to allow sending messages to a given endpoint.
    /// </summary>
    internal sealed class SendingEndpoint : ISendingEndpoint
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection that maps between the endpoint ID and the channel that is used to
        /// send messages to the given endpoint.
        /// </summary>
        private readonly Dictionary<EndpointId, Tuple<IMessageSendingEndpoint, IDataTransferingEndpoint>> m_EndpointMap
            = new Dictionary<EndpointId, Tuple<IMessageSendingEndpoint, IDataTransferingEndpoint>>();

        /// <summary>
        /// The endpoint ID of the local endpoint.
        /// </summary>
        private readonly EndpointId m_LocalEndpoint;

        /// <summary>
        /// The function that is used to retrieve the message sending channel for a given endpoint.
        /// </summary>
        private readonly Func<EndpointId, IMessageSendingEndpoint> m_MessageSenderBuilder;

        /// <summary>
        /// The function that is used to retrieve the data sending channel for a given endpoint.
        /// </summary>
        private readonly Func<EndpointId, IDataTransferingEndpoint> m_DataSenderBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendingEndpoint"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="messageSenderBuilder">
        /// The function used to create new message sending channels for a given endpoint.
        /// </param>
        /// <param name="dataSenderBuilder">
        /// The function used to create a new data transferring channel for a given endpoint.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageSenderBuilder"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataSenderBuilder"/> is <see langword="null" />.
        /// </exception>
        public SendingEndpoint(
            EndpointId localEndpoint,
            Func<EndpointId, IMessageSendingEndpoint> messageSenderBuilder,
            Func<EndpointId, IDataTransferingEndpoint> dataSenderBuilder)
        {
            {
                Lokad.Enforce.Argument(() => localEndpoint);
                Lokad.Enforce.Argument(() => messageSenderBuilder);
                Lokad.Enforce.Argument(() => dataSenderBuilder);
            }

            m_LocalEndpoint = localEndpoint;
            m_MessageSenderBuilder = messageSenderBuilder;
            m_DataSenderBuilder = dataSenderBuilder;
        }

        /// <summary>
        /// Returns the collection of known endpoints.
        /// </summary>
        /// <returns>
        /// The collection of known endpoints.
        /// </returns>
        public IEnumerable<EndpointId> KnownEndpoints()
        {
            return m_EndpointMap.Keys;
        }

        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="message">The message to be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="message"/> is <see langword="null" />.
        /// </exception>
        public void Send(EndpointId endpoint, ICommunicationMessage message)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => message);
            }

            var channel = MessageChannelFor(endpoint);
            channel.Send(message);
        }

        private IMessageSendingEndpoint MessageChannelFor(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                if (!m_EndpointMap.ContainsKey(endpoint))
                {
                    m_EndpointMap.Add(endpoint, new Tuple<IMessageSendingEndpoint, IDataTransferingEndpoint>(null, null));
                }

                if (m_EndpointMap[endpoint].Item1 == null)
                {
                    var messageChannel = m_MessageSenderBuilder(endpoint);

                    var tuple = m_EndpointMap[endpoint];
                    m_EndpointMap[endpoint] = new Tuple<IMessageSendingEndpoint, IDataTransferingEndpoint>(
                        messageChannel,
                        tuple.Item2);
                }

                return m_EndpointMap[endpoint].Item1;
            }
        }

        private IDataTransferingEndpoint DataChannelFor(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                if (!m_EndpointMap.ContainsKey(endpoint))
                {
                    m_EndpointMap.Add(endpoint, new Tuple<IMessageSendingEndpoint, IDataTransferingEndpoint>(null, null));
                }

                if (m_EndpointMap[endpoint].Item2 == null)
                {
                    var dataChannel = m_DataSenderBuilder(endpoint);

                    var tuple = m_EndpointMap[endpoint];
                    m_EndpointMap[endpoint] = new Tuple<IMessageSendingEndpoint, IDataTransferingEndpoint>(
                        tuple.Item1,
                        dataChannel);
                }

                return m_EndpointMap[endpoint].Item2;
            }
        }

        /// <summary>
        /// Transfers the stream to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the data should be send.</param>
        /// <param name="data">The data to be send.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="data"/> is <see langword="null" />.
        /// </exception>
        public void Send(EndpointId endpoint, Stream data)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => data);
            }

            var channel = DataChannelFor(endpoint);
            var msg = new DataTransferMessage
                {
                    SendingEndpoint = m_LocalEndpoint,
                    ReceivingEndpoint = endpoint,
                    Data = data,
                };
            channel.Send(msg);
        }

        /// <summary>
        /// Closes the channel that connects to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint to which the connection should be closed.</param>
        public void CloseChannelTo(EndpointId endpoint)
        {
            lock (m_Lock)
            {
                if (!m_EndpointMap.ContainsKey(endpoint))
                {
                    return;
                }

                var tuple = m_EndpointMap[endpoint];
                m_EndpointMap.Remove(endpoint);

                if (tuple.Item1 != null)
                {
                    tuple.Item1.Dispose();
                }

                if (tuple.Item2 != null)
                {
                    tuple.Item2.Dispose();
                }
            }
        }
    }
}
