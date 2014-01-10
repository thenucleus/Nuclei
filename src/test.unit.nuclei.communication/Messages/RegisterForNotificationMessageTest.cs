//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class RegisterForNotificationMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("sendingEndpoint");
            var notification = new SerializedEvent(new SerializedType("a", "a"), "b");
            var msg = new RegisterForNotificationMessage(id, notification);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(notification, msg.Notification);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("sendingEndpoint");
            var notification = new SerializedEvent(new SerializedType("a", "a"), "b");
            var msg = new RegisterForNotificationMessage(id, notification);
            var otherMsg = AssertExtensions.RoundTripSerialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(notification, otherMsg.Notification);
        }
    }
}
