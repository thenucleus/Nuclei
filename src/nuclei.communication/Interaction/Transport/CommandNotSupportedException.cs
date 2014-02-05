//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// An exception thrown the user requests a given command from an endpoint that does
    /// not support the given command.
    /// </summary>
    [Serializable]
    public sealed class CommandNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotSupportedException"/> class.
        /// </summary>
        public CommandNotSupportedException()
            : this(Resources.Exceptions_Messages_CommandNotSupported)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotSupportedException"/> class.
        /// </summary>
        /// <param name="commandType">The type of the command that was requested.</param>
        public CommandNotSupportedException(Type commandType)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_CommandNotSupported_WithCommand, commandType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CommandNotSupportedException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CommandNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandNotSupportedException"/> class.
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
        private CommandNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
