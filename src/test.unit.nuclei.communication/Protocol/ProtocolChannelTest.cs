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
                template.Object,
                hostBuilder,
                messageReceiverBuilder,
                dataReceiverBuilder,
                senderBuilder,
                versionedMessageSenderBuilder,
                versionedDataSenderBuilder);

            var connection = channel.LocalConnectionPointForVersion(ProtocolVersions.V1);
            Assert.IsNull(connection);
        }

        [Test]
        public void OpenChannel()
        {
            var id = new EndpointId("a");
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

            var connection = channel.LocalConnectionPointForVersion(ProtocolVersions.V1);
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
            var endpointInfo = new ProtocolInformation(
                ProtocolVersions.V1,
                new Uri("http://localhost/messages"),
                new Uri("http://localhost/data"));
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

            var msg = new EndpointDisconnectMessage(id);
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { endpointInfo });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
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

            channel.Send(endpointInfo, msg, 1);
            sendingEndpoint.Verify(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());

            channel.OpenChannel();
            host.Verify(h => h.CloseConnection(), Times.Exactly(2));
            sendingEndpoint.Verify(e => e.KnownEndpoints(), Times.Once());
            sendingEndpoint.Verify(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()), Times.Once());
            sendingEndpoint.Verify(
                e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), 
                Times.Exactly(2));
            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(4));
        }

        [Test]
        public void CloseChannel()
        {
            var id = new EndpointId("a");
            var endpointInfo = new EndpointInformation(
                new EndpointId("b"),
                new DiscoveryInformation(new Uri("http://localhost/discovery")), 
                new ProtocolInformation(
                    ProtocolVersions.V1, 
                    new Uri("http://localhost/messages"),
                    new Uri("http://localhost/data")));
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

            var msg = new EndpointDisconnectMessage(id);
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { endpointInfo.ProtocolInformation });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
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

            channel.Send(endpointInfo.ProtocolInformation, msg, 1);
            sendingEndpoint.Verify(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());

            channel.CloseChannel();
            host.Verify(h => h.CloseConnection(), Times.Exactly(2));
            sendingEndpoint.Verify(e => e.KnownEndpoints(), Times.Once());
            sendingEndpoint.Verify(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()), Times.Once());
            sendingEndpoint.Verify(
                e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), 
                Times.Exactly(2));
        }

        [Test]
        public void CloseChannelWithNeverOpenedChannel()
        {
            var id = new EndpointId("a");
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

            var endpointInfo = new ProtocolInformation(
                ProtocolVersions.V1,
                new Uri("http://localhost/messages"),
                new Uri("http://localhost/data"));
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { endpointInfo });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, r) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
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
            sendingEndpoint.Verify(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()), Times.Never());
            sendingEndpoint.Verify(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void EndpointDisconnectedWithOpenConnection()
        {
            var id = new EndpointId("a");
            var endpointInfo = new ProtocolInformation(
                ProtocolVersions.V1,
                new Uri("http://localhost/messages"),
                new Uri("http://localhost/data"));
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

            var msg = new EndpointDisconnectMessage(id);
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { endpointInfo });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>(
                        (e, m, t) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
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

            channel.Send(endpointInfo, msg, 1);
            sendingEndpoint.Verify(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());

            host.Verify(
                 h => h.OpenChannel(It.IsAny<IReceiveInformationFromRemoteEndpoints>(), It.IsAny<Func<ServiceHost, ServiceEndpoint>>()),
                 Times.Exactly(2));

            channel.EndpointDisconnected(endpointInfo);
            sendingEndpoint.Verify(s => s.CloseChannelTo(It.IsAny<ProtocolInformation>()), Times.Once());
        }

        [Test]
        public void Send()
        {
            var id = new EndpointId("a");
            var endpointInfo = new ProtocolInformation(
                ProtocolVersions.V1,
                new Uri("http://localhost/messages"),
                new Uri("http://localhost/data"));
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

            var msg = new SuccessMessage(id, new MessageId());
            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { endpointInfo });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, ICommunicationMessage, int>((e, m, r) => Assert.AreSame(msg, m))
                    .Verifiable();
            }

            BuildSendingEndpoint senderBuilder = (endpoint, builder, endpointBuilder) => sendingEndpoint.Object;

            var messageEndpoint = new Mock<IMessageSendingEndpoint>();
            Func<Version, Uri, IMessageSendingEndpoint> versionedMessageSenderBuilder = (version, uri) => messageEndpoint.Object;

            var dataEndpoint = new Mock<IDataTransferingEndpoint>();
            Func<Version, Uri, IDataTransferingEndpoint> versionedDataSenderBuilder = (version, uri) => dataEndpoint.Object;

            var channel = new ProtocolChannel(
                id,
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

            channel.Send(endpointInfo, msg, 1);
            sendingEndpoint.Verify(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<ICommunicationMessage>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void TransferData()
        {
            var path = TempFile();

            var text = "Some random text";
            using (var writer = new StreamWriter(path, false))
            {
                writer.Write(text);
            }

            var id = new EndpointId("a");
            var endpointInfo = new ProtocolInformation(
                ProtocolVersions.V1,
                new Uri("http://localhost/messages"),
                new Uri("http://localhost/data"));
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

            var sendingEndpoint = new Mock<ISendingEndpoint>();
            {
                sendingEndpoint.Setup(e => e.KnownEndpoints())
                    .Returns(new[] { endpointInfo });
                sendingEndpoint.Setup(e => e.CloseChannelTo(It.IsAny<ProtocolInformation>()))
                    .Verifiable();
                sendingEndpoint.Setup(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<Stream>(), It.IsAny<int>()))
                    .Callback<ProtocolInformation, Stream, int>(
                        (e, m, r) =>
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

            var task = channel.TransferData(endpointInfo, path, new CancellationToken(), new CurrentThreadTaskScheduler(), 1);
            task.Wait();
            sendingEndpoint.Verify(e => e.Send(It.IsAny<ProtocolInformation>(), It.IsAny<Stream>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public void OnMessageReception()
        {
            var id = new EndpointId("a");
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
