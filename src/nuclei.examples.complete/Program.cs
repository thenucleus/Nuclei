//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Autofac;
using Mono.Options;
using Nuclei.Communication;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;
using Nuclei.Examples.Complete.Properties;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Defines the main entry point for the test application.
    /// </summary>
    /// <remarks>
    /// This application is being used to manually test the communication system of Nuclei.
    /// The reason there is a manual test application is that testing the communication layer
    /// (and especially the TCP based communication) requires at least two machines in order
    /// to setup a network connection (it's not possible to have 2 applications on the same
    /// machine sending data to and reading from the same (TCP) port).
    /// </remarks>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// The exit code used when the application exits normally.
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// The exit code used when the application experiences an unhandled exception.
        /// </summary>
        private const int UnhandledExceptionExitCode = 1;

        /// <summary>
        /// The DI container.
        /// </summary>
        private static IContainer s_Container;

        [STAThread]
        [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
            Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We're just catching and then exiting the app.")]
        static int Main(string[] args)
        {
            {
                Debug.Assert(args != null, "The arguments array should not be null.");
            }

            Func<int> applicationLogic =
                () =>
                {
                    var context = new ApplicationContext()
                    {
                        Tag = args
                    };

                    // To stop the application from running use the ApplicationContext
                    // and call context.ExitThread();
                    RunApplication(args, context);

                    // Prepare the application for running. This includes setting up the communication channel etc.
                    // Then once that is done we can start with the message processing loop and then we 
                    // wait for it to either get terminated or until we kill ourselves.
                    System.Windows.Forms.Application.Run(context);
                    return NormalApplicationExitCode;
                };

            try
            {
                return applicationLogic();
            }
            catch (Exception)
            {
                return UnhandledExceptionExitCode;
            }
        }

        private static void RunApplication(string[] args, ApplicationContext context)
        {
            string hostIdText = null;
            string channelTypeText = null;
            string channelUriText = null;
            var communicationSubjects = new List<CommunicationSubject>();

            var options = new OptionSet 
                {
                    { 
                        Resources.CommandLine_Param_Host_Key, 
                        Resources.CommandLine_Param_Host_Description, 
                        v => hostIdText = v
                    },
                    {
                        Resources.CommandLine_Param_ChannelType_Key,
                        Resources.CommandLine_Param_ChannelType_Description,
                        v => channelTypeText = v
                    },
                    {
                        Resources.CommandLine_Param_ChannelUri_Key,
                        Resources.CommandLine_Param_ChannelUri_Description,
                        v => channelUriText = v
                    },
                    {
                        Resources.CommandLine_Options_CommunicationSubject_Key,
                        Resources.CommandLine_Options_CommunicationSubject_Description,
                        v => communicationSubjects.Add(new CommunicationSubject(v))
                    },
                };
            try
            {
                options.Parse(args);
            }
            catch (OptionException)
            {
                return;
            }

            var allowChannelDiscovery = (hostIdText == null) || (channelTypeText == null) || (channelUriText == null);
            s_Container = DependencyInjection.CreateContainer(context, communicationSubjects, allowChannelDiscovery);

            if (!allowChannelDiscovery)
            {
                var hostId = EndpointIdExtensions.Deserialize(hostIdText);
                var channelType = (ChannelType)Enum.Parse(typeof(ChannelType), channelTypeText);

                var diagnostics = s_Container.Resolve<SystemDiagnostics>();
                diagnostics.Log(
                    LevelToLog.Debug,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Log_Messages_ConnectingToHost_WithConnectionParameters,
                        hostId,
                        channelType,
                        channelUriText));

                var resolver = s_Container.Resolve<ManualEndpointConnection>();
                resolver(hostId, channelType, channelUriText);
            }

            var window = s_Container.Resolve<IInteractiveWindow>();
            ElementHost.EnableModelessKeyboardInterop(window as Window);
            window.Show();
        }
    }
}
