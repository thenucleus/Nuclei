using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclei.Communication.Interaction
{
    internal interface IStoreInteractionSubjects : IEnumerable<CommunicationSubject>
    {
        bool ContainsGroupRequirementForSubject(CommunicationSubject subject);
        
        CommunicationSubjectGroup GroupRequirementsFor(CommunicationSubject subject);

        bool ContainsGroupProvisionsForSubject(CommunicationSubject subject);

        CommunicationSubjectGroup GroupProvisionsFor(CommunicationSubject subject);

        // Required commands and notifications.
    }
}
