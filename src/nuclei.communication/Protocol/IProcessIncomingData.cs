//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Processes incoming data streams and invokes the desired functions based on pre-registered information.
    /// </summary>
    internal interface IProcessIncomingData
    {
        /// <summary>
        /// Processes the data and invokes the desired functions based on the pre-registered information.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        void ProcessData(DataTransferMessage message);

        /// <summary>
        /// Handles the case that an endpoint signs off.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that has signed off.</param>
        void OnEndpointSignedOff(EndpointId endpoint);

        /// <summary>
        /// Handles the case that the local channel, from which the input messages are send,
        /// is closed.
        /// </summary>
        void OnLocalChannelClosed();
    }
}
