//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that store information about the API's that are available for the
    /// current endpoint.
    /// </summary>
    internal interface IStoreProtocolSubjects
    {
        /// <summary>
        /// Registers a new subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        void RegisterApplicationSubject(CommunicationSubject subject);

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
