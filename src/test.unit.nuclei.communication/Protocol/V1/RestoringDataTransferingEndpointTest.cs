//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.ServiceModel;
using Moq;
using Nuclei.Communication.Protocol.V1.DataObjects;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RestoringDataTransferingEndpointTest
    {
        [ServiceBehavior(
            ConcurrencyMode = ConcurrencyMode.Multiple,
            InstanceContextMode = InstanceContextMode.Single)]
        internal sealed class MockDataReceivingEndpoint : IDataReceivingEndpoint
        {
            private readonly Func<StreamData, StreamReceptionConfirmation> m_OnAcceptStream;

            public MockDataReceivingEndpoint(Func<StreamData, StreamReceptionConfirmation> onAcceptStream)
            {
                m_OnAcceptStream = onAcceptStream;
            }

            public StreamReceptionConfirmation AcceptStream(StreamData data)
            {
                return m_OnAcceptStream(data);
            }
        }

        [Test]
        public void SendWithNoChannel()
        {
            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            writer.Flush();
            data.Position = 0;

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var sendingEndpoint = new EndpointId("a");
            var msg = new DataTransferMessage
                {
                    SendingEndpoint = sendingEndpoint,
                    Data = data,
                };

            bool hasBeenInvoked = false;
            Func<StreamData, StreamReceptionConfirmation> handler =
                s =>
                {
                    hasBeenInvoked = true;
                    Assert.AreEqual(sendingEndpoint, s.SendingEndpoint);

                    var storedText = new StreamReader(s.Data).ReadToEnd();
                    Assert.AreEqual(text, storedText);
                    return new StreamReceptionConfirmation
                        {
                            WasDataReceived = true,
                        };
                };
            var receiver = new MockDataReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
                    MaxReceivedMessageSize = CommunicationConstants.DefaultBindingMaxReceivedSizeForMessagesInBytes,
                    TransferMode = TransferMode.Streamed,
                };
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            host.AddServiceEndpoint(typeof(IDataReceivingEndpoint), binding, address);
            host.Faulted += (s, e) => Assert.Fail();

            host.Open();
            try
            {
                var configuration = new Mock<IConfiguration>();
                {
                    configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                        .Returns(false);
                }

                var localAddress = string.Format("{0}/{1}", uri.OriginalString, address);
                var template = new NamedPipeProtocolChannelTemplate(configuration.Object, new ProtocolDataContractResolver());
                var sender = new RestoringDataTransferingEndpoint(
                    new Uri(localAddress),
                    template,
                    systemDiagnostics);

                sender.Send(msg, 1);
                Assert.IsTrue(hasBeenInvoked);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void SendWithFaultedChannel()
        {
            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            writer.Flush();
            data.Position = 0;
            
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var sendingEndpoint = new EndpointId("a");
            var msg = new DataTransferMessage
            {
                SendingEndpoint = sendingEndpoint,
                Data = data,
            };

            var count = 0;
            Func<StreamData, StreamReceptionConfirmation> handler =
                s =>
                {
                    if (count == 0)
                    {
                        count++;
                        throw new Exception("Lets bail the first one");
                    }

                    count++;
                    Assert.AreEqual(sendingEndpoint, s.SendingEndpoint);
                    Assert.AreEqual(text, new StreamReader(s.Data).ReadToEnd());

                    return new StreamReceptionConfirmation
                        {
                            WasDataReceived = true,
                        };
                };
            var receiver = new MockDataReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
                    MaxReceivedMessageSize = CommunicationConstants.DefaultBindingMaxReceivedSizeForMessagesInBytes,
                    TransferMode = TransferMode.Streamed,
                };
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            host.AddServiceEndpoint(typeof(IDataReceivingEndpoint), binding, address);

            host.Open();
            try
            {
                var configuration = new Mock<IConfiguration>();
                {
                    configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                        .Returns(false);
                }

                var localAddress = string.Format("{0}/{1}", uri.OriginalString, address);
                var template = new NamedPipeProtocolChannelTemplate(configuration.Object, new ProtocolDataContractResolver());
                var sender = new RestoringDataTransferingEndpoint(
                    new Uri(localAddress),
                    template,
                    systemDiagnostics);

                // This message should fault the channel
                Assert.Throws<FailedToSendMessageException>(() => sender.Send(msg, 1));

                // This message should still go through
                sender.Send(msg, 1);
                Assert.AreEqual(2, count);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void SendWithRetry()
        {
            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            writer.Flush();
            data.Position = 0;

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var sendingEndpoint = new EndpointId("a");
            var msg = new DataTransferMessage
            {
                SendingEndpoint = sendingEndpoint,
                Data = data,
            };

            var count = 0;
            Func<StreamData, StreamReceptionConfirmation> handler =
                s =>
                {
                    if (count == 0)
                    {
                        count++;
                        throw new Exception("Lets bail the first one");
                    }

                    count++;
                    Assert.AreEqual(sendingEndpoint, s.SendingEndpoint);
                    Assert.AreEqual(text, new StreamReader(s.Data).ReadToEnd());

                    return new StreamReceptionConfirmation
                        {
                            WasDataReceived = true,
                        };
                };
            var receiver = new MockDataReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                MaxReceivedMessageSize = CommunicationConstants.DefaultBindingMaxReceivedSizeForMessagesInBytes,
                TransferMode = TransferMode.Streamed,
            };
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            host.AddServiceEndpoint(typeof(IDataReceivingEndpoint), binding, address);
            host.Faulted += (s, e) => Assert.Fail();

            host.Open();
            try
            {
                var configuration = new Mock<IConfiguration>();
                {
                    configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                        .Returns(false);
                }

                var localAddress = string.Format("{0}/{1}", uri.OriginalString, address);
                var template = new NamedPipeProtocolChannelTemplate(configuration.Object, new ProtocolDataContractResolver());
                var sender = new RestoringDataTransferingEndpoint(
                    new Uri(localAddress),
                    template,
                    systemDiagnostics);

                Assert.DoesNotThrow(() => sender.Send(msg, 2));
                Assert.AreEqual(2, count);
            }
            finally
            {
                host.Close();
            }
        }

        [Test]
        public void SendWithFailureOnRetry()
        {
            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            writer.Flush();
            data.Position = 0;

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var sendingEndpoint = new EndpointId("a");
            var msg = new DataTransferMessage
            {
                SendingEndpoint = sendingEndpoint,
                Data = data,
            };

            var count = 0;
            Func<StreamData, StreamReceptionConfirmation> handler = 
                s =>
                {
                    count++;
                    throw new Exception("Always bail");
                };
            var receiver = new MockDataReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
            {
                MaxReceivedMessageSize = CommunicationConstants.DefaultBindingMaxReceivedSizeForMessagesInBytes,
                TransferMode = TransferMode.Streamed,
            };
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            host.AddServiceEndpoint(typeof(IDataReceivingEndpoint), binding, address);
            host.Faulted += (s, e) => Assert.Fail();

            host.Open();
            try
            {
                var configuration = new Mock<IConfiguration>();
                {
                    configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                        .Returns(false);
                }

                var localAddress = string.Format("{0}/{1}", uri.OriginalString, address);
                var template = new NamedPipeProtocolChannelTemplate(configuration.Object, new ProtocolDataContractResolver());
                var sender = new RestoringDataTransferingEndpoint(
                    new Uri(localAddress),
                    template,
                    systemDiagnostics);

                Assert.Throws<FailedToSendMessageException>(() => sender.Send(msg, 2));
                Assert.AreEqual(2, count);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
