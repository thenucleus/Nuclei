//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nuclei.Fusion;

namespace Nuclei.AppDomains
{
    /// <content>
    /// Contains the definition of the <see cref="DirectoryBasedResolver"/> class.
    /// </content>
    internal static partial class AppDomainBuilder
    {
        /// <summary>
        /// Attaches a method to the <see cref="AppDomain.AssemblyResolve"/> event and provides
        /// assembly resolution based on the files available in a set of predefined directories.
        /// </summary>
        private sealed class DirectoryBasedResolver : MarshalByRefObject, IAppDomainAssemblyResolver
        {
            /// <summary>
            /// Stores the directories as a collection of directory paths.
            /// </summary>
            /// <design>
            /// Explicitly store the directory paths in strings because DirectoryInfo objects are eventually
            /// nuked because DirectoryInfo is a MarshalByRefObject and can thus go out of scope.
            /// </design>
            private IEnumerable<string> m_Directories;

            /// <summary>
            /// Stores the paths to the relevant directories.
            /// </summary>
            /// <param name="directoryPaths">The paths to the relevant directories.</param>
            /// <exception cref="ArgumentNullException">
            /// Thrown when <paramref name="directoryPaths"/> is <see langword="null"/>.
            /// </exception>
            public void StoreDirectoryPaths(IEnumerable<string> directoryPaths)
            {
                {
                    Lokad.Enforce.Argument(() => directoryPaths);
                }

                m_Directories = directoryPaths;
            }

            /// <summary>
            /// Attaches the assembly resolution method to the <see cref="AppDomain.AssemblyResolve"/>
            /// event of the current <see cref="AppDomain"/>.
            /// </summary>
            /// <exception cref="InvalidOperationException">
            /// Thrown when <see cref="DirectoryBasedResolver.StoreDirectoryPaths"/> has not been called prior to
            /// attaching the directory resolver to an <see cref="AppDomain"/>.
            /// </exception>
            public void Attach()
            {
                {
                    Lokad.Enforce.NotNull(() => m_Directories);
                }

                var domain = AppDomain.CurrentDomain;
                {
                    var helper = new FusionHelper(
                        () => m_Directories.SelectMany(
                            dir => Directory.GetFiles(
                                dir, 
                                "*.dll", 
                                SearchOption.AllDirectories)));
                    domain.AssemblyResolve += helper.LocateAssemblyOnAssemblyLoadFailure;
                }
            }
        }
    }
}
