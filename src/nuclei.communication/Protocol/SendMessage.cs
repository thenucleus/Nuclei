//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the signature for functions that send messages without waiting for a response.
    /// </summary>
    /// <param name="endpoint">The endpoint to which the message is send.</param>
    /// <param name="message">The message.</param>
    /// <param name="maximumNumberOfRetries">
    /// The maximum number of times the endpoint will try to send the message if delivery fails. 
    /// </param>
    internal delegate void SendMessage(EndpointId endpoint, ICommunicationMessage message, int maximumNumberOfRetries);
}
