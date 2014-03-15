//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for objects that register <see cref="CommunicationSubjectGroup"/> data.
    /// </summary>
    internal interface IRegisterSubjectGroups
    {
        /// <summary>
        /// Registers an existing command with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="version">The version of the command which is used to order commands that provide similar functionality.</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of commands that provide similar functionality.
        /// </param>
        void RegisterCommandForProvidedSubjectGroup(CommunicationSubject subject, Type commandType, Version version, string groupIdentifier);

        /// <summary>
        /// Registers a required command with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="commandType">The type of the command.</param>
        /// <param name="version">The version of the command which is used to order commands that provide similar functionality.</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of commands that provide similar functionality.
        /// </param>
        void RegisterCommandForRequiredSubjectGroup(CommunicationSubject subject, Type commandType, Version version, string groupIdentifier);

        /// <summary>
        /// Registers an existing notification with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="notificationType">The type of the notification.</param>
        /// <param name="version">The version of the notification which is used to order commands that provide similar functionality.</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of notifications that provide similar functionality.
        /// </param>
        void RegisterNotificationForProvidedSubjectGroup(
            CommunicationSubject subject, 
            Type notificationType, 
            Version version, 
            string groupIdentifier);

        /// <summary>
        /// Registers a required notification with a specific subject group.
        /// </summary>
        /// <param name="subject">The subject for the group.</param>
        /// <param name="notificationType">The type of the notification.</param>
        /// <param name="version">The version of the notification which is used to order commands that provide similar functionality.</param>
        /// <param name="groupIdentifier">
        /// The identifier which is used to group different versions of notifications that provide similar functionality.
        /// </param>
        void RegisterNotificationForRequiredSubjectGroup(
            CommunicationSubject subject, 
            Type notificationType, 
            Version version, 
            string groupIdentifier);
    }
}
