//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines a function that is used to verify the status of a remote endpoint.
    /// </summary>
    /// <param name="id">The ID of the remote endpoint.</param>
    /// <param name="timeout">The maximum amount of time the response operation is allowed to take.</param>
    /// <param name="data">The custom data that should be send to the remote endpoint.</param>
    /// <returns>A task containing the response data from the remote endpoint.</returns>
    internal delegate Task<object> VerifyEndpointConnectionStatusWithCustomData(EndpointId id, TimeSpan timeout, object data = null);
}
