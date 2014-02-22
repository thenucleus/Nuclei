//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
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
        /// <param name="allowAutomaticChannelDiscovery">
        /// A flag that indicates whether or not the channel should provide automatic channel discovery.
        /// </param>
        void OpenChannel(bool allowAutomaticChannelDiscovery);

        /// <summary>
        /// Closes the channel.
        /// </summary>
        void CloseChannel();
    }
}
