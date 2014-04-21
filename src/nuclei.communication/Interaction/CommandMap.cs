//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the mapping of the methods of a <see cref="ICommandSet"/> to a set of delegates.
    /// </summary>
    public sealed class CommandMap
    {
        /// <summary>
        /// The type of the command set.
        /// </summary>
        private readonly Type m_CommandType;

        /// <summary>
        /// The mappings for each of the command methods.
        /// </summary>
        private readonly CommandDefinition[] m_Definitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMap"/> class.
        /// </summary>
        /// <param name="commandType">The type of the command set.</param>
        /// <param name="definitions">The mappings of each of the command methods.</param>
        internal CommandMap(Type commandType, CommandDefinition[] definitions)
        {
            {
                Lokad.Enforce.Argument(() => commandType);
                Lokad.Enforce.Argument(() => definitions);
            }

            m_CommandType = commandType;
            m_Definitions = definitions;
        }

        /// <summary>
        /// Gets the type of the command set.
        /// </summary>
        internal Type CommandType
        {
            [DebuggerStepThrough]
            get
            {
                return m_CommandType;
            }
        }

        /// <summary>
        /// Gets the mappings for each of the command methods.
        /// </summary>
        internal CommandDefinition[] Definitions
        {
            [DebuggerStepThrough]
            get
            {
                return m_Definitions;
            }
        }
    }
}
