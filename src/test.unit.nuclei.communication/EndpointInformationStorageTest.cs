//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointInformationStorageTest
    {
        [Test]
        public void TryAddWithNullEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var connection = new ChannelConnectionInformation(
                new EndpointId("a"), 
                ChannelType.NamedPipe, 
                new Uri("http://localhost"));
            Assert.IsFalse(storage.TryAdd(null, connection));
        }

        [Test]
        public void TryAddWithNullConnectionInformation()
        {
            var storage = new EndpointInformationStorage();
            Assert.IsFalse(storage.TryAdd(new EndpointId("a"), null));
        }

        [Test]
        public void TryAddWithExistingEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri("http://localhost"));
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            var newConnection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.TcpIP,
                new Uri("http://localhost"));
            Assert.IsFalse(storage.TryAdd(endpoint, newConnection));

            ChannelConnectionInformation otherConnection;
            var result = storage.TryGetConnectionFor(endpoint, out otherConnection);
            Assert.IsTrue(result);
            Assert.AreSame(connection, otherConnection);
        }

        [Test]
        public void TryAdd()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                new EndpointId("a"),
                ChannelType.NamedPipe,
                new Uri("http://localhost"));
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            ChannelConnectionInformation otherConnection;
            var result = storage.TryGetConnectionFor(endpoint, out otherConnection);
            Assert.IsTrue(result);
            Assert.AreSame(connection, otherConnection);
        }

        [Test]
        public void TryStartApproveWithNullEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());
            Assert.IsFalse(storage.TryStartApproval(null, description));
        }

        [Test]
        public void TryStartApproveWithNullDescription()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            Assert.IsFalse(storage.TryStartApproval(endpoint, null));
        }

        [Test]
        public void TryStartApproveWithUnknownEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());
            Assert.IsFalse(storage.TryStartApproval(endpoint, description));
        }

        [Test]
        public void TryStartApproveWithApprovedEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                new EndpointId("a"),
                ChannelType.NamedPipe,
                new Uri("http://localhost"));
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());
            Assert.IsTrue(storage.TryStartApproval(endpoint, description));
            Assert.IsTrue(storage.TryCompleteApproval(endpoint));
            Assert.IsFalse(storage.TryStartApproval(endpoint, description));
        }

        [Test]
        public void TryStartApprove()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                new EndpointId("a"),
                ChannelType.NamedPipe,
                new Uri("http://localhost"));
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());
            Assert.IsTrue(storage.TryStartApproval(endpoint, description));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsFalse(storage.HasBeenContacted(endpoint));
            Assert.IsTrue(storage.IsWaitingForApproval(endpoint));
        }

        [Test]
        public void TryApproveWithNullEndpoint()
        {
            var storage = new EndpointInformationStorage();
            Assert.IsFalse(storage.TryCompleteApproval(null));
        }

        [Test]
        public void TryApproveWithApprovedEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri("http://localhost"));
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());
            Assert.IsTrue(storage.TryStartApproval(endpoint, description));
            Assert.IsTrue(storage.TryCompleteApproval(endpoint));
            Assert.IsFalse(storage.TryCompleteApproval(endpoint));
        }

        [Test]
        public void TryApproveWithNonAddedEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            Assert.IsFalse(storage.TryCompleteApproval(endpoint));
        }

        [Test]
        public void TryApprove()
        {
            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri("http://localhost"));

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());

            var storage = new EndpointInformationStorage();

            var wasApproved = false;
            storage.OnEndpointConnected += 
                (s, e) =>
                {
                    wasApproved = true;
                    Assert.AreSame(connection, e.ConnectionInformation);
                    Assert.AreSame(description, e.Description);
                };

            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            Assert.IsTrue(storage.TryStartApproval(endpoint, description));
            Assert.IsTrue(storage.TryCompleteApproval(endpoint));
            Assert.IsTrue(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsFalse(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));
            Assert.IsTrue(wasApproved);
        }

        [Test]
        public void TryUpdateWithNullInformation()
        {
            var storage = new EndpointInformationStorage();
            Assert.IsFalse(storage.TryUpdate(null));
        }

        [Test]
        public void TryUpdateWithApprovedEndpoint()
        {
            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri("http://localhost"));

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());

            var storage = new EndpointInformationStorage();

            var wasApproved = false;
            storage.OnEndpointConnected +=
                (s, e) =>
                {
                    wasApproved = true;
                    Assert.AreSame(connection, e.ConnectionInformation);
                    Assert.AreSame(description, e.Description);
                };

            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsTrue(storage.TryStartApproval(endpoint, description));
            Assert.IsTrue(storage.TryCompleteApproval(endpoint));
            Assert.IsTrue(wasApproved);

            var newConnection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri(@"http://localhost"),
                new Uri(@"http://localhost/data"));
            Assert.IsFalse(storage.TryUpdate(newConnection));
        }

        [Test]
        public void TryUpdateWithContactedEndpoint()
        {
            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri("http://localhost"));

            var storage = new EndpointInformationStorage();
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            
            var newConnection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri(@"http://localhost"),
                new Uri(@"http://localhost/data"));
            Assert.IsTrue(storage.TryUpdate(newConnection));

            ChannelConnectionInformation storedConnection;
            storage.TryGetConnectionFor(endpoint, out storedConnection);
            Assert.AreSame(newConnection, storedConnection);
        }

        [Test]
        public void TryUpdateWithApprovalUnderWay()
        {
            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri("http://localhost"));

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());

            var storage = new EndpointInformationStorage();
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsTrue(storage.TryStartApproval(endpoint, description));

            var newConnection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri(@"http://localhost"),
                new Uri(@"http://localhost/data"));
            Assert.IsTrue(storage.TryUpdate(newConnection));

            ChannelConnectionInformation storedConnection;
            storage.TryGetConnectionFor(endpoint, out storedConnection);
            Assert.AreSame(newConnection, storedConnection);
        }

        [Test]
        public void TryRemoveWithNullEndpoint()
        {
            var storage = new EndpointInformationStorage();
            Assert.IsFalse(storage.TryRemoveEndpoint(null));
        }

        [Test]
        public void TryRemoveWithUnknownEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            Assert.IsFalse(storage.TryRemoveEndpoint(endpoint));
        }

        [Test]
        public void TryRemoveWithNonApprovedEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                new EndpointId("a"),
                ChannelType.NamedPipe,
                new Uri("http://localhost"));
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            Assert.IsTrue(storage.TryRemoveEndpoint(endpoint));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));
        }

        [Test]
        public void TryRemoveWithApprovedEndpoint()
        {
            var storage = new EndpointInformationStorage();

            var endpoint = new EndpointId("a");
            var connection = new ChannelConnectionInformation(
                endpoint,
                ChannelType.NamedPipe,
                new Uri("http://localhost"));
            Assert.IsTrue(storage.TryAdd(endpoint, connection));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsTrue(storage.HasBeenContacted(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());
            Assert.IsTrue(storage.TryStartApproval(endpoint, description));
            Assert.IsTrue(storage.TryCompleteApproval(endpoint));
            Assert.IsTrue(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));

            Assert.IsTrue(storage.TryRemoveEndpoint(endpoint));
            Assert.IsFalse(storage.CanCommunicateWithEndpoint(endpoint));
            Assert.IsFalse(storage.IsWaitingForApproval(endpoint));
            Assert.IsFalse(storage.HasBeenContacted(endpoint));
        }
    }
}
