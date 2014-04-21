//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores information about a single method of a <see cref="ICommandSet"/>.
    /// </summary>
    public sealed class CommandDefinition
    {
        /// <summary>
        /// The ID of the command.
        /// </summary>
        private readonly CommandId m_Id;

        /// <summary>
        /// A flag that indicates whether the command returns a value or not.
        /// </summary>
        private readonly bool m_HasReturnValue;

        /// <summary>
        /// The collection of parameters for the command.
        /// </summary>
        private readonly List<CommandParameterMap> m_Parameters;

        /// <summary>
        /// The delegate that should be invoked when the command is invoked.
        /// </summary>
        private readonly Delegate m_CommandToExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandDefinition"/> class.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <param name="parameters">The collection of parameters for the command.</param>
        /// <param name="hasReturnValue">A flag that indicates whether the command returns a value or not.</param>
        /// <param name="commandToExecute">The command that should be invoked.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="id"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameters"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandToExecute"/> is <see langword="null" />.
        /// </exception>
        internal CommandDefinition(CommandId id, CommandParameterMap[] parameters, bool hasReturnValue, Delegate commandToExecute)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.Argument(() => parameters);
                Lokad.Enforce.Argument(() => commandToExecute);
            }

            m_Id = id;
            m_HasReturnValue = hasReturnValue;
            m_Parameters = new List<CommandParameterMap>(parameters);
            m_CommandToExecute = commandToExecute;
        }

        /// <summary>
        /// Gets the ID of the command.
        /// </summary>
        public CommandId Id
        {
            [DebuggerStepThrough]
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the command returns a value.
        /// </summary>
        public bool HasReturnValue
        {
            [DebuggerStepThrough]
            get
            {
                return m_HasReturnValue;
            }
        }

        /// <summary>
        /// Invokes the command and returns the command return value.
        /// </summary>
        /// <param name="parameters">The parameters for the command.</param>
        /// <returns>The return value for the command.</returns>
        public object Invoke(Tuple<Type, string, object>[] parameters)
        {
            {
                Lokad.Enforce.Argument(() => parameters);
            }

            var mappedParameterValues = new object[m_Parameters.Count];
            for (int i = 0; i < m_Parameters.Count; i++)
            {
                var expectedParameter = m_Parameters[i];
                if (expectedParameter.Orgin == CommandParameterOrigin.FromCommand)
                {
                    var providedParameter = parameters.FirstOrDefault(m => string.Equals(m.Item2, expectedParameter.Name, StringComparison.Ordinal));
                    if (providedParameter == null)
                    {
                        throw new MissingCommandParameterException();
                    }

                    mappedParameterValues[i] = providedParameter.Item3;
                    continue;
                }

                throw new InvalidCommandParameterOriginException();
            }

            return m_CommandToExecute.DynamicInvoke(mappedParameterValues);
        }
    }
}
