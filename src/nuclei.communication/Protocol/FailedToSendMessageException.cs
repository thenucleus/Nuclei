//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// An exception thrown when the WCF channel fails to send a message due to a communication problem.
    /// </summary>
    [Serializable]
    public sealed class FailedToSendMessageException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToSendMessageException"/> class.
        /// </summary>
        public FailedToSendMessageException()
            : this(Resources.Exceptions_Messages_FailedToSendMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToSendMessageException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FailedToSendMessageException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToSendMessageException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FailedToSendMessageException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedToSendMessageException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information
        ///     about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private FailedToSendMessageException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
