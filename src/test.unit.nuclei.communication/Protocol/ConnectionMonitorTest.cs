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
using Nuclei.Communication.Protocol.Messages;
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
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localId);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()))
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
                layer.Object,
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
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
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

            var localId = new EndpointId("b");
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localId);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()))
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
                layer.Object,
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

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
                Times.Never());
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

            var localId = new EndpointId("b");
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localId);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()))
                    .Returns<EndpointId, ICommunicationMessage, TimeSpan>(
                        (id, msg, t) =>
                        {
                            var token = new CancellationTokenSource().Token;
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () => new ConnectionVerificationResponseMessage(id, msg.Id),
                                token,
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        })
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
                layer.Object,
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

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
                Times.Once());

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
                Times.Once());
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

            var localId = new EndpointId("b");
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localId);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()))
                    .Returns<EndpointId, ICommunicationMessage, TimeSpan>(
                        (id, msg, t) =>
                        {
                            var token = new CancellationTokenSource().Token;
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () =>
                                {
                                    throw new FailedToSendMessageException();
                                },
                                token,
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        })
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
                layer.Object,
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

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
                Times.Once());

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
                Times.Exactly(2));
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

            var localId = new EndpointId("b");
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(localId);
                layer.Setup(l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()))
                    .Returns<EndpointId, ICommunicationMessage, TimeSpan>(
                        (id, msg, t) =>
                        {
                            var source = new CancellationTokenSource();
                            source.Cancel();
                            return Task<ICommunicationMessage>.Factory.StartNew(
                                () =>
                                {
                                    return null;
                                },
                                source.Token,
                                TaskCreationOptions.None,
                                new CurrentThreadTaskScheduler());
                        })
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
                layer.Object,
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

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
                Times.Once());

            timer.Raise(t => t.OnElapsed += null, EventArgs.Empty);

            layer.Verify(
                l => l.SendMessageAndWaitForResponse(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>(), It.IsAny<TimeSpan>()),
                Times.Exactly(2));
        }
    }
}
