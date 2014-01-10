//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInvokedResponseMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("sendingEndpoint");
            var response = new MessageId();
            var result = 10;
            var msg = new CommandInvokedResponseMessage(id, response, result);

            Assert.AreSame(id, msg.OriginatingEndpoint);
            Assert.AreSame(response, msg.InResponseTo);
            Assert.AreEqual(result, (int)msg.Result);
        }

        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("sendingEndpoint");
            var response = new MessageId();
            var result = 10;
            var msg = new CommandInvokedResponseMessage(id, response, result);
            var otherMsg = AssertExtensions.RoundTripSerialize(msg);

            Assert.AreEqual(id, otherMsg.OriginatingEndpoint);
            Assert.AreEqual(response, otherMsg.InResponseTo);
            Assert.AreEqual(msg.Id, otherMsg.Id);
            Assert.AreEqual(result, (int)otherMsg.Result);
        }
    }
}
