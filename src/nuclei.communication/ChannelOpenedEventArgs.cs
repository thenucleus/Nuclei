﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that indicates which channel has been opened.
    /// </summary>
    public sealed class ChannelOpenedEventArgs : EventArgs
    {
        /// <summary>
        /// The ID of the endpoint for which a channel was opened.
        /// </summary>
        private readonly EndpointId m_Endpoint;

        /// <summary>
        /// The type of channel that was opened.
        /// </summary>
        private readonly ChannelType m_ChannelType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelOpenedEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint for which the channel was opened.</param>
        /// <param name="channelType">The type of channel which was opened.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="InvalidChannelTypeException">
        ///     Thrown if <paramref name="channelType"/> is <see cref="Communication.ChannelType.None"/>.
        /// </exception>
        public ChannelOpenedEventArgs(EndpointId endpoint, ChannelType channelType)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.With<InvalidChannelTypeException>(
                    channelType != Communication.ChannelType.None,
                    Resources.Exceptions_Messages_AChannelTypeMustBeDefined);
            }

            m_Endpoint = endpoint;
            m_ChannelType = channelType;
        }

        /// <summary>
        /// Gets the ID of the endpoint for which a channel was opened.
        /// </summary>
        public EndpointId Endpoint
        {
            get
            {
                return m_Endpoint;
            }
        }

        /// <summary>
        /// Gets the type of channel that was opened.
        /// </summary>
        public ChannelType ChannelType
        {
            get
            {
                return m_ChannelType;
            }
        }
    }
}