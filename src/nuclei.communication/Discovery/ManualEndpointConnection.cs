//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// A delegate used to manually add endpoints to the collection of known endpoints. 
    /// </summary>
    /// <remarks>
    /// This delegate is mainly to be used through the <see cref="CommunicationModule"/> where this 
    /// delegate is registered.
    /// </remarks>
    /// <param name="address">The URI of the discovery point for the remote endpoint.</param>
    public delegate void ManualEndpointConnection(string address);
}
