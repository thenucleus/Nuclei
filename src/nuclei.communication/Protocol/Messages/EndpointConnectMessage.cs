//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol.Messages
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint has connected to
    /// the current endpoint.
    /// </summary>
    internal sealed class EndpointConnectMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointConnectMessage"/> class.
        /// </summary>
        /// <param name="origin">The ID of the endpoint that send the message.</param>
        /// <param name="discoveryInformation">The object containing the information about the discovery channel for the remote endpoint.</param>
        /// <param name="protocolInformation">The object containing the information about the protocol channels for the remote endpoint.</param>
        /// <param name="information">
        ///     The information describing the version of the communication protocol
        ///     used by the sender, the desired communication API's for the sender and 
        ///     the available communication API's provided by the sender.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="discoveryInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="protocolInformation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="information"/> is <see langword="null" />.
        /// </exception>
        public EndpointConnectMessage(
            EndpointId origin,
            DiscoveryInformation discoveryInformation,
            ProtocolInformation protocolInformation,
            ProtocolDescription information)
            : base(origin)
        {
            {
                Lokad.Enforce.Argument(() => discoveryInformation);
                Lokad.Enforce.Argument(() => protocolInformation);
                Lokad.Enforce.Argument(() => information);
            }

            DiscoveryInformation = discoveryInformation;
            ProtocolInformation = protocolInformation;
            Information = information;
        }

        /// <summary>
        /// Gets a value describing the discovery channel for the remote endpoint.
        /// </summary>
        public DiscoveryInformation DiscoveryInformation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value describing the protocol channel for the remote endpoint.
        /// </summary>
        public ProtocolInformation ProtocolInformation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the information describing the version of the communication protocol
        /// used by the sender, the desired communication API's for the sender and 
        /// the available communication API's provided by the sender.
        /// </summary>
        public ProtocolDescription Information
        {
            get;
            private set;
        }
    }
}
