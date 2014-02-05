//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using Nuclei.Configuration;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines a <see cref="IChannelTemplate"/> that uses named pipe connections for communication between applications
    /// on the same machine.
    /// </summary>
    internal abstract class NamedPipeChannelTemplate : IChannelTemplate
    {
        /// <summary>
        /// Returns the process ID of the process that is currently executing
        /// this code.
        /// </summary>
        /// <returns>
        /// The ID number of the current process.
        /// </returns>
        protected static int CurrentProcessId()
        {
            var process = Process.GetCurrentProcess();
            return process.Id;
        }

        /// <summary>
        /// The object that stores the configuration values for the
        /// named pipe WCF connection.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipeChannelTemplate"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        protected NamedPipeChannelTemplate(IConfiguration tcpConfiguration)
        {
            {
                Lokad.Enforce.Argument(() => tcpConfiguration);
            }

            m_Configuration = tcpConfiguration;
        }

        /// <summary>
        /// Gets the configuration object.
        /// </summary>
        protected IConfiguration Configuration
        {
            get
            {
                return m_Configuration;
            }
        }

        /// <summary>
        /// Gets the type of the channel.
        /// </summary>
        public ChannelTemplate ChannelTemplate
        {
            get
            {
                return ChannelTemplate.NamedPipe;
            }
        }

        /// <summary>
        /// Generates a new URI for the channel.
        /// </summary>
        /// <returns>
        /// The newly generated URI.
        /// </returns>
        public Uri GenerateNewChannelUri()
        {
            var channelUri = string.Format(
                CultureInfo.InvariantCulture,
                CommunicationConstants.DefaultNamedPipeChannelUriTemplate,
                CurrentProcessId());
            return new Uri(channelUri);
        }
    }
}
