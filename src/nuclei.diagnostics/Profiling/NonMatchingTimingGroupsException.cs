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
    /// An exception thrown when the user tries to use two or more <see cref="ITimerInterval"/>
    /// objects with different <see cref="TimingGroup"/> instances.
    /// </summary>
    [Serializable]
    public sealed class NonMatchingTimingGroupsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingTimingGroupsException"/> class.
        /// </summary>
        public NonMatchingTimingGroupsException()
            : this(Resources.Exceptions_Messages_NonMatchingTimingGroups)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingTimingGroupsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NonMatchingTimingGroupsException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingTimingGroupsException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NonMatchingTimingGroupsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonMatchingTimingGroupsException"/> class.
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
        private NonMatchingTimingGroupsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
