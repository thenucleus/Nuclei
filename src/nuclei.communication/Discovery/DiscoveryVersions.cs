//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

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
                return Base;
            }
        }

        /// <summary>
        /// Gets the version of the base discovery layer.
        /// </summary>
        public static Version Base
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }
    }
}