//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// An <see cref="EventArgs"/> class that stores information about an endpoint that has signed in with the current endpoint.
    /// </summary>
    internal sealed class EndpointSignInEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointSignInEventArgs"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection information.</param>
        /// <param name="description">The communication description.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="connectionInfo"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="description"/> is <see langword="null" />.
        /// </exception>
        public EndpointSignInEventArgs(ChannelConnectionInformation connectionInfo, CommunicationDescription description)
        {
            {
                Lokad.Enforce.Argument(() => connectionInfo);
                Lokad.Enforce.Argument(() => description);
            }

            ConnectionInformation = connectionInfo;
            Description = description;
        }

        /// <summary>
        /// Gets a value providing the information necessary to connect to a given endpoint.
        /// </summary>
        public ChannelConnectionInformation ConnectionInformation
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value providing the description of the capabilities of the given endpoint.
        /// </summary>
        public CommunicationDescription Description
        {
            get;
            private set;
        }
    }
}
