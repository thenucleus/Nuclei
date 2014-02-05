//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Autofac;
using Nuclei.Communication.Interaction;
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
                    c.Resolve<INotifyOfEndpointStateChange>(),
                    c.Resolve<CommandProxyBuilder>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<ISendCommandsToRemoteEndpoints>()
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
                            var layer = ctx.Resolve<ISendDataViaChannels>();
                            return SendMessageWithResponse(config, layer, endpoint, msg);
                        },
                        c.Resolve<SystemDiagnostics>());
                });
        }

        private static void RegisterCommandCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalCommandCollection(
                    c.Resolve<IStoreCommunicationDescriptions>()))
                .As<ICommandCollection>()
                .SingleInstance();
        }

        private static void RegisterNotificationHub(ContainerBuilder builder)
        {
            builder.Register(c => new RemoteNotificationHub(
                    c.Resolve<INotifyOfEndpointStateChange>(),
                    c.Resolve<NotificationProxyBuilder>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<INotifyOfRemoteEndpointEvents>()
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
                            var layer = ctx.Resolve<ISendDataViaChannels>();
                            SendMessageWithResponse(config, layer, endpoint, msg);
                        },
                        c.Resolve<SystemDiagnostics>());
                });
        }

        private static void RegisterNotificationCollection(ContainerBuilder builder)
        {
            builder.Register(c => new LocalNotificationCollection(
                    c.Resolve<ISendDataViaChannels>(),
                    c.Resolve<IStoreCommunicationDescriptions>()))
                .As<INotificationCollection>()
                .As<ISendNotifications>()
                .SingleInstance();
        }
    }
}
