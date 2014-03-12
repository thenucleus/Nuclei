//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointInteractionInformationResponseMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("a");
            var response = new MessageId();
            var state = InteractionConnectionState.Desired;
            var msg = new EndpointInteractionInformationResponseMessage(id, response, state);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(response, msg.InResponseTo);
            Assert.AreEqual(state, msg.State);
        }
    }
}
