//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// A data structure that indicates that a certain action has failed.
    /// </summary>
    [DataContract]
    internal sealed class FailureData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets a value indicating why the action failed.
        /// </summary>
        [DataMember]
        public string FailureReason
        {
            get;
            set;
        }
    }
}
