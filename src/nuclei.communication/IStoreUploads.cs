//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that register files for uploading.
    /// </summary>
    public interface IStoreUploads
    {
        /// <summary>
        /// Registers a new file path for uploading
        /// and returns a new token for use with the path.
        /// </summary>
        /// <param name="path">The full path to the file that should be uploaded.</param>
        /// <returns>
        ///     The token that can be used to retrieve the file path.
        /// </returns>
        UploadToken Register(string path);

        /// <summary>
        /// Reregisters a file path for uploading with a given path.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="path">The full path to the file that should be uploaded.</param>
        void Reregister(UploadToken token, string path);

        /// <summary>
        /// Deregisters the file from upload and returns the path.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The file path that was registered with the given token.</returns>
        string Deregister(UploadToken token);

        /// <summary>
        /// Determines if a path is stored for the given token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        ///     <see langword="true" /> if a path is stored for the given token;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        bool HasRegistration(UploadToken token);
    }
}
