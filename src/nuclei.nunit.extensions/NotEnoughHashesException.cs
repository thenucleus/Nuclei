//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.Serialization;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// An exception thrown if not enough hashcodes are provided to the <see cref="HashcodeContractVerifier"/>.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotEnoughHashesException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
        /// </summary>
        public NotEnoughHashesException()
            : this("Not enough hashes.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public NotEnoughHashesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
        /// </summary>
        /// <param name="expected">The expected number of hashes.</param>
        /// <param name="actual">The actual number of hashes.</param>
        public NotEnoughHashesException(int expected, int actual)
            : this(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Expected at least {0} hashes. Got {1} hashes.",
                    expected,
                    actual))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public NotEnoughHashesException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotEnoughHashesException"/> class.
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
        private NotEnoughHashesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
