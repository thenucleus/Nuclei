//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Nuclei.Communication
{
    /// <summary>
    /// The globally unique ID for a communication endpoint.
    /// </summary>
    [Serializable]
    [DataContract]
    public sealed class EndpointId : Id<EndpointId, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointId"/> class.
        /// </summary>
        /// <param name="id">The ID text.</param>
        public EndpointId(string id)
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
        protected override EndpointId Clone(string value)
        {
            return new EndpointId(value);
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
