//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Linq;
using Autofac;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Discovery.V1;
using Nuclei.Diagnostics;

namespace Nuclei.Communication
{
    /// <content>
    /// Defines the component registrations for the discovery namespace.
    /// </content>
    public sealed partial class CommunicationModule
    {
        private static void RegisterDiscoveryV1Endpoints(ContainerBuilder builder)
        {
            builder.Register(
                c =>
                {
                    var storage = c.Resolve<IStoreInformationForActiveChannels>();
                    return new InformationEndpoint(
                        storage.ActiveChannels().ToArray());
                })
                .As<IInformationEndpoint>()
                .As<IVersionedDiscoveryEndpoint>()
                .SingleInstance()
                .WithMetadata<IDiscoveryVersionMetaData>(
                    m => m.For(meta => meta.Version, DiscoveryVersions.V1));
        }

        private static void RegisterDiscoveryV1ChannelInformationTranslators(ContainerBuilder builder)
        {
            builder.Register(c => new DiscoveryChannelTranslator(
                    Protocol.ProtocolVersions.SupportedVersions().ToArray(),
                    c.Resolve<IDiscoveryChannelTemplate>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<ITranslateVersionedChannelInformation>()
                .SingleInstance()
                .WithMetadata<IDiscoveryVersionMetaData>(
                    m => m.For(meta => meta.Version, DiscoveryVersions.V1));
        }
    }
}
