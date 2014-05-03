//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for the self-resurrecting WCF data transfer channel.
    /// </summary>
    internal interface IDataTransferingEndpoint : IDisposable
    {
        /// <summary>
        /// Sends the given message.
        /// </summary>
        /// <param name="message">The message to be send.</param>
        /// <param name="maximumNumberOfRetries">The maximum number of times the endpoint will try to transfer the data if delivery fails.</param>
        void Send(DataTransferMessage message, int maximumNumberOfRetries);
    }
}
