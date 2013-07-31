//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Messages
{
    internal static class CommunicationMessageExtensions
    {
        /// <summary>
        /// Indicates if the message is a handshake message that is used to determine if 
        /// two endpoints can communicate and how they will communicate.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// <see langword="true" /> if the message is a handshake message; otherwise, <see langword="false" />.
        /// </returns>
        public static bool IsHandshake(this ICommunicationMessage message)
        {
            return message is EndpointConnectMessage;
        }
    }
}
