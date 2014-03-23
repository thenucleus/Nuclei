//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that provide the connection information for the local endpoint.
    /// </summary>
    public interface IProvideLocalConnectionInformation
    {
        /// <summary>
        /// Gets the ID of the local endpoint.
        /// </summary>
        EndpointId Id
        {
            get;
        }

        /// <summary>
        /// Returns the URI of the local entry channel for the given channel template.
        /// </summary>
        /// <param name="template">The channel template for which the entry channel should be provided.</param>
        /// <returns>The URI of the local entry channel.</returns>
        Uri EntryChannel(ChannelTemplate template);
    }
}
