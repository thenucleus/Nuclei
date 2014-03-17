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
    /// Defines a message that indicates that the an <see cref="ICommandSet"/> method was
    /// invoked.
    /// </summary>
    internal sealed class CommandInvokedMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="methodInvocation">The information about the <see cref="ICommandSet"/> method that was invoked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="methodInvocation"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedMessage(EndpointId origin, CommandInvokedData methodInvocation)
            : this(origin, new MessageId(), methodInvocation)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="id">The ID of the current message.</param>
        /// <param name="methodInvocation">The information about the <see cref="ICommandSet"/> method that was invoked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="methodInvocation"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedMessage(EndpointId origin, MessageId id, CommandInvokedData methodInvocation)
            : base(origin, id)
        {
            {
                Lokad.Enforce.Argument(() => methodInvocation);
            }

            Invocation = methodInvocation;
        }

        /// <summary>
        /// Gets information about the <see cref="ICommandSet"/> method that was invoked.
        /// </summary>
        public CommandInvokedData Invocation
        {
            get;
            private set;
        }
    }
}
