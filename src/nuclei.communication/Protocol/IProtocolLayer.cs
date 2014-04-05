//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the methods for communicating with a remote endpoint.
    /// </summary>
    internal interface IProtocolLayer : IStoreInformationForActiveChannels, INotifyOfEndpointStateChange, IDisposable
    {
        /// <summary>
        /// Gets the endpoint ID of the local endpoint.
        /// </summary>
        EndpointId Id
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has signed on with
        /// the network.
        /// </summary>
        bool IsSignedIn
        { 
            get; 
        }

        /// <summary>
        /// Gets the connection information for the channel of a given type created by the current application.
        /// </summary>
        /// <param name="protocolVersion">The version of the protocol for which the connection information is required.</param>
        /// <param name="channelTemplate">The type of channel for which the connection information is required.</param>
        /// <returns>
        /// A tuple containing the <see cref="EndpointId"/>, the <see cref="Uri"/> of the message channel and the 
        /// <see cref="Uri"/> of the data channel; returns <see langword="null" /> if no channel of the given type exists.
        /// </returns>
        Tuple<EndpointId, Uri, Uri> LocalConnectionFor(Version protocolVersion, ChannelTemplate channelTemplate);

        /// <summary>
        /// Returns a collection containing the endpoint IDs of the known remote endpoints.
        /// </summary>
        /// <returns>
        ///     The collection that contains the endpoint IDs of the remote endpoints.
        /// </returns>
        IEnumerable<EndpointId> KnownEndpoints();

        /// <summary>
        /// Connects to the network and broadcasts a sign on message.
        /// </summary>
        void SignIn();

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        void SignOut();

        /// <summary>
        /// An event raised when the layer has signed in.
        /// </summary>
        event EventHandler<EventArgs> OnSignedIn;

        /// <summary>
        /// An event raised when the layer has signed out.
        /// </summary>
        event EventHandler<EventArgs> OnSignedOut;

        /// <summary>
        /// Returns a value indicating if the given endpoint has provided the information required to
        /// contact it if it isn't offline.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint has provided the information necessary to contact 
        ///     it over the network. Otherwise; <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsEndpointContactable(EndpointId endpoint);

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="connection">The connection information for the endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        void SendMessageToUnregisteredEndpoint(EndpointInformation connection, ICommunicationMessage message);

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        void SendMessageTo(EndpointId endpoint, ICommunicationMessage message);

        /// <summary>
        /// Sends the given message to the specified endpoint and returns a task that
        /// will eventually contain the return message.
        /// </summary>
        /// <param name="connection">The connection information for the endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <param name="timeout">The maximum amount of time the response operation is allowed to take.</param>
        /// <returns>A task object that will eventually contain the response message.</returns>
        Task<ICommunicationMessage> SendMessageToUnregisteredEndpointAndWaitForResponse(
            EndpointInformation connection, 
            ICommunicationMessage message,
            TimeSpan timeout);

        /// <summary>
        /// Sends the given message to the specified endpoint and returns a task that
        /// will eventually contain the return message.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <param name="timeout">The maximum amount of time the response operation is allowed to take.</param>
        /// <returns>A task object that will eventually contain the response message.</returns>
        Task<ICommunicationMessage> SendMessageAndWaitForResponse(EndpointId endpoint, ICommunicationMessage message, TimeSpan timeout);

        /// <summary>
        /// Uploads a given file to a specific endpoint.
        /// </summary>
        /// <param name="receivingEndpoint">The endpoint that will receive the data stream.</param>
        /// <param name="filePath">The full path to the file that should be transferred.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task.</param>
        /// <returns>
        ///     A task that will return once the upload is complete.
        /// </returns>
        Task UploadData(
            EndpointId receivingEndpoint,
            string filePath,
            CancellationToken token,
            TaskScheduler scheduler);
    }
}
