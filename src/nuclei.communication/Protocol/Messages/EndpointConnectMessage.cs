//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Properties;

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
        /// <param name="channelTemplate">
        ///     The <see cref="IChannelTemplate"/> of the channel that was used to send this message.
        /// </param>
        /// <param name="originatingMessageAddress">The address of the originating endpoint that is used for message reception.</param>
        /// <param name="originatingDataAddres">The address of the originating endpoint that is used for data reception.</param>
        /// <param name="information">
        ///     The information describing the version of the communication protocol
        ///     used by the sender, the desired communication API's for the sender and 
        ///     the available communication API's provided by the sender.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidChannelTypeException">
        ///     Thrown if <paramref name="channelTemplate"/> is <see cref="Protocol.ChannelTemplate.None"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="originatingMessageAddress"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="originatingMessageAddress"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="originatingDataAddres"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="originatingDataAddres"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="information"/> is <see langword="null" />.
        /// </exception>
        public EndpointConnectMessage(
            EndpointId origin, 
            ChannelTemplate channelTemplate, 
            string originatingMessageAddress, 
            string originatingDataAddres,
            CommunicationDescription information)
            : base(origin)
        {
            {
                Lokad.Enforce.With<InvalidChannelTypeException>(
                    channelTemplate != ChannelTemplate.None, 
                    Resources.Exceptions_Messages_AChannelTypeMustBeDefined);

                Lokad.Enforce.Argument(() => originatingMessageAddress);
                Lokad.Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(originatingMessageAddress),
                    Resources.Exceptions_Messages_ChannelAddresssMustBeDefined);

                Lokad.Enforce.Argument(() => originatingDataAddres);
                Lokad.Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(originatingDataAddres),
                    Resources.Exceptions_Messages_ChannelAddresssMustBeDefined);

                Lokad.Enforce.Argument(() => information);
            }

            ChannelTemplate = channelTemplate;
            MessageAddress = originatingMessageAddress;
            DataAddress = originatingDataAddres;
            Information = information;
        }

        /// <summary>
        /// Gets a value indicating what kind of channel was used
        /// to send this message.
        /// </summary>
        public ChannelTemplate ChannelTemplate
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the URI of the channel that is used for message reception.
        /// </summary>
        public string MessageAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating the URI of the channel that is used for data reception.
        /// </summary>
        public string DataAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the information describing the version of the communication protocol
        /// used by the sender, the desired communication API's for the sender and 
        /// the available communication API's provided by the sender.
        /// </summary>
        public CommunicationDescription Information
        {
            get;
            private set;
        }
    }
}
