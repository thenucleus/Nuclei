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
    public sealed class EndpointIdExtensionsTest
    {
        [Test]
        public void Deserialize()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var text = id.ToString();

            var otherId = EndpointIdExtensions.Deserialize(text);
            Assert.AreEqual(id, otherId);
        }

        [Test]
        public void DeserializeWithEmptyString()
        {
            Assert.Throws<ArgumentException>(() => EndpointIdExtensions.Deserialize(string.Empty));
        }

        [Test]
        public void IsOnMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            Assert.IsTrue(id.IsOnMachine(Environment.MachineName));
            Assert.IsFalse(id.IsOnMachine("foobar"));
        }

        [Test]
        public void IsOnLocalMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            Assert.IsTrue(id.IsOnLocalMachine()); 
        }

        [Test]
        public void OriginatesOnMachine()
        {
            var id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess();
            var machineName = id.OriginatesOnMachine();
            Assert.AreEqual(Environment.MachineName, machineName);
        }
    }
}
