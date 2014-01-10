//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointConnectMessageTest
    {
        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        public interface IMockNotificationSetWithEventHandler : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Test]
        public void Create()
        {
            var id = new EndpointId("sendingEndpoint");
            var channelType = ChannelType.TcpIP;
            var messageAddress = "bla";
            var dataAddress = "bladibla";
            var description = new CommunicationDescription(
                new Version(1, 0), 
                new List<CommunicationSubject>(), 
                new List<ISerializedType>(), 
                new List<ISerializedType>());
            var msg = new EndpointConnectMessage(id, channelType, messageAddress, dataAddress, description);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(messageAddress, msg.MessageAddress);
            Assert.AreEqual(channelType, msg.ChannelType);
            Assert.AreSame(description, msg.Information);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("sendingEndpoint");
            var channelType = ChannelType.TcpIP;
            var messageAddress = "bla";
            var dataAddress = "bladibla";
            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>
                    {
                        new CommunicationSubject("a")
                    },
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn))
                    },
                new List<ISerializedType>
                    {
                        ProxyExtensions.FromType(typeof(IMockNotificationSetWithEventHandler))
                    });
            var msg = new EndpointConnectMessage(id, channelType, messageAddress, dataAddress, description);
            var otherMsg = AssertExtensions.RoundTripSerialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(MessageId.None, otherMsg.InResponseTo);
            Assert.AreEqual(messageAddress, otherMsg.MessageAddress);
            Assert.AreEqual(channelType, otherMsg.ChannelType);
            Assert.AreEqual(description.CommunicationVersion, otherMsg.Information.CommunicationVersion);
            Assert.That(otherMsg.Information.Subjects, Is.EquivalentTo(description.Subjects));
            Assert.That(otherMsg.Information.CommandProxies, Is.EquivalentTo(description.CommandProxies));
            Assert.That(otherMsg.Information.NotificationProxies, Is.EquivalentTo(description.NotificationProxies));
        }
    }
}
