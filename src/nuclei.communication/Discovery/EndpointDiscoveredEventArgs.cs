//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines an <see cref="EventArgs"/> class that stores information about an endpoint that was
    /// recently discovered.
    /// </summary>
    [Serializable]
    internal sealed class EndpointDiscoveredEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointDiscoveredEventArgs"/> class.
        /// </summary>
        /// <param name="channelInformation">The connection information for the new endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelInformation"/> is <see langword="null" />.
        /// </exception>
        public EndpointDiscoveredEventArgs(IDiscoveryInformation channelInformation)
        {
            {
                Lokad.Enforce.Argument(() => channelInformation);
            }

            ConnectionInformation = channelInformation;
        }

        /// <summary>
        /// Gets the information for the channel on which the new endpoint can be contacted.
        /// </summary>
        public IDiscoveryInformation ConnectionInformation
        {
            get;
            private set;
        }
    }
}
