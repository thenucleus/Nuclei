//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines an ID number for messages.
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class MessageId : Id<MessageId, Guid>
    {
        /// <summary>
        /// The GUID that indicates that the message ID is the 'none' ID.
        /// </summary>
        private static readonly Guid s_NoneId = new Guid("{D04D3867-DC14-437E-9789-287207DEDF41}");

        /// <summary>
        /// Gets a value indicating the None ID.
        /// </summary>
        public static MessageId None
        {
            get 
            {
                return new MessageId(s_NoneId);
            }
        }

        /// <summary>
        /// Returns a new Guid.
        /// </summary>
        /// <returns>A new Guid.</returns>
        private static Guid Next()
        {
            return Guid.NewGuid();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageId"/> class.
        /// </summary>
        public MessageId()
            : this(Next())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageId"/> class.
        /// </summary>
        /// <param name="id">The Guid for the ID.</param>
        private MessageId(Guid id)
            : base(id)
        { 
        }
        
        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override MessageId Clone(Guid value)
        {
            return new MessageId(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "MessageId: [{0}]",
                InternalValue);
        }
    }
}
