//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Protocol.Messages
{
    /// <summary>
    /// An exception thrown when an unknown communication message is encountered.
    /// </summary>
    [Serializable]
    public sealed class UnknownMessageTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMessageTypeException"/> class.
        /// </summary>
        public UnknownMessageTypeException()
            : this(Resources.Exceptions_Messages_UnknownMessageType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMessageTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UnknownMessageTypeException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMessageTypeException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UnknownMessageTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownMessageTypeException"/> class.
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
        private UnknownMessageTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
