using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;

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
            // Do nothing for now ...
        }

        /// <summary>
        /// Registers all the notifications that the current application requires.
        /// </summary>
        public void RegisterRequiredNotifications()
        {
            // Do nothing for now ...
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
