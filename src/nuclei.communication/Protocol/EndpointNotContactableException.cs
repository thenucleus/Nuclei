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
    /// An exception thrown when the user tries to send a message to an endpoint for which 
    /// no contact information is available.
    /// </summary>
    [Serializable]
    public sealed class EndpointNotContactableException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotContactableException"/> class.
        /// </summary>
        public EndpointNotContactableException()
            : this(Resources.Exceptions_Messages_EndpointNotContactable)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotContactableException"/> class.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        public EndpointNotContactableException(EndpointId endpoint)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_EndpointNotContactable_WithEndpoint, endpoint))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotContactableException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public EndpointNotContactableException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotContactableException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public EndpointNotContactableException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotContactableException"/> class.
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
        private EndpointNotContactableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
