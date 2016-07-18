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

namespace Nuclei
{
    [TestFixture]
    [SuppressMessage(
        "Microsoft.StyleCop.CSharp.DocumentationRules",
        "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class AssemblyExtensionsTest
    {
        private static string GetAssemblyDirectory(Assembly assembly)
        {
            var codebase = assembly.CodeBase;
            var uri = new Uri(codebase);
            return Path.GetDirectoryName(uri.LocalPath);
        }

        private static string GetAssemblyPath(Assembly assembly)
        {
            var codebase = assembly.CodeBase;
            var uri = new Uri(codebase);
            return uri.LocalPath;
        }

        [Test]
        public void LocalDirectoryPath()
        {
            // Note that this test isn't complete by a long shot. We should really test
            // networked paths too
            // and failures (i.e. dynamically generated code)
            // and ...???
            Assert.AreEqual(GetAssemblyDirectory(typeof(string).Assembly), typeof(string).Assembly.LocalDirectoryPath());
            Assert.AreEqual(GetAssemblyDirectory(typeof(SetUpAttribute).Assembly), typeof(SetUpAttribute).Assembly.LocalDirectoryPath());
            Assert.AreEqual(GetAssemblyDirectory(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly().LocalDirectoryPath());
        }

        [Test]
        public void LocalFilePath()
        {
            // Note that this test isn't complete by a long shot. We should really test
            // networked paths too
            // and failures (i.e. dynamically generated code)
            // and ...???
            Assert.AreEqual(GetAssemblyPath(typeof(string).Assembly), typeof(string).Assembly.LocalFilePath());
            Assert.AreEqual(GetAssemblyPath(typeof(SetUpAttribute).Assembly), typeof(SetUpAttribute).Assembly.LocalFilePath());
            Assert.AreEqual(GetAssemblyPath(Assembly.GetExecutingAssembly()), Assembly.GetExecutingAssembly().LocalFilePath());
        }

        [Test]
        public void StrongName()
        {
            var assembly = typeof(string).Assembly;
            Assert.IsTrue(assembly.IsStrongNamed());

            var strongName = assembly.StrongName();

            Assert.AreEqual(assembly.GetName().Name, strongName.Name);
            Assert.AreEqual(assembly.GetName().Version, strongName.Version);
        }
    }
}
