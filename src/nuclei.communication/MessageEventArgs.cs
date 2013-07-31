//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// An <see cref="EventArgs"/> object that carries a message.
    /// </summary>
    internal sealed class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageEventArgs(ICommunicationMessage message)
        {
            {
                Lokad.Enforce.Argument(() => message);
            }

            Message = message;
        }

        /// <summary>
        /// Gets a value indicating the message.
        /// </summary>
        public ICommunicationMessage Message
        {
            get;
            private set;
        }
    }
}
