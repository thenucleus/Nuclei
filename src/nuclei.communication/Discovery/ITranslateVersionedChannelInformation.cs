//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the interface for objects that get channel information via a versioned discovery channel.
    /// </summary>
    internal interface ITranslateVersionedChannelInformation
    {
        /// <summary>
        /// Returns channel information obtained from a specific versioned discovery channel.
        /// </summary>
        /// <param name="address">The address of the versioned discovery channel.</param>
        /// <returns>The channel information.</returns>
        ChannelInformation FromUri(Uri address);
    }
}
