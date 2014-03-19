//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class InteractionSubjectGroupStorageTest
    {
        [Test]
        public void RegisterCommandForProvidedSubjectGroupWithFirstCommand()
        {
            var storage = new InteractionSubjectGroupStorage();
            
            var subject = new CommunicationSubject("a");
            var type = typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn);
            var version = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterCommandForProvidedSubjectGroup(subject, type, version, groupId);
            Assert.IsTrue(storage.ProvidedSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupProvisionsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupRequirementsForSubject(subject));

            var group = storage.GroupProvisionsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Notifications.Length);
            Assert.AreEqual(1, group.Commands.Length);
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                type.FullName,
                                type.Assembly.GetName()), 
                            new Version(1, 0)))));
        }

        [Test]
        public void RegisterCommandForProvidedSubjectGroupWithSecondCommandInGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn);
            var firstVersion = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterCommandForProvidedSubjectGroup(subject, firstType, firstVersion, groupId);

            var secondType = typeof(InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn);
            var secondVersion = new Version(1, 1);
            storage.RegisterCommandForProvidedSubjectGroup(subject, secondType, secondVersion, groupId);
            Assert.IsTrue(storage.ProvidedSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupProvisionsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupRequirementsForSubject(subject));

            var group = storage.GroupProvisionsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Notifications.Length);
            Assert.AreEqual(1, group.Commands.Length);
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 1)))));
        }

        [Test]
        public void RegisterCommandForProvidedSubjectGroupForSecondGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn);
            var firstVersion = new Version(1, 0);
            var firstGroupId = "group1";
            storage.RegisterCommandForProvidedSubjectGroup(subject, firstType, firstVersion, firstGroupId);

            var secondType = typeof(InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn);
            var secondVersion = new Version(1, 0);
            var secondGroupId = "group2";
            storage.RegisterCommandForProvidedSubjectGroup(subject, secondType, secondVersion, secondGroupId);
            Assert.IsTrue(storage.ProvidedSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupProvisionsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupRequirementsForSubject(subject));

            var group = storage.GroupProvisionsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Notifications.Length);
            Assert.AreEqual(2, group.Commands.Length);
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));

            Assert.IsTrue(
                group.Commands[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Commands[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
        }

        [Test]
        public void RegisterNotificationForProvidedSubjectGroupWithFirstNotification()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var type = typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler);
            var version = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterNotificationForProvidedSubjectGroup(subject, type, version, groupId);
            Assert.IsTrue(storage.ProvidedSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupProvisionsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupRequirementsForSubject(subject));

            var group = storage.GroupProvisionsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Commands.Length);
            Assert.AreEqual(1, group.Notifications.Length);
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                type.FullName,
                                type.Assembly.GetName()),
                            new Version(1, 0)))));
        }

        [Test]
        public void RegisterNotificationForProvidedSubjectGroupWithSecondNotificationInGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler);
            var firstVersion = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterNotificationForProvidedSubjectGroup(subject, firstType, firstVersion, groupId);

            var secondType = typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler);
            var secondVersion = new Version(1, 1);
            storage.RegisterNotificationForProvidedSubjectGroup(subject, secondType, secondVersion, groupId);
            Assert.IsTrue(storage.ProvidedSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupProvisionsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupRequirementsForSubject(subject));

            var group = storage.GroupProvisionsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Commands.Length);
            Assert.AreEqual(1, group.Notifications.Length);
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 1)))));
        }

        [Test]
        public void RegisterNotificationForProvidedSubjectGroupForSecondGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler);
            var firstVersion = new Version(1, 0);
            var firstGroupId = "group1";
            storage.RegisterNotificationForProvidedSubjectGroup(subject, firstType, firstVersion, firstGroupId);

            var secondType = typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler);
            var secondVersion = new Version(1, 0);
            var secondGroupId = "group2";
            storage.RegisterNotificationForProvidedSubjectGroup(subject, secondType, secondVersion, secondGroupId);
            Assert.IsTrue(storage.ProvidedSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupProvisionsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupRequirementsForSubject(subject));

            var group = storage.GroupProvisionsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Commands.Length);
            Assert.AreEqual(2, group.Notifications.Length);
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));

            Assert.IsTrue(
                group.Notifications[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Notifications[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
        }

        [Test]
        public void RegisterCommandForRequiredSubjectGroupWithFirstCommand()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var type = typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn);
            var version = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterCommandForRequiredSubjectGroup(subject, type, version, groupId);
            Assert.IsTrue(storage.RequiredSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupRequirementsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupProvisionsForSubject(subject));

            var group = storage.GroupRequirementsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Notifications.Length);
            Assert.AreEqual(1, group.Commands.Length);
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                type.FullName,
                                type.Assembly.GetName()),
                            new Version(1, 0)))));
        }

        [Test]
        public void RegisterCommandForRequiredSubjectGroupWithSecondCommandInGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn);
            var firstVersion = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterCommandForRequiredSubjectGroup(subject, firstType, firstVersion, groupId);

            var secondType = typeof(InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn);
            var secondVersion = new Version(1, 1);
            storage.RegisterCommandForRequiredSubjectGroup(subject, secondType, secondVersion, groupId);
            Assert.IsTrue(storage.RequiredSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupRequirementsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupProvisionsForSubject(subject));

            var group = storage.GroupRequirementsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Notifications.Length);
            Assert.AreEqual(1, group.Commands.Length);
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 1)))));
        }

        [Test]
        public void RegisterCommandForRequiredSubjectGroupForSecondGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn);
            var firstVersion = new Version(1, 0);
            var firstGroupId = "group1";
            storage.RegisterCommandForRequiredSubjectGroup(subject, firstType, firstVersion, firstGroupId);

            var secondType = typeof(InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn);
            var secondVersion = new Version(1, 0);
            var secondGroupId = "group2";
            storage.RegisterCommandForRequiredSubjectGroup(subject, secondType, secondVersion, secondGroupId);
            Assert.IsTrue(storage.RequiredSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupRequirementsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupProvisionsForSubject(subject));

            var group = storage.GroupRequirementsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Notifications.Length);
            Assert.AreEqual(2, group.Commands.Length);
            Assert.IsTrue(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));

            Assert.IsTrue(
                group.Commands[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Commands[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
        }

        [Test]
        public void RegisterNotificationForRequiredSubjectGroupWithFirstNotification()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var type = typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler);
            var version = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterNotificationForRequiredSubjectGroup(subject, type, version, groupId);
            Assert.IsTrue(storage.RequiredSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupRequirementsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupProvisionsForSubject(subject));

            var group = storage.GroupRequirementsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Commands.Length);
            Assert.AreEqual(1, group.Notifications.Length);
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                type.FullName,
                                type.Assembly.GetName()),
                            new Version(1, 0)))));
        }

        [Test]
        public void RegisterNotificationForRequiredSubjectGroupWithSecondNotificationInGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler);
            var firstVersion = new Version(1, 0);
            var groupId = "group1";
            storage.RegisterNotificationForRequiredSubjectGroup(subject, firstType, firstVersion, groupId);

            var secondType = typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler);
            var secondVersion = new Version(1, 1);
            storage.RegisterNotificationForRequiredSubjectGroup(subject, secondType, secondVersion, groupId);
            Assert.IsTrue(storage.RequiredSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupRequirementsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupProvisionsForSubject(subject));

            var group = storage.GroupRequirementsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Commands.Length);
            Assert.AreEqual(1, group.Notifications.Length);
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 1)))));
        }

        [Test]
        public void RegisterNotificationForRequiredSubjectGroupForSecondGroup()
        {
            var storage = new InteractionSubjectGroupStorage();

            var subject = new CommunicationSubject("a");
            var firstType = typeof(InteractionExtensionsTest.IMockNotificationSetWithEventHandler);
            var firstVersion = new Version(1, 0);
            var firstGroupId = "group1";
            storage.RegisterNotificationForRequiredSubjectGroup(subject, firstType, firstVersion, firstGroupId);

            var secondType = typeof(InteractionExtensionsTest.IMockNotificationSetWithTypedEventHandler);
            var secondVersion = new Version(1, 0);
            var secondGroupId = "group2";
            storage.RegisterNotificationForRequiredSubjectGroup(subject, secondType, secondVersion, secondGroupId);
            Assert.IsTrue(storage.RequiredSubjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.Subjects().Any(s => s.Equals(subject)));
            Assert.IsTrue(storage.ContainsGroupRequirementsForSubject(subject));
            Assert.IsFalse(storage.ContainsGroupProvisionsForSubject(subject));

            var group = storage.GroupRequirementsFor(subject);
            Assert.AreSame(subject, group.Subject);
            Assert.AreEqual(0, group.Commands.Length);
            Assert.AreEqual(2, group.Notifications.Length);
            Assert.IsTrue(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));

            Assert.IsTrue(
                group.Notifications[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                secondType.FullName,
                                secondType.Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsFalse(
                group.Notifications[1].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                firstType.FullName,
                                firstType.Assembly.GetName()),
                            new Version(1, 0)))));
        }
    }
}
