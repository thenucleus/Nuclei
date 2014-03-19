//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using Moq;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Discovery.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DiscoveryChannelTranslatorTest
    {
        [ServiceBehavior(
            ConcurrencyMode = ConcurrencyMode.Multiple,
            InstanceContextMode = InstanceContextMode.Single)]
        private sealed class MockEndpoint : IInformationEndpoint
        {
            private Func<Version> m_Version;
            private Func<Version[]> m_ProtocolVersions;
            private Func<Version, VersionedChannelInformation> m_ConnectionInformationForProtocol;

            public MockEndpoint(
                Func<Version> version, 
                Func<Version[]> protocolVersions, 
                Func<Version, VersionedChannelInformation> connectionInformationForProtocol)
            {
                m_Version = version;
                m_ProtocolVersions = protocolVersions;
                m_ConnectionInformationForProtocol = connectionInformationForProtocol;
            }

            public Version Version()
            {
                return m_Version();
            }

            /// <summary>
            /// Returns an array containing all the versions of the supported communication protocols.
            /// </summary>
            /// <returns>An array containing the versions of the supported communication protocols.</returns>
            public Version[] ProtocolVersions()
            {
                return m_ProtocolVersions();
            }

            /// <summary>
            /// Returns the discovery information for the communication protocol with the given version.
            /// </summary>
            /// <param name="version">The version of the protocol for which the discovery information should be provided.</param>
            /// <returns>The discovery information for the communication protocol with the given version.</returns>
            public VersionedChannelInformation ConnectionInformationForProtocol(Version version)
            {
                return m_ConnectionInformationForProtocol(version);
            }
        }

        [Test]
        public void FromUriWithNonMatchingVersions()
        {
            var protocolVersions = new[]
                {
                    new Version(1, 0), 
                };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new NamedPipeDiscoveryChannelTemplate(configuration.Object);
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = t => template;

            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var translator = new DiscoveryChannelTranslator(
                protocolVersions,
                templateBuilder,
                diagnostics);

            var uri = new Uri("net.pipe://localhost/pipe/discovery");
            var receiver = new MockEndpoint(
                () => DiscoveryVersions.V1,
                () => new[] { new Version(2, 0), },
                null);

            var host = new ServiceHost(receiver, uri);
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
                    TransferMode = TransferMode.Buffered,
                };
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IInformationEndpoint), binding, address);

            host.Open();
            try
            {
                var info = translator.FromUri(endpoint.ListenUri);
                Assert.IsNull(info);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void FromUriWithCommunicationFault()
        {
            var protocolVersions = new[]
                {
                    new Version(1, 0), 
                };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new NamedPipeDiscoveryChannelTemplate(configuration.Object);
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = t => template;

            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var translator = new DiscoveryChannelTranslator(
                protocolVersions,
                templateBuilder,
                diagnostics);

            var uri = new Uri("net.pipe://localhost/pipe/discovery");
            var receiver = new MockEndpoint(
                () => DiscoveryVersions.V1,
                () =>
                {
                    throw new ArgumentException();
                },
                null);

            var host = new ServiceHost(receiver, uri);
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
                    TransferMode = TransferMode.Buffered,
                };
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IInformationEndpoint), binding, address);

            host.Open();
            try
            {
                var info = translator.FromUri(endpoint.ListenUri);
                Assert.IsNull(info);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void FromUri()
        {
            var protocolVersions = new[]
                {
                    new Version(1, 0), 
                };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new NamedPipeDiscoveryChannelTemplate(configuration.Object);
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = t => template;

            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var translator = new DiscoveryChannelTranslator(
                protocolVersions,
                templateBuilder,
                diagnostics);

            var info = new VersionedChannelInformation
                {
                    ProtocolVersion = new Version(1, 0),
                    Address = new Uri("http://localhost/protocol/invalid")
                };
            var receiver = new MockEndpoint(
                () => DiscoveryVersions.V1,
                () => new[] { new Version(1, 0), },
                v => info);

            var uri = new Uri("net.pipe://localhost/pipe/discovery");
            var host = new ServiceHost(receiver, uri);
            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
                    TransferMode = TransferMode.Buffered,
                };
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IInformationEndpoint), binding, address);

            host.Open();
            try
            {
                var receivedInfo = translator.FromUri(endpoint.ListenUri);
                Assert.IsNotNull(receivedInfo);
                Assert.AreEqual(info.ProtocolVersion, receivedInfo.Version);
                Assert.AreEqual(info.Address, receivedInfo.MessageAddress);
                Assert.IsNull(receivedInfo.DataAddress);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
