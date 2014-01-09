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

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines a collection that contains command objects for the local endpoint.
    /// </summary>
    internal sealed class LocalCommandCollection : ICommandCollection
    {
        /// <summary>
        /// The collection of registered commands.
        /// </summary>
        private readonly SortedList<Type, ICommandSet> m_Commands
            = new SortedList<Type, ICommandSet>(new TypeComparer());

        /// <summary>
        /// The object that stores the communication descriptions for the application.
        /// </summary>
        private readonly IStoreCommunicationDescriptions m_Descriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalCommandCollection"/> class.
        /// </summary>
        /// <param name="descriptions">The object that stores the communication descriptions for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="descriptions"/> is <see langword="null" />.
        /// </exception>
        public LocalCommandCollection(IStoreCommunicationDescriptions descriptions)
        {
            {
                Lokad.Enforce.Argument(() => descriptions);
            }

            m_Descriptions = descriptions;
        }

        /// <summary>
        /// Registers a <see cref="ICommandSet"/> object.
        /// </summary>
        /// <para>
        /// A proper command set class has the following characteristics:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The interface must derrive from <see cref="ICommandSet"/>.</description>
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
        /// <param name="commandType">The interface through which the commands will be executed.</param>
        /// <param name="commands">The object that will execute the commands.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="commands"/> does not implement the <paramref name="commandType"/> interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     If the given type is not a valid <see cref="ICommandSet"/> interface.
        /// </exception>
        public void Register(Type commandType, ICommandSet commands)
        {
            {
                Lokad.Enforce.Argument(() => commandType);
                Lokad.Enforce.Argument(() => commands);
                Lokad.Enforce.With<ArgumentException>(
                    commandType.IsInstanceOfType(commands),
                    Resources.Exceptions_Messages_CommandObjectMustImplementCommandInterface);
            }

            CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(commandType);
            if (m_Commands.ContainsKey(commandType))
            {
                throw new CommandAlreadyRegisteredException();
            }

            m_Commands.Add(commandType, commands);
            m_Descriptions.RegisterCommandType(commandType);
        }

        /// <summary>
        /// Returns the command object that was registered for the given interface type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="ICommandSet"/> derived interface type.</param>
        /// <returns>
        /// The desired command set.
        /// </returns>
        public ICommandSet CommandsFor(Type interfaceType)
        {
            return !m_Commands.ContainsKey(interfaceType) ? null : m_Commands[interfaceType];
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<KeyValuePair<Type, ICommandSet>> GetEnumerator()
        {
            return m_Commands.GetEnumerator();
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
