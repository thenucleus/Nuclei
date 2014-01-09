//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that filter <see cref="ICommunicationMessage" /> objects.
    /// </summary>
    internal interface IMessageFilter
    {
        /// <summary>
        /// Returns a value indicating if the message passes through the filter or not.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        ///   <see langword="true" /> if the message passes through the filter; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
              Justification = "Documentation can start with a language keyword")]
        bool PassThrough(ICommunicationMessage message);
    }
}
