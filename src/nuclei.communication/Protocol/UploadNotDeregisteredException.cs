//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// An exception thrown when a user tries to re-register a file for uploading if it 
    /// hasn't been unregistered.
    /// </summary>
    [Serializable]
    public sealed class UploadNotDeregisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadNotDeregisteredException"/> class.
        /// </summary>
        public UploadNotDeregisteredException()
            : this(Resources.Exceptions_Messages_UploadNotDeregistered)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadNotDeregisteredException"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        internal UploadNotDeregisteredException(UploadToken token)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_UploadNotDeregistered_WithToken, token))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadNotDeregisteredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public UploadNotDeregisteredException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadNotDeregisteredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public UploadNotDeregisteredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadNotDeregisteredException"/> class.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized
        ///     object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is null.
        /// </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">
        /// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
        /// </exception>
        private UploadNotDeregisteredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
