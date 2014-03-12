//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointInteractionInformationMessageTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("a");
            var group = new[]
                {
                    new CommunicationSubjectGroup(
                        new CommunicationSubject("a"),
                        new[]
                            {
                                new VersionedTypeFallback(
                                    new Tuple<OfflineTypeInformation, Version>(
                                        new OfflineTypeInformation(
                                            typeof(int).FullName,
                                            typeof(int).Assembly.GetName()),
                                        new Version(1, 0))),
                            },
                        new[]
                            {
                                new VersionedTypeFallback(
                                    new Tuple<OfflineTypeInformation, Version>(
                                        new OfflineTypeInformation(
                                            typeof(double).FullName,
                                            typeof(double).Assembly.GetName()),
                                        new Version(1, 2))),
                            }),
                };
            var msg = new EndpointInteractionInformationMessage(id, group);

            Assert.AreSame(id, msg.Sender);
            Assert.AreSame(group, msg.SubjectGroups);
        }
    }
}
