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
    /// An exception thrown when the mapping expression in either one of the <c>From</c> methods on the <see cref="CommandMapper{TCommand}"/>
    /// or one of the <c>To</c> methods on the <see cref="MethodMapper"/> is not a valid method expression.
    /// </summary>
    [Serializable]
    public sealed class InvalidCommandMethodExpressionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandMethodExpressionException"/> class.
        /// </summary>
        public InvalidCommandMethodExpressionException()
            : this(Resources.Exceptions_Messages_InvalidCommandMethodExpression)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandMethodExpressionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public InvalidCommandMethodExpressionException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandMethodExpressionException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidCommandMethodExpressionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandMethodExpressionException"/> class.
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
        private InvalidCommandMethodExpressionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
