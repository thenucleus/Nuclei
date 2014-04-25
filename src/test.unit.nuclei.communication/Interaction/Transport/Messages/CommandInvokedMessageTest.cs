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
    public sealed class CommandInvokedMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("sendingEndpoint");
            var commandId = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));
            var invocationData = new CommandInvokedData(
                commandId,
                new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "other", CommandParameterOrigin.FromCommand), 
                            1.0), 
                    });
            var msg = new CommandInvokedMessage(id, invocationData);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(invocationData, msg.Invocation);
        }
    }
}
