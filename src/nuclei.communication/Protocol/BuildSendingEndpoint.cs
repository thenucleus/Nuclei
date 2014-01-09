//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// A delegate used to build <see cref="ISendingEndpoint"/> instances.
    /// </summary>
    /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
    /// <param name="messageSendingEndpointBuilder">The function used to build the sending endpoint for messages.</param>
    /// <param name="dataSendingEndpointBuilder">The function used to build the sending endpoint for data.</param>
    /// <returns>The new <see cref="ISendingEndpoint"/> instance.</returns>
    internal delegate ISendingEndpoint BuildSendingEndpoint(
        EndpointId localEndpoint,
        Func<EndpointId, IMessageSendingEndpoint> messageSendingEndpointBuilder,
        Func<EndpointId, IDataTransferingEndpoint> dataSendingEndpointBuilder);
}
