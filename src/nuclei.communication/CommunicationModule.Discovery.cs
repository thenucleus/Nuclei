//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Autofac;
using Autofac.Features.Metadata;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Properties;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication
{
    /// <content>
    /// Defines the component registrations for the discovery namespace.
    /// </content>
    public sealed partial class CommunicationModule
    {
        private static void RegisterEndpointDiscoverySources(ContainerBuilder builder, bool allowChannelDiscovery)
        {
            if (allowChannelDiscovery)
            {
                builder.Register(
                        c =>
                        {
                            var translators = TranslatorsByVersion(c);
                            return new UdpBasedDiscoverySource(
                                translators,
                                c.Resolve<IDiscoveryChannelTemplate>(),
                                c.Resolve<SystemDiagnostics>());
                        })
                    .As<IDiscoverOtherServices>()
                    .SingleInstance();
            }

            // For now we're marking this as a single instance because
            // we want it to be linked to the CommunicationLayer at all times
            // and yet we want to be able to give it out to users without 
            // having to worry if we have given out the correct instance. Maybe
            // there is a cleaner solution to this problem though ...
            builder.Register(
                    c =>
                    {
                        var translators = TranslatorsByVersion(c);
                        return new ManualDiscoverySource(
                            translators,
                            c.Resolve<IDiscoveryChannelTemplate>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IDiscoverOtherServices>()
                .As<IAcceptExternalEndpointInformation>()
                .SingleInstance();
        }

        private static Tuple<Version, ITranslateVersionedChannelInformation>[] TranslatorsByVersion(IComponentContext ctx)
        {
            var allEndpointsLazy = ctx.Resolve<IEnumerable<Meta<ITranslateVersionedChannelInformation>>>();

            // Now find the reader that we want
            // This is done by comparing the version numbers. If the
            // first 2 digits of the input version number match the
            // first 2 digits of the version number stored in the
            // meta data, then we assume we found our reader.
            // This is based on the idea that if we change the
            // XML config format then we have to increment at least
            // the minor version number.
            return allEndpointsLazy
                .Select(r => new Tuple<Version, ITranslateVersionedChannelInformation>(r.Metadata["Version"] as Version, r.Value))
                .ToArray();
        }

        private static void RegisterManualEndpointConnection(ContainerBuilder builder)
        {
            // This function is used to resolve connection information from
            // a set of strings.
            builder.Register<ManualEndpointConnection>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return (id, address) =>
                    {
                        var diagnostics = ctx.Resolve<SystemDiagnostics>();
                        diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_ManualConnectionOfRemoteEndpoint_WithConnectionInformation,
                                id,
                                address));

                        ctx.Resolve<IAcceptExternalEndpointInformation>().RecentlyConnectedEndpoint(id, new Uri(address));
                    };
                })
                .SingleInstance();
        }

        private static void RegisterManualEndpointDisconnection(ContainerBuilder builder)
        {
            // This function is used to resolve connection information from
            // a set of strings.
            builder.Register<ManualEndpointDisconnection>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return id =>
                    {
                        var diagnostics = ctx.Resolve<SystemDiagnostics>();
                        diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_ManualDisconnectionOfRemoteEndpoint_WithConnectionInformation,
                                id));

                        ctx.Resolve<IAcceptExternalEndpointInformation>().RecentlyDisconnectedEndpoint(id);
                    };
                })
                .SingleInstance();
        }

        private static void RegisterBootstrapChannel(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    var channelTemplate = c.Resolve<IDiscoveryChannelTemplate>();
                    return new BootstrapChannel(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        channelTemplate,
                        c.Resolve<Func<Version, Tuple<Type, IVersionedDiscoveryEndpoint>>>(),
                        () => ctx.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IDiscoveryChannelTemplate), channelTemplate)));
                })
                .As<IBootstrapChannel>()
                .As<IDisposable>()
                .SingleInstance();
        }

        private static void RegisterVersionedDiscoveryEndpointSelector(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    Func<Version, Tuple<Type, IVersionedDiscoveryEndpoint>> selector =
                        version =>
                        {
                            // Get a collection of lazy resolved meta data objects for the
                            // IVersionedDiscoveryEndpoint
                            var allEndpointsLazy = ctx.Resolve<IEnumerable<Meta<IVersionedDiscoveryEndpoint>>>();

                            // Now find the reader that we want
                            // This is done by comparing the version numbers. If the
                            // first 2 digits of the input version number match the
                            // first 2 digits of the version number stored in the
                            // meta data, then we assume we found our reader.
                            // This is based on the idea that if we change the
                            // XML config format then we have to increment at least
                            // the minor version number.
                            IVersionedDiscoveryEndpoint selectedEndpoint = null;
                            foreach (var reader in allEndpointsLazy)
                            {
                                var storedVersion = reader.Metadata["Version"] as Version;
                                if (storedVersion.Equals(version))
                                {
                                    selectedEndpoint = reader.Value;
                                }
                            }

                            return new Tuple<Type, IVersionedDiscoveryEndpoint>(selectedEndpoint.GetType(), selectedEndpoint);
                        };

                    return selector;
                });
        }

        private static void RegisterDiscoveryChannelTemplate(ContainerBuilder builder)
        {
            builder.Register(c => new TcpDiscoveryChannelTemplate(
                    c.Resolve<IConfiguration>()))
                .As<IChannelTemplate>()
                .As<IDiscoveryChannelTemplate>();
        }

        private static void RegisterDiscoveryEndpoints(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var storage = c.Resolve<IStoreInformationForActiveChannels>();
                    return new Discovery.V1.InformationEndpoint(
                        storage.ActiveChannels().ToArray());
                })
                .As<Discovery.V1.IInformationEndpoint>()
                .As<IVersionedDiscoveryEndpoint>()
                .SingleInstance()
                .WithMetadata<IDiscoveryVersionMetaData>(
                    m => m.For(endpoint => endpoint.Version, DiscoveryVersions.V1));
        }

        private static void RegisterChannelInformationTranslators(ContainerBuilder builder)
        {
            builder.Register(c => new Discovery.V1.DiscoveryChannelTranslator(
                    Protocol.ProtocolVersions.SupportedVersions().ToArray(),
                    c.Resolve<IDiscoveryChannelTemplate>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<ITranslateVersionedChannelInformation>()
                .SingleInstance()
                .WithMetadata<IDiscoveryVersionMetaData>(
                    m => m.For(endpoint => endpoint.Version, DiscoveryVersions.V1));
        }
    }
}
