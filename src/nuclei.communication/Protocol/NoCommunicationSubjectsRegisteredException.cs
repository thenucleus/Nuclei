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
    /// An exception thrown when no communication subjects have been registered, potentially resulting in
    /// connection rejection from all remote endpoints.
    /// </summary>
    [Serializable]
    public sealed class NoCommunicationSubjectsRegisteredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoCommunicationSubjectsRegisteredException"/> class.
        /// </summary>
        public NoCommunicationSubjectsRegisteredException()
            : this(Resources.Exceptions_Messages_NoCommunicationSubjectsRegistered)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoCommunicationSubjectsRegisteredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NoCommunicationSubjectsRegisteredException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoCommunicationSubjectsRegisteredException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NoCommunicationSubjectsRegisteredException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoCommunicationSubjectsRegisteredException"/> class.
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
        private NoCommunicationSubjectsRegisteredException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
