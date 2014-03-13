//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Autofac;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Interaction.Transport;
using Nuclei.Communication.Interaction.Transport.Messages.Processors;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Nuclei.Communication
{
    /// <content>
    /// Defines the component registrations for the interaction namespace.
    /// </content>
    public sealed partial class CommunicationModule
    {
        private static void RegisterCommandHub(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteCommandHub(
                    c.Resolve<IStoreInformationAboutEndpoints>(),
                    c.Resolve<CommandProxyBuilder>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<ISendCommandsToRemoteEndpoints>()
                .As<IStoreRemoteCommandProxies>()
                .SingleInstance();

            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommandProxyBuilder(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        (endpoint, msg) =>
                        {
                            var config = ctx.Resolve<IConfiguration>();
                            var layer = ctx.Resolve<IProtocolLayer>();
                            return SendMessageWithResponse(config, layer, endpoint, msg);
                        },
                        c.Resolve<SystemDiagnostics>());
                });
        }

        private static void RegisterCommandCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalCommandCollection())
                .As<ICommandCollection>()
                .SingleInstance();
        }

        private static void RegisterNotificationHub(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteNotificationHub(
                    c.Resolve<IStoreInformationAboutEndpoints>(),
                    c.Resolve<NotificationProxyBuilder>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<INotifyOfRemoteEndpointEvents>()
                .As<IStoreRemoteNotificationProxies>()
                .SingleInstance();

            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return new NotificationProxyBuilder(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        (endpoint, msg) =>
                        {
                            var config = ctx.Resolve<IConfiguration>();
                            var layer = ctx.Resolve<IProtocolLayer>();
                            SendMessageWithResponse(config, layer, endpoint, msg);
                        },
                        c.Resolve<SystemDiagnostics>());
                });
        }

        private static void RegisterNotificationCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalNotificationCollection(
                    c.Resolve<IProtocolLayer>()))
                .As<INotificationCollection>()
                .As<ISendNotifications>()
                .SingleInstance();
        }

        private static void RegisterInteractionMessageProcessingActions(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new CommandInvokedProcessAction(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            (endpoint, msg) =>
                            {
                                var config = ctx.Resolve<IConfiguration>();
                                var layer = ctx.Resolve<IProtocolLayer>();
                                SendMessageWithoutResponse(config, layer, endpoint, msg);
                            },
                            c.Resolve<ICommandCollection>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageProcessAction>();

            builder.Register(c => new EndpointInteractionInformationProcessAction(
                    c.Resolve<IHandleInteractionHandshakes>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new RegisterForNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new UnregisterFromNotificationProcessAction(
                    c.Resolve<ISendNotifications>()))
                .As<IMessageProcessAction>();

            builder.Register(c => new NotificationRaisedProcessAction(
                    c.Resolve<INotifyOfRemoteEndpointEvents>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessageProcessAction>();
        }

        private static void RegisterInteractionHandshakeConductor(ContainerBuilder builder)
        {
            builder.Register(c => new InteractionHandshakeConductor(
                    c.Resolve<IStoreInformationAboutEndpoints>(),
                    c.Resolve<IStoreInteractionSubjects>(),
                    c.Resolve<IStoreRemoteCommandProxies>(),
                    c.Resolve<IStoreRemoteNotificationProxies>(),
                    c.Resolve<IProtocolLayer>()))
                .As<IHandleInteractionHandshakes>()
                .SingleInstance();
        }

        private static void RegisterInteractionSubjectStorage(ContainerBuilder builder)
        {
            builder.Register(c => new InteractionSubjectGroupStorage())
                .As<IRegisterSubjectGroups>()
                .As<IStoreInteractionSubjects>()
                .SingleInstance();
        }

        private static void RegisterCommandRegistrationFunctions(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        RegisterLocalCommand func = (type, command, subjects) =>
                        {
                            type.VerifyThatTypeIsACorrectCommandSet();
                            if (!type.IsInstanceOfType(command))
                            {
                                throw new ArgumentException(Resources.Exceptions_Messages_CommandObjectMustImplementCommandInterface);
                            }

                            var collection = ctx.Resolve<ICommandCollection>();
                            var subjectCollection = ctx.Resolve<IRegisterSubjectGroups>();

                            collection.Register(type, command);
                            foreach (var subject in subjects)
                            {
                                subjectCollection.RegisterCommandForProvidedSubjectGroup(subject.Subject, type, subject.Version, subject.Group);
                            }
                        };

                        return func;
                    })
                .As<RegisterLocalCommand>()
                .SingleInstance();

            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        RegisterRequiredCommand func = (type, subjects) =>
                        {
                            type.VerifyThatTypeIsACorrectCommandSet();

                            var subjectCollection = ctx.Resolve<IRegisterSubjectGroups>();
                            foreach (var subject in subjects)
                            {
                                subjectCollection.RegisterCommandForRequiredSubjectGroup(subject.Subject, type, subject.Version, subject.Group);
                            }
                        };

                        return func;
                    })
                .As<RegisterRequiredCommand>()
                .SingleInstance();
        }

        private static void RegisterNotificationRegistrationFunctions(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        RegisterLocalNotification func = (type, notification, subjects) =>
                        {
                            type.VerifyThatTypeIsACorrectNotificationSet();
                            if (!type.IsInstanceOfType(notification))
                            {
                                throw new ArgumentException(Resources.Exceptions_Messages_NotificationObjectMustImplementNotificationInterface);
                            }

                            var collection = ctx.Resolve<INotificationCollection>();
                            var subjectCollection = ctx.Resolve<IRegisterSubjectGroups>();

                            collection.Register(type, notification);
                            foreach (var subject in subjects)
                            {
                                subjectCollection.RegisterNotificationForProvidedSubjectGroup(subject.Subject, type, subject.Version, subject.Group);
                            }
                        };

                        return func;
                    })
                .As<RegisterLocalNotification>()
                .SingleInstance();

            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        RegisterRequiredNotification func = (type, subjects) =>
                        {
                            type.VerifyThatTypeIsACorrectNotificationSet();

                            var subjectCollection = ctx.Resolve<IRegisterSubjectGroups>();
                            foreach (var subject in subjects)
                            {
                                subjectCollection.RegisterNotificationForRequiredSubjectGroup(subject.Subject, type, subject.Version, subject.Group);
                            }
                        };

                        return func;
                    })
                .As<RegisterRequiredNotification>()
                .SingleInstance();
        }

        private static void RegisterObjectSerializers(ContainerBuilder builder)
        {
            builder.Register(c => new NonTransformingObjectSerializer())
                .OnActivated(
                    a =>
                    {
                        var collection = a.Context.Resolve<IStoreObjectSerializers>();
                        collection.Add(a.Instance);
                    })
                .As<ISerializeObjectData>();
        }

        private static void RegisterObjectSerializerStorage(ContainerBuilder builder)
        {
            builder.Register(c => new ObjectSerializerStorage())
                .As<IStoreObjectSerializers>()
                .SingleInstance();
        }
    }
}
