//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Threading.Tasks;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// A delegate used to download data from a remote endpoint. 
    /// </summary>
    /// <param name="endpointToDownloadFrom">The endpoint ID of the endpoint from which the data should be transferred.</param>
    /// <param name="uploadToken">The token that indicates which file should be uploaded.</param>
    /// <param name="filePath">The full path to the file to which the downloaded data should be written.</param>
    /// <returns>
    /// The task which will return the pointer to the file once the download is complete.
    /// </returns>
    public delegate Task<FileInfo> DownloadDataFromRemoteEndpoints(EndpointId endpointToDownloadFrom, UploadToken uploadToken, string filePath);
}
