//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Defines methods that transform <see cref="ChannelInformation"/> to <see cref="VersionedChannelInformation"/>
    /// for the V1.0 discovery protocol.
    /// </summary>
    internal static class ChannelInformationToTransportConverter
    {
        /// <summary>
        /// Translates the current version of the <see cref="ChannelInformation"/> to the <see cref="VersionedChannelInformation"/>.
        /// </summary>
        /// <param name="info">The channel information describing the available protocol channels.</param>
        /// <returns>A object describing the versioned channel information.</returns>
        public static VersionedChannelInformation ToVersioned(ChannelInformation info)
        {
            return new VersionedChannelInformation
            {
                Id = info.Id,
                ProtocolVersion = info.ProtocolVersion, 
                Address = info.Address
            };
        }

        /// <summary>
        /// Translates the <see cref="VersionedChannelInformation"/> to the current version of <see cref="ChannelInformation"/>.
        /// </summary>
        /// <param name="info">The object describing the versioned channel information.</param>
        /// <returns>The channel information describing the available protocol levels.</returns>
        public static ChannelInformation FromVersioned(VersionedChannelInformation info)
        {
            return new ChannelInformation(info.Id, info.ProtocolVersion, info.Address);
        }
    }
}
