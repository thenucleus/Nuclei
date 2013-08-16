//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication
{
    /// <summary>
    /// A delegate used to manually add endpoints to the collection of known endpoints. 
    /// </summary>
    /// <remarks>
    /// This delegate is mainly to be used through the <see cref="CommunicationModule"/> where this 
    /// delegate is registered.
    /// </remarks>
    /// <param name="endpointId">The endpoint ID of the remote endpoint.</param>
    /// <param name="channelType">The channel type of the remote endpoint.</param>
    /// <param name="address">The address of the remote endpoint.</param>
    public delegate void ManualEndpointConnection(EndpointId endpointId, ChannelType channelType, string address);
}
