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
    /// Defines the interface for objects that direct incoming data streams.
    /// </summary>
    internal interface IDirectIncomingData
    {
        /// <summary>
        /// On arrival of data from the given <paramref name="messageReceiver"/> the caller will be
        /// able to get the data stream from disk through the <see cref="Task{T}"/> object.
        /// </summary>
        /// <param name="messageReceiver">The ID of the endpoint to which the original message was send.</param>
        /// <param name="filePath">The full path to the file to which the data stream should be written.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> implementation which returns the full path of the file which contains the data stream.
        /// </returns>
        Task<FileInfo> ForwardData(EndpointId messageReceiver, string filePath);
    }
}
