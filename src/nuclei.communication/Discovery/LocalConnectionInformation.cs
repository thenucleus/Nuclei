//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Stores information about the connection information for the local endpoint.
    /// </summary>
    internal sealed class LocalConnectionInformation : IProvideLocalConnectionInformation
    {
        /// <summary>
        /// The ID of the local endpoint.
        /// </summary>
        private readonly EndpointId m_Id;

        /// <summary>
        /// The URI of the local entry channel.
        /// </summary>
        private readonly Uri m_EntryChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalConnectionInformation"/> class.
        /// </summary>
        /// <param name="id">The ID of the local endpoint.</param>
        /// <param name="entryChannel">The URI of the local entry channel.</param>
        public LocalConnectionInformation(EndpointId id, Uri entryChannel)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => entryChannel);
            }

            m_Id = id;
            m_EntryChannel = entryChannel;
        }

        /// <summary>
        /// Gets the ID of the local endpoint.
        /// </summary>
        public EndpointId Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets the URI of the local entry channel.
        /// </summary>
        public Uri EntryChannel
        {
            get
            {
                return m_EntryChannel;
            }
        }
    }
}
