//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;
using Nuclei.Configuration;
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            VerifyEndpointConnectionStatus func = (id, timeout) => null;

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            VerifyEndpointConnectionStatus func = (id, timeout) => null;

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

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
            Assert.IsNull(eventEndpoint);
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            VerifyEndpointConnectionStatus func = (id, timeout) => null;

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

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
            Assert.IsNull(eventEndpoint);
            Assert.IsFalse(entryPoint.KnownEndpoints().Any());
            Assert.IsNull(entryPoint.FromUri(address));
        }

        [Test]
        public void IsconnectionActiveWithIdAndUnknownConnection()
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            VerifyEndpointConnectionStatus func = (id, timeout) => null;

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

            Assert.IsFalse(entryPoint.IsConnectionActive(new EndpointId("a")));
        }

        [Test]
        public void IsConnectionActiveWithIdAndInactiveConnection()
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            VerifyEndpointConnectionStatus func = 
                (id, timeout) =>
                {
                    return Task.Factory.StartNew(
                        () =>
                        {
                            throw new TimeoutException();
                        },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            commands.Raise(c => c.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            notifications.Raise(n => n.OnEndpointConnected += null, new EndpointEventArgs(endpoint));

            Assert.IsFalse(entryPoint.IsConnectionActive(endpoint));
        }

        [Test]
        public void IsConnectionActiveWithIdAndUnresponsiveConnection()
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var source = new CancellationTokenSource();
            source.Cancel();
            VerifyEndpointConnectionStatus func =
                (id, timeout) =>
                {
                    return Task.Factory.StartNew(
                        () =>
                        {
                        },
                        source.Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            commands.Raise(c => c.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            notifications.Raise(n => n.OnEndpointConnected += null, new EndpointEventArgs(endpoint));

            Assert.IsFalse(entryPoint.IsConnectionActive(endpoint));
        }

        [Test]
        public void IsConnectionActiveWithIdAndResponsiveConnection()
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var source = new CancellationTokenSource();
            VerifyEndpointConnectionStatus func =
                (id, timeout) =>
                {
                    return Task.Factory.StartNew(
                        () =>
                        {
                        },
                        source.Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            commands.Raise(c => c.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            notifications.Raise(n => n.OnEndpointConnected += null, new EndpointEventArgs(endpoint));

            Assert.IsTrue(entryPoint.IsConnectionActive(endpoint));
        }

        [Test]
        public void IsConnectionActiveWithUrlAndUnknownConnection()
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            VerifyEndpointConnectionStatus func = (id, timeout) => null;

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

            Assert.IsFalse(entryPoint.IsConnectionActive(new Uri("http://localhost/invalid")));
        }

        [Test]
        public void IsConnectionActiveWithUrlAndResponsiveConnection()
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
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var source = new CancellationTokenSource();
            VerifyEndpointConnectionStatus func =
                (id, timeout) =>
                {
                    return Task.Factory.StartNew(
                        () =>
                        {
                        },
                        source.Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var entryPoint = new CommunicationEntryPoint(
                endpoints.Object,
                commands.Object,
                notifications.Object,
                func,
                configuration.Object);

            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            commands.Raise(c => c.OnEndpointConnected += null, new EndpointEventArgs(endpoint));
            notifications.Raise(n => n.OnEndpointConnected += null, new EndpointEventArgs(endpoint));

            Assert.IsTrue(entryPoint.IsConnectionActive(address));
        }
    }
}
