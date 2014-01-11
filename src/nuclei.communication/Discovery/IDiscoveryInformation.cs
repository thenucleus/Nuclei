//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the interface for objects that store information regarding newly discovered
    /// endpoints.
    /// </summary>
    internal interface IDiscoveryInformation
    {
        /// <summary>
        /// Gets the version of the information object.
        /// </summary>
        Version DiscoveryVersion
        {
            get;
        }
    }
}
