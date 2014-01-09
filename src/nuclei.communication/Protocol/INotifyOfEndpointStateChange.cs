//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that provide notification of remote endpoint sign in and sign out.
    /// </summary>
    internal interface INotifyOfEndpointStateChange
    {
        /// <summary>
        /// An event raised when an endpoint has signed in.
        /// </summary>
        event EventHandler<EndpointSignInEventArgs> OnEndpointConnected;

        /// <summary>
        /// An event raised when an endpoint has signed out.
        /// </summary>
        event EventHandler<EndpointSignedOutEventArgs> OnEndpointDisconnected;
    }
}
