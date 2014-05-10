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
    /// Defines the signature for functions that send messages and then wait for a response.
    /// </summary>
    /// <param name="endpoint">The endpoint to which the message is send.</param>
    /// <param name="message">The message.</param>
    /// <param name="maximumNumberOfRetries">
        /// The maximum number of times the endpoint will try to send the message if delivery fails. 
        /// </param>
    /// <param name="timeout">The maximum amount of time the response operation is allowed to take.</param>
    /// <returns>
    /// A task object that will eventually contain the response message.
    /// </returns>
    internal delegate Task<ICommunicationMessage> SendMessageAndWaitForResponse(
        EndpointId endpoint, 
        ICommunicationMessage message, 
        int maximumNumberOfRetries,
        TimeSpan timeout);
}