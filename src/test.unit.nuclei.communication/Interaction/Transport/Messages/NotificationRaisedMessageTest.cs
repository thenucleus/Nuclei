//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport.Messages
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
            var notification = new NotificationData(typeof(int), "b");
            var args = new MockEventArgs(1);
            var notificationRaised = new NotificationRaisedData(notification, args);
            var msg = new NotificationRaisedMessage(id, notificationRaised);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(notification, msg.Notification);
            Assert.AreSame(args, msg.Notification.EventArgs);
        }
    }
}
