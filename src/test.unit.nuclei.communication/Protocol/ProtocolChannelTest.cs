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
    public sealed class ProtocolChannelTest
    {
        [Test]
        public void LocalConnectionPointForVersionWithUnknownVersion()
        {
        }

        [Test]
        public void LocalConnectionPointForVersion()
        {
        }

        [Test]
        public void OpenChannel()
        {
        }

        [Test]
        public void OpenChannelWithAlreadyOpenChannel()
        {
        }

        [Test]
        public void CloseChannel()
        {
        }

        [Test]
        public void CloseChannelWithNeverOpenedChannel()
        {
        }

        [Test]
        public void EndpointDisconnectedWithOpenConnection()
        {
        }

        [Test]
        public void EndpointDisconnectedWithUncontactableEndpoint()
        {
        }

        [Test]
        public void Send()
        {
        }

        [Test]
        public void SendWithUncontactableEndpoint()
        {
        }

        [Test]
        public void TransferData()
        {
        }

        [Test]
        public void TransferDataWithUncontactableEndpoint()
        {
        }

        [Test]
        public void OnMessageReception()
        {
        }

        [Test]
        public void OnDataReception()
        {
        }
    }
}
