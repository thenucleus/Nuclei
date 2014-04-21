//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for collections that store one or more <see cref="ICommandSet"/>
    /// objects.
    /// </summary>
    internal interface ICommandCollection : IEnumerable<CommandId>
    {
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
        /// <param name="definitions">The definitions that map the command interface methods to the object methods.</param>
        void Register(CommandDefinition[] definitions);

        /// <summary>
        /// Returns the command definition that was registered for the given command method.
        /// </summary>
        /// <param name="id">The ID of the command method.</param>
        /// <returns>
        /// The definition that contains the registered command method.
        /// </returns>
        CommandDefinition CommandToInvoke(CommandId id);
    }
}
