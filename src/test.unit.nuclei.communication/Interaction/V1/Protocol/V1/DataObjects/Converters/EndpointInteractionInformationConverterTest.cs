//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects.Converters
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointInteractionInformationConverterTest
    {
        [Test]
        public void MessageTypeToTranslate()
        {
            var translator = new EndpointInteractionInformationConverter();
            Assert.AreEqual(typeof(EndpointInteractionInformationMessage), translator.MessageTypeToTranslate);
        }

        [Test]
        public void DataTypeToTranslate()
        {
            var translator = new EndpointInteractionInformationConverter();
            Assert.AreEqual(typeof(EndpointInteractionInformationData), translator.DataTypeToTranslate);
        }

        [Test]
        public void ToMessageWithNonMatchingDataType()
        {
            var translator = new EndpointInteractionInformationConverter();

            var data = new SuccessData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(UnknownMessageTypeMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreSame(data.InResponseTo, msg.InResponseTo);
        }

        [Test]
        public void ToMessage()
        {
            var translator = new EndpointInteractionInformationConverter();

            var data = new EndpointInteractionInformationData
            {
                Id = new MessageId(),
                InResponseTo = new MessageId(),
                Sender = new EndpointId("a"),
                Groups = new[]
                    {
                        new SerializedSubjectInformation
                            {
                                Subject = "a",
                                Commands = new[]
                                    {
                                        new SerializedTypeFallback
                                            {
                                                Types = new[]
                                                    {
                                                        new SerializedVersionedType
                                                            {
                                                                Type = new SerializedType
                                                                    {
                                                                        FullName = typeof(int).FullName,
                                                                        AssemblyName = typeof(int).Assembly.GetName().Name,
                                                                    },
                                                                Version = new Version(1, 0)
                                                            }, 
                                                    }
                                            }, 
                                    },
                                Notifications = new[]
                                    {
                                        new SerializedTypeFallback
                                            {
                                                Types = new[]
                                                    {
                                                         new SerializedVersionedType
                                                            {
                                                                Type = new SerializedType
                                                                    {
                                                                        FullName = typeof(double).FullName,
                                                                        AssemblyName = typeof(double).Assembly.GetName().Name,
                                                                    },
                                                                Version = new Version(1, 0)
                                                            }, 
                                                    }
                                            }, 
                                    },
                            }, 
                    }
            };
            var msg = translator.ToMessage(data);
            Assert.IsInstanceOf(typeof(EndpointInteractionInformationMessage), msg);
            Assert.AreSame(data.Id, msg.Id);
            Assert.AreSame(data.Sender, msg.Sender);
            Assert.AreEqual(data.Groups.Length, ((EndpointInteractionInformationMessage)msg).SubjectGroups.Length);
            Assert.AreEqual(new CommunicationSubject(data.Groups[0].Subject), ((EndpointInteractionInformationMessage)msg).SubjectGroups[0].Subject);
            Assert.AreEqual(data.Groups[0].Commands.Length, ((EndpointInteractionInformationMessage)msg).SubjectGroups[0].Commands.Length);
            Assert.IsTrue(
                ((EndpointInteractionInformationMessage)msg).SubjectGroups[0].Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                typeof(int).FullName,
                                typeof(int).Assembly.GetName()), 
                            new Version(1, 0)))));
            Assert.IsFalse(
                ((EndpointInteractionInformationMessage)msg).SubjectGroups[0].Commands[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                typeof(double).FullName,
                                typeof(double).Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.AreEqual(data.Groups[0].Notifications.Length, ((EndpointInteractionInformationMessage)msg).SubjectGroups[0].Notifications.Length);
            Assert.IsFalse(
                ((EndpointInteractionInformationMessage)msg).SubjectGroups[0].Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                typeof(int).FullName,
                                typeof(int).Assembly.GetName()),
                            new Version(1, 0)))));
            Assert.IsTrue(
                ((EndpointInteractionInformationMessage)msg).SubjectGroups[0].Notifications[0].IsPartialMatch(
                    new VersionedTypeFallback(
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(
                                typeof(double).FullName,
                                typeof(double).Assembly.GetName()),
                            new Version(1, 0)))));
        }

        [Test]
        public void FromMessageWithNonMatchingMessageType()
        {
            var translator = new EndpointInteractionInformationConverter();

            var msg = new SuccessMessage(new EndpointId("a"), new MessageId());
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(UnknownMessageTypeData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
        }

        [Test]
        public void FromMessage()
        {
            var translator = new EndpointInteractionInformationConverter();

            var msg = new EndpointInteractionInformationMessage(
                new EndpointId("a"),
                new[]
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
                    });
            var data = translator.FromMessage(msg);
            Assert.IsInstanceOf(typeof(EndpointInteractionInformationData), data);
            Assert.AreSame(msg.Id, data.Id);
            Assert.AreSame(msg.Sender, data.Sender);
            Assert.AreSame(msg.InResponseTo, data.InResponseTo);
            Assert.AreEqual(msg.SubjectGroups.Length, ((EndpointInteractionInformationData)data).Groups.Length);

            Assert.AreEqual(
                msg.SubjectGroups[0].Subject,
                new CommunicationSubject(((EndpointInteractionInformationData)data).Groups[0].Subject));
            Assert.AreEqual(
                msg.SubjectGroups[0].Commands.Length,
                ((EndpointInteractionInformationData)data).Groups[0].Commands.Length);
            Assert.AreEqual(1, ((EndpointInteractionInformationData)data).Groups[0].Commands[0].Types.Length);
            Assert.AreEqual(
                typeof(int).FullName,
                ((EndpointInteractionInformationData)data).Groups[0].Commands[0].Types[0].Type.FullName);
            Assert.AreEqual(
                typeof(int).Assembly.GetName().Name,
                ((EndpointInteractionInformationData)data).Groups[0].Commands[0].Types[0].Type.AssemblyName);
            Assert.AreEqual(
                new Version(1, 0), 
                ((EndpointInteractionInformationData)data).Groups[0].Commands[0].Types[0].Version);
            Assert.AreEqual(
                msg.SubjectGroups[0].Notifications.Length,
                ((EndpointInteractionInformationData)data).Groups[0].Notifications.Length);
            Assert.AreEqual(1, ((EndpointInteractionInformationData)data).Groups[0].Notifications[0].Types.Length);
            Assert.AreEqual(
                typeof(double).FullName,
                ((EndpointInteractionInformationData)data).Groups[0].Notifications[0].Types[0].Type.FullName);
            Assert.AreEqual(
                typeof(double).Assembly.GetName().Name,
                ((EndpointInteractionInformationData)data).Groups[0].Notifications[0].Types[0].Type.AssemblyName);
            Assert.AreEqual(
                new Version(1, 2),
                ((EndpointInteractionInformationData)data).Groups[0].Notifications[0].Types[0].Version);
        }
    }
}
