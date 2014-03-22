//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Autofac;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects.Converters;
using Nuclei.Diagnostics;

namespace Nuclei.Communication
{
    /// <content>
    /// Defines the component registrations for the V1 part of the protocol namespace.
    /// </content>
    public sealed partial class CommunicationModule
    {
        private static void RegisterProtocolV1Endpoints(ContainerBuilder builder)
        {
            builder.Register(c => new MessageReceivingEndpoint(
                    c.Resolve<IEnumerable<IConvertCommunicationMessages>>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessagePipe>()
                .WithMetadata<IProtocolVersionMetaData>(m => m.For(meta => meta.Version, ProtocolVersions.V1));

            builder.Register(c => new DataReceivingEndpoint(
                    c.Resolve<SystemDiagnostics>()))
                .As<IDataPipe>()
                .WithMetadata<IProtocolVersionMetaData>(m => m.For(meta => meta.Version, ProtocolVersions.V1));
        }

        private static void RegisterProtocolV1MessageConverters(ContainerBuilder builder)
        {
            builder.Register(c => new DownloadRequestConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new EndpointConnectConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new EndpointDisconnectConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new FailureConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new SuccessConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new UnknownMessageTypeConverter())
                .As<IConvertCommunicationMessages>();
        }

        private static void RegisterV1SendingEndpoints(ContainerBuilder builder)
        {
            builder.Register(
                    (c, p) =>
                    {
                        var uri = p.TypedAs<Uri>();
                        var template = c.ResolveKeyed<IProtocolChannelTemplate>(uri.ToChannelTemplate());
                        return new RestoringMessageSendingEndpoint(
                            uri,
                            template,
                            c.Resolve<ProtocolDataContractResolver>(),
                            c.Resolve<IEnumerable<IConvertCommunicationMessages>>(),
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageSendingEndpoint>()
                .WithMetadata<IProtocolVersionMetaData>(m => m.For(meta => meta.Version, ProtocolVersions.V1));

            builder.Register(
                    (c, p) =>
                    {
                        var uri = p.TypedAs<Uri>();
                        var template = c.ResolveKeyed<IProtocolChannelTemplate>(uri.ToChannelTemplate());
                        return new RestoringDataTransferingEndpoint(
                            uri,
                            template,
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IDataTransferingEndpoint>()
                .WithMetadata<IProtocolVersionMetaData>(m => m.For(meta => meta.Version, ProtocolVersions.V1));
        }

        private static void RegisterV1ConnectionApprover(ContainerBuilder builder)
        {
            builder.Register(c => new EndpointConnectionApprover(
                    c.Resolve<IStoreProtocolSubjects>()))
                .As<IApproveEndpointConnections>();
        }
    }
}
