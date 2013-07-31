//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that handle communication with a remote application.
    /// </summary>
    internal interface ICommunicationChannel
    {
        /// <summary>
        /// Gets the connection information that describes the local endpoint.
        /// </summary>
        ChannelConnectionInformation LocalConnectionPoint
        {
            get;
        }

        /// <summary>
        /// Opens the channel and provides information on how to connect to the given channel.
        /// </summary>
        void OpenChannel();

        /// <summary>
        /// Closes the current channel.
        /// </summary>
        void CloseChannel();

        /// <summary>
        /// Disconnects from the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint from which the channel needs to disconnect.</param>
        void DisconnectFrom(EndpointId endpoint);

        /// <summary>
        /// Transfers the data to the receiving endpoint.
        /// </summary>
        /// <param name="receivingEndpoint">The endpoint that will receive the data stream.</param>
        /// <param name="filePath">The file path to the file that should be transferred.</param>
        /// <param name="token">The cancellation token that is used to cancel the task if necessary.</param>
        /// <param name="scheduler">The scheduler that is used to run the return task with.</param>
        /// <returns>
        /// An task that indicates when the transfer is complete.
        /// </returns>
        Task TransferData(
            EndpointId receivingEndpoint,
            string filePath, 
            CancellationToken token,
            TaskScheduler scheduler);

        /// <summary>
        /// Sends the given message to the receiving endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="message">The message that should be send.</param>
        void Send(EndpointId endpoint, ICommunicationMessage message);

        /// <summary>
        /// An event raised when a new message is received.
        /// </summary>
        event EventHandler<MessageEventArgs> OnMessageReception;

        /// <summary>
        /// An event raised when a new data stream is received.
        /// </summary>
        event EventHandler<DataTransferEventArgs> OnDataReception;

        /// <summary>
        /// An event raised when the the channel is closed.
        /// </summary>
        event EventHandler<ChannelClosedEventArgs> OnClosed;
    }
}
