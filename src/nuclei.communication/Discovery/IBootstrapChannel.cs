//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the interface for a channel that allows discovery bootstrapping.
    /// </summary>
    internal interface IBootstrapChannel
    {
        /// <summary>
        /// Opens the channel.
        /// </summary>
        void OpenChannel();

        /// <summary>
        /// Closes the channel.
        /// </summary>
        void CloseChannel();
    }
}
