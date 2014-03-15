//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using Nuclei.Communication.Interaction;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CommunicationEntryPointTest
    {
        [Test]
        public void OnEndpointConnected()
        {
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var address = new Uri("http://localhost/discovery");
            var endpointInfo = new EndpointInformation(
                endpoint,
                new DiscoveryInformation(address), 
                new ProtocolInformation(
                    new Version(), 
                    new Uri("http://localhost/messages"),
                    new Uri("http://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true)
                    .Verifiable();
            }

            var commands = new Mock<ISendCommandsToRemoteEndpoints>();
            var notifications = new Mock<INotifyOfRemoteEndpointEvents>();

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object);

            EndpointId eventEndpoint = null;
            entryPoint.OnEndpointConnected +=
                (s, e) =>
                {
                    eventEndpoint = e.Endpoint;
                };

            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.IsNull(eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Never());
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));
            
            commands.Raise(c => c.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.IsNull(eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Never());
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));
            
            notifications.Raise(n => n.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.AreSame(endpoint, eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Once());
            Assert.IsTrue(entryPoint.KnownEndpoints().Any());
            Assert.AreSame(endpoint, entryPoint.FromUri(address));
        }

        [Test]
        public void OnEndpointDisconnected()
        {
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var address = new Uri("http://localhost/discovery");
            var endpointInfo = new EndpointInformation(
                endpoint,
                new DiscoveryInformation(address),
                new ProtocolInformation(
                    new Version(),
                    new Uri("http://localhost/messages"),
                    new Uri("http://localhost/data")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo))
                    .Returns(true)
                    .Verifiable();
            }

            var commands = new Mock<ISendCommandsToRemoteEndpoints>();
            var notifications = new Mock<INotifyOfRemoteEndpointEvents>();

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object);

            EndpointId eventEndpoint = null;
            entryPoint.OnEndpointConnected +=
                (s, e) =>
                {
                    eventEndpoint = e.Endpoint;
                };

            entryPoint.OnEndpointDisconnected +=
                (s, e) =>
                {
                    eventEndpoint = e.Endpoint;
                };

            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.IsNull(eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Never());
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));

            commands.Raise(c => c.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.IsNull(eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Never());
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));

            notifications.Raise(n => n.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            Assert.AreSame(endpoint, eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Once());
            Assert.IsTrue(entryPoint.KnownEndpoints().Any());
            Assert.AreSame(endpoint, entryPoint.FromUri(address));

            eventEndpoint = null;
            endpoints.Raise(e => e.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.AreSame(endpoint, eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Once());
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));

            eventEndpoint = null;
            commands.Raise(c => c.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.IsNull(eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Once());
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));

            notifications.Raise(n => n.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.AreSame(endpoint, eventEndpoint);
            endpoints.Verify(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out endpointInfo), Times.Once());
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));
        }

        [Test]
        public void OnEndpointDisconnectedForAnUnknownEndpoint()
        {
            var endpoint = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var address = new Uri("http://localhost/discovery");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var commands = new Mock<ISendCommandsToRemoteEndpoints>();
            var notifications = new Mock<INotifyOfRemoteEndpointEvents>();

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object);

            EndpointId eventEndpoint = null;
            entryPoint.OnEndpointDisconnected +=
                (s, e) =>
                {
                    eventEndpoint = e.Endpoint;
                };

            endpoints.Raise(e => e.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.IsNull(eventEndpoint);
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));

            commands.Raise(c => c.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.IsNull(eventEndpoint);
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));

            notifications.Raise(n => n.OnEndpointDisconnected += null, new EndpointEventArgs(endpoint));
            Assert.AreSame(endpoint, eventEndpoint);
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));
        }
    }
}
