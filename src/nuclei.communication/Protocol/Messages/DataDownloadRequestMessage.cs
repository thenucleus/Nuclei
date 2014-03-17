//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol.Messages
{
    /// <summary>
    /// Defines a message that requests the download of a specific file from the receiver.
    /// </summary>
    internal sealed class DataDownloadRequestMessage : CommunicationMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataDownloadRequestMessage"/> class.
        /// </summary>
        /// <param name="origin">The ID of the endpoint that send the message.</param>
        /// <param name="token">The token that indicates which file should be uploaded.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="token"/> is <see langword="null" />.
        /// </exception>
        public DataDownloadRequestMessage(EndpointId origin, UploadToken token)
            : this(origin, new MessageId(), token)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDownloadRequestMessage"/> class.
        /// </summary>
        /// <param name="origin">The ID of the endpoint that send the message.</param>
        /// <param name="id">The ID of the current message.</param>
        /// <param name="token">The token that indicates which file should be uploaded.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="token"/> is <see langword="null" />.
        /// </exception>
        public DataDownloadRequestMessage(EndpointId origin, MessageId id, UploadToken token)
            : base(origin, id)
        {
            {
                Lokad.Enforce.Argument(() => token);
            }

            Token = token;
        }

        /// <summary>
        /// Gets the token that indicates which file should be uploaded.
        /// </summary>
        public UploadToken Token
        {
            get;
            private set;
        }
    }
}
