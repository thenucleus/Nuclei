//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Fusion;

namespace Nuclei.AppDomains
{
    /// <content>
    /// Contains the definition of the <see cref="FileBasedResolver"/> class.
    /// </content>
    internal static partial class AppDomainBuilder
    {
        /// <summary>
        /// Attaches a method to the <see cref="AppDomain.AssemblyResolve"/> event and
        /// provides assembly resolution based on a set of predefined files.
        /// </summary>
        private sealed class FileBasedResolver : MarshalByRefObject, IAppDomainAssemblyResolver
        {
            /// <summary>
            /// Stores the files as a collection of file paths.
            /// </summary>
            /// <design>
            /// Explicitly store the file paths in strings because FileInfo objects are eventually
            /// nuked because FileInfo is a MarshalByRefObject and can thus go out of scope.
            /// </design>
            private IEnumerable<string> m_Files;

            /// <summary>
            /// Stores the paths to the relevant assemblies.
            /// </summary>
            /// <param name="filePaths">The paths to the relevant assemblies.</param>
            /// <exception cref="ArgumentNullException">
            ///     Thrown when <paramref name="filePaths"/> is <see langword="null" />.
            /// </exception>
            public void StoreFilePaths(IEnumerable<string> filePaths)
            {
                {
                    Lokad.Enforce.Argument(() => filePaths); 
                }

                m_Files = filePaths;
            }

            /// <summary>
            /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
            /// event of the current <see cref="AppDomain"/>.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            /// Thrown when <see cref="FileBasedResolver.StoreFilePaths"/> has not been called prior to
            /// attaching the directory resolver to an <see cref="AppDomain"/>.
            /// </exception>
            public void Attach()
            {
                {
                    Lokad.Enforce.NotNull(() => m_Files);
                }

                var domain = AppDomain.CurrentDomain;
                {
                    var helper = new FusionHelper(() => m_Files);
                    domain.AssemblyResolve += helper.LocateAssemblyOnAssemblyLoadFailure;
                }
            }
        }
    }
}
