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
            var receiver = new Mock<IInformationEndpoint>();
            {
                receiver.Setup(r => r.ProtocolVersions())
                    .Returns(new[] { new Version(2, 0), })
                    .Verifiable();
            }

            var host = new ServiceHost(receiver.Object, uri);
            var binding = new NetNamedPipeBinding();
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

            receiver.Verify(r => r.ProtocolVersions(), Times.Once());
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
            var receiver = new Mock<IInformationEndpoint>();
            {
                receiver.Setup(r => r.ProtocolVersions())
                    .Throws(new ArgumentException())
                    .Verifiable();
            }

            var host = new ServiceHost(receiver.Object, uri);
            var binding = new NetNamedPipeBinding();
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

            receiver.Verify(r => r.ProtocolVersions(), Times.Once());
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
            var receiver = new Mock<IInformationEndpoint>();
            {
                receiver.Setup(r => r.ProtocolVersions())
                    .Returns(new[] { new Version(1, 0), })
                    .Verifiable();
                receiver.Setup(r => r.ConnectionInformationForProtocol(It.IsAny<Version>()))
                    .Returns(info)
                    .Verifiable();
            }

            var uri = new Uri("net.pipe://localhost/pipe/discovery");
            var host = new ServiceHost(receiver.Object, uri);
            var binding = new NetNamedPipeBinding();
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IInformationEndpoint), binding, address);

            host.Open();
            try
            {
                var receivedInfo = translator.FromUri(endpoint.ListenUri);
                Assert.IsNotNull(receivedInfo);
                Assert.AreEqual(info.ProtocolVersion, receivedInfo.Version);
                Assert.AreEqual(info.ProtocolVersion, receivedInfo.MessageAddress);
                Assert.IsNull(receivedInfo.DataAddress);
            }
            finally
            {
                host.Close();
            }

            receiver.Verify(r => r.ProtocolVersions(), Times.Once());
        }
    }
}
