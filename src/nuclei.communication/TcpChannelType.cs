//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using Nuclei.Configuration;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines a <see cref="IChannelType"/> that uses TCP/IP connections for communication between applications
    /// on different machines.
    /// </summary>
    internal abstract class TcpChannelType : IChannelType
    {
        /// <summary>
        /// Returns the DNS name of the machine.
        /// </summary>
        /// <returns>The DNS name of the machine.</returns>
        private static string MachineDnsName()
        {
            try
            {
                var searcher = new ManagementObjectSearcher(
                    "root\\CIMV2",
                    "SELECT * FROM Win32_NetworkAdapterConfiguration WHERE ServiceName != 'tunnel' AND DNSHostName != null");

                string dnsHostName = (from ManagementObject queryObj in searcher.Get()
                                      select queryObj["DNSHostName"] as string).FirstOrDefault();

                return (!string.IsNullOrWhiteSpace(dnsHostName)) ? dnsHostName : Environment.MachineName;
            }
            catch (ManagementException)
            {
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Returns the next available TCP/IP port.
        /// </summary>
        /// <returns>
        /// The number of the port.
        /// </returns>
        private static int DetermineNextAvailablePort()
        {
            var endPoint = new IPEndPoint(IPAddress.Any, 0);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.Bind(endPoint);
                var local = (IPEndPoint)socket.LocalEndPoint;
                return local.Port;
            }
        }

        /// <summary>
        /// The object that stores the configuration values for the
        /// named pipe WCF connection.
        /// </summary>
        private readonly IConfiguration m_Configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TcpChannelType"/> class.
        /// </summary>
        /// <param name="tcpConfiguration">The configuration for the WCF tcp channel.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="tcpConfiguration"/> is <see langword="null" />.
        /// </exception>
        protected TcpChannelType(IConfiguration tcpConfiguration)
        {
            {
                Lokad.Enforce.Argument(() => tcpConfiguration);
            }

            m_Configuration = tcpConfiguration;
        }

        /// <summary>
        /// 
        /// </summary>
        protected IConfiguration Configuration
        {
            get
            {
                return m_Configuration;
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
            int port = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpPort) ?
                m_Configuration.Value<int>(CommunicationConfigurationKeys.TcpPort) :
                DetermineNextAvailablePort();
            string address = m_Configuration.HasValueFor(CommunicationConfigurationKeys.TcpBaseAddress) ?
                m_Configuration.Value<string>(CommunicationConfigurationKeys.TcpBaseAddress) :
                MachineDnsName();

            var channelUri = string.Format(CultureInfo.InvariantCulture, CommunicationConstants.DefaultTcpIpChannelUriTemplate, address, port);
            return new Uri(channelUri);
        }
    }
}
