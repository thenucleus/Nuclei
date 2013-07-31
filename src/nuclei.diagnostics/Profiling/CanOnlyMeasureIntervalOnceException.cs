//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using Nuclei.Diagnostics.Properties;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// An exception thrown when the <see cref="TimerInterval"/> is requested to 
    /// start the interval measurement more than once.
    /// </summary>
    [Serializable]
    public sealed class CanOnlyMeasureIntervalOnceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanOnlyMeasureIntervalOnceException"/> class.
        /// </summary>
        public CanOnlyMeasureIntervalOnceException()
            : this(Resources.Exceptions_Messages_CanOnlyMeasureIntervalOnce)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanOnlyMeasureIntervalOnceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public CanOnlyMeasureIntervalOnceException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanOnlyMeasureIntervalOnceException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CanOnlyMeasureIntervalOnceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CanOnlyMeasureIntervalOnceException"/> class.
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
        private CanOnlyMeasureIntervalOnceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
