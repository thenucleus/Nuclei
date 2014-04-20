//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines a collection that contains command objects for the local endpoint.
    /// </summary>
    internal sealed class LocalCommandCollection : ICommandCollection
    {
        /// <summary>
        /// The collection of registered commands.
        /// </summary>
        private readonly Dictionary<CommandId, Delegate> m_Commands
            = new Dictionary<CommandId, Delegate>();

        /// <summary>
        /// Registers a <see cref="ICommandSet"/> object.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A proper command set class has the following characteristics:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The interface must derive from <see cref="ICommandSet"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>The interface must only have methods, no properties or events.</description>
        ///     </item>
        ///     <item>
        ///         <description>Each method must return either <see cref="Task"/> or <see cref="Task{T}"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>If a method returns a <see cref="Task{T}"/> then <c>T</c> must be a closed constructed type.</description>
        ///     </item>
        ///     <item>
        ///         <description>If a method returns a <see cref="Task{T}"/> then <c>T</c> must be serializable.</description>
        ///     </item>
        ///     <item>
        ///         <description>All method parameters must be serializable.</description>
        ///     </item>
        ///     <item>
        ///         <description>None of the method parameters may be <c>ref</c> or <c>out</c> parameters.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="map">The map that maps the command interface methods to the object methods.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="map"/> is <see langword="null" />.
        /// </exception>
        public void Register(CommandMap map)
        {
            {
                Lokad.Enforce.Argument(() => map);
            }

            foreach (var id in map.Commands())
            {
                if (m_Commands.ContainsKey(id))
                {
                    throw new CommandAlreadyRegisteredException();
                }

                m_Commands.Add(id, map.ToExecute(id));
            }
        }

        /// <summary>
        /// Returns the delegate that was registered for the given command method.
        /// </summary>
        /// <param name="id">The ID of the command method.</param>
        /// <returns>
        /// The delegate to the registered command method.
        /// </returns>
        public Delegate CommandToInvoke(CommandId id)
        {
            {
                Lokad.Enforce.Argument(() => id);
                Lokad.Enforce.With<UnknownCommandException>(
                    m_Commands.ContainsKey(id),
                    Resources.Exceptions_Messages_UnknownCommand);
            }

            return m_Commands[id];
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<Tuple<CommandId, Delegate>> GetEnumerator()
        {
            foreach (var pair in m_Commands)
            {
                yield return new Tuple<CommandId, Delegate>(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An System.Collections.IEnumerator object that can be used to iterate through 
        /// the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
