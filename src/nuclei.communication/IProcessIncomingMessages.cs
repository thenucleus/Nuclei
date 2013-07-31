//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication
{
    /// <summary>
    /// Processes incoming messages and invokes the required functions based on the 
    /// contents of the message.
    /// </summary>
    internal interface IProcessIncomingMessages
    {
        /// <summary>
        /// Processes the message and invokes the desired functions based on the 
        /// message contents or type.
        /// </summary>
        /// <param name="message">The message that should be processed.</param>
        void ProcessMessage(ICommunicationMessage message);

        /// <summary>
        /// Handles the case that the local channel, from which the input messages are send,
        /// is closed.
        /// </summary>
        void OnLocalChannelClosed();
    }
}
