//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication
{
    /// <summary>
    /// An exception thrown the user requests a given notification from an endpoint that does
    /// not support the given command.
    /// </summary>
    [Serializable]
    public sealed class NotificationNotSupportedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationNotSupportedException"/> class.
        /// </summary>
        public NotificationNotSupportedException()
            : this(Resources.Exceptions_Messages_NotificationNotSupported)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationNotSupportedException"/> class.
        /// </summary>
        /// <param name="commandType">The type of the command that was requested.</param>
        public NotificationNotSupportedException(Type commandType)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_NotificationNotSupported_WithNotification, commandType))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NotificationNotSupportedException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationNotSupportedException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotificationNotSupportedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationNotSupportedException"/> class.
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
        private NotificationNotSupportedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
