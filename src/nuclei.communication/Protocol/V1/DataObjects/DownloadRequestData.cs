//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// Defines a message that indicates that the sending endpoint requests the
    /// current endpoint to download a specific data stream.
    /// </summary>
    [DataContract]
    internal sealed class DownloadRequestData : DataObjectBase
    {
        /// <summary>
        /// Gets or sets the token that indicates which file should be uploaded.
        /// </summary>
        [DataMember]
        public UploadToken Token
        {
            get;
            set;
        }
    }
}
