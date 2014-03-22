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
        /// <summary>
        /// Stores the URI of the bootstrap channel for the current endpoint.
        /// </summary>
        private static Uri s_BootstrapChannelUri;

        private static void RegisterEndpointDiscoverySources(ContainerBuilder builder, bool allowChannelDiscovery)
        {
            if (allowChannelDiscovery)
            {
                builder.Register(
                        c =>
                        {
                            var ctx = c.Resolve<IComponentContext>();
                            var translators = TranslatorsByVersion(ctx);
                            return new UdpBasedDiscoverySource(
                                translators,
                                template => ctx.ResolveKeyed<IDiscoveryChannelTemplate>(template),
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
                        var ctx = c.Resolve<IComponentContext>();
                        var translators = TranslatorsByVersion(ctx);
                        return new ManualDiscoverySource(
                            translators,
                            template => ctx.ResolveKeyed<IDiscoveryChannelTemplate>(template),
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
                    var channelTemplate = c.ResolveKeyed<IDiscoveryChannelTemplate>(ChannelTemplate.NamedPipe);
                    return new BootstrapChannel(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        channelTemplate,
                        c.Resolve<Func<Version, ChannelTemplate, Tuple<Type, IVersionedDiscoveryEndpoint>>>(),
                        () => ctx.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IChannelTemplate), channelTemplate)),
                        uri => s_BootstrapChannelUri = uri);
                })
                .Keyed<IBootstrapChannel>(ChannelTemplate.NamedPipe)
                .As<IDisposable>()
                .SingleInstance();

            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    var channelTemplate = c.ResolveKeyed<IDiscoveryChannelTemplate>(ChannelTemplate.TcpIP);
                    return new BootstrapChannel(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        channelTemplate,
                        c.Resolve<Func<Version, ChannelTemplate, Tuple<Type, IVersionedDiscoveryEndpoint>>>(),
                        () => ctx.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IChannelTemplate), channelTemplate)),
                        uri => s_BootstrapChannelUri = uri);
                })
                .Keyed<IBootstrapChannel>(ChannelTemplate.TcpIP)
                .As<IDisposable>()
                .SingleInstance();
        }

        private static void RegisterLocalConnectionInformation(ContainerBuilder builder)
        {
            builder.Register(c => new LocalConnectionInformation(
                    EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                    () => s_BootstrapChannelUri))
                .As<IProvideLocalConnectionInformation>();
        }

        private static void RegisterVersionedDiscoveryEndpointSelector(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    Func<Version, ChannelTemplate, Tuple<Type, IVersionedDiscoveryEndpoint>> selector =
                        (version, template) =>
                        {
                            // Get a collection of lazy resolved meta data objects for the
                            // IVersionedDiscoveryEndpoint
                            var allEndpointsLazy = ctx.ResolveKeyed<IEnumerable<Meta<IVersionedDiscoveryEndpoint>>>(template);

                            // Now find the reader that we want
                            // This is done by comparing the version numbers. If the
                            // first 2 digits of the input version number match the
                            // first 2 digits of the version number stored in the
                            // meta data, then we assume we found our reader.
                            // This is based on the idea that if we change the
                            // XML config format then we have to increment at least
                            // the minor version number.
                            Type selectedType = null;
                            IVersionedDiscoveryEndpoint selectedEndpoint = null;
                            foreach (var endpoint in allEndpointsLazy)
                            {
                                var storedVersion = endpoint.Metadata["Version"] as Version;
                                var storedType = endpoint.Metadata["RegisteredType"] as Type;
                                if (storedVersion.Equals(version))
                                {
                                    selectedEndpoint = endpoint.Value;
                                    selectedType = storedType;
                                }
                            }

                            return new Tuple<Type, IVersionedDiscoveryEndpoint>(selectedType, selectedEndpoint);
                        };

                    return selector;
                });
        }

        private static void RegisterDiscoveryChannelTemplates(ContainerBuilder builder)
        {
            builder.Register(c => new NamedPipeDiscoveryChannelTemplate(
                    c.Resolve<IConfiguration>()))
                .As<IChannelTemplate>()
                .Keyed<IDiscoveryChannelTemplate>(ChannelTemplate.NamedPipe);

            builder.Register(c => new TcpDiscoveryChannelTemplate(
                    c.Resolve<IConfiguration>()))
                .As<IChannelTemplate>()
                .Keyed<IDiscoveryChannelTemplate>(ChannelTemplate.TcpIP);
        }
    }
}
