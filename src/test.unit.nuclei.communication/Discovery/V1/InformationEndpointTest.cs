//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Nuclei.Communication.Discovery.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class InformationEndpointTest
    {
        [Test]
        public void Version()
        {
            var endpoint = new InformationEndpoint(new ChannelInformation[0]);
            Assert.AreEqual(new Version(1, 0, 0, 0), endpoint.Version());
        }

        [Test]
        public void ProtocolVersions()
        {
            var info = new[]
                {
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v3")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v1")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v2")),
                };

            var endpoint = new InformationEndpoint(info);
            var versions = endpoint.ProtocolVersions();
            Assert.That(
                versions,
                Is.EquivalentTo(
                    info.Select(i => i.ProtocolVersion)
                        .OrderBy(v => v)));
        }

        [Test]
        public void ConnectionInformationForProtocolWithNullVersion()
        {
            var info = new[]
                {
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v3")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v1")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v2")),
                };

            var endpoint = new InformationEndpoint(info);
            Assert.IsNull(endpoint.ConnectionInformationForProtocol(null));
        }

        [Test]
        public void ConnectionInformationForProtocolWithNonSupportedVersion()
        {
            var info = new[]
                {
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v3")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v1")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v2")),
                };

            var endpoint = new InformationEndpoint(info);
            Assert.IsNull(endpoint.ConnectionInformationForProtocol(new Version(4, 0, 0, 0)));
            Assert.IsNull(endpoint.ConnectionInformationForProtocol(new Version(1, 1, 0, 0)));
            Assert.IsNull(endpoint.ConnectionInformationForProtocol(new Version(1, 0, 1, 0)));
            Assert.IsNull(endpoint.ConnectionInformationForProtocol(new Version(1, 0, 0, 1)));
        }

        [Test]
        public void ConnectionInformationForProtocol()
        {
            var info = new[]
                {
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v3")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v1")),
                    new ChannelInformation(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/invalid/v2")),
                };

            var endpoint = new InformationEndpoint(info);
            var output = endpoint.ConnectionInformationForProtocol(new Version(2, 0, 0, 0));
            
            Assert.IsNotNull(output);
            Assert.AreSame(info[2].Id, output.Id);
            Assert.AreSame(info[2].ProtocolVersion, output.ProtocolVersion);
            Assert.AreSame(info[2].Address, output.Address);
        }
    }
}
