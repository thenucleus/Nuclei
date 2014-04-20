//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Maps the methods of a <see cref="ICommandSet"/> to one or more delegates.
    /// </summary>
    public sealed class CommandMap
    {
        /// <summary>
        /// The collection containing the command mappings for a given command interface.
        /// </summary>
        private readonly Dictionary<CommandId, Delegate> m_Commands
            = new Dictionary<CommandId, Delegate>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMap"/> class.
        /// </summary>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandMappings"/> is <see langword="null" />.
        /// </exception>
        public CommandMap(IEnumerable<Tuple<CommandId, Delegate>> commandMappings)
        {
            {
                Lokad.Enforce.Argument(() => commandMappings);
            }

            foreach (var pair in commandMappings)
            {
                m_Commands.Add(pair.Item1, pair.Item2);
            }
        }

        /// <summary>
        /// Returns the IDs for the command methods on a <see cref="ICommandSet"/> interface.
        /// </summary>
        /// <returns>The IDs for the command methods.</returns>
        public IEnumerable<CommandId> Commands()
        {
            return m_Commands.Keys;
        }

        /// <summary>
        /// Returns the delegate that should be invoked if the command with the given ID is
        /// executed.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <returns>The delegate that should be invoked if the command with the given ID is executed.</returns>
        public Delegate ToExecute(CommandId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownCommandException>(
                    m_Commands.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownCommand);
            }

            return m_Commands[id];
        }
    }
}