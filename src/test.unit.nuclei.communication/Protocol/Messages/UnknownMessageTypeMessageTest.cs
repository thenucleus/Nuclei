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
    public sealed class UnknownMessageTypeMessageTest
    {
        [Test]
        public void Create()
        {
            var sender = new EndpointId("sendingEndpoint");
            var response = new MessageId();
            var msg = new UnknownMessageTypeMessage(sender, response);

            Assert.IsNotNull(msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(response, msg.InResponseTo);
        }

        [Test]
        public void CreateWithId()
        {
            var sender = new EndpointId("sendingEndpoint");
            var id = new MessageId();
            var response = new MessageId();
            var msg = new UnknownMessageTypeMessage(sender, id, response);

            Assert.AreSame(id, msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(response, msg.InResponseTo);
        }
    }
}
