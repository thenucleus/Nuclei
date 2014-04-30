//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
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
        /// Calculates a value from two inputs.
        /// </summary>
        /// <param name="first">The first input.</param>
        /// <param name="second">The second input.</param>
        /// <returns>A task that returns when the calculation has been completed.</returns>
        Task<int> Calculate(int first, int second);

        /// <summary>
        /// Starts a download.
        /// </summary>
        /// <param name="token">The upload token that allows the receiver to indicate which data stream should be downloaded.</param>
        /// <returns>A task that returns when the download has been started.</returns>
        Task StartDownload(UploadToken token);
    }
}
