//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
    public sealed class NotificationRaisedMessageTest
    {
        [Serializable]
        private sealed class MockEventArgs : EventArgs 
        {
            public MockEventArgs(int someValue)
            {
                Value = someValue;
            }

            public int Value
            {
                get;
                private set;
            }
        }

        [Test]
        public void Create()
        {
            var id = new EndpointId("sendingEndpoint");
            var notification = new SerializedEvent(new SerializedType("a", "a"), "b");
            var args = new MockEventArgs(1);
            var msg = new NotificationRaisedMessage(id, notification, args);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(notification, msg.Notification);
            Assert.AreSame(args, msg.Arguments);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("sendingEndpoint");
            var notification = new SerializedEvent(new SerializedType("a", "a"), "b");
            var args = new MockEventArgs(1);
            var msg = new NotificationRaisedMessage(id, notification, args);
            var otherMsg = AssertExtensions.RoundTripSerialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(notification, otherMsg.Notification);
            Assert.AreEqual(args.GetType(), otherMsg.Arguments.GetType());
            Assert.AreEqual(args.Value, ((MockEventArgs)otherMsg.Arguments).Value);
        }
    }
}
