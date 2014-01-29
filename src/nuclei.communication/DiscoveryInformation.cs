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
    /// Stores information regarding the discovery channels for a given endpoint.
    /// </summary>
    internal sealed class DiscoveryInformation
    {
        /// <summary>
        /// The address of the discovery entry channel for the remote endpoint.
        /// </summary>
        private readonly Uri m_Address;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoveryInformation"/> class.
        /// </summary>
        /// <param name="address">The address of the discovery entry channel for the remote endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="address"/> is <see langword="null" />.
        /// </exception>
        public DiscoveryInformation(Uri address)
        {
            {
                Lokad.Enforce.Argument(() => address);
            }

            m_Address = address;
        }

        /// <summary>
        /// Gets the address of the discovery channel for the given endpoint.
        /// </summary>
        public Uri Address
        {
            [DebuggerStepThrough]
            get
            {
                return m_Address;
            }
        }
    }
}
