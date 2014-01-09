//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that direct incoming communication messages.
    /// </summary>
    internal interface IDirectIncomingMessages
    {
        /// <summary>
        /// On arrival of a message which responds to the message with the
        /// <paramref name="inResponseTo"/> ID number the caller will be
        /// able to get the message through the <see cref="Task{T}"/> object.
        /// </summary>
        /// <param name="messageReceiver">The ID of the endpoint to which the original message was send.</param>
        /// <param name="inResponseTo">The ID number of the message for which a response is expected.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> implementation which returns the response message.
        /// </returns>
        Task<ICommunicationMessage> ForwardResponse(EndpointId messageReceiver, MessageId inResponseTo);

        /// <summary>
        /// On arrival of a message which passes the given filter the caller
        /// will be notified though the given delegate.
        /// </summary>
        /// <param name="messageFilter">The message filter.</param>
        /// <param name="notifyAction">The action invoked when a matching message arrives.</param>
        void ActOnArrival(IMessageFilter messageFilter, IMessageProcessAction notifyAction);
    }
}
