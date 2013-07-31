//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Build
{
    /// <summary>
    /// An attribute used to indicate which build and revision were used to create the current package.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    public sealed class AssemblyBuildInformationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBuildInformationAttribute"/> class.
        /// </summary>
        /// <param name="buildNumber">The number of the build that created the current package.</param>
        /// <param name="versionControlInformation">
        /// A string which provides information about the revision under which the current package is
        /// committed in the version control system.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="versionControlInformation"/> is <see langword="null" />.
        /// </exception>
        public AssemblyBuildInformationAttribute(int buildNumber, string versionControlInformation)
        {
            {
                Lokad.Enforce.Argument(() => versionControlInformation);
            }

            BuildNumber = buildNumber;
            VersionControlInformation = versionControlInformation;
        }

        /// <summary>
        /// Gets the number of the build that created the current package.
        /// </summary>
        public int BuildNumber
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a string which provides information about the revision under which the current package is
        /// committed in the version control system.
        /// </summary>
        public string VersionControlInformation
        {
            get;
            private set;
        }
    }
}
