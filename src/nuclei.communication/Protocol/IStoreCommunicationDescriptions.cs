//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Communication.Interaction;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that store information about the API's that are available for the
    /// current endpoint.
    /// </summary>
    internal interface IStoreCommunicationDescriptions
    {
        /// <summary>
        /// Registers a new subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        void RegisterApplicationSubject(CommunicationSubject subject);

        /// <summary>
        /// Registers a <see cref="ICommandSet"/> type.
        /// </summary>
        /// <param name="commandType">The <see cref="ICommandSet"/> type.</param>
        void RegisterCommandType(Type commandType);

        /// <summary>
        /// Registers a <see cref="INotificationSet"/> type.
        /// </summary>
        /// <param name="notificationType">The <see cref="INotificationSet"/> type.</param>
        void RegisterNotificationType(Type notificationType);

        /// <summary>
        /// Returns a collection containing all the subjects registered for the current application.
        /// </summary>
        /// <returns>A collection containing all the subjects registered for the current application.</returns>
        IEnumerable<CommunicationSubject> Subjects();

        /// <summary>
        /// Creates a new <see cref="CommunicationDescription"/> instance which contains all the 
        /// information about the current state of the communication system.
        /// </summary>
        /// <returns>The new <see cref="CommunicationDescription"/> instance.</returns>
        CommunicationDescription ToStorage();
    }
}
