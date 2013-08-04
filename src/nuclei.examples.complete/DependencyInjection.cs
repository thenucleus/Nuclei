//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autofac;
using Nuclei.Communication;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Examples.Complete.Models;
using Nuclei.Examples.Complete.Views;
using Test.Manual.Nuclei.Communication;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Provides methods for use with the Dependency Injection of the different
    /// objects.
    /// </summary>
    internal static class DependencyInjection
    {
        /// <summary>
        /// Creates the DI container.
        /// </summary>
        /// <param name="context">The application context that can be used to terminate the application.</param>
        /// <param name="subjects">The collection of communication subjects for the current application.</param>
        /// <param name="allowChannelDiscovery">Indicates if automatic channel discovery should be turned on.</param>
        /// <returns>A new DI container.</returns>
        public static IContainer CreateContainer(
            ApplicationContext context, 
            IEnumerable<CommunicationSubject> subjects,
            bool allowChannelDiscovery)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterModule(new CommunicationModule(subjects, allowChannelDiscovery));
                builder.RegisterModule(new UtilsModule());

                builder.Register(
                        c =>
                        {
                            var ctx = c.Resolve<IComponentContext>();
                            Action<string> echoAction = text =>
                            {
                                var model = ctx.Resolve<ConnectionViewModel>();
                                model.AddNewMessage(null, text);
                            };
                            return new TestCommands(
                                c.Resolve<DownloadDataFromRemoteEndpoints>(),
                                echoAction);
                        })
                    .OnActivated(
                        a =>
                        {
                            var collection = a.Context.Resolve<ICommandCollection>();
                            collection.Register(typeof(ITestCommandSet), a.Instance);
                        })
                    .As<ITestCommandSet>()
                    .As<ICommandSet>()
                    .SingleInstance();

                // Register the elements from the current assembly
                builder.Register(c => new InteractiveWindow(
                        context,
                        c.Resolve<IHandleCommunication>()))
                    .OnActivated(a => 
                        {
                            a.Instance.ConnectionState = a.Context.Resolve<ConnectionViewModel>();
                        })
                    .As<InteractiveWindow>()
                    .As<IInteractiveWindow>()
                    .SingleInstance();

                builder.Register(c => new XmlConfiguration(
                            CommunicationConfigurationKeys.ToCollection()
                            .Append(DiagnosticsConfigurationKeys.ToCollection())
                            .ToList(),
                        TestConstants.ConfigurationSectionApplicationSettings))
                    .As<IConfiguration>()
                    .SingleInstance();

                builder.Register(c => new ApplicationCentral(
                        c.Resolve<ICommunicationLayer>(),
                        c.Resolve<ConnectionViewModel>()))
                    .As<IFormTheApplicationCenter>()
                    .As<IStartable>()
                    .SingleInstance();

                builder.Register(c => new ConnectionViewModel(c.Resolve<InteractiveWindow>().Dispatcher))
                    .OnActivated(
                        a => 
                        {
                            foreach (var subject in subjects)
                            {
                                a.Instance.AddSubject(new CommunicationSubjectViewModel(subject));
                            }
                        })
                    .SingleInstance();

                builder.Register(c => new CommunicationPassThrough(
                        c.Resolve<ICommunicationLayer>(),
                        c.Resolve<ISendCommandsToRemoteEndpoints>(),
                        c.Resolve<IStoreUploads>()))
                    .As<IHandleCommunication>();
            }

            return builder.Build();
        }
    }
}
