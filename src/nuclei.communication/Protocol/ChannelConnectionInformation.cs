//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Stores the information required to contact a given WCF endpoint on a specific machine.
    /// </summary>
    [Serializable]
    internal sealed class ChannelConnectionInformation
    {
        /// <summary>
        /// A URL that points to an invalid host.
        /// </summary>
        private static readonly Uri s_InvalidUri = new Uri(@"http://invalidhost/invalid_host_address");

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelConnectionInformation"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which this connection information is valid.</param>
        /// <param name="channelTemplate">
        ///     The type of the <see cref="IChannelTemplate"/> which indicates which kind of channel this connection
        ///     information describes.
        /// </param>
        /// <param name="messageAddress">The full URI for the message receiving channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidChannelTypeException">
        ///     Thrown if <paramref name="channelTemplate"/> is <see cref="Protocol.ChannelTemplate.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageAddress"/> is <see langword="null" />.
        /// </exception>
        public ChannelConnectionInformation(EndpointId endpoint, ChannelTemplate channelTemplate, Uri messageAddress)
            : this(endpoint, channelTemplate, messageAddress, s_InvalidUri)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelConnectionInformation"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which this connection information is valid.</param>
        /// <param name="channelTemplate">
        ///     The type of the <see cref="IChannelTemplate"/> which indicates which kind of channel this connection
        ///     information describes.
        /// </param>
        /// <param name="messageAddress">The full URI for the message receiving channel.</param>
        /// <param name="dataAddress">The full URI for the data receiving channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidChannelTypeException">
        ///     Thrown if <paramref name="channelTemplate"/> is <see cref="Protocol.ChannelTemplate.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageAddress"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="dataAddress"/> is <see langword="null" />.
        /// </exception>
        public ChannelConnectionInformation(EndpointId endpoint, ChannelTemplate channelTemplate, Uri messageAddress, Uri dataAddress)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.With<InvalidChannelTypeException>(
                    channelTemplate != ChannelTemplate.None, 
                    Resources.Exceptions_Messages_AChannelTypeMustBeDefined);

                Lokad.Enforce.Argument(() => messageAddress);
                Lokad.Enforce.Argument(() => dataAddress);
            }

            Id = endpoint;
            ChannelTemplate = channelTemplate;
            MessageAddress = messageAddress;
            DataAddress = dataAddress;
        }

        /// <summary>
        /// Gets a value indicating whether the connection information is complete.
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return !ReferenceEquals(DataAddress, s_InvalidUri);
            }
        }

        /// <summary>
        /// Gets a value indicating the ID of the endpoint for which this information
        /// is valid.
        /// </summary>
        public EndpointId Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating which type the channel is. The <see cref="Type"/>
        /// will be a derivative of <see cref="IChannelTemplate"/>.
        /// </summary>
        public ChannelTemplate ChannelTemplate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the message URI of the channel.
        /// </summary>
        public Uri MessageAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the data URI of the channel.
        /// </summary>
        public Uri DataAddress
        {
            get;
            private set;
        }
    }
}
