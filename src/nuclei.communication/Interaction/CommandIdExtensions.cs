//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Provides extension methods for the <see cref="CommandId"/> class.
    /// </summary>
    public static class CommandIdExtensions
    {
        /// <summary>
        /// Serializes an <see cref="CommandId"/> to a string in a reversible manner.
        /// </summary>
        /// <param name="id">The ID that should be serialized.</param>
        /// <returns>The serialized ID.</returns>
        public static string Serialize(CommandId id)
        {
            return id.ToString();
        }

        /// <summary>
        /// Deserializes an <see cref="CommandId"/> from a string.
        /// </summary>
        /// <param name="serializedCommandId">The string containing the serialized <see cref="CommandId"/> information.</param>
        /// <returns>A new <see cref="CommandId"/> based on the given <paramref name="serializedCommandId"/>.</returns>
        public static CommandId Deserialize(string serializedCommandId)
        {
            {
                Lokad.Enforce.Argument(() => serializedCommandId);
                Lokad.Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(serializedCommandId),
                    Resources.Exceptions_Messages_CommandIdCannotBeDeserializedFromAnEmptyString);
            }

            // @todo: do we check that this string has the right format?
            return new CommandId(serializedCommandId);
        }
    }
}
