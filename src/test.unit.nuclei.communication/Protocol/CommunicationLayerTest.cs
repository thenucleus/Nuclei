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

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommunicationLayerTest
    {
        [Test]
        public void OnEndpointApproved()
        {
            Assert.Ignore();
        }

        [Test]
        public void OnEndpointDisconnected()
        {
            Assert.Ignore();
        }

        [Test]
        public void SignIn()
        {
            Assert.Ignore();
        }

        [Test]
        public void SignInWhileSignedIn()
        {
            Assert.Ignore();
        }

        [Test]
        public void SignOut()
        {
            Assert.Ignore();
        }

        [Test]
        public void SignOutWithoutBeingSignedIn()
        {
            Assert.Ignore();
        }

        [Test]
        public void IsEndpointContactableWithUnknownEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void IsEndpointContactableWithNonContactableEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void IsEndpointContactableWithContactableEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToWithUnknownEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToWithUncontactableEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToWithUnopenedChannel()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToWithOpenChannel()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToAndWaitForResponseWithUnknownEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToAndWaitForResponseWithUncontactableEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToAndWaitForResponseWithUnopenedChannel()
        {
            Assert.Ignore();
        }

        [Test]
        public void SendMessageToAndWaitForResponseWithOpenedChannel()
        {
            Assert.Ignore();
        }

        [Test]
        public void UploadDataWithUnknownEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void UploadDataWithUncontactableEndpoint()
        {
            Assert.Ignore();
        }

        [Test]
        public void UploadDataWithUnopenedChannel()
        {
            Assert.Ignore();
        }

        [Test]
        public void UploadDataWithOpenedChannel()
        {
            Assert.Ignore();
        }
    }
}
