//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Autofac;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Interaction.Transport;
using Nuclei.Communication.Interaction.Transport.Messages.Processors;
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
                            var layer = ctx.Resolve<IProtocolLayer>();
                            var configuration = ctx.Resolve<IConfiguration>();
                            var sendTimeout = configuration.HasValueFor(CommunicationConfigurationKeys.WaitForResponseTimeoutInMilliSeconds)
                                ? TimeSpan.FromMilliseconds(
                                    configuration.Value<int>(CommunicationConfigurationKeys.WaitForResponseTimeoutInMilliSeconds))
                                : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultWaitForResponseTimeoutInMilliSeconds);

                            return SendMessageWithResponse(layer, endpoint, msg, sendTimeout);
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
                .As<IRaiseProxyNotifications>()
                .SingleInstance();

            builder.Register(c => new NotificationProxyBuilder(
                EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                c.Resolve<SendMessage>(),
                c.Resolve<SystemDiagnostics>()));
        }

        private static void RegisterNotificationCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalNotificationCollection(
                    EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    c.Resolve<SendMessage>()))
                .As<INotificationCollection>()
                .As<ISendNotifications>()
                .SingleInstance();
        }

        private static void RegisterInteractionMessageProcessingActions(ContainerBuilder builder)
        {
            builder.Register(c => new CommandInvokedProcessAction(
                    EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    c.Resolve<SendMessage>(),
                    c.Resolve<ICommandCollection>(),
                    c.Resolve<SystemDiagnostics>()))
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
                    c.Resolve<IRaiseProxyNotifications>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessageProcessAction>();
        }

        private static void RegisterInteractionHandshakeConductor(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var configuration = c.Resolve<IConfiguration>();
                        var sendTimeout = configuration.HasValueFor(CommunicationConfigurationKeys.WaitForResponseTimeoutInMilliSeconds)
                            ? TimeSpan.FromMilliseconds(
                                configuration.Value<int>(CommunicationConfigurationKeys.WaitForResponseTimeoutInMilliSeconds))
                            : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultWaitForResponseTimeoutInMilliSeconds);

                        return new InteractionHandshakeConductor(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            c.Resolve<IStoreInformationAboutEndpoints>(),
                            c.Resolve<IStoreInteractionSubjects>(),
                            c.Resolve<IStoreRemoteCommandProxies>(),
                            c.Resolve<IStoreRemoteNotificationProxies>(),
                            c.Resolve<SendMessage>(),
                            c.Resolve<SendMessageAndWaitForResponse>(),
                            sendTimeout,
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IHandleInteractionHandshakes>()
                .SingleInstance();
        }

        private static void RegisterInteractionSubjectStorage(ContainerBuilder builder)
        {
            builder.Register(c => new InteractionSubjectGroupStorage())
                .As<IRegisterSubjectGroups>()
                .As<IStoreInteractionSubjects>()
                .As<IStoreProtocolSubjects>()
                .SingleInstance();
        }

        private static void RegisterCommandRegistrationFunctions(ContainerBuilder builder)
        {
            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        RegisterCommand func = (map, subjects) =>
                        {
                            var collection = ctx.Resolve<ICommandCollection>();
                            var subjectCollection = ctx.Resolve<IRegisterSubjectGroups>();

                            collection.Register(map.Definitions);
                            foreach (var subject in subjects)
                            {
                                subjectCollection.RegisterCommandForProvidedSubjectGroup(
                                    subject.Subject, 
                                    map.CommandType, 
                                    subject.Version, 
                                    subject.Group);
                            }
                        };

                        return func;
                    })
                .As<RegisterCommand>()
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
                        RegisterNotification func = (map, subjects) =>
                        {
                            var collection = ctx.Resolve<INotificationCollection>();
                            var subjectCollection = ctx.Resolve<IRegisterSubjectGroups>();

                            collection.Register(map.Definitions);
                            foreach (var subject in subjects)
                            {
                                subjectCollection.RegisterNotificationForProvidedSubjectGroup(
                                    subject.Subject, 
                                    map.NotificationType,
                                    subject.Version, 
                                    subject.Group);
                            }
                        };

                        return func;
                    })
                .As<RegisterNotification>()
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
                .As<ISerializeObjectData>()
                .SingleInstance();
        }

        private static void RegisterObjectSerializerStorage(ContainerBuilder builder)
        {
            builder.Register(c => new ObjectSerializerStorage())
                .OnActivated(
                    a =>
                    {
                        var serializers = a.Context.Resolve<IEnumerable<ISerializeObjectData>>();
                        foreach (var serializer in serializers)
                        {
                            a.Instance.Add(serializer);
                        }
                    })
                .As<IStoreObjectSerializers>()
                .SingleInstance();
        }
    }
}
