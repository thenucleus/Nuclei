//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that handle incoming connections.
    /// </summary>
    internal interface IHandleConnections
    {
        /// <summary>
        /// An event raised when data is received from a remote endpoint.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnConfirmIncomingChannelIntegrity;

        /// <summary>
        /// An event raised when data is send to a remote endpoint.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnConfirmOutgoingChannelIntegrity;
    }
}
