//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Registers a set of required commands with the given communication subjects.
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
    ///         <description>If a method returns a <see cref="Task{TResult}"/> then <c>T</c> must be serializable.</description>
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
    /// <param name="commandInterface">The interface that defines the command methods.</param>
    /// <param name="subject">The communication subjects to which the current commands belongs.</param>
    internal delegate void RegisterRequiredCommand(Type commandInterface, params SubjectGroupIdentifier[] subject);
}
