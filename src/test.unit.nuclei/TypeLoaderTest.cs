//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class TypeLoaderTest
    {
        [Test]
        public void FromPartialInformationWithTypeName()
        {
            var type = typeof(TypeLoader);
            var loadedType = TypeLoader.FromPartialInformation(type.FullName, throwOnError: false);
            Assert.AreEqual(type, loadedType);
        }

        [Test]
        public void FromPartialInformationWithTypeNameAndAssemblyName()
        {
            var type = typeof(TypeLoader);
            var loadedType = TypeLoader.FromPartialInformation(type.FullName, type.Assembly.GetName().Name, throwOnError: false);
            Assert.AreEqual(type, loadedType);
        }

        [Test]
        public void FromPartialInformationWithTypeNameAssemblyNameAndAssemblyVersion()
        {
            var type = typeof(TypeLoader);
            var loadedType = TypeLoader.FromPartialInformation(type.FullName, type.Assembly.GetName().Name, type.Assembly.GetName().Version, false);
            Assert.AreEqual(type, loadedType);
        }

        [Test]
        public void FromFullyQualifiedName()
        {
            var type = typeof(TypeLoader);
            var loadedType = TypeLoader.FromFullyQualifiedName(type.AssemblyQualifiedName, false);
            Assert.AreEqual(type, loadedType);
        }
    }
}
