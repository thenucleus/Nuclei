//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores data about a command invocation.
    /// </summary>
    internal sealed class CommandInvokedData
    {
        /// <summary>
        /// The command that was invoked.
        /// </summary>
        private readonly CommandData m_Command;

        /// <summary>
        /// The parameters for the command invocation.
        /// </summary>
        private readonly List<Tuple<Type, object>> m_ParameterValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvokedData"/> class.
        /// </summary>
        /// <param name="command">The command that was invoked.</param>
        /// <param name="parameterValues">The parameters for the command invocation.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="command"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterValues"/> is <see langword="null" />.
        /// </exception>
        public CommandInvokedData(CommandData command, List<Tuple<Type, object>> parameterValues)
        {
            {
                Lokad.Enforce.Argument(() => command);
                Lokad.Enforce.Argument(() => parameterValues);
            }

            m_Command = command;
            m_ParameterValues = parameterValues;
        }

        /// <summary>
        /// Gets the command that was invoked.
        /// </summary>
        public CommandData Command
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
        public List<Tuple<Type, object>> ParameterValues
        {
            [DebuggerStepThrough]
            get
            {
                return m_ParameterValues;
            }
        }
    }
}
