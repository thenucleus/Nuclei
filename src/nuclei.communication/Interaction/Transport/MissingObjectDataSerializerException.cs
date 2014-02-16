//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// An exception thrown when the endpoint tries to process or create a serialized data object without the
    /// correct <see cref="ISerializeObjectData"/> implementations.
    /// </summary>
    [Serializable]
    public sealed class MissingObjectDataSerializerException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectDataSerializerException"/> class.
        /// </summary>
        public MissingObjectDataSerializerException()
            : this(Resources.Exceptions_Messages_MissingObjectDataSerializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectDataSerializerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MissingObjectDataSerializerException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectDataSerializerException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public MissingObjectDataSerializerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MissingObjectDataSerializerException"/> class.
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
        private MissingObjectDataSerializerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
