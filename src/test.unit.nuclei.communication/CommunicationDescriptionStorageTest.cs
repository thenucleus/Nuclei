//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using NUnit.Framework;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommunicationDescriptionStorageTest
    {
        public interface IMockCommandSetWithTypedTaskReturn : ICommandSet
        {
            Task<int> MyMethod(int input);
        }

        public interface IMockNotificationSetWithEventHandlerEvent : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        [Test]
        public void RegisterApplicationSubject()
        {
            var storage = new CommunicationDescriptionStorage();

            var subject = new CommunicationSubject("a");
            storage.RegisterApplicationSubject(subject);

            Assert.That(storage.Subjects(), Is.EquivalentTo(new List<CommunicationSubject> { subject }));
        }

        [Test]
        public void RegisterCommandType()
        {
            var storage = new CommunicationDescriptionStorage();
            var type = typeof(IMockCommandSetWithTypedTaskReturn);
            storage.RegisterCommandType(type);

            var subject = new CommunicationSubject("a");
            storage.RegisterApplicationSubject(subject);

            var description = storage.ToStorage();
            Assert.That(
                description.CommandProxies,
                Is.EquivalentTo(
                    new List<ISerializedType> 
                    {  
                        ProxyExtensions.FromType(type),
                    }));
        }

        [Test]
        public void RegisterNotificationType()
        {
            var storage = new CommunicationDescriptionStorage();
            var type = typeof(IMockNotificationSetWithEventHandlerEvent);
            storage.RegisterNotificationType(type);

            var subject = new CommunicationSubject("a");
            storage.RegisterApplicationSubject(subject);

            var description = storage.ToStorage();
            Assert.That(
                description.NotificationProxies,
                Is.EquivalentTo(
                    new List<ISerializedType> 
                    {  
                        ProxyExtensions.FromType(type),
                    }));
        }

        [Test]
        public void ToStorage()
        {
            var storage = new CommunicationDescriptionStorage();

            var subject = new CommunicationSubject("a");
            storage.RegisterApplicationSubject(subject);

            var commandType = typeof(IMockCommandSetWithTypedTaskReturn);
            storage.RegisterCommandType(commandType);

            var notificationType = typeof(IMockNotificationSetWithEventHandlerEvent);
            storage.RegisterNotificationType(notificationType);

            var description = storage.ToStorage();

            Assert.That(
                description.Subjects,
                Is.EquivalentTo(
                    new List<CommunicationSubject> 
                    { 
                        subject 
                    }));
            Assert.That(
                description.CommandProxies,
                Is.EquivalentTo(
                    new List<ISerializedType> 
                    {  
                        ProxyExtensions.FromType(commandType),
                    }));
            Assert.That(
                description.NotificationProxies,
                Is.EquivalentTo(
                    new List<ISerializedType> 
                    {  
                        ProxyExtensions.FromType(notificationType),
                    }));
        }
    }
}
