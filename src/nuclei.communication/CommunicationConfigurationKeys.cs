//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Configuration;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the <see cref="ConfigurationKey"/> objects for the communication layers.
    /// </summary>
    public static class CommunicationConfigurationKeys
    {
        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP port (int).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey TcpPort 
            = new ConfigurationKey("TcpPort", typeof(int));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP base address (string).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey TcpBaseAddress 
            = new ConfigurationKey("TcpBaseAddress", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP discovery sub-address (string).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey TcpDiscoveryPath
            = new ConfigurationKey("TcpDiscoveryPath", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the TCP protocol sub-address (string).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey TcpProtocolPath 
            = new ConfigurationKey("TcpSubAddress", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the named pipe discovery sub-address (string).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey NamedPipeDiscoveryPath
            = new ConfigurationKey("NamedPipeDiscoveryPath", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the named pipe protocol sub-address (string).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey NamedPipeProtocolPath 
            = new ConfigurationKey("NamedPipeSubAddress", typeof(string));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the maximum
        /// number of connections for the WCF binding (int).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey BindingMaximumNumberOfConnections 
            = new ConfigurationKey("BindingMaximumNumberOfConnections", typeof(int));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the receive timeout (in milliseconds)
        /// for the WCF binding (int).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey BindingReceiveTimeoutInMilliseconds 
            = new ConfigurationKey("BindingReceiveTimeout", typeof(int));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the maximum buffer size (in bytes)
        /// for the WCF binding, used when processing messages (int).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey BindingMaxBufferSizeForMessagesInBytes
            = new ConfigurationKey("BindingMaxBufferSizeForMessagesInBytes", typeof(int));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the maximum message size (in bytes)
        /// for the WCF binding, used when processing messages (long).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey BindingMaxReceivedSizeForMessagesInBytes
            = new ConfigurationKey("BindingMaxReceivedSizeForMessagesInBytes", typeof(long));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the maximum buffer size (in bytes)
        /// for the WCF binding, used when processing streamed data (int).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey BindingMaxBufferSizeForDataInBytes
            = new ConfigurationKey("BindingMaxBufferSizeForDataInBytes", typeof(int));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the maximum message size (in bytes)
        /// for the WCF binding, used when processing streamed data (long).
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey BindingMaxReceivedSizeForDataInBytes
            = new ConfigurationKey("BindingMaxReceivedSizeForDataInBytes", typeof(long));

        /// <summary>
        /// The <see cref="ConfigurationKey"/> that is used to retrieve the value for the receive timeout (in milliseconds)
        /// for an endpoint to sign in before discarding messages to that endpoint.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes",
            Justification = "ConfigurationKey is immutable")]
        public static readonly ConfigurationKey WaitForConnectionTimeoutInMilliseconds
            = new ConfigurationKey("ConnectionTimeout", typeof(int));

        /// <summary>
        /// Returns a collection containing all the configuration keys for the communication section.
        /// </summary>
        /// <returns>A collection containing all the configuration keys for the communication section.</returns>
        public static IEnumerable<ConfigurationKey> ToCollection()
        {
            return new List<ConfigurationKey>
                {
                    TcpPort,
                    TcpBaseAddress,
                    TcpDiscoveryPath,
                    TcpProtocolPath,
                    NamedPipeDiscoveryPath,
                    NamedPipeProtocolPath,
                    BindingMaximumNumberOfConnections,
                    BindingReceiveTimeoutInMilliseconds,
                    BindingMaxBufferSizeForMessagesInBytes,
                    BindingMaxReceivedSizeForMessagesInBytes,
                    BindingMaxBufferSizeForDataInBytes,
                    BindingMaxReceivedSizeForDataInBytes,
                    WaitForConnectionTimeoutInMilliseconds,
                };
        }
    }
}
