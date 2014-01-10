//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceModel;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class RestoringMessageSendingEndpointTest
    {
        [Test]
        public void SendWithNoChannel()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            var receiver = new MessageReceivingEndpoint(systemDiagnostics);
            receiver.OnNewMessage += (s, e) => Assert.AreEqual(endpointId, e.Message.OriginatingEndpoint);

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);
            
            var binding = new NetNamedPipeBinding();
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            host.AddServiceEndpoint(typeof(IMessageReceivingEndpoint), binding, address);

            host.Open();
            try
            {
                var localAddress = string.Format("{0}/{1}", uri.OriginalString, address);
                var factory = new ChannelFactory<IMessageReceivingEndpointProxy>(binding, localAddress);
                var sender = new RestoringMessageSendingEndpoint(factory, systemDiagnostics);

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
            var count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectMessage(endpointId);

            var receiver = new MessageReceivingEndpoint(systemDiagnostics);
            receiver.OnNewMessage +=
                (s, e) =>
                {
                    if (count == 0)
                    {
                        count++;
                        throw new FaultException("Lets bail the first one");
                    }
                    else
                    {
                        Assert.AreEqual(endpointId, e.Message.OriginatingEndpoint);
                    }
                };

            var uri = new Uri("net.pipe://localhost/test/pipe");
            var host = new ServiceHost(receiver, uri);

            var binding = new NetNamedPipeBinding();
            var address = string.Format("{0}_{1}", "ThroughNamedPipe", Process.GetCurrentProcess().Id);
            host.AddServiceEndpoint(typeof(IMessageReceivingEndpoint), binding, address);

            host.Open();
            try
            {
                var localAddress = string.Format("{0}/{1}", uri.OriginalString, address);
                var factory = new ChannelFactory<IMessageReceivingEndpointProxy>(binding, localAddress);
                var sender = new RestoringMessageSendingEndpoint(factory, systemDiagnostics);

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
