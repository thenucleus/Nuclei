//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;

namespace Nuclei
{
    /// <summary>
    /// Defines extension methods for <see cref="Assembly"/> objects.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns the local file path from where a specific <see cref="Assembly"/>
        /// was loaded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The local file path from where the assembly was loaded.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assembly"/> is <see langword="null" />.
        /// </exception>
        public static string LocalFilePath(this Assembly assembly)
        {
            {
                Lokad.Enforce.Argument(() => assembly);
            }

            // Get the location of the assembly before it was shadow-copied
            // Note that Assembly.Codebase gets the path to the manifest-containing
            // file, not necessarily the path to the file that contains a
            // specific type.
            var uncPath = new Uri(assembly.CodeBase);

            // Get the local path. This may not work if the assembly isn't
            // local. For now we assume it is.
            return uncPath.LocalPath;
        }

        /// <summary>
        /// Returns the local directory path from where a specific <see cref="Assembly"/>
        /// was loaded.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        /// The local directory path from where the assembly was loaded.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assembly"/> is <see langword="null" />.
        /// </exception>
        public static string LocalDirectoryPath(this Assembly assembly)
        {
            {
                Lokad.Enforce.Argument(() => assembly);
            }

            // Get the location of the assembly before it was shadow-copied
            // Note that Assembly.Codebase gets the path to the manifest-containing
            // file, not necessarily the path to the file that contains a
            // specific type.
            var uncPath = new Uri(assembly.CodeBase);

            // Get the local path. This may not work if the assembly isn't
            // local. For now we assume it is.
            return Path.GetDirectoryName(uncPath.LocalPath);
        }

        /// <summary>
        /// Gets the strong name of the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The strong name of the assembly.</returns>
        public static StrongName GetStrongName(this Assembly assembly)
        {
            {
                Lokad.Enforce.Argument(() => assembly);
            }

            var assemblyName = assembly.GetName();
            byte[] publicKey = assemblyName.GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "{0} is not strongly named", assembly));
            }

            var keyBlob = new StrongNamePublicKeyBlob(publicKey);
            return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
        }
    }
}
