//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    /// <summary>
    /// Defines a message that contains the return value for an <see cref="ICommandSet"/> method invocation.
    /// </summary>
    internal sealed class CommandInvokedResponseMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedResponseMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <param name="result">The result from the <see cref="ICommandSet"/> method that was invoked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="result"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedResponseMessage(EndpointId origin, MessageId inResponseTo, object result)
            : base(origin, inResponseTo)
        {
            {
                Lokad.Enforce.Argument(() => result);
            }

            Result = result;
        }

        /// <summary>
        /// Gets the result from the <see cref="ICommandSet"/> method invocation.
        /// </summary>
        public object Result 
        { 
            get; 
            private set; 
        }
    }
}
