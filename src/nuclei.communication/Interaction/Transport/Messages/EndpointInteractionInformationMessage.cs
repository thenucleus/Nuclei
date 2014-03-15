//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Interaction.Transport.Messages
{
    /// <summary>
    /// Defines a message that contains information describing all the provided <see cref="CommunicationSubject"/>s for the
    /// sending endpoint and the commands and notifications belonging to those subjects.
    /// </summary>
    internal sealed class EndpointInteractionInformationMessage : CommunicationMessage
    {
        /// <summary>
        /// The collection containing all the subject groups for the endpoint.
        /// </summary>
        private readonly CommunicationSubjectGroup[] m_Groups;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInteractionInformationMessage"/> class.
        /// </summary>
        /// <param name="origin">The endpoint that send the original message.</param>
        /// <param name="groups">The collection containing communication subjects for the endpoint.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="origin"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="groups"/> is <see langword="null" />.
        /// </exception>
        public EndpointInteractionInformationMessage(EndpointId origin, CommunicationSubjectGroup[] groups) : 
            base(origin)
        {
            {
                Lokad.Enforce.Argument(() => groups);
            }

            m_Groups = groups;
        }

        /// <summary>
        /// Gets the collection containing all the subject groups.
        /// </summary>
        public CommunicationSubjectGroup[] SubjectGroups
        {
            [DebuggerStepThrough]
            get
            {
                return m_Groups;
            }
        }
    }
}
