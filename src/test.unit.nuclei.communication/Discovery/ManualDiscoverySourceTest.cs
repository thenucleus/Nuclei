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

namespace Nuclei.Communication.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ManualDiscoverySourceTest
    {
        [Test]
        public void RecentlyConnectedEndpointWithoutDiscoveryPermissions()
        {
            var translator = new Mock<ITranslateVersionedChannelInformation>();
                var translators = new[]
            {
                new Tuple<Version, ITranslateVersionedChannelInformation>(new Version(1, 0), translator.Object),
            };

            var template = new Mock<IDiscoveryChannelTemplate>();
            {
                template.Setup(t => t.GenerateBinding())
                    .Verifiable();
            }

            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = 
                t =>
                {
                    return template.Object;
                };
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var discovery = new ManualDiscoverySource(
                translators,
                templateBuilder,
                diagnostics);
            discovery.RecentlyConnectedEndpoint(new EndpointId("a"), new Uri("http://localhost/discovery/invalid"));
            template.Verify(t => t.GenerateBinding(), Times.Never());
        }

        [Test]
        public void RecentlyConnectedEndpointWithoutSuitableDiscoveryVersion()
        {
            var translator = new Mock<ITranslateVersionedChannelInformation>();
            var translators = new[]
            {
                new Tuple<Version, ITranslateVersionedChannelInformation>(new Version(1, 0), translator.Object),
            };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new NamedPipeDiscoveryChannelTemplate(configuration.Object);
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = t => template;
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var discovery = new ManualDiscoverySource(
                translators,
                templateBuilder,
                diagnostics);

            discovery.OnEndpointBecomingAvailable += (s, e) => Assert.Fail();
            discovery.StartDiscovery();

            var uri = new Uri("net.pipe://localhost/pipe/discovery");
            var channels = new[]
                {
                    new Tuple<Version, Uri>(new Version(2, 0), new Uri(uri, new Uri("/2.0", UriKind.Relative))), 
                };
            var receiver = new BootstrapEndpoint(channels);

            var host = new ServiceHost(receiver, uri);
            var binding = new NetNamedPipeBinding();
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IBootstrapEndpoint), binding, address);

            host.Open();
            try
            {
                discovery.RecentlyConnectedEndpoint(new EndpointId("a"), endpoint.ListenUri);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void RecentlyConnectedEndpointWithNoOutputProtocolInformation()
        {
            var translator = new Mock<ITranslateVersionedChannelInformation>();
            var translators = new[]
            {
                new Tuple<Version, ITranslateVersionedChannelInformation>(new Version(1, 0), translator.Object),
            };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new NamedPipeDiscoveryChannelTemplate(configuration.Object);
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = t => template;
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var discovery = new ManualDiscoverySource(
                translators,
                templateBuilder,
                diagnostics);

            discovery.OnEndpointBecomingAvailable += (s, e) => Assert.Fail();
            discovery.StartDiscovery();

            var uri = new Uri("net.pipe://localhost/pipe/discovery");
            var channels = new[]
                {
                    new Tuple<Version, Uri>(new Version(2, 0), null), 
                };
            var receiver = new BootstrapEndpoint(channels);

            var host = new ServiceHost(receiver, uri);
            var binding = new NetNamedPipeBinding();
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IBootstrapEndpoint), binding, address);

            host.Open();
            try
            {
                discovery.RecentlyConnectedEndpoint(new EndpointId("a"), endpoint.ListenUri);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void RecentlyConnectedEndpointWithCommunicationFault()
        {
            var translator = new Mock<ITranslateVersionedChannelInformation>();
            var translators = new[]
            {
                new Tuple<Version, ITranslateVersionedChannelInformation>(new Version(1, 0), translator.Object),
            };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new NamedPipeDiscoveryChannelTemplate(configuration.Object);
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = t => template;
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var discovery = new ManualDiscoverySource(
                translators,
                templateBuilder,
                diagnostics);

            discovery.OnEndpointBecomingAvailable += (s, e) => Assert.Fail();
            discovery.StartDiscovery();

            var uri = new Uri("net.pipe://localhost/pipe/discovery");
            var receiver = new Mock<IBootstrapEndpoint>();
            {
                receiver.Setup(r => r.DiscoveryVersions())
                    .Throws(new ArgumentException());
            }

            var host = new ServiceHost(receiver.Object, uri);
            var binding = new NetNamedPipeBinding();
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IBootstrapEndpoint), binding, address);

            host.Open();
            try
            {
                discovery.RecentlyConnectedEndpoint(new EndpointId("a"), endpoint.ListenUri);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void RecentlyConnectedEndpoint()
        {
            var uri = new Uri("net.pipe://localhost/pipe/discovery/");
            var versionedUri = new Uri(uri, new Uri("/1.0", UriKind.Relative));
            var protocolInfo = new ProtocolInformation(
                new Version(2, 0),
                new Uri("http://localhost/protocol/message/invalid"),
                new Uri("http://localhost/protocol/data/invalid"));
            var translator = new Mock<ITranslateVersionedChannelInformation>();
            {
                translator.Setup(t => t.FromUri(It.IsAny<Uri>()))
                    .Callback<Uri>(u => Assert.AreEqual(versionedUri, u))
                    .Returns(protocolInfo);
            }

            var translators = new[]
            {
                new Tuple<Version, ITranslateVersionedChannelInformation>(new Version(1, 0), translator.Object),
            };

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new NamedPipeDiscoveryChannelTemplate(configuration.Object);
            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder = t => template;
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var discovery = new ManualDiscoverySource(
                translators,
                templateBuilder,
                diagnostics);

            var eventWasRaised = false;
            discovery.OnEndpointBecomingAvailable += (s, e) =>
                {
                    eventWasRaised = true;
                };
            discovery.StartDiscovery();

            var channels = new[]
                {
                    new Tuple<Version, Uri>(new Version(1, 0), versionedUri), 
                };
            var receiver = new BootstrapEndpoint(channels);

            var host = new ServiceHost(receiver, uri);
            var binding = new NetNamedPipeBinding();
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var endpoint = host.AddServiceEndpoint(typeof(IBootstrapEndpoint), binding, address);

            host.Open();
            try
            {
                discovery.RecentlyConnectedEndpoint(new EndpointId("a"), endpoint.ListenUri);
            }
            finally
            {
                host.Close();
            }

            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        public void RecentlyDisconnectedEndpointWithoutDiscoveryPermissions()
        {
            var translator = new Mock<ITranslateVersionedChannelInformation>();
            var translators = new[]
            {
                new Tuple<Version, ITranslateVersionedChannelInformation>(new Version(1, 0), translator.Object),
            };

            var template = new Mock<IDiscoveryChannelTemplate>();
            {
                template.Setup(t => t.GenerateBinding())
                    .Verifiable();
            }

            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder =
                t =>
                {
                    return template.Object;
                };
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var discovery = new ManualDiscoverySource(
                translators,
                templateBuilder,
                diagnostics);
            discovery.OnEndpointBecomingUnavailable += (s, e) => Assert.Fail();
            discovery.RecentlyDisconnectedEndpoint(new EndpointId("a"));
        }

        [Test]
        public void RecentlyDisconnectedEndpoint()
        {
            var translator = new Mock<ITranslateVersionedChannelInformation>();
            var translators = new[]
            {
                new Tuple<Version, ITranslateVersionedChannelInformation>(new Version(1, 0), translator.Object),
            };

            var template = new Mock<IDiscoveryChannelTemplate>();
            {
                template.Setup(t => t.GenerateBinding())
                    .Verifiable();
            }

            Func<ChannelTemplate, IDiscoveryChannelTemplate> templateBuilder =
                t =>
                {
                    return template.Object;
                };
            var diagnostics = new SystemDiagnostics((l, s) => { }, null);
            var discovery = new ManualDiscoverySource(
                translators,
                templateBuilder,
                diagnostics);

            var eventWasRaised = false;
            discovery.OnEndpointBecomingUnavailable += (s, e) =>
            {
                eventWasRaised = true;
            };
            discovery.RecentlyDisconnectedEndpoint(new EndpointId("a"));

            Assert.IsTrue(eventWasRaised);
        }
    }
}
