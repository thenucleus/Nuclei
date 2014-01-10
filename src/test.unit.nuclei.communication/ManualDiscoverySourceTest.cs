//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Protocol;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ManualDiscoverySourceTest
    {
        [Test]
        public void StartDiscovery()
        {
            var source = new ManualDiscoverySource();

            EndpointId connectedEndpoint = null;
            ChannelType channelType = ChannelType.None;
            Uri channelUri = null;
            source.OnEndpointBecomingAvailable +=
                (s, e) =>
                {
                    connectedEndpoint = e.ConnectionInformation.Id;
                    channelType = e.ConnectionInformation.ChannelType;
                    channelUri = e.ConnectionInformation.MessageAddress;
                };

            source.RecentlyConnectedEndpoint(new EndpointId("sendingEndpoint"), ChannelType.TcpIP, new Uri("net.pipe://localhost/apollo_test"));
            Assert.IsNull(connectedEndpoint);

            source.StartDiscovery();

            var newEndpoint = new EndpointId("other");
            var type = ChannelType.TcpIP;
            var uri = new Uri("net.pipe://localhost/apollo_test");
            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);

            Assert.AreSame(newEndpoint, connectedEndpoint);
            Assert.AreEqual(type, channelType);
            Assert.AreSame(uri, channelUri);
        }

        [Test]
        public void EndDiscovery()
        {
            var source = new ManualDiscoverySource();
            source.StartDiscovery();

            EndpointId connectedEndpoint = null;
            ChannelType channelType = ChannelType.None;
            Uri channelUri = null;
            source.OnEndpointBecomingAvailable +=
                (s, e) =>
                {
                    connectedEndpoint = e.ConnectionInformation.Id;
                    channelType = e.ConnectionInformation.ChannelType;
                    channelUri = e.ConnectionInformation.MessageAddress;
                };

            var newEndpoint = new EndpointId("other");
            var type = ChannelType.TcpIP;
            var uri = new Uri("net.pipe://localhost/apollo_test");
            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);

            Assert.AreSame(newEndpoint, connectedEndpoint);
            Assert.AreEqual(type, channelType);
            Assert.AreSame(uri, channelUri);

            source.EndDiscovery();
            connectedEndpoint = null;
            channelType = ChannelType.None;
            channelUri = null;

            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);
            Assert.IsNull(connectedEndpoint);
            Assert.AreEqual(ChannelType.None, channelType);
            Assert.IsNull(channelUri);
        }

        [Test]
        public void RecentlyConnectedEndpoint()
        {
            var source = new ManualDiscoverySource();
            source.StartDiscovery();

            EndpointId connectedEndpoint = null;
            ChannelType channelType = ChannelType.None;
            Uri channelUri = null;
            source.OnEndpointBecomingAvailable +=
                (s, e) =>
                {
                    connectedEndpoint = e.ConnectionInformation.Id;
                    channelType = e.ConnectionInformation.ChannelType;
                    channelUri = e.ConnectionInformation.MessageAddress;
                };

            var newEndpoint = new EndpointId("other");
            var type = ChannelType.TcpIP;
            var uri = new Uri("net.pipe://localhost/apollo_test");
            source.RecentlyConnectedEndpoint(newEndpoint, type, uri);

            Assert.AreSame(newEndpoint, connectedEndpoint);
            Assert.AreEqual(type, channelType);
            Assert.AreSame(uri, channelUri);
        }
    }
}
