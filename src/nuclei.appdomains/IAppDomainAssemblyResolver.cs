//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.AppDomains
{
    /// <summary>
    /// Defines the interface for classes which deal with assembly resolution.
    /// </summary>
    internal interface IAppDomainAssemblyResolver
    {
        /// <summary>
        /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
        /// event.
        /// </summary>
        void Attach();
    }
}
