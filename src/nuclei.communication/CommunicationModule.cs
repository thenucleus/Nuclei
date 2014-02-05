//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nuclei.Communication.Properties;
using Nuclei.Diagnostics;

namespace Nuclei.Communication
{
    /// <summary>
    /// Handles the component registrations for the communication components.
    /// </summary>
    public sealed partial class CommunicationModule : Module
    {
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
        /// The collection of communication subjects for the current application.
        /// </summary>
        private readonly IEnumerable<CommunicationSubject> m_Subjects;

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
        /// <param name="subjects">The collection containing the communication subjects for the application.</param>
        /// <param name="allowedChannelTemplates">The collection of channel types on which the application is allowed to connect.</param>
        /// <param name="allowAutomaticChannelDiscovery">
        ///     A flag that indicates if the communication channels are allowed to provide
        ///     discovery.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subjects"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="allowedChannelTemplates"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="allowedChannelTemplates"/> is an empty collection.
        /// </exception>
        public CommunicationModule(
            IEnumerable<CommunicationSubject> subjects, 
            IEnumerable<ChannelTemplate> allowedChannelTemplates, 
            bool allowAutomaticChannelDiscovery)
        {
            {
                Lokad.Enforce.Argument(() => subjects);
                Lokad.Enforce.Argument(() => allowedChannelTemplates);
                Lokad.Enforce.With<ArgumentException>(
                    allowedChannelTemplates.Any(), 
                    Resources.Exceptions_Messages_AtLeastOneChannelTypeMustBeAllowed);
            }

            m_Subjects = subjects;
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
            RegisterCommunicationLayer(builder, m_AllowedChannelTemplates);
            RegisterHandshakeLayer(builder, m_AllowedChannelTemplates);
            RegisterMessageHandler(builder);
            RegisterDataHandler(builder);
            RegisterProtocolMessageProcessingActions(builder);
            RegisterConnectionHolders(builder);
            RegisterCommunicationChannel(builder);
            RegisterEndpoints(builder);
            RegisterProtocolChannelTemplates(builder);
            RegisterEndpointStorage(builder);
            RegisterCommunicationDescriptions(builder, m_Subjects);
            RegisterUploads(builder);
            RegisterDownloads(builder);
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
        }

        private void RegisterInteractionLayerV1(ContainerBuilder builder)
        {
            // Do Nothing for now
        }
    }
}
