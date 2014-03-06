//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class SubjectGroupIdentifierTest
    {
        [Test]
        public void Create()
        {
            var subject = new CommunicationSubject("a");
            var version = new Version(1, 0);
            var group = "b";
            var identifier = new SubjectGroupIdentifier(subject, version, group);

            Assert.AreSame(subject, identifier.Subject);
            Assert.AreSame(version, identifier.Version);
            Assert.AreSame(group, identifier.Group);
        }
    }
}
