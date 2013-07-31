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
    public sealed class EndpointSignInEventArgsTest
    {
        [Test]
        public void Create()
        {
            var endpoint = new EndpointId("id");
            var channelType = ChannelType.TcpIP;
            var uri = new Uri("http://localhost");

            var info = new ChannelConnectionInformation(endpoint, channelType, uri);
            var description = new CommunicationDescription(
                new Version(1, 0),
                new List<CommunicationSubject>(),
                new List<ISerializedType>(),
                new List<ISerializedType>());
            var args = new EndpointSignInEventArgs(info, description);

            Assert.AreSame(info, args.ConnectionInformation);
            Assert.AreSame(description, args.Description);
        }
    }
}
