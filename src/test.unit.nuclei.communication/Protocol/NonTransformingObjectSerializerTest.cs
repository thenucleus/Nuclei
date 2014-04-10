//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NonTransformingObjectSerializerTest
    {
        [Test]
        public void Serialize()
        {
            var serializer = new NonTransformingObjectSerializer();

            var input = new object();
            var output = serializer.Serialize(input);
            Assert.AreSame(input, output);
        }

        [Test]
        public void Deserialize()
        {
            var serializer = new NonTransformingObjectSerializer();

            var input = new object();
            var output = serializer.Deserialize(input);
            Assert.AreSame(input, output);
        }
    }
}
