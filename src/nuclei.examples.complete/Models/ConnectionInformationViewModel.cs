//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Communication;

namespace Nuclei.Examples.Complete.Models
{
    /// <summary>
    /// Stores information about an endpoint.
    /// </summary>
    internal sealed class ConnectionInformationViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionInformationViewModel"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        public ConnectionInformationViewModel(EndpointId endpoint)
        {
            Id = endpoint;
        }

        /// <summary>
        /// Gets the ID number of the current endpoint.
        /// </summary>
        public EndpointId Id
        {
            get;
            private set;
        }
    }
}
