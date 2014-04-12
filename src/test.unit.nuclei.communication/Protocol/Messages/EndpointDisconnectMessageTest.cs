//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointDisconnectMessageTest
    {
        [Test]
        public void Create()
        {
            var sender = new EndpointId("sendingEndpoint");
            var msg = new EndpointDisconnectMessage(sender);

            Assert.IsNotNull(msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreEqual(string.Empty, msg.ClosingReason);
            Assert.AreEqual(MessageId.None, msg.InResponseTo);
        }

        [Test]
        public void CreateWithReason()
        {
            var sender = new EndpointId("sendingEndpoint");
            var reason = "reason";
            var msg = new EndpointDisconnectMessage(sender, reason);
            
            Assert.IsNotNull(msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(reason, msg.ClosingReason);
            Assert.AreEqual(MessageId.None, msg.InResponseTo);
        }

        [Test]
        public void CreateWithIdAndReason()
        {
            var sender = new EndpointId("sendingEndpoint");
            var id = new MessageId();
            var reason = "reason";
            var msg = new EndpointDisconnectMessage(sender, id, reason);

            Assert.AreSame(id, msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(reason, msg.ClosingReason);
            Assert.AreEqual(MessageId.None, msg.InResponseTo);
        }
    }
}
