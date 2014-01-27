//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Defines methods that transform <see cref="EndpointInformation"/> to <see cref="VersionedChannelInformation"/>
    /// for the V1.0 discovery protocol.
    /// </summary>
    internal static class ChannelInformationToTransportConverter
    {
        /// <summary>
        /// Translates the current version of the <see cref="EndpointInformation"/> to the <see cref="VersionedChannelInformation"/>.
        /// </summary>
        /// <param name="info">The channel information describing the available protocol channel.</param>
        /// <returns>A object describing the versioned channel information.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="info"/> is <see langword="null" />.
        /// </exception>
        public static VersionedChannelInformation ToVersioned(ProtocolInformation info)
        {
            {
                Lokad.Enforce.Argument(() => info);
            }

            return new VersionedChannelInformation
            {
                ProtocolVersion = info.Version, 
                Address = info.MessageAddress
            };
        }

        /// <summary>
        /// Translates the <see cref="VersionedChannelInformation"/> to the current version of <see cref="EndpointInformation"/>.
        /// </summary>
        /// <param name="info">The object describing the versioned channel information.</param>
        /// <returns>A pair containing the ID of the endpoint and the protocol information for the endpoint.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="info"/> is <see langword="null" />.
        /// </exception>
        public static ProtocolInformation FromVersioned(VersionedChannelInformation info)
        {
            {
                Lokad.Enforce.Argument(() => info);
            }

            return new ProtocolInformation(info.ProtocolVersion, info.Address);
        }
    }
}
