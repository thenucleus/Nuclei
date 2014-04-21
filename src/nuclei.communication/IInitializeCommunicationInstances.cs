//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that initialize parts of the communication system.
    /// </summary>
    public interface IInitializeCommunicationInstances
    {
        /// <summary>
        /// Registers all the commands that are provided by the current application.
        /// </summary>
        void RegisterProvidedCommands();

        /// <summary>
        /// Registers all the commands that the current application requires.
        /// </summary>
        void RegisterRequiredCommands();

        /// <summary>
        /// Registers all the notifications that are provided by the current application.
        /// </summary>
        void RegisterProvidedNotifications();

        /// <summary>
        /// Registers all the notifications that the current application requires.
        /// </summary>
        void RegisterRequiredNotifications();

        /// <summary>
        /// Performs initialization routines that need to be performed before to the starting of the
        /// communication system.
        /// </summary>
        void InitializeBeforeCommunicationSignIn();

        /// <summary>
        /// Performs initialization routines that need to be performed after the sign in of the
        /// communication system.
        /// </summary>
        void InitializeAfterCommunicationSignIn();
    }
}
