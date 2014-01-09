//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Stores a collection of file paths for files that should be
    /// uploaded.
    /// </summary>
    internal sealed class WaitingUploads : IStoreUploads
    {
        /// <summary>
        /// The collection that maps tokens to file paths.
        /// </summary>
        private readonly IDictionary<UploadToken, string> m_Uploads
            = new ConcurrentDictionary<UploadToken, string>();

        /// <summary>
        /// Registers a new file path for uploading
        /// and returns a new token for use with the path.
        /// </summary>
        /// <param name="path">The full path to the file that should be uploaded.</param>
        /// <returns>
        ///     The token that can be used to retrieve the file path.
        /// </returns>
        public UploadToken Register(string path)
        {
            {
                Lokad.Enforce.Argument(() => path);
                Lokad.Enforce.Argument(() => path, Lokad.Rules.StringIs.NotEmpty);
            }

            var token = new UploadToken();
            m_Uploads.Add(token, path);

            return token;
        }

        /// <summary>
        /// Reregisters a file path for uploading with a given path.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="path">The full path to the file that should be uploaded.</param>
        public void Reregister(UploadToken token, string path)
        {
            {
                Lokad.Enforce.Argument(() => path);
                Lokad.Enforce.Argument(() => path, Lokad.Rules.StringIs.NotEmpty);
            }

            if (m_Uploads.ContainsKey(token))
            {
                throw new UploadNotDeregisteredException(token);
            }

            m_Uploads.Add(token, path);
        }

        /// <summary>
        /// Deregisters the file from upload and returns the path.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The file path that was registered with the given token.</returns>
        public string Deregister(UploadToken token)
        {
            if (!m_Uploads.ContainsKey(token))
            {
                throw new FileRegistrationNotFoundException(token);
            }

            var result = m_Uploads[token];
            m_Uploads.Remove(token);

            return result;
        }

        /// <summary>
        /// Determines if a path is stored for the given token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     <see langword="true" /> if a path is stored for the given token;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        public bool HasRegistration(UploadToken token)
        {
            return m_Uploads.ContainsKey(token);
        }
    }
}
