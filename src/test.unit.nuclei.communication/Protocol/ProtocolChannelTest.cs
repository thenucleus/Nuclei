//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Communication.Protocol.Messages;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ProtocolChannelTest
    {
        private string TempFile()
        {
            var fileName = Path.GetRandomFileName();

            // Get the location of the assembly before it was shadow-copied
            // Note that Assembly.Codebase gets the path to the manifest-containing
            // file, not necessarily the path to the file that contains a
            // specific type.
            var uncPath = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            return Path.Combine(Path.GetDirectoryName(uncPath.LocalPath), fileName);
        }

        [Test]
        public void LocalConnectionPointForVersionWithUnknownVersion()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var template = new Mock<IProtocolChannelTemplate>();

            var host = new Mock<IHoldServiceConnections>();
            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder = version => new Tuple<Type, IMessagePipe>(null, null);
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder = version => new Tuple<Type, IDataPipe>(null, null);

            var sendingEndpoint = new Mock<ISendingEndpoint>();
            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            var connection = channel.LocalConnectionPointForVersion(new Version(1, 0));
            Assert.IsNull(connection);
        }

        [Test]
        public void OpenChannel()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var sendingEndpoint = new Mock<ISendingEndpoint>();
            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());
            Assert.That(protocols.Select(p => p.Version), Is.EquivalentTo(ProtocolVersions.SupportedVersions()));

            var connection = channel.LocalConnectionPointForVersion(new Version(1, 0));
            Assert.AreEqual(ProtocolVersions.V1, connection.Version);
            Assert.AreEqual(messageUri, connection.MessageAddress);
            Assert.AreEqual(dataUri, connection.DataAddress);

            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(2));
        }

        [Test]
        public void OpenChannelWithAlreadyOpenChannel()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>((e, m) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();

            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(2));

            channel.OpenChannel();
            host.Verify(h => h.CloseConnection(), Times.Exactly(2));
            sendingEndpoint.Verify(e => e.KnownEndpoints(), Times.Once());
            sendingEndpoint.Verify(e => e.CloseChannelTo(It.IsAny<EndpointId>()), Times.Once());
            sendingEndpoint.Verify(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(4));
        }

        [Test]
        public void CloseChannel()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>((e, m) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(2));

            channel.CloseChannel();
            host.Verify(h => h.CloseConnection(), Times.Exactly(2));
            sendingEndpoint.Verify(e => e.KnownEndpoints(), Times.Once());
            sendingEndpoint.Verify(e => e.CloseChannelTo(It.IsAny<EndpointId>()), Times.Once());
            sendingEndpoint.Verify(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        public void CloseChannelWithNeverOpenedChannel()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>((e, m) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.CloseChannel();
            host.Verify(h => h.CloseConnection(), Times.Never());
            sendingEndpoint.Verify(e => e.KnownEndpoints(), Times.Never());
            sendingEndpoint.Verify(e => e.CloseChannelTo(It.IsAny<EndpointId>()), Times.Never());
            sendingEndpoint.Verify(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());
        }

        [Test]
        public void EndpointDisconnectedWithOpenConnection()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
            }

            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>((e, m) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(2));

            channel.EndpointDisconnected(remoteEndpoint);
            sendingEndpoint.Verify(s => s.CloseChannelTo(It.IsAny<EndpointId>()), Times.Once());
        }

        [Test]
        public void EndpointDisconnectedWithUncontactableEndpoint()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>((e, m) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(2));

            channel.EndpointDisconnected(remoteEndpoint);
            sendingEndpoint.Verify(s => s.CloseChannelTo(It.IsAny<EndpointId>()), Times.Never());
        }

        [Test]
        public void Send()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(true);
            }

            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var msg = new SuccessMessage(id, new MessageId());
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>((e, m) => Assert.AreSame(msg, m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            channel.Send(remoteEndpoint, msg);
            sendingEndpoint.Verify(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
        }

        [Test]
        public void SendWithUncontactableEndpoint()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var msg = new SuccessMessage(id, new MessageId());
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>((e, m) => Assert.AreSame(msg, m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            Assert.Throws<EndpointNotContactableException>(() => channel.Send(remoteEndpoint, msg));
            sendingEndpoint.Verify(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Never());
        }

        [Test]
        public void TransferData()
        {
            var path = TempFile();

            var text = "Some random text";
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(text);
            }

            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<Stream>()))
                    .Callback<EndpointId, Stream>(
                        (e, m) =>
                        {
                            string fileText;
                            using (var reader = new StreamReader(m))
                            {
                                fileText = reader.ReadToEnd();
                            }

                            Assert.AreEqual(text, fileText);
                        })
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            var task = channel.TransferData(remoteEndpoint, path, new CancellationToken(), new CurrentThreadTaskScheduler());
            task.Wait();
            sendingEndpoint.Verify(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<Stream>()), Times.Once());
        }

        [Test]
        public void TransferDataWithUncontactableEndpoint()
        {
            var path = TempFile();

            var text = "Some random text";
            using (var writer = new StreamWriter(path, false))
            {
                writer.WriteLine(text);
            }

            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            {
                endpoints.Setup(e => e.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var template = new Mock<IProtocolChannelTemplate>();

            var messageUri = new Uri("http://localhost/messages/invalid");
            var dataUri = new Uri("http://localhost/data/invalid");
            var host = new Mock<IHoldServiceConnections>();
            {
                host.Setup(
                    h =>
                        h.OpenChannel(
                            It.IsAny<IReceiveInformationFromRemoteEndpoints>(),
                            It.IsAny<Func<ServiceHost, ServiceEndpoint>>()))
                    .Returns<IReceiveInformationFromRemoteEndpoints, Func<ServiceHost, ServiceEndpoint>>(
                        (r, f) => r is IMessagePipe ? messageUri : dataUri)
                    .Verifiable();
                host.Setup(h => h.CloseConnection())
                    .Verifiable();
            }

            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var remoteEndpoint = new EndpointId("b");
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { remoteEndpoint });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<EndpointId>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<Stream>()))
                    .Callback<EndpointId, Stream>(
                        (e, m) =>
                        {
                            string fileText;
                            using (var reader = new StreamReader(m))
                            {
                                fileText = reader.ReadToEnd();
                            }

                            Assert.AreEqual(text, fileText);
                        })
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            Assert.Throws<EndpointNotContactableException>(
                () => channel.TransferData(remoteEndpoint, path, new CancellationToken(), new CurrentThreadTaskScheduler()));
            sendingEndpoint.Verify(e => e.Send(It.IsAny<EndpointId>(), It.IsAny<Stream>()), Times.Never());
        }

        [Test]
        public void OnMessageReception()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var template = new Mock<IProtocolChannelTemplate>();

            var host = new Mock<IHoldServiceConnections>();
            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var sendingEndpoint = new Mock<ISendingEndpoint>();
            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            var eventWasRaised = false;
            var msg = new SuccessMessage(id, new MessageId());
            channel.OnMessageReception +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(msg, e.Message);
                };

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            messagePipe.Raise(m => m.OnNewMessage += null, new MessageEventArgs(msg));
            Assert.IsTrue(eventWasRaised);
        }

        [Test]
        public void OnDataReception()
        {
            var id = new EndpointId("a");
            var endpoints = new Mock<IStoreInformationAboutEndpoints>();
            var template = new Mock<IProtocolChannelTemplate>();

            var host = new Mock<IHoldServiceConnections>();
            Func<IHoldServiceConnections> hostBuilder = () => host.Object;

            var messagePipe = new Mock<IMessagePipe>();
            Func<Version, Tuple<Type, IMessagePipe>> messageReceiverBuilder =
                version => new Tuple<Type, IMessagePipe>(typeof(IMessagePipe), messagePipe.Object);

            var dataPipe = new Mock<IDataPipe>();
            Func<Version, Tuple<Type, IDataPipe>> dataReceiverBuilder =
                version => new Tuple<Type, IDataPipe>(typeof(IDataPipe), dataPipe.Object);

            var sendingEndpoint = new Mock<ISendingEndpoint>();
            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
                endpoints.Object,
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            var eventWasRaised = false;
            var msg = new DataTransferMessage();
            channel.OnDataReception +=
                (s, e) =>
                {
                    eventWasRaised = true;
                    Assert.AreEqual(msg, e.Data);
                };

            channel.OpenChannel();
            var protocols = channel.LocalConnectionPoints().ToList();
            Assert.AreEqual(1, protocols.Count());

            dataPipe.Raise(m => m.OnNewData += null, new DataTransferEventArgs(msg));
            Assert.IsTrue(eventWasRaised);
        }
    }
}
