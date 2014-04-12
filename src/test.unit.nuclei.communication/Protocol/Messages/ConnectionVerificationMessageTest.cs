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
    public sealed class ConnectionVerificationMessageTest
    {
        [Test]
        public void Create()
        {
            var sender = new EndpointId("sendingEndpoint");
            var customData = new object();
            var msg = new ConnectionVerificationMessage(sender, customData);

            Assert.IsNotNull(msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreEqual(MessageId.None, msg.InResponseTo);
            Assert.AreSame(customData, msg.CustomData);
        }

        [Test]
        public void CreateWithId()
        {
            var sender = new EndpointId("sendingEndpoint");
            var id = new MessageId();
            var customData = new object();
            var msg = new ConnectionVerificationMessage(sender, id, customData);

            Assert.AreSame(id, msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreEqual(MessageId.None, msg.InResponseTo);
            Assert.AreSame(customData, msg.CustomData);
        }
    }
}
