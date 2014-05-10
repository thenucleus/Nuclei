//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Defines the signature for functions that send command data to the owning remote endpoint.
    /// </summary>
    /// <param name="commandData">The command data.</param>
    /// <param name="maximumNumberOfRetries">
    /// The maximum number of times the endpoint will try to send the message if delivery fails. 
    /// </param>
    /// <param name="timeout">The maximum amount of time the response operation is allowed to take.</param>
    /// <returns>A task object that will eventually contain the command response data.</returns>
    internal delegate Task<ICommunicationMessage> SendCommandData(CommandInvokedData commandData, int maximumNumberOfRetries, TimeSpan timeout);
}