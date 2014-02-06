//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores information about a given command type.
    /// </summary>
    internal sealed class OfflineTypeInformation
    {
        /// <summary>
        /// The full name of the type that defines the commands.
        /// </summary>
        private readonly string m_CommandTypeFullName;

        /// <summary>
        /// The assembly information describing the assembly that contains the commands.
        /// </summary>
        private readonly AssemblyName m_AssemblyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfflineTypeInformation"/> class.
        /// </summary>
        /// <param name="commandTypeFullName">The full name of the type that defines the commands.</param>
        /// <param name="assemblyName">The assembly information describing the assembly that contains the commands.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandTypeFullName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="assemblyName"/> is <see langword="null" />.
        /// </exception>
        public OfflineTypeInformation(string commandTypeFullName, AssemblyName assemblyName)
        {
            {
                Lokad.Enforce.Argument(() => commandTypeFullName);
                Lokad.Enforce.Argument(() => assemblyName);
            }

            m_CommandTypeFullName = commandTypeFullName;
            m_AssemblyName = assemblyName;
        }

        /// <summary>
        /// Gets the full name of the type that defines the commands.
        /// </summary>
        public string CommandTypeFullName
        {
            [DebuggerStepThrough]
            get
            {
                return m_CommandTypeFullName;
            }
        }

        /// <summary>
        /// Gets the assembly information describing the assembly that contains the commands.
        /// </summary>
        public AssemblyName AssemblyName
        {
            [DebuggerStepThrough]
            get
            {
                return m_AssemblyName;
            }
        }
    }
}
