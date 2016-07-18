//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Nuclei.Samples
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage(
        "Microsoft.Naming",
        "CA1704:IdentifiersShouldBeSpelledCorrectly",
        MessageId = "Extracter",
        Justification = "Noun version of extraction. Seems reasonable.")]
    public sealed class EmbeddedResourceExtracterSample
    {
        [Test]
        public void LoadFromEmbeddedStream()
        {
            var stream = EmbeddedResourceExtracter.LoadEmbeddedStream(
                Assembly.GetExecutingAssembly(),
                "Nuclei.Samples.ExtracterFile.txt");
            using (var reader = new StreamReader(stream))
            {
                var text = reader.ReadToEnd();
                Assert.IsNotNull(text);
                Assert.Greater(text.Length, 0);
            }
        }

        [Test]
        public void LoadFromEmbeddedTextFile()
        {
            var text = EmbeddedResourceExtracter.LoadEmbeddedTextFile(
                Assembly.GetExecutingAssembly(),
                "Nuclei.Samples.ExtracterFile.txt");
            Assert.IsNotNull(text);
            Assert.Greater(text.Length, 0);
        }
    }
}
