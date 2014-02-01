//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Autofac.Features.Metadata;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.Messages.Processors;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Nuclei.Communication
{
    /// <content>
    /// Defines the component registrations for the protocol namespace.
    /// </content>
    public sealed partial class CommunicationModule
    {
        private static void RegisterCommunicationLayer(ContainerBuilder builder, IEnumerable<ChannelTemplate> allowedChannelTemplates)
        {
            builder.Register(
                c =>
                {
                    // Autofac 2.4.5 forces the 'c' variable to disappear. See here:
                    // http://stackoverflow.com/questions/5383888/autofac-registration-issue-in-release-v2-4-5-724
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommunicationLayer(
                        c.Resolve<IStoreInformationAboutEndpoints>(),
                        c.Resolve<IEnumerable<IDiscoverOtherServices>>(),
                        (t, id) => Tuple.Create(
                            ctx.ResolveKeyed<ICommunicationChannel>(t, new TypedParameter(typeof(EndpointId), id)),
                            ctx.Resolve<IDirectIncomingMessages>()),
                        allowedChannelTemplates,
                        c.Resolve<SystemDiagnostics>());
                })
                .As<ICommunicationLayer>()
                .SingleInstance();
        }

        private static void RegisterHandshakeLayer(ContainerBuilder builder, IEnumerable<ChannelTemplate> allowedChannelTemplates)
        {
            builder.Register(
                c => new HandshakeConductor(
                    c.Resolve<IStoreEndpointApprovalState>(),
                    c.Resolve<IProvideLocalConnectionInformation>(),
                    c.Resolve<IEnumerable<IDiscoverOtherServices>>(),
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<IStoreCommunicationDescriptions>(),
                    c.Resolve<IEnumerable<IApproveEndpointConnections>>(),
                    allowedChannelTemplates,
                    c.Resolve<SystemDiagnostics>()))
                .As<IHandleHandshakes>()
                .SingleInstance();
        }

        private static void RegisterMessageHandler(ContainerBuilder builder)
        {
            builder.Register(c => new MessageHandler(
                    c.Resolve<IStoreInformationAboutEndpoints>(),
                    c.Resolve<SystemDiagnostics>()))
                .OnActivated(AttachMessageProcessingActions)
                .As<IProcessIncomingMessages>()
                .As<IDirectIncomingMessages>()
                .SingleInstance();
        }

        private static void AttachMessageProcessingActions(IActivatedEventArgs<MessageHandler> args)
        {
            var handler = args.Instance;
            var filterActions = args.Context.Resolve<IEnumerable<IMessageProcessAction>>();
            foreach (var action in filterActions)
            {
                handler.ActOnArrival(
                   new MessageKindFilter(action.MessageTypeToProcess),
                   action);
            }
        }

        private static void RegisterDataHandler(ContainerBuilder builder)
        {
            builder.Register(c => new DataHandler(
                    c.Resolve<SystemDiagnostics>()))
                .OnActivated(AttachLayer)
                .As<IProcessIncomingData>()
                .As<IDirectIncomingData>()
                .SingleInstance();
        }

        private static void AttachLayer(IActivatedEventArgs<DataHandler> args)
        {
            var handler = args.Instance;
            var layer = args.Context.Resolve<ICommunicationLayer>();
            layer.OnEndpointDisconnected += (s, e) => handler.OnEndpointSignedOff(e.Endpoint);
        }

        private static void RegisterMessageProcessingActions(ContainerBuilder builder)
        {
            builder.Register(c => new DataDownloadProcessAction(
                    c.Resolve<IStoreUploads>(),
                    c.Resolve<ICommunicationLayer>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessageProcessAction>();

            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new EndpointConnectProcessAction(
                            c.Resolve<IHandleHandshakes>(),
                            from channelType in ctx.Resolve<IEnumerable<IProtocolChannelTemplate>>() select channelType.ChannelTemplate,
                            c.Resolve<SystemDiagnostics>());
                    })
                .As<IMessageProcessAction>();

            builder.Register(c => new EndpointDisconnectProcessAction(
                    c.Resolve<IStoreEndpointApprovalState>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessageProcessAction>();

            builder.Register(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return new UnknownMessageTypeProcessAction(
                        EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                        (endpoint, msg) =>
                        {
                            var config = ctx.Resolve<IConfiguration>();
                            var layer = ctx.Resolve<ICommunicationLayer>();
                            SendMessageWithoutResponse(config, layer, endpoint, msg);
                        },
                        c.Resolve<SystemDiagnostics>());
                })
                .As<IMessageProcessAction>();

            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new CommandInvokedProcessAction(
                            EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                            (endpoint, msg) =>
                            {
                                var config = ctx.Resolve<IConfiguration>();
                                var layer = ctx.Resolve<ICommunicationLayer>();
                                SendMessageWithoutResponse(config, layer, endpoint, msg);
                            },
                            c.Resolve<ICommandCollection>(),
                            c.Resolve<SystemDiagnostics>());
                    })
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

        private static void SendMessageWithoutResponse(
            IConfiguration configuration,
            ICommunicationLayer layer,
            EndpointId endpoint,
            ICommunicationMessage message)
        {
            if (!layer.IsEndpointContactable(endpoint))
            {
                var timeout = configuration.HasValueFor(CommunicationConfigurationKeys.WaitForConnectionTimeoutInMilliseconds)
                    ? TimeSpan.FromMilliseconds(configuration.Value<int>(CommunicationConfigurationKeys.WaitForConnectionTimeoutInMilliseconds))
                    : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultWaitForConnectionTimeoutInMilliSeconds);

                var resetEvent = new AutoResetEvent(false);
                var notifier = Observable.FromEventPattern<EndpointEventArgs>(
                        h => layer.OnEndpointConnected += h,
                        h => layer.OnEndpointConnected -= h)
                    .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                    .Take(1)
                    .Subscribe(args => resetEvent.Set());

                using (notifier)
                {
                    if (!layer.IsEndpointContactable(endpoint))
                    {
                        resetEvent.WaitOne(timeout);
                    }
                }
            }

            layer.SendMessageTo(endpoint, message);
        }

        private static Task<ICommunicationMessage> SendMessageWithResponse(
            IConfiguration configuration,
            ICommunicationLayer layer,
            EndpointId endpoint,
            ICommunicationMessage message)
        {
            if (!layer.IsEndpointContactable(endpoint))
            {
                var timeout = configuration.HasValueFor(CommunicationConfigurationKeys.WaitForConnectionTimeoutInMilliseconds)
                       ? TimeSpan.FromMilliseconds(configuration.Value<int>(CommunicationConfigurationKeys.WaitForConnectionTimeoutInMilliseconds))
                       : TimeSpan.FromMilliseconds(CommunicationConstants.DefaultWaitForConnectionTimeoutInMilliSeconds);

                var resetEvent = new AutoResetEvent(false);
                var notifier = Observable.FromEventPattern<EndpointEventArgs>(
                        h => layer.OnEndpointConnected += h,
                        h => layer.OnEndpointConnected -= h)
                    .Where(args => args.EventArgs.Endpoint.Equals(endpoint))
                    .Take(1)
                    .Subscribe(args => resetEvent.Set());

                using (notifier)
                {
                    if (!layer.IsEndpointContactable(endpoint))
                    {
                        resetEvent.WaitOne(timeout);
                    }
                }
            }

            return layer.SendMessageAndWaitForResponse(endpoint, message);
        }

        private static void RegisterConnectionHolders(ContainerBuilder builder)
        {
            builder.Register((c, p) => new ServiceConnectionHolder(
                    p.TypedAs<IChannelTemplate>(),
                    () => DateTimeOffset.Now,
                    c.Resolve<SystemDiagnostics>()))
                .As<IHoldServiceConnections>();
        }

        private static void RegisterCommunicationChannel(ContainerBuilder builder)
        {
            // CommunicationChannel.
            // Register one channel for each communication type. At the moment
            // that is only named pipe and TCP.
            builder.Register(
                (c, p) =>
                {
                    var channelTemplate = c.ResolveKeyed<IProtocolChannelTemplate>(ChannelTemplate.NamedPipe);
                    var ctx = c.Resolve<IComponentContext>();

                    return new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<IStoreInformationAboutEndpoints>(),
                        channelTemplate,
                        () => ctx.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IProtocolChannelTemplate), channelTemplate)),
                        BuildMessagePipeSelector(ctx),
                        BuildDataPipeSelector(ctx),
                        (id, msgProxy, dataProxy) => ctx.Resolve<ISendingEndpoint>(
                            new TypedParameter(
                                typeof(EndpointId),
                                id),
                            new TypedParameter(
                                typeof(Func<EndpointId, IMessageSendingEndpoint>),
                                msgProxy),
                            new TypedParameter(
                                typeof(Func<EndpointId, IDataTransferingEndpoint>),
                                dataProxy)),
                        BuildMessageSenderSelector(ctx),
                        BuildDataTransferSelector(ctx));
                })
                .OnActivated(ConnectToMessageHandler)
                .Keyed<ICommunicationChannel>(ChannelTemplate.NamedPipe)
                .SingleInstance();

            builder.Register(
                (c, p) =>
                {
                    var channelTemplate = c.ResolveKeyed<IProtocolChannelTemplate>(ChannelTemplate.TcpIP);
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<IStoreInformationAboutEndpoints>(),
                        channelTemplate,
                        () => ctx.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IChannelTemplate), channelTemplate)),
                        BuildMessagePipeSelector(ctx),
                        BuildDataPipeSelector(ctx),
                        (id, msgProxy, dataProxy) => ctx.Resolve<ISendingEndpoint>(
                            new TypedParameter(
                                typeof(EndpointId),
                                id),
                            new TypedParameter(
                                typeof(Func<EndpointId, IMessageSendingEndpoint>),
                                msgProxy),
                            new TypedParameter(
                                typeof(Func<EndpointId, IDataTransferingEndpoint>),
                                dataProxy)),
                        BuildMessageSenderSelector(ctx),
                        BuildDataTransferSelector(ctx));
                })
                .OnActivated(ConnectToMessageHandler)
                .Keyed<ICommunicationChannel>(ChannelTemplate.TcpIP)
                .SingleInstance();
        }

        private static Func<Version, Tuple<Type, IMessagePipe>> BuildMessagePipeSelector(IComponentContext context)
        {
            Func<Version, Tuple<Type, IMessagePipe>> result =
                version =>
                {
                    var allPipesLazy = context.Resolve<IEnumerable<Meta<IMessagePipe>>>();

                    IMessagePipe selectedPipe = null;
                    foreach (var pipe in allPipesLazy)
                    {
                        var storedVersion = pipe.Metadata["Version"] as Version;
                        if (storedVersion.Equals(version))
                        {
                            selectedPipe = pipe.Value;
                        }
                    }

                    return new Tuple<Type, IMessagePipe>(selectedPipe.GetType(), selectedPipe);
                };

            return result;
        }

        private static Func<Version, Tuple<Type, IDataPipe>> BuildDataPipeSelector(IComponentContext context)
        {
            Func<Version, Tuple<Type, IDataPipe>> result =
                version =>
                {
                    var allPipesLazy = context.Resolve<IEnumerable<Meta<IDataPipe>>>();

                    IDataPipe selectedPipe = null;
                    foreach (var pipe in allPipesLazy)
                    {
                        var storedVersion = pipe.Metadata["Version"] as Version;
                        if (storedVersion.Equals(version))
                        {
                            selectedPipe = pipe.Value;
                        }
                    }

                    return new Tuple<Type, IDataPipe>(selectedPipe.GetType(), selectedPipe);
                };

            return result;
        }

        private static Func<Version, Uri, IMessageSendingEndpoint> BuildMessageSenderSelector(IComponentContext context)
        {
            Func<Version, Uri, IMessageSendingEndpoint> result =
                (version, uri) =>
                {
                    var allEndpointsLazy = context.Resolve<IEnumerable<Meta<IMessageSendingEndpoint>>>(
                        new TypedParameter(typeof(Uri), uri));

                    IMessageSendingEndpoint selectedPipe = null;
                    foreach (var endpoint in allEndpointsLazy)
                    {
                        var storedVersion = endpoint.Metadata["Version"] as Version;
                        if (storedVersion.Equals(version))
                        {
                            selectedPipe = endpoint.Value;
                        }
                    }

                    return selectedPipe;
                };

            return result;
        }

        private static Func<Version, Uri, IDataTransferingEndpoint> BuildDataTransferSelector(IComponentContext context)
        {
            Func<Version, Uri, IDataTransferingEndpoint> result =
                (version, uri) =>
                {
                    var allEndpointsLazy = context.Resolve<IEnumerable<Meta<IDataTransferingEndpoint>>>(
                        new TypedParameter(typeof(Uri), uri));

                    IDataTransferingEndpoint selectedPipe = null;
                    foreach (var endpoint in allEndpointsLazy)
                    {
                        var storedVersion = endpoint.Metadata["Version"] as Version;
                        if (storedVersion.Equals(version))
                        {
                            selectedPipe = endpoint.Value;
                        }
                    }

                    return selectedPipe;
                };

            return result;
        }

        private static void ConnectToMessageHandler(IActivatedEventArgs<ICommunicationChannel> args)
        {
            var messageHandler = args.Context.Resolve<IProcessIncomingMessages>();
            args.Instance.OnMessageReception += (s, e) => messageHandler.ProcessMessage(e.Message);
            args.Instance.OnClosed += (s, e) => messageHandler.OnLocalChannelClosed();

            var dataHandler = args.Context.Resolve<IProcessIncomingData>();
            args.Instance.OnDataReception += (s, e) => dataHandler.ProcessData(e.Data);
            args.Instance.OnClosed += (s, e) => dataHandler.OnLocalChannelClosed();
        }

        private static void RegisterEndpoints(ContainerBuilder builder)
        {
            builder.Register((c, p) => new SendingEndpoint(
                    p.TypedAs<EndpointId>(),
                    p.TypedAs<Func<EndpointId, IMessageSendingEndpoint>>(),
                    p.TypedAs<Func<EndpointId, IDataTransferingEndpoint>>()))
                .As<ISendingEndpoint>();
        }

        private static void RegisterChannelTypes(ContainerBuilder builder)
        {
            builder.Register(c => new NamedPipeProtocolChannelTemplate(
                    c.Resolve<IConfiguration>()))
                .As<IChannelTemplate>()
                .Keyed<IProtocolChannelTemplate>(ChannelTemplate.NamedPipe);

            builder.Register(c => new TcpProtocolChannelTemplate(
                    c.Resolve<IConfiguration>()))
                .As<IChannelTemplate>()
                .Keyed<IProtocolChannelTemplate>(ChannelTemplate.TcpIP);
        }

        private static void RegisterEndpointStorage(ContainerBuilder builder)
        {
            builder.Register(c => new EndpointInformationStorage())
                .As<IStoreInformationAboutEndpoints>()
                .As<INotifyOfEndpointStateChange>()
                .SingleInstance();
        }

        private static void RegisterCommunicationDescriptions(ContainerBuilder builder, IEnumerable<CommunicationSubject> subjects)
        {
            builder.Register(c => new CommunicationDescriptionStorage())
                .OnActivated(
                    a =>
                    {
                        foreach (var subject in subjects)
                        {
                            a.Instance.RegisterApplicationSubject(subject);
                        }
                    })
                .As<IStoreCommunicationDescriptions>()
                .SingleInstance();
        }

        private static void RegisterUploads(ContainerBuilder builder)
        {
            builder.Register(c => new WaitingUploads())
                .As<IStoreUploads>()
                .SingleInstance();
        }

        private static void RegisterDownloads(ContainerBuilder builder)
        {
            builder.Register(
                   c =>
                   {
                       var ctx = c.Resolve<IComponentContext>();
                       DownloadDataFromRemoteEndpoints func =
                           (endpoint, token, filePath) =>
                           {
                               var handler = ctx.Resolve<IDirectIncomingData>();
                               var result = handler.ForwardData(endpoint, filePath);

                               var layer = ctx.Resolve<ICommunicationLayer>();
                               var msg = new DataDownloadRequestMessage(layer.Id, token);
                               var response = layer.SendMessageAndWaitForResponse(endpoint, msg);
                               return Task<FileInfo>.Factory.StartNew(
                                   () =>
                                   {
                                       Task.WaitAll(result, response);
                                       return result.Result;
                                   });
                           };

                       return func;
                   })
               .SingleInstance();
        }
    }
}
