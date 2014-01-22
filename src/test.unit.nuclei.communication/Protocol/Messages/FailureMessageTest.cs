//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class FailureMessageTest
    {
        [Test]
        public void RoundTripSerialise()
        {
            var id = new EndpointId("sendingEndpoint");
            var response = new MessageId();
            var msg = new FailureMessage(id, response);
            var otherMsg = AssertExtensions.RoundTripSerialize(msg);

            Assert.AreEqual(id, otherMsg.Sender);
            Assert.AreEqual(response, otherMsg.InResponseTo);
        }
    }
}
