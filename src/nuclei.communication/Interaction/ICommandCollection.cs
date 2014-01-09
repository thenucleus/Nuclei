//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for collections that store one or more <see cref="ICommandSet"/>
    /// objects.
    /// </summary>
    public interface ICommandCollection : IEnumerable<KeyValuePair<Type, ICommandSet>>
    {
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
        /// <param name="commandType">The interface that defines the command methods.</param>
        /// <param name="commands">The commands.</param>
        void Register(Type commandType, ICommandSet commands);

        /// <summary>
        /// Returns the command object that was registered for the given interface type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="ICommandSet"/> derived interface type.</param>
        /// <returns>
        /// The desired command set.
        /// </returns>
        ICommandSet CommandsFor(Type interfaceType);
    }
}
