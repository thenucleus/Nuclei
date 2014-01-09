﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ChannelConnectionInformationTest
    {
        [Test]
        public void CreateWithIncorrectChannelType()
        {
            Assert.Throws<InvalidChannelTypeException>(
                () => new ChannelConnectionInformation(
                    new EndpointId("a"), 
                    ChannelType.None, 
                    new Uri(@"net.pipe://localhost/pipe/sendingEndpoint"),
                    new Uri(@"net.pipe://localhost/pipe/sendingEndpoint/Data")));
        }

        [Test]
        public void Create()
        {
            var endpoint = new EndpointId("a");
            var type = ChannelType.NamedPipe;
            var messageUri = new Uri(@"net.pipe://localhost/pipe/sendingEndpoint");
            var dataUri = new Uri(@"net.pipe://localhost/pipe/sendingEndpoint/Data");
            var info = new ChannelConnectionInformation(endpoint, type, messageUri, dataUri);

            Assert.AreSame(endpoint, info.Id);
            Assert.AreEqual(type, info.ChannelType);
            Assert.AreSame(messageUri, info.MessageAddress);
            Assert.AreSame(dataUri, info.DataAddress);
        }
    }
}
