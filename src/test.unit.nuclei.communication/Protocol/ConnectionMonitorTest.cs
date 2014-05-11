//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Configuration;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ConnectionMonitorTest
    {
        [Test]
        public void HandleKeepAliveIntervalElapsedWithAllConnectionsConfirmed()
        {
            var info = new EndpointInformation(
                new EndpointId("a"), 
                new DiscoveryInformation(new Uri("http://localhost/discovery/invalid")), 
                new ProtocolInformation(
                    new Version(1, 0), 
                    new Uri("http://localhost/protocol/invalid")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out info))
                    .Returns(true);
            }

            var localId = new EndpointId("b");
            VerifyEndpointConnectionStatusWithCustomData func = 
                (id, timeout, data) =>
                {
                    Assert.AreEqual(info.Id, id);
                    return Task<object>.Factory.StartNew(
                        () => null,
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localId);
                layer.Setup(l => l.SendMessageAndWaitForResponse(
                        It.IsAny<EndpointId>(), 
                        It.IsAny<ICommunicationMessage>(), 
                        It.IsAny<int>(),
                        It.IsAny<TimeSpan>()))
                    .Verifiable();
            }

            var timer = new Mock<ITimer>();

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(
                        c => c.HasValueFor(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(true);
                configuration.Setup(
                        c => c.Value<int>(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(1);
            }

            DateTimeOffset time = DateTimeOffset.Now;
            Func<DateTimeOffset> now = () => time;

            var monitor = new ConnectionMonitor(
                endpoints.Object,
                func,
                timer.Object,
                now,
                configuration.Object);
            
            // New connection
            var connection = new Mock<IConfirmConnections>();
            monitor.Register(connection.Object);

            // New endpoint
            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(info.Id));
            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(
                    It.IsAny<EndpointId>(), 
                    It.IsAny<ICommunicationMessage>(), 
                    It.IsAny<int>(),
                    It.IsAny<TimeSpan>()),
                Times.Never());
        }

        [Test]
        public void HandleKeepAliveIntervalElapsedWithActiveConnection()
        {
            var info = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("http://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("http://localhost/protocol/invalid")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out info))
                    .Returns(true);
            }

            var wasInvoked = false;
            VerifyEndpointConnectionStatusWithCustomData func =
                (id, timeout, data) =>
                {
                    wasInvoked = true;
                    Assert.AreEqual(info.Id, id);
                    return Task<object>.Factory.StartNew(
                        () => null,
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var timer = new Mock<ITimer>();

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(
                        c => c.HasValueFor(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(true);
                configuration.Setup(
                        c => c.Value<int>(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(1);
            }

            DateTimeOffset time = DateTimeOffset.Now;
            Func<DateTimeOffset> now = () => time;

            var monitor = new ConnectionMonitor(
                endpoints.Object,
                func,
                timer.Object,
                now,
                configuration.Object);

            // New connection
            var connection = new Mock<IConfirmConnections>();
            monitor.Register(connection.Object);

            // New endpoint
            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(info.Id));
            time = time + TimeSpan.FromMinutes(2);

            connection.Raise(c => c.OnConfirmChannelIntegrity += null, new EndpointEventArgs(info.Id));
            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            Assert.IsFalse(wasInvoked);
        }

        [Test]
        public void HandleKeepAliveIntervalElapsedWithOpenConnection()
        {
            var info = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("http://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("http://localhost/protocol/invalid")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out info))
                    .Returns(true);
            }

            var wasInvoked = false;
            VerifyEndpointConnectionStatusWithCustomData func =
                (id, timeout, data) =>
                {
                    wasInvoked = true;
                    Assert.AreEqual(info.Id, id);
                    return Task<object>.Factory.StartNew(
                        () => null,
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var timer = new Mock<ITimer>();

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(
                        c => c.HasValueFor(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(true);
                configuration.Setup(
                        c => c.Value<int>(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(1);
            }

            DateTimeOffset time = DateTimeOffset.Now;
            Func<DateTimeOffset> now = () => time;

            var monitor = new ConnectionMonitor(
                endpoints.Object,
                func,
                timer.Object,
                now,
                configuration.Object);

            // New connection
            var connection = new Mock<IConfirmConnections>();
            monitor.Register(connection.Object);

            // New endpoint
            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(info.Id));
            time = time + TimeSpan.FromMinutes(2);

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            Assert.IsTrue(wasInvoked);
            wasInvoked = false;

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            Assert.IsFalse(wasInvoked);
        }

        [Test]
        public void HandleKeepAliveIntervalElapsedWithoutOpenConnection()
        {
            var info = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("http://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("http://localhost/protocol/invalid")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out info))
                    .Returns(true);
            }

            var wasInvoked = false;
            VerifyEndpointConnectionStatusWithCustomData func =
                (id, timeout, data) =>
                {
                    wasInvoked = true;
                    Assert.AreEqual(info.Id, id);
                    return Task<object>.Factory.StartNew(
                        () =>
                        {
                            throw new TimeoutException();
                        },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var timer = new Mock<ITimer>();

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(
                        c => c.HasValueFor(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(true);
                configuration.Setup(
                        c => c.Value<int>(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(1);
            }

            DateTimeOffset time = DateTimeOffset.Now;
            Func<DateTimeOffset> now = () => time;

            var monitor = new ConnectionMonitor(
                endpoints.Object,
                func,
                timer.Object,
                now,
                configuration.Object);

            // New connection
            var connection = new Mock<IConfirmConnections>();
            monitor.Register(connection.Object);

            // New endpoint
            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(info.Id));
            time = time + TimeSpan.FromMinutes(2);

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            Assert.IsTrue(wasInvoked);
            wasInvoked = false;

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);
            Assert.IsTrue(wasInvoked);
        }

        [Test]
        public void HandleKeepAliveIntervalElapsedWithConnectionTimeout()
        {
            var info = new EndpointInformation(
                new EndpointId("a"),
                new DiscoveryInformation(new Uri("http://localhost/discovery/invalid")),
                new ProtocolInformation(
                    new Version(1, 0),
                    new Uri("http://localhost/protocol/invalid")));
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.TryGetConnectionFor(It.IsAny<EndpointId>(), out info))
                    .Returns(true);
            }

            var wasInvoked = false;
            VerifyEndpointConnectionStatusWithCustomData func =
                (id, timeout, data) =>
                {
                    wasInvoked = true;
                    Assert.AreEqual(info.Id, id);
                    var source = new CancellationTokenSource();
                    source.Cancel();
                    return Task<object>.Factory.StartNew(
                        () => null,
                        source.Token,
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var timer = new Mock<ITimer>();

            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(
                        c => c.HasValueFor(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(true);
                configuration.Setup(
                        c => c.Value<int>(It.Is<ConfigurationKey>(k => k.Equals(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds))))
                    .Returns(1);
            }

            DateTimeOffset time = DateTimeOffset.Now;
            Func<DateTimeOffset> now = () => time;

            var monitor = new ConnectionMonitor(
                endpoints.Object,
                func,
                timer.Object,
                now,
                configuration.Object);

            // New connection
            var connection = new Mock<IConfirmConnections>();
            monitor.Register(connection.Object);

            // New endpoint
            endpoints.Raise(e => e.OnEndpointConnected += null, new EndpointEventArgs(info.Id));
            time = time + TimeSpan.FromMinutes(2);

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            Assert.IsTrue(wasInvoked);
            wasInvoked = false;

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            Assert.IsTrue(wasInvoked);
        }
    }
}
