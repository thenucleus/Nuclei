//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the methods for communicating with a remote endpoint.
    /// </summary>
    internal interface IProtocolLayer : IStoreInformationForActiveChannels, IConfirmConnections, IDisposable
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
        /// Verifies that the connection to the given endpoint can be used.
        /// </summary>
        /// <param name="id">The endpoint ID of the endpoint to which the connection should be verified.</param>
        /// <param name="timeout">The maximum amount of time the response operation is allowed to take.</param>
        /// <param name="verificationData">The data that should be send to the endpoint for verification of the connection.</param>
        /// <returns>
        /// A task that contains the response to the verification data value, or <see langword="null" /> if no
        /// verification data was provided.
        /// </returns>
        Task<object> VerifyConnectionIsActive(EndpointId id, TimeSpan timeout, object verificationData = null);

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="connection">The connection information for the endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <param name="maximumNumberOfRetries">
        /// The maximum number of times the endpoint will try to send the message if delivery fails. 
        /// Defaults to <see cref="CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending"/>.
        /// </param>
        void SendMessageToUnregisteredEndpoint(
            EndpointInformation connection, 
            ICommunicationMessage message,
            int maximumNumberOfRetries = CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);

        /// <summary>
        /// Sends the given message to the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message has to be send.</param>
        /// <param name="message">The message that has to be send.</param>
        /// <param name="maximumNumberOfRetries">
        /// The maximum number of times the endpoint will try to send the message if delivery fails. 
        /// Defaults to <see cref="CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending"/>.
        /// </param>
        void SendMessageTo(
            EndpointId endpoint, 
            ICommunicationMessage message,
            int maximumNumberOfRetries = CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);

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
        /// <param name="maximumNumberOfRetries">
        /// The maximum number of times the endpoint will try to send the message if delivery fails. 
        /// Defaults to <see cref="CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending"/>.
        /// </param>
        /// <returns>
        ///     A task that will return once the upload is complete.
        /// </returns>
        Task UploadData(
            EndpointId receivingEndpoint,
            string filePath,
            CancellationToken token,
            TaskScheduler scheduler = null,
            int maximumNumberOfRetries = CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);
    }
}
