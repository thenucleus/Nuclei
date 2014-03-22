//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Properties;
using Nuclei.Diagnostics;

namespace Nuclei.Communication
{
    /// <summary>
    /// Handles the component registrations for the communication components.
    /// </summary>
    public sealed partial class CommunicationModule : Module
    {
        private static void RegisterEntryPoint(ContainerBuilder builder)
        {
            builder.Register(c => new CommunicationEntryPoint(
                    c.Resolve<IStoreInformationAboutEndpoints>(),
                    c.Resolve<ISendCommandsToRemoteEndpoints>(),
                    c.Resolve<INotifyOfRemoteEndpointEvents>()))
                .As<ICommunicationFacade>()
                .SingleInstance();
        }

        private static void RegisterStartables(
            ContainerBuilder builder, 
            IEnumerable<ChannelTemplate> allowedChannelTemplates, 
            bool allowAutomaticChannelDiscovery)
        {
            builder.Register(c => new CommunicationLayerStarter(
                    c.Resolve<IComponentContext>(),
                    c.Resolve<SystemDiagnostics>(),
                    allowedChannelTemplates,
                    allowAutomaticChannelDiscovery))
                .As<IStartable>();
        }

        /// <summary>
        /// The collection containing the types of channel that should be opened.
        /// </summary>
        private readonly IEnumerable<ChannelTemplate> m_AllowedChannelTemplates;

        /// <summary>
        /// Indicates if the communication channels are allowed to provide discovery.
        /// </summary>
        private readonly bool m_AllowAutomaticChannelDiscovery;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationModule"/> class.
        /// </summary>
        /// <param name="allowedChannelTemplates">The collection of channel types on which the application is allowed to connect.</param>
        /// <param name="allowAutomaticChannelDiscovery">
        ///     A flag that indicates if the communication channels are allowed to provide
        ///     discovery.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="allowedChannelTemplates"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="allowedChannelTemplates"/> is an empty collection.
        /// </exception>
        public CommunicationModule(
            IEnumerable<ChannelTemplate> allowedChannelTemplates, 
            bool allowAutomaticChannelDiscovery)
        {
            {
                Lokad.Enforce.Argument(() => allowedChannelTemplates);
                Lokad.Enforce.With<ArgumentException>(
                    allowedChannelTemplates.Any(), 
                    Resources.Exceptions_Messages_AtLeastOneChannelTypeMustBeAllowed);
            }

            m_AllowedChannelTemplates = allowedChannelTemplates;
            m_AllowAutomaticChannelDiscovery = allowAutomaticChannelDiscovery;
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterDiscoveryLayer(builder);
            RegisterDiscoveryLayerV1(builder);
            RegisterProtocolLayer(builder);
            RegisterProtocolLayerV1(builder);
            RegisterInteractionLayer(builder);
            RegisterInteractionLayerV1(builder);

            RegisterEntryPoint(builder);
            RegisterStartables(builder, m_AllowedChannelTemplates, m_AllowAutomaticChannelDiscovery);
        }

        private void RegisterDiscoveryLayer(ContainerBuilder builder)
        {
            RegisterEndpointDiscoverySources(builder, m_AllowAutomaticChannelDiscovery);
            RegisterManualEndpointConnection(builder);
            RegisterManualEndpointDisconnection(builder);
            RegisterBootstrapChannel(builder);
            RegisterVersionedDiscoveryEndpointSelector(builder);
            RegisterDiscoveryChannelTemplates(builder);
            RegisterLocalConnectionInformation(builder);
        }

        private void RegisterDiscoveryLayerV1(ContainerBuilder builder)
        {
            RegisterDiscoveryV1Endpoints(builder);
            RegisterDiscoveryV1ChannelInformationTranslators(builder);
        }

        private void RegisterProtocolLayer(ContainerBuilder builder)
        {
            RegisterProtocolLayer(builder, m_AllowedChannelTemplates);
            RegisterProtocolHandshakeConductor(builder, m_AllowedChannelTemplates);
            RegisterMessageHandler(builder);
            RegisterDataHandler(builder);
            RegisterProtocolMessageProcessingActions(builder);
            RegisterConnectionHolders(builder);
            RegisterProtocolChannel(builder);
            RegisterEndpoints(builder);
            RegisterProtocolChannelTemplates(builder);
            RegisterEndpointStorage(builder);
            RegisterUploads(builder);
            RegisterDownloads(builder);
            RegisterDataContractResolver(builder);
        }

        private void RegisterProtocolLayerV1(ContainerBuilder builder)
        {
            RegisterProtocolV1Endpoints(builder);
            RegisterProtocolV1MessageConverters(builder);
            RegisterV1SendingEndpoints(builder);
            RegisterV1ConnectionApprover(builder);
        }

        private void RegisterInteractionLayer(ContainerBuilder builder)
        {
            RegisterCommandHub(builder);
            RegisterCommandCollection(builder);
            RegisterNotificationHub(builder);
            RegisterNotificationCollection(builder);
            RegisterInteractionMessageProcessingActions(builder);
            RegisterInteractionHandshakeConductor(builder);
            RegisterInteractionSubjectStorage(builder);
            RegisterCommandRegistrationFunctions(builder);
            RegisterNotificationRegistrationFunctions(builder);
            RegisterObjectSerializerStorage(builder);
            RegisterObjectSerializers(builder);
        }

        private void RegisterInteractionLayerV1(ContainerBuilder builder)
        {
            RegisterInteractionV1ProtocolV1MessageConverters(builder);
        }
    }
}
