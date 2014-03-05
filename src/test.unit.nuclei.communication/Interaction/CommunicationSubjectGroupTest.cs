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
    public sealed class CommunicationSubjectGroupTest
    {
        [Test]
        public void Create()
        {
            var subject = new CommunicationSubject("a");
            var commands = new[]
                {
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(string).FullName, typeof(string).Assembly.GetName()), 
                            new Version(1, 2))), 
                };
            var notifications = new[]
                {
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(TestAttribute).FullName, typeof(TestAttribute).Assembly.GetName()), 
                            new Version(3, 4))), 
                };

            var group = new CommunicationSubjectGroup(subject, commands, notifications);
            Assert.AreSame(subject, group.Subject);
            Assert.AreSame(commands, group.Commands);
            Assert.AreSame(notifications, group.Notifications);
        }
    }
}
