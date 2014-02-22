//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for objects that store <see cref="CommunicationSubjectGroup"/> instances.
    /// </summary>
    internal interface IStoreInteractionSubjects : IStoreProtocolSubjects, IEnumerable<CommunicationSubject>
    {
        /// <summary>
        /// Returns a value indicating if a subject group with required commands and notifications exists for 
        /// the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>
        /// <see langword="true" /> if a subject group with required commands and notifications exists for the
        /// given subject; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ContainsGroupRequirementForSubject(CommunicationSubject subject);
        
        /// <summary>
        /// Returns the required subject group that is related with the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>The required subject group that is related with the given subject.</returns>
        CommunicationSubjectGroup GroupRequirementsFor(CommunicationSubject subject);

        /// <summary>
        /// Returns a value indicating if a subject group with provided commands and notifications exists for 
        /// the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>
        /// <see langword="true" /> if a subject group with provided commands and notifications exists for the
        /// given subject; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool ContainsGroupProvisionsForSubject(CommunicationSubject subject);

        /// <summary>
        /// Returns the provided subject group that is related with the given subject.
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <returns>The provided subject group that is related with the given subject.</returns>
        CommunicationSubjectGroup GroupProvisionsFor(CommunicationSubject subject);
    }
}
