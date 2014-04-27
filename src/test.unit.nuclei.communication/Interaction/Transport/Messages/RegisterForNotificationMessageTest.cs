//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport.Messages
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
            var notification = NotificationId.Create(
                typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler).GetEvent("OnMyEvent"));
            var msg = new RegisterForNotificationMessage(id, notification);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(notification, msg.Notification);
        }
    }
}
