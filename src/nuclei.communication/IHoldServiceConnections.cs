//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the methods used to create a <see cref="ServiceHost"/> and to keep the connection
    /// through this host alive.
    /// </summary>
    internal interface IHoldServiceConnections : IDisposable
    {
        /// <summary>
        /// Opens the channel and provides information on how to connect to the given channel.
        /// </summary>
        /// <param name="receiver">The object that receives the transmissions from the remote endpoint.</param>
        /// <param name="attachEndpoint">The function that attaches an endpoint to the service host.</param>
        /// <returns>The URL of the newly opened channel.</returns>
        Uri OpenChannel(IReceiveInformationFromRemoteEndpoints receiver, Func<ServiceHost, ServiceEndpoint> attachEndpoint);

        /// <summary>
        /// Closes the current connection.
        /// </summary>
        void CloseConnection();
    }
}
