//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ProtocolInformationTest
    {
        [Test]
        public void CreateWithNullVersion()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ProtocolInformation(
                    null,
                    new Uri(@"net.pipe://localhost/pipe/sendingEndpoint"),
                    new Uri(@"net.pipe://localhost/pipe/sendingEndpoint/Data")));
        }

        [Test]
        public void CreateWithNullMessageAddress()
        {
            Assert.Throws<ArgumentNullException>(
                () => new ProtocolInformation(
                    new Version(), 
                    null,
                    new Uri(@"net.pipe://localhost/pipe/sendingEndpoint/Data")));
        }

        [Test]
        public void Create()
        {
            var version = new Version(1, 2, 3, 4);
            var messageUri = new Uri(@"net.pipe://localhost/pipe/sendingEndpoint");
            var dataUri = new Uri(@"net.pipe://localhost/pipe/sendingEndpoint/Data");
            var info = new ProtocolInformation(version, messageUri, dataUri);

            Assert.AreEqual(version, info.Version);
            Assert.AreSame(messageUri, info.MessageAddress);
            Assert.AreSame(dataUri, info.DataAddress);
        }
    }
}
