//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Messages
{
    /// <summary>
    /// Defines a message that indicates that a certain action has failed.
    /// </summary>
    [Serializable]
    internal sealed class FailureMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMessage"/> class.
        /// </summary>
        /// <param name="endpoint">The endpoint which send the current message.</param>
        /// <param name="inResponseTo">The ID number of the message to which the current message is a response.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inResponseTo"/> is <see langword="null" />.
        /// </exception>
        public FailureMessage(EndpointId endpoint, MessageId inResponseTo)
            : base(endpoint, inResponseTo)
        {
        }
    }
}
