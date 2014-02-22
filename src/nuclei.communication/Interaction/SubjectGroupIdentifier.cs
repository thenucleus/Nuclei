using System;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores information about the subject group a command or notification belongs to.
    /// </summary>
    public sealed class SubjectGroupIdentifier
    {
        /// <summary>
        /// The communication subject that is related to the subject group.
        /// </summary>
        private readonly CommunicationSubject m_Subject;

        /// <summary>
        /// The 'version' of the interaction object that is related to the subject group.
        /// </summary>
        private readonly Version m_Version;

        /// <summary>
        /// The identifier that is used to group interaction objects that perform similar functions.
        /// </summary>
        private readonly string m_Group;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubjectGroupIdentifier"/> class.
        /// </summary>
        /// <param name="subject">The communication subject that is related to the subject group.</param>
        /// <param name="version">The 'version' of the interaction object that is related to the subject group.</param>
        /// <param name="group">The identifier that is used to group interaction objects that perform similar functions.</param>
        public SubjectGroupIdentifier(CommunicationSubject subject, Version version, string @group)
        {
            {
                Lokad.Enforce.Argument(() => subject);
                Lokad.Enforce.Argument(() => version);
                Lokad.Enforce.Argument(() => @group);
            }

            m_Subject = subject;
            m_Version = version;
            m_Group = @group;
        }

        /// <summary>
        /// Gets the communication subject that is related to the subject group.
        /// </summary>
        public CommunicationSubject Subject
        {
            [DebuggerStepThrough]
            get
            {
                return m_Subject;
            }
        }

        /// <summary>
        /// Gets the 'version' of the interaction object that is related to the subject group.
        /// </summary>
        public Version Version
        {
            [DebuggerStepThrough]
            get
            {
                return m_Version;
            }
        }

        /// <summary>
        /// Gets the identifier that is used to group interaction objects that perform similar functions.
        /// </summary>
        public string Group
        {
            [DebuggerStepThrough]
            get
            {
                return m_Group;
            }
        }
    }
}
