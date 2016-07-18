//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
    public sealed class AssemblyExtensionsSample
    {
        [Test]
        public void LocalFilePath()
        {
            var filePath = Assembly.GetExecutingAssembly().LocalFilePath();

            var path = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            Assert.AreEqual(path, filePath);
        }

        [Test]
        public void LocalDirectoryPath()
        {
            var directoryPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();

            var path = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            Assert.AreEqual(path, directoryPath);
        }

        [Test]
        public void StrongName()
        {
            var assembly = typeof(string).Assembly;
            var isStrongNamed = assembly.IsStrongNamed();

            Assert.IsTrue(isStrongNamed);

            var strongName = assembly.StrongName();

            Assert.AreEqual(assembly.GetName().Name, strongName.Name);
            Assert.AreEqual(assembly.GetName().Version, strongName.Version);
        }
    }
}
