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
            var commandData = new CommandData(typeof(int), "a");
            var invocationData = new CommandInvokedData(
                commandData,
                new[]
                    {
                        new Tuple<Type, object>(typeof(double), 1.0), 
                    });
            var msg = new CommandInvokedMessage(id, invocationData);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(invocationData, msg.Invocation);
        }
    }
}
