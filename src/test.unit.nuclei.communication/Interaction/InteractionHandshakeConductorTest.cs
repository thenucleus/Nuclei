//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class InteractionHandshakeConductorTest
    {
        [Test]
        public void HandshakeWithSuccessResponseWithLocalSendFirst()
        {
            Assert.Ignore();
        }

        [Test]
        public void HandshakeWithSuccessResponseWithRemoteSendFirst()
        {
            Assert.Ignore();
        }

        [Test]
        public void HandshakeWithRemoteRejection()
        {
            Assert.Ignore();
        }

        [Test]
        public void HandshakeWithLocalRejection()
        {
            Assert.Ignore();
        }
    }
}
