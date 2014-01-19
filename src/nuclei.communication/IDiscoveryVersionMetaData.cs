//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Discovery;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for strong-typed meta data describing an
    /// <see cref="IVersionedDiscoveryEndpoint"/>.
    /// </summary>
    internal interface IDiscoveryVersionMetaData
    {
        /// <summary>
        /// Gets the version of the discovery protocol that the attached
        /// endpoint can handle.
        /// </summary>
        Version Version
        {
            get;
        }
    }
}
