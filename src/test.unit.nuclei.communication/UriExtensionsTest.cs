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
    public sealed class UriExtensionsTest
    {
        [Test]
        public void ToChannelTemplateForNetPipe()
        {
            var uri = new Uri("net.pipe://localhost");
            Assert.AreEqual(ChannelTemplate.NamedPipe, uri.ToChannelTemplate());
        }

        [Test]
        public void ToChannelTemplateForNetTcp()
        {
            var uri = new Uri("net.tcp://localhost");
            Assert.AreEqual(ChannelTemplate.TcpIP, uri.ToChannelTemplate());
        }

        [Test]
        public void ToChannelTemplateForHttp()
        {
            var uri = new Uri("http://localhost");
            Assert.AreEqual(ChannelTemplate.Http, uri.ToChannelTemplate());
        }

        [Test]
        public void ToChannelTemplateForHttps()
        {
            var uri = new Uri("https://localhost");
            Assert.AreEqual(ChannelTemplate.Https, uri.ToChannelTemplate());
        }
    }
}
