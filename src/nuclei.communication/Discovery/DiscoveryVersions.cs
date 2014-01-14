//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the known versions of the discovery layer.
    /// </summary>
    internal static class DiscoveryVersions
    {
        /// <summary>
        /// Gets the current version of the discovery layer.
        /// </summary>
        public static Version Current
        {
            get
            {
                return V1;
            }
        }

        /// <summary>
        /// Gets the version of the base discovery layer.
        /// </summary>
        public static Version V1
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        /// <summary>
        /// Returns a collection containing all the supported versions of the discovery protocol.
        /// </summary>
        /// <returns>A collection containing all the supported versions of the discovery protocol.</returns>
        public static IEnumerable<Version> SupportedVersions()
        {
            return new[]
                {
                    V1,
                };
        }
    }
}