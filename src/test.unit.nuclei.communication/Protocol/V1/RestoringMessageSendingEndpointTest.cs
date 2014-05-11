//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using Moq;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1.DataObjects.Converters;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RestoringMessageSendingEndpointTest
    {
        [ServiceBehavior(
            ConcurrencyMode = ConcurrencyMode.Multiple,
            InstanceContextMode = InstanceContextMode.Single)]
        internal sealed class MockMessageReceivingEndpoint : IMessageReceivingEndpoint
        {
            private readonly Func<IStoreV1CommunicationData, MessageReceptionConfirmation> m_OnAcceptMessage;

            public MockMessageReceivingEndpoint(Func<IStoreV1CommunicationData, MessageReceptionConfirmation> onAcceptMessage)
            {
                m_OnAcceptMessage = onAcceptMessage;
            }

            public MessageReceptionConfirmation AcceptMessage(IStoreV1CommunicationData message)
            {
                return m_OnAcceptMessage(message);
            }
        }

        [Test]
        public void SendWithNoChannel()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            var autoReset = new AutoResetEvent(false);
            bool hasBeenInvoked = false;
            Func<IStoreV1CommunicationData, MessageReceptionConfirmation> handler =
                s =>
                {
                    hasBeenInvoked = true;
                    autoReset.Set();
                    Assert.AreEqual(endpointId, s.Sender);
                    return new MessageReceptionConfirmation
                        {
                            WasDataReceived = true,
                        };
                };
            var receiver = new MockMessageReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);
            host.Faulted += (s, e) => Assert.Fail();

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
                    TransferMode = TransferMode.Buffered,
                };

            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var remoteEndpoint = host.AddServiceEndpoint(typeof(IMessageReceivingEndpoint), binding, address);
            foreach (var operation in remoteEndpoint.Contract.Operations)
            {
                var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior == null)
                {
                    behavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(behavior);
                }

                behavior.DataContractResolver = new ProtocolDataContractResolver();
            }

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
                var sender = new RestoringMessageSendingEndpoint(
                    new Uri(localAddress),
                    template,
                    new ProtocolDataContractResolver(),
                    new IConvertCommunicationMessages[]
                        {
                            new EndpointDisconnectConverter(), 
                        }, 
                    systemDiagnostics);

                sender.Send(msg, 1);

                autoReset.WaitOne(500);
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
            var count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            Func<IStoreV1CommunicationData, MessageReceptionConfirmation> handler =
                s =>
                {
                    if (count == 0)
                    {
                        count++;
                        throw new Exception("Lets bail the first one");
                    }

                    count++;
                    Assert.AreEqual(endpointId, s.Sender);

                    return new MessageReceptionConfirmation
                        {
                            WasDataReceived = true,
                        };
                };
            var receiver = new MockMessageReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var remoteEndpoint = host.AddServiceEndpoint(typeof(IMessageReceivingEndpoint), binding, address);
            foreach (var operation in remoteEndpoint.Contract.Operations)
            {
                var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior == null)
                {
                    behavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(behavior);
                }

                behavior.DataContractResolver = new ProtocolDataContractResolver();
            }

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
                var sender = new RestoringMessageSendingEndpoint(
                    new Uri(localAddress),
                    template,
                    new ProtocolDataContractResolver(),
                    new IConvertCommunicationMessages[]
                        {
                            new EndpointDisconnectConverter(), 
                        },
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
            var count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            Func<IStoreV1CommunicationData, MessageReceptionConfirmation> handler =
                s =>
                {
                    if (count == 0)
                    {
                        count++;
                        throw new FaultException("Lets bail the first one");
                    }

                    count++;
                    Assert.AreEqual(endpointId, s.Sender);

                    return new MessageReceptionConfirmation
                        {
                            WasDataReceived = true,
                        };
                };
            var receiver = new MockMessageReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var remoteEndpoint = host.AddServiceEndpoint(typeof(IMessageReceivingEndpoint), binding, address);
            foreach (var operation in remoteEndpoint.Contract.Operations)
            {
                var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior == null)
                {
                    behavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(behavior);
                }

                behavior.DataContractResolver = new ProtocolDataContractResolver();
            }

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
                var sender = new RestoringMessageSendingEndpoint(
                    new Uri(localAddress),
                    template,
                    new ProtocolDataContractResolver(),
                    new IConvertCommunicationMessages[]
                        {
                            new EndpointDisconnectConverter(), 
                        },
                    systemDiagnostics);

                // This message should fault the channel
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
            var count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            Func<IStoreV1CommunicationData, MessageReceptionConfirmation> handler =
                s =>
                {
                    count++;
                    throw new Exception("Lets bail the first one");
                };
            var receiver = new MockMessageReceivingEndpoint(handler);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            var remoteEndpoint = host.AddServiceEndpoint(typeof(IMessageReceivingEndpoint), binding, address);
            foreach (var operation in remoteEndpoint.Contract.Operations)
            {
                var behavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior == null)
                {
                    behavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(behavior);
                }

                behavior.DataContractResolver = new ProtocolDataContractResolver();
            }

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
                var sender = new RestoringMessageSendingEndpoint(
                    new Uri(localAddress),
                    template,
                    new ProtocolDataContractResolver(),
                    new IConvertCommunicationMessages[]
                        {
                            new EndpointDisconnectConverter(), 
                        },
                    systemDiagnostics);

                // This message should fault the channel
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
