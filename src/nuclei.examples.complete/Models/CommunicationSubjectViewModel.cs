//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Communication;
using Nuclei.Communication.Protocol;

namespace Nuclei.Examples.Complete.Models
{
    /// <summary>
    /// Stores information about a given communication subject.
    /// </summary>
    internal sealed class CommunicationSubjectViewModel
    {
        /// <summary>
        /// The subject.
        /// </summary>
        private readonly CommunicationSubject m_Subject;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSubjectViewModel"/> class.
        /// </summary>
        /// <param name="subject">The subject.</param>
        public CommunicationSubjectViewModel(CommunicationSubject subject)
        {
            m_Subject = subject;
        }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        public CommunicationSubject Subject
        {
            get
            {
                return m_Subject;
            }
        }
    }
}
