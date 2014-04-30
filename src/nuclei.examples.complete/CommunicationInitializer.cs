//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Defines an <see cref="IInitializeCommunicationInstances"/> for the example application.
    /// </summary>
    internal sealed class CommunicationInitializer : IInitializeCommunicationInstances
    {
        /// <summary>
        /// The dependency injection context that is used to resolve instances.
        /// </summary>
        private readonly IComponentContext m_Context;

        /// <summary>
        /// The collection of communication subjects.
        /// </summary>
        private readonly IEnumerable<CommunicationSubject> m_Subjects;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationInitializer"/> class.
        /// </summary>
        /// <param name="context">The dependency injection context that is used to resolve instances.</param>
        /// <param name="subjects">The collection of communication subjects.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="context"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="subjects"/> is <see langword="null" />.
        /// </exception>
        public CommunicationInitializer(IComponentContext context, IEnumerable<CommunicationSubject> subjects)
        {
            {
                Lokad.Enforce.Argument(() => context);
                Lokad.Enforce.Argument(() => subjects);
            }

            m_Context = context;
            m_Subjects = subjects;
        }

        /// <summary>
        /// Registers all the commands that are provided by the current application.
        /// </summary>
        public void RegisterProvidedCommands()
        {
            RegisterTestCommands();
        }

        private void RegisterTestCommands()
        {
            var instance = m_Context.Resolve<TestCommands>();

            var map = CommandMapper<ITestCommandSet>.Create();
            map.From<string>((command, name) => command.Echo(name))
                .To((string n) => instance.Echo(n));

            map.From<int, int>((command, first, second) => command.Calculate(first, second))
                .To((int first, int second) => instance.Calculate(first, second));

            map.From<UploadToken>((command, token) => command.StartDownload(token))
                .To((EndpointId e, UploadToken t) => instance.StartDownload(e, t));

            var collection = m_Context.Resolve<RegisterCommand>();
            collection(
                map.ToMap(),
                m_Subjects.Select(s => new SubjectGroupIdentifier(s, new Version(1, 0), "a")).ToArray());
        }

        /// <summary>
        /// Registers all the commands that the current application requires.
        /// </summary>
        public void RegisterRequiredCommands()
        {
            var registration = m_Context.Resolve<RegisterRequiredCommand>();
            registration(typeof(ITestCommandSet), m_Subjects.Select(s => new SubjectGroupIdentifier(s, new Version(1, 0), "a")).ToArray());
        }

        /// <summary>
        /// Registers all the notifications that are provided by the current application.
        /// </summary>
        public void RegisterProvidedNotifications()
        {
            var instance = m_Context.Resolve<TestNotifications>();

            var map = NotificationMapper<ITestNotificationSet>.Create();
            instance.OnNotify += map.From(t => t.OnNotify += null)
                .GenerateHandler();

            var collection = m_Context.Resolve<RegisterNotification>();
            collection(map.ToMap(), m_Subjects.Select(s => new SubjectGroupIdentifier(s, new Version(1, 0), "a")).ToArray());
        }

        /// <summary>
        /// Registers all the notifications that the current application requires.
        /// </summary>
        public void RegisterRequiredNotifications()
        {
            var registration = m_Context.Resolve<RegisterRequiredNotification>();
            registration(typeof(ITestNotificationSet), m_Subjects.Select(s => new SubjectGroupIdentifier(s, new Version(1, 0), "a")).ToArray());
        }

        /// <summary>
        /// Performs initialization routines that need to be performed before to the starting of the
        /// communication system.
        /// </summary>
        public void InitializeBeforeCommunicationSignIn()
        {
            // Do nothing for now ...
        }

        /// <summary>
        /// Performs initialization routines that need to be performed after the sign in of the
        /// communication system.
        /// </summary>
        public void InitializeAfterCommunicationSignIn()
        {
            // Do nothing for now ...
        }
    }
}
