//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that indicates which channel has been closed.
    /// </summary>
    public sealed class ChannelClosedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChannelClosedEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that was closed.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        public ChannelClosedEventArgs(EndpointId endpoint)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
            }

            ClosedChannel = endpoint;
        }

        /// <summary>
        /// Gets a value indicating the ID of the closed channel.
        /// </summary>
        public EndpointId ClosedChannel
        {
            get;
            private set;
        }
    }
}
