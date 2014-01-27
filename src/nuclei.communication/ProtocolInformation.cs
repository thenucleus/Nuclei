//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Nuclei.Communication
{
    /// <summary>
    /// Stores information regarding the protocol channels for a given endpoint.
    /// </summary>
    internal sealed class ProtocolInformation
    {
        /// <summary>
        /// The version of the protocol for the current channel.
        /// </summary>
        private readonly Version m_Version;

        /// <summary>
        /// The address of the message channel for the given endpoint.
        /// </summary>
        private readonly Uri m_MessageAddress;

        /// <summary>
        /// The address of the data channel for the given endpoint.
        /// </summary>
        private readonly Uri m_DataAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolInformation"/> class.
        /// </summary>
        /// <param name="version">The version of the protocol for the given endpoint.</param>
        /// <param name="messageAddress">The address of the message channel for the given endpoint.</param>
        /// <param name="dataAddress">The address of the data channel for the given endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="version"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="messageAddress"/> is <see langword="null" />.
        /// </exception>
        public ProtocolInformation(Version version, Uri messageAddress, Uri dataAddress = null)
        {
            {
                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => messageAddress);
            }

            m_Version = version;
            m_MessageAddress = messageAddress;
            m_DataAddress = dataAddress;
        }

        /// <summary>
        /// Gets the version of the information object.
        /// </summary>
        public Version Version
        {
            [DebuggerStepThrough]
            get
            {
                return m_Version;
            }
        }

        /// <summary>
        ///  Gets a value indicating the message URI of the channel.
        /// </summary>
        public Uri MessageAddress
        {
            [DebuggerStepThrough]
            get
            {
                return m_MessageAddress;
            }
        }

        /// <summary>
        /// Gets a value indicating the data URI of the channel.
        /// </summary>
        public Uri DataAddress
        {
            [DebuggerStepThrough]
            get
            {
                return m_DataAddress;
            }
        }
    }
}
