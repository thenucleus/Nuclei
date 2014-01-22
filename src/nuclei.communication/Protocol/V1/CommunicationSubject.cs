//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Defines a 'subject' for which the communication system is used to communicate about.
    /// </summary>
    [Serializable]
    public sealed class CommunicationSubject : Id<CommunicationSubject, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSubject"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public CommunicationSubject(string value) 
            : base(value)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override CommunicationSubject Clone(string value)
        {
            return new CommunicationSubject(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}
