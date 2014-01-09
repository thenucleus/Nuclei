//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that provides the endpoint information for an endpoint that 
    /// has signed off.
    /// </summary>
    public sealed class EndpointSignedOutEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointSignedOutEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <param name="channelType">The type of channel on which the endpoint was connected.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        public EndpointSignedOutEventArgs(EndpointId endpoint, ChannelType channelType)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
            }

            Endpoint = endpoint;
            ChannelType = channelType;
        }

        /// <summary>
        /// Gets a value indicating the ID of the affected endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of channel on which the affected endpoint was connected.
        /// </summary>
        public ChannelType ChannelType
        {
            get;
            private set;
        }
    }
}
