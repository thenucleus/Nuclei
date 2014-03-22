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
        [Test]
        public void SendWithNoChannel()
        {
            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var sendingEndpoint = new EndpointId("a");
            var receivingEndpoint = new EndpointId("b");
            var msg = new DataTransferMessage
                {
                    SendingEndpoint = sendingEndpoint,
                    ReceivingEndpoint = receivingEndpoint,
                    Data = data,
                };

            var receiver = new DataReceivingEndpoint(systemDiagnostics);
            receiver.OnNewData += 
                (s, e) =>
                {
                    Assert.AreEqual(sendingEndpoint, e.Data.SendingEndpoint);
                    Assert.AreEqual(receivingEndpoint, e.Data.ReceivingEndpoint);
                    Assert.AreEqual(text, new StreamReader(e.Data.Data).ReadToEnd());
                };

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
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

                sender.Send(msg);
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
            
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var sendingEndpoint = new EndpointId("a");
            var receivingEndpoint = new EndpointId("b");
            var msg = new DataTransferMessage
            {
                SendingEndpoint = sendingEndpoint,
                ReceivingEndpoint = receivingEndpoint,
                Data = data,
            };

            var count = 0;
            var receiver = new DataReceivingEndpoint(systemDiagnostics);
            receiver.OnNewData +=
                (s, e) =>
                {
                    if (count == 0)
                    {
                        count++;
                        throw new FaultException("Lets bail the first one");
                    }
                    
                    Assert.AreEqual(sendingEndpoint, e.Data.SendingEndpoint);
                    Assert.AreEqual(receivingEndpoint, e.Data.ReceivingEndpoint);
                    Assert.AreEqual(text, new StreamReader(e.Data.Data).ReadToEnd());
                };

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                {
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
                sender.Send(msg);

                // This message should still go through
                sender.Send(msg);
            }
            finally
            {
                host.Close();
            }
        }
    }
}
