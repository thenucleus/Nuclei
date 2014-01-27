//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that store information about all active channels.
    /// </summary>
    internal interface IStoreInformationForActiveChannels
    {
        /// <summary>
        /// Returns a collection containing information describing all the active channels.
        /// </summary>
        /// <returns>The collection containing information describing all the active channels.</returns>
        IEnumerable<EndpointInformation> ActiveChannels();
    }
}
