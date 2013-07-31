//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that pipe messages from one object to another object.
    /// </summary>
    internal interface IMessagePipe : IMessageReceivingEndpoint
    {
        /// <summary>
        /// An event raised when a new message is available in the pipe.
        /// </summary>
        event EventHandler<MessageEventArgs> OnNewMessage;
    }
}
