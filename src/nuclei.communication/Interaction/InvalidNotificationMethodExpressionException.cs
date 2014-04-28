//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// An exception thrown when the mapping expression in the <c>From</c> method on the 
    /// <see cref="NotificationMapper{TNotification}"/> is not a valid event registration expression.
    /// </summary>
    [Serializable]
    public sealed class InvalidNotificationMethodExpressionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNotificationMethodExpressionException"/> class.
        /// </summary>
        public InvalidNotificationMethodExpressionException()
            : this(Resources.Exceptions_Messages_InvalidNotificationMethodExpression)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNotificationMethodExpressionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidNotificationMethodExpressionException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNotificationMethodExpressionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidNotificationMethodExpressionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidNotificationMethodExpressionException"/> class.
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
        private InvalidNotificationMethodExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
