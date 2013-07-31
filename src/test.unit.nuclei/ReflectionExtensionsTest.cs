//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using NUnit.Framework;

namespace Nuclei
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ReflectionExtensionsTest
    {
        [Test]
        public void IsSameWithIdenticalStrongNamedAssemblyName()
        {
            var first = typeof(string).Assembly.GetName();
            var second = typeof(string).Assembly.GetName();

            Assert.IsTrue(first.IsSame(second));
        }

        [Test]
        public void IsSameWithIdenticalNonStrongNamedAssemblyName()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };

            Assert.IsTrue(first.IsSame(second));
        }

        [Test]
        public void IsSameWithAssemblyNameWithDifferentName()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = Assembly.GetExecutingAssembly().GetName().Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };

            Assert.IsFalse(first.IsSame(second));
        }

        [Test]
        public void IsSameWithAssemblyNameWithDifferentCulture()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = new CultureInfo("en-US"),
                    Version = assemblyName.Version,
                };

            Assert.IsFalse(first.IsSame(second));
        }

        [Test]
        public void IsSameWithAssemblyNameWithDifferentVersion()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = new Version(1, 0, 0, 0),
                };

            Assert.IsFalse(first.IsSame(second));
        }

        [Test]
        public void IsSameWithAssemblyNameWithDifferentStrongName()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            first.SetPublicKey(assemblyName.GetPublicKey());
            first.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            second.SetPublicKey(Assembly.GetExecutingAssembly().GetName().GetPublicKey());
            second.SetPublicKeyToken(Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken());

            Assert.IsFalse(first.IsSame(second));
        }

        [Test]
        public void IsMatchWithIdenticalStrongNamedAssemblyName()
        {
            var first = typeof(string).Assembly.GetName();
            var second = typeof(string).Assembly.GetName();

            Assert.IsTrue(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithStrongNamedAssemblyNameWithDifferentName()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            first.SetPublicKey(assemblyName.GetPublicKey());
            first.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            var second = new AssemblyName
                {
                    Name = Assembly.GetExecutingAssembly().GetName().Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            second.SetPublicKey(assemblyName.GetPublicKey());
            second.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            Assert.IsFalse(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithStrongNamedAssemblyNameWithDifferentCulture()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            first.SetPublicKey(assemblyName.GetPublicKey());
            first.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = new CultureInfo("en-US"),
                    Version = assemblyName.Version,
                };
            second.SetPublicKey(assemblyName.GetPublicKey());
            second.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            Assert.IsFalse(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithStrongNamedAssemblyNameWithDifferentVersion()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            first.SetPublicKey(assemblyName.GetPublicKey());
            first.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = new Version(1, 0, 0, 0),
                };
            second.SetPublicKey(assemblyName.GetPublicKey());
            second.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            Assert.IsFalse(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithStrongNamedAssemblyNameWithDifferentStrongName()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            first.SetPublicKey(assemblyName.GetPublicKey());
            first.SetPublicKeyToken(assemblyName.GetPublicKeyToken());

            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            second.SetPublicKey(Assembly.GetExecutingAssembly().GetName().GetPublicKey());
            second.SetPublicKeyToken(Assembly.GetExecutingAssembly().GetName().GetPublicKeyToken());

            Assert.IsFalse(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithIdenticalNonStrongNamedAssemblyName()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };

            Assert.IsTrue(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithNonStrongNamedAssemblyNameWithDifferentName()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = Assembly.GetExecutingAssembly().GetName().Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };

            Assert.IsFalse(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithNonStrongNamedAssemblyNameWithDifferentCulture()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = new CultureInfo("en-US"),
                    Version = assemblyName.Version,
                };

            Assert.IsFalse(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithNonStrongNamedAssemblyNameWithLowerVersion()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = new Version(assemblyName.Version.Major - 1, assemblyName.Version.Minor, 0, 0),
                };

            Assert.IsTrue(first.IsMatch(second));
        }

        [Test]
        public void IsMatchWithNonStrongNamedAssemblyNameWithHigherVersion()
        {
            var assemblyName = typeof(string).Assembly.GetName();
            var first = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = assemblyName.Version,
                };
            var second = new AssemblyName
                {
                    Name = assemblyName.Name,
                    CultureInfo = assemblyName.CultureInfo,
                    Version = new Version(assemblyName.Version.Major + 1, 0, 0, 0),
                };

            Assert.IsFalse(first.IsMatch(second));
        }
    }
}
