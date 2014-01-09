//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Protocol.Messages
{
    /// <summary>
    /// Filters messages based on their <see cref="Type"/>.
    /// </summary>
    internal sealed class MessageKindFilter : IMessageFilter
    {
        /// <summary>
        /// The <see cref="Type"/> of the message which is allowed to pass through the filter.
        /// </summary>
        private readonly Type m_MessageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageKindFilter"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The <see cref="Type"/> of the message which is allowed to pass through the filter.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="messageType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="messageType"/> does not implement the <see cref="ICommunicationMessage"/> interface.
        /// </exception>
        public MessageKindFilter(Type messageType)
        {
            {
                Lokad.Enforce.Argument(() => messageType);
                Lokad.Enforce.With<ArgumentException>(
                    typeof(ICommunicationMessage).IsAssignableFrom(messageType),
                    Resources.Exceptions_Messages_MessageToFilterOnNeedsToImplementICommunicationMessage);
            }

            m_MessageType = messageType;
        }

        /// <summary>
        /// Returns a value indicating if the message passes through the filter or not.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <see langword="true" /> if the message passes through the filter; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
              Justification = "Documentation can start with a language keyword")]
        public bool PassThrough(ICommunicationMessage message)
        {
            return (message != null) && m_MessageType.IsAssignableFrom(message.GetType());
        }
    }
}
