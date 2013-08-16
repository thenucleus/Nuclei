﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Nuclei.Communication.Messages;
using Nuclei.Communication.Messages.Processors;
using Nuclei.Communication.Properties;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication
{
    /// <summary>
    /// Handles the component registrations for the communication components.
    /// </summary>
    public sealed class CommunicationModule : Module
    {
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

        private static void AttachLayer(IActivatedEventArgs<DataHandler> args)
        {
            var handler = args.Instance;
            var layer = args.Context.Resolve<ICommunicationLayer>();
            layer.OnEndpointSignedOut += (s, e) => handler.OnEndpointSignedOff(e.Endpoint);
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

        private static void SendMessageWithoutResponse(
            IConfiguration configuration,
            ISendDataViaChannels layer, 
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
                        h => layer.OnEndpointSignedIn += h,
                        h => layer.OnEndpointSignedIn -= h)
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
            ISendDataViaChannels layer, 
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
                        h => layer.OnEndpointSignedIn += h,
                        h => layer.OnEndpointSignedIn -= h)
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

        private static void RegisterCommunicationLayer(ContainerBuilder builder)
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
                        c.Resolve<SystemDiagnostics>());
                })
                .As<ISendDataViaChannels>()
                .As<ICommunicationLayer>()
                .SingleInstance();
        }

        private static void RegisterEndpointDiscoverySources(ContainerBuilder builder, bool allowChannelDiscovery)
        {
            if (allowChannelDiscovery)
            {
                builder.Register(c => new UdpBasedDiscoverySource())
                    .As<IDiscoverOtherServices>()
                    .SingleInstance();
            }

            // For now we're marking this as a single instance because
            // we want it to be linked to the CommunicationLayer at all times
            // and yet we want to be able to give it out to users without 
            // having to worry if we have given out the correct instance. Maybe
            // there is a cleaner solution to this problem though ...
            builder.Register(c => new ManualDiscoverySource())
                .As<IDiscoverOtherServices>()
                .As<IAcceptExternalEndpointInformation>()
                .SingleInstance();
        }

        private static void RegisterManualEndpointConnection(ContainerBuilder builder)
        {
            // This function is used to resolve connection information from
            // a set of strings.
            builder.Register<ManualEndpointConnection>(
                c =>
                {
                    var ctx = c.Resolve<IComponentContext>();
                    return (id, channelType, address) =>
                    {
                        var diagnostics = ctx.Resolve<SystemDiagnostics>();

                        // We need to make sure that the communication layer is ready to start
                        // sending / receiving messages, otherwise the we won't be able to
                        // tell it that there are new endpoints.
                        //
                        // NOTE: this is kinda yucky because really we're only interested in
                        // the IAcceptExternalEndpointInformation object and its willingness
                        // to process information. However the only way that object is going to
                        // be willing is if the communication layer is signed in so ...
                        var layer = ctx.Resolve<ICommunicationLayer>();
                        if (!layer.IsSignedIn)
                        {
                            var resetEvent = new AutoResetEvent(false);
                            var availability =
                                Observable.FromEventPattern<EventArgs>(
                                    h => layer.OnSignedIn += h,
                                    h => layer.OnSignedIn -= h)
                                    .Take(1)
                                    .Subscribe(args => resetEvent.Set());

                            using (availability)
                            {
                                if (!layer.IsSignedIn)
                                {
                                    diagnostics.Log(
                                        LevelToLog.Trace,
                                        CommunicationConstants.DefaultLogTextPrefix,
                                        Resources.Log_Messages_ManualDisoveryWaitingForLayerSignIn);

                                    resetEvent.WaitOne();
                                }
                            }
                        }

                        diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_ManualConnectionOfRemoteEndpoint_WithConnectionInformation,
                                id,
                                channelType,
                                address));

                        ctx.Resolve<IAcceptExternalEndpointInformation>().RecentlyConnectedEndpoint(id, channelType, new Uri(address));
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
                    return (id, channelType) =>
                    {
                        var diagnostics = ctx.Resolve<SystemDiagnostics>();

                        // We need to make sure that the communication layer is ready to start
                        // sending / receiving messages, otherwise the we won't be able to
                        // tell it that there are new endpoints.
                        //
                        // NOTE: this is kinda yucky because really we're only interested in
                        // the IAcceptExternalEndpointInformation object and its willingness
                        // to process information. However the only way that object is going to
                        // be willing is if the communication layer is signed in so ...
                        var layer = ctx.Resolve<ICommunicationLayer>();
                        if (!layer.IsSignedIn)
                        {
                            var resetEvent = new AutoResetEvent(false);
                            var availability =
                                Observable.FromEventPattern<EventArgs>(
                                    h => layer.OnSignedIn += h,
                                    h => layer.OnSignedIn -= h)
                                    .Take(1)
                                    .Subscribe(args => resetEvent.Set());

                            using (availability)
                            {
                                if (!layer.IsSignedIn)
                                {
                                    diagnostics.Log(
                                        LevelToLog.Trace,
                                        CommunicationConstants.DefaultLogTextPrefix,
                                        Resources.Log_Messages_ManualDisoveryWaitingForLayerSignIn);

                                    resetEvent.WaitOne();
                                }
                            }
                        }

                        diagnostics.Log(
                            LevelToLog.Trace,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_ManualDisconnectionOfRemoteEndpoint_WithConnectionInformation,
                                id,
                                channelType));

                        ctx.Resolve<IAcceptExternalEndpointInformation>().RecentlyDisconnectedEndpoint(id, channelType);
                    };
                })
                .SingleInstance();
        }

        private static void RegisterHandshakeLayer(ContainerBuilder builder)
        {
            builder.Register(
                c => new HandshakeProtocolLayer(
                    c.Resolve<IStoreInformationAboutEndpoints>(),
                    c.Resolve<IEnumerable<IDiscoverOtherServices>>(),
                    c.Resolve<ISendDataViaChannels>(),
                    c.Resolve<IStoreCommunicationDescriptions>(),
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

        private static void RegisterDataHandler(ContainerBuilder builder)
        {
            builder.Register(c => new DataHandler(
                    c.Resolve<SystemDiagnostics>()))
                .OnActivated(AttachLayer)
                .As<IProcessIncomingData>()
                .As<IDirectIncomingData>()
                .SingleInstance();
        }

        private static void RegisterMessageProcessingActions(ContainerBuilder builder)
        {
            builder.Register(c => new DataDownloadProcessAction(
                    c.Resolve<IStoreUploads>(),
                    c.Resolve<ISendDataViaChannels>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessageProcessAction>();

            builder.Register(
                    c =>
                    {
                        var ctx = c.Resolve<IComponentContext>();
                        return new EndpointConnectProcessAction(
                            c.Resolve<IHandleHandshakes>(),
                            from channelType in ctx.Resolve<IEnumerable<IChannelType>>() select channelType.ChannelType,
                            c.Resolve<SystemDiagnostics>());
                    })
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
                            var layer = ctx.Resolve<ISendDataViaChannels>();
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
                                var layer = ctx.Resolve<ISendDataViaChannels>();
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

        private static void RegisterConnectionHolders(ContainerBuilder builder)
        {
            builder.Register((c, p) => new ServiceConnectionHolder(
                    p.TypedAs<IChannelType>(),
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
                    var channelType = c.Resolve<NamedPipeChannelType>();
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<IStoreInformationAboutEndpoints>(),
                        channelType,
                        c.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IChannelType), channelType)),
                        c.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IChannelType), channelType)),
                        ctx.Resolve<IMessagePipe>,
                        ctx.Resolve<IDataPipe>,
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
                        c.Resolve<SystemDiagnostics>());
                })
                .OnActivated(ConnectToMessageHandler)
                .Keyed<ICommunicationChannel>(ChannelType.NamedPipe)
                .SingleInstance();

            builder.Register(
                (c, p) =>
                {
                    var channelType = c.Resolve<TcpChannelType>();
                    var ctx = c.Resolve<IComponentContext>();
                    return new CommunicationChannel(
                        p.TypedAs<EndpointId>(),
                        c.Resolve<IStoreInformationAboutEndpoints>(),
                        channelType,
                        c.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IChannelType), channelType)),
                        c.Resolve<IHoldServiceConnections>(new TypedParameter(typeof(IChannelType), channelType)),
                        ctx.Resolve<IMessagePipe>,
                        ctx.Resolve<IDataPipe>,
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
                        c.Resolve<SystemDiagnostics>());
                })
                .OnActivated(ConnectToMessageHandler)
                .Keyed<ICommunicationChannel>(ChannelType.TcpIP)
                .SingleInstance();
        }

        private static void RegisterEndpoints(ContainerBuilder builder)
        {
            builder.Register((c, p) => new SendingEndpoint(
                    p.TypedAs<EndpointId>(),
                    p.TypedAs<Func<EndpointId, IMessageSendingEndpoint>>(),
                    p.TypedAs<Func<EndpointId, IDataTransferingEndpoint>>()))
                .As<ISendingEndpoint>();

            builder.Register(c => new MessageReceivingEndpoint(
                    c.Resolve<SystemDiagnostics>()))
                .As<IMessagePipe>();

            builder.Register(c => new DataReceivingEndpoint(
                    c.Resolve<SystemDiagnostics>()))
                .As<IDataPipe>();
        }

        private static void RegisterChannelTypes(ContainerBuilder builder, bool allowChannelDiscovery)
        {
            builder.Register(c => new NamedPipeChannelType(
                    c.Resolve<IConfiguration>(),
                    allowChannelDiscovery))
                .As<IChannelType>()
                .As<NamedPipeChannelType>();

            builder.Register(c => new TcpChannelType(
                    c.Resolve<IConfiguration>(),
                    allowChannelDiscovery))
                .As<IChannelType>()
                .As<TcpChannelType>();
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
                .As<INotificationSendersCollection>()
                .As<ISendNotifications>()
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

                               var layer = ctx.Resolve<ISendDataViaChannels>();
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

        private static void RegisterStartables(ContainerBuilder builder)
        {
            builder.Register(c => new CommunicationLayerStarter(
                    c.Resolve<IComponentContext>(),
                    c.Resolve<SystemDiagnostics>()))
                .As<IStartable>();
        }

        /// <summary>
        /// The collection of communication subjects for the current application.
        /// </summary>
        private readonly IEnumerable<CommunicationSubject> m_Subjects;

        /// <summary>
        /// Indicates if the communication channels are allowed to provide discovery.
        /// </summary>
        private readonly bool m_AllowChannelDiscovery;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationModule"/> class.
        /// </summary>
        /// <param name="subjects">The collection containing the communication subjects for the application.</param>
        /// <param name="allowChannelDiscovery">
        ///     A flag that indicates if the communication channels are allowed to provide
        ///     discovery.
        /// </param>
        public CommunicationModule(IEnumerable<CommunicationSubject> subjects, bool allowChannelDiscovery)
        {
            {
                Lokad.Enforce.Argument(() => subjects);
            }

            m_Subjects = subjects;
            m_AllowChannelDiscovery = allowChannelDiscovery;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterCommunicationLayer(builder);
            RegisterEndpointDiscoverySources(builder, m_AllowChannelDiscovery);
            RegisterManualEndpointConnection(builder);
            RegisterManualEndpointDisconnection(builder);
            RegisterHandshakeLayer(builder);
            RegisterMessageHandler(builder);
            RegisterDataHandler(builder);
            RegisterMessageProcessingActions(builder);
            RegisterConnectionHolders(builder);
            RegisterCommunicationChannel(builder);
            RegisterEndpoints(builder);
            RegisterChannelTypes(builder, m_AllowChannelDiscovery);
            RegisterEndpointStorage(builder);
            RegisterCommunicationDescriptions(builder, m_Subjects);
            RegisterCommandHub(builder);
            RegisterCommandCollection(builder);
            RegisterNotificationHub(builder);
            RegisterNotificationCollection(builder);
            RegisterUploads(builder);
            RegisterDownloads(builder);
            RegisterStartables(builder);
        }
    }
}