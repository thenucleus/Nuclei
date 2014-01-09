//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Defines the interface for objects that provide a set of test commands.
    /// </summary>
    public interface ITestCommandSet : ICommandSet
    {
        /// <summary>
        /// Echo's the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A task that returns when the echo message has been send.</returns>
        Task Echo(string name);

        /// <summary>
        /// Starts a download.
        /// </summary>
        /// <param name="downloadOwningEndpoint">The endpoint ID of the endpoint that owns the data stream.</param>
        /// <param name="token">The upload token that allows the receiver to indicate which data stream should be downloaded.</param>
        /// <returns>A task that returns when the download has been started.</returns>
        Task StartDownload(EndpointId downloadOwningEndpoint, UploadToken token);
    }
}
