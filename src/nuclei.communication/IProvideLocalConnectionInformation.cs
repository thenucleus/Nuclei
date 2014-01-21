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
        /// Gets the URI of the local entry channel.
        /// </summary>
        Uri EntryChannel
        {
            get;
        }
    }
}
