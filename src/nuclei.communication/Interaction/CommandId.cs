//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Uniquely identifies a single command on an <see cref="ICommandSet"/> interface.
    /// </summary>
    [Serializable]
    public sealed class CommandId : Id<CommandId, string>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="CommandId"/> class based on the given method info identifying the
        /// command.
        /// </summary>
        /// <param name="method">The method that is invoked when the command is executed.</param>
        /// <returns>The ID of the command.</returns>
        public static CommandId Create(MethodInfo method)
        {
            var parameterTypes = method.GetParameters().Select(p => p.ParameterType.FullName).ToList();
            var parametersAsText = parameterTypes.Count > 0
                ? string.Join(", ", parameterTypes)
                : string.Empty;
            var id = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}.{2}({3})",
                method.ReturnType.FullName,
                method.DeclaringType.FullName,
                method.Name,
                parametersAsText);

            return new CommandId(id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandId"/> class.
        /// </summary>
        /// <param name="id">The value.</param>
        private CommandId(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override CommandId Clone(string value)
        {
            return new CommandId(value);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return InternalValue;
        }
    }
}