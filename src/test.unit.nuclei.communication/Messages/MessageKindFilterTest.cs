//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using NUnit.Framework;

namespace Nuclei.Communication.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MessageKindFilterTest
    {
        [Test]
        public void PassThroughWithPassingMessage()
        {
            var filter = new MessageKindFilter(typeof(SuccessMessage));
            Assert.IsTrue(filter.PassThrough(new SuccessMessage(new EndpointId("id"), new MessageId())));
        }

        [Test]
        public void PassThroughWithNonPassingMessage()
        {
            var filter = new MessageKindFilter(typeof(SuccessMessage));
            Assert.IsFalse(filter.PassThrough(new FailureMessage(new EndpointId("id"), new MessageId())));
        }
    }
}
