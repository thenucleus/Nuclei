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
    /// Stores data about a command invocation.
    /// </summary>
    internal sealed class CommandInvokedData
    {
        /// <summary>
        /// The ID of the command that was invoked.
        /// </summary>
        private readonly CommandId m_Command;

        /// <summary>
        /// The parameters for the command invocation.
        /// </summary>
        private readonly CommandParameterValueMap[] m_Parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedData"/> class.
        /// </summary>
        /// <param name="command">The ID of the command that was invoked.</param>
        /// <param name="parameters">The parameters for the command invocation.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="command"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameters"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedData(CommandId command, CommandParameterValueMap[] parameters)
        {
            {
                Lokad.Enforce.Argument(() => command);
                Lokad.Enforce.Argument(() => parameters);
            }

            m_Command = command;
            m_Parameters = parameters;
        }

        /// <summary>
        /// Gets the ID of the command that was invoked.
        /// </summary>
        public CommandId Command
        {
            [DebuggerStepThrough]
            get
            {
                return m_Command;
            }
        }

        /// <summary>
        /// Gets the collection of parameters for the command invocation.
        /// </summary>
        public CommandParameterValueMap[] Parameters
        {
            [DebuggerStepThrough]
            get
            {
                return m_Parameters;
            }
        }
    }
}
