//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Nuclei.Communication.Discovery;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines a initialization method for starting the communication layer when the application
    /// starts.
    /// </summary>
    internal sealed class CommunicationLayerStarter : IStartable
    {
        /// <summary>
        /// The DI container component context.
        /// </summary>
        private readonly IComponentContext m_Context;

        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The collection containing the types of channel that should be opened.
        /// </summary>
        private readonly IEnumerable<ChannelTemplate> m_AllowedChannelTemplates;

        /// <summary>
        /// Indicates if the communication channels are allowed to provide discovery.
        /// </summary>
        private readonly bool m_AllowAutomaticChannelDiscovery;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationLayerStarter"/> class.
        /// </summary>
        /// <param name="context">The DI container component context.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <param name="allowedChannelTemplates">The collection of channel types on which the application is allowed to connect.</param>
        /// <param name="allowAutomaticChannelDiscovery">
        ///     A flag that indicates if the communication channels are allowed to provide
        ///     discovery.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="allowedChannelTemplates"/> is <see langword="null" />.
        /// </exception>
        public CommunicationLayerStarter(
            IComponentContext context, 
            SystemDiagnostics diagnostics,
            IEnumerable<ChannelTemplate> allowedChannelTemplates, 
            bool allowAutomaticChannelDiscovery)
        {
            {
                Lokad.Enforce.Argument(() => context);
                Lokad.Enforce.Argument(() => diagnostics);
                Lokad.Enforce.Argument(() => allowedChannelTemplates);
                Lokad.Enforce.With<ArgumentException>(
                    allowedChannelTemplates.Any(),
                    Resources.Exceptions_Messages_AtLeastOneChannelTypeMustBeAllowed);
            }

            m_Context = context;
            m_Diagnostics = diagnostics;
            m_AllowedChannelTemplates = allowedChannelTemplates;
            m_AllowAutomaticChannelDiscovery = allowAutomaticChannelDiscovery;
        }

        /// <summary>
        /// Perform once-off startup processing.
        /// </summary>
        public void Start()
        {
            // Starting the communication layer takes quite a while
            // so lets not block the current thread which is being used
            // to start the application.
            Task.Factory.StartNew(
                () =>
                {
                    try
                    {
                        PreStartInitialize();

                        // Start the communication layer so that we can actuallly use it.
                        var layer = m_Context.Resolve<IProtocolLayer>();
                        layer.SignIn();

                        foreach (var template in m_AllowedChannelTemplates)
                        {
                            var discovery = m_Context.ResolveKeyed<IBootstrapChannel>(template);
                            discovery.OpenChannel(m_AllowAutomaticChannelDiscovery);
                        }

                        // Initiate discovery of other services. 
                        var discoverySources = m_Context.Resolve<IEnumerable<IDiscoverOtherServices>>();
                        foreach (var source in discoverySources)
                        {
                            source.StartDiscovery();
                        }

                        PostStartInitialize();
                    }
                    catch (Exception e)
                    {
                        m_Diagnostics.Log(
                            LevelToLog.Fatal,
                            CommunicationConstants.DefaultLogTextPrefix,
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Log_Messages_FailedToStartCommunicationSystem_WithError,
                                e));

                        throw;
                    }
                });
        }

        private void PreStartInitialize()
        {
            var initializers = m_Context.Resolve<IEnumerable<IInitializeCommunicationInstances>>();
            foreach (var initializer in initializers)
            {
                initializer.RegisterProvidedCommands();
                initializer.RegisterProvidedNotifications();
                initializer.RegisterRequiredCommands();
                initializer.RegisterRequiredNotifications();
                initializer.InitializeBeforeCommunicationSignIn();
            }
        }

        private void PostStartInitialize()
        {
            var initializers = m_Context.Resolve<IEnumerable<IInitializeCommunicationInstances>>();
            foreach (var initializer in initializers)
            {
                initializer.InitializeAfterCommunicationSignIn();
            }
        }
    }
}
