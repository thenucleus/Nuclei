//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Nuclei.Communication.Interaction;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInvokedMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("sendingEndpoint");
            var methodInvocation = ProxyExtensions.FromMethodInfo(MethodInfo.GetCurrentMethod(), new object[0]);
            var msg = new CommandInvokedMessage(id, methodInvocation);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(methodInvocation, msg.Invocation);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("sendingEndpoint");
            var methodInvocation = ProxyExtensions.FromMethodInfo(MethodInfo.GetCurrentMethod(), new object[0]);
            var msg = new CommandInvokedMessage(id, methodInvocation);
            var otherMsg = AssertExtensions.RoundTripSerialize(msg);

            Assert.AreEqual(id, otherMsg.Sender);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(MessageId.None, otherMsg.InResponseTo);
            Assert.That(
                otherMsg.Invocation, 
                Is.EqualTo(methodInvocation));
        }
    }
}
