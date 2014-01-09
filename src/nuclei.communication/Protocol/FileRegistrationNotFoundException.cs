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
    /// An exception thrown if a user tries to deregister an upload for a token that is not registered.
    /// </summary>
    [Serializable]
    public sealed class FileRegistrationNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRegistrationNotFoundException"/> class.
        /// </summary>
        public FileRegistrationNotFoundException()
            : this(Resources.Exceptions_Messages_FileRegistrationNotFound)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRegistrationNotFoundException"/> class.
        /// </summary>
        /// <param name="token">The token.</param>
        internal FileRegistrationNotFoundException(UploadToken token)
            : this(string.Format(CultureInfo.InvariantCulture, Resources.Exceptions_Messages_FileRegistrationNotFound_WithToken, token))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRegistrationNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FileRegistrationNotFoundException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRegistrationNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public FileRegistrationNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRegistrationNotFoundException"/> class.
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
        private FileRegistrationNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
