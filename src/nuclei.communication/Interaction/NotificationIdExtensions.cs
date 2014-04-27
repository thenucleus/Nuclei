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
    /// Provides extension methods for the <see cref="NotificationId"/> class.
    /// </summary>
    public static class NotificationIdExtensions
    {
        /// <summary>
        /// Serializes an <see cref="NotificationId"/> to a string in a reversible manner.
        /// </summary>
        /// <param name="id">The ID that should be serialized.</param>
        /// <returns>The serialized ID.</returns>
        public static string Serialize(NotificationId id)
        {
            return id.ToString();
        }

        /// <summary>
        /// Deserializes an <see cref="NotificationId"/> from a string.
        /// </summary>
        /// <param name="serializedNotificationId">The string containing the serialized <see cref="NotificationId"/> information.</param>
        /// <returns>A new <see cref="NotificationId"/> based on the given <paramref name="serializedNotificationId"/>.</returns>
        public static NotificationId Deserialize(string serializedNotificationId)
        {
            {
                Lokad.Enforce.Argument(() => serializedNotificationId);
                Lokad.Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(serializedNotificationId),
                    Resources.Exceptions_Messages_NotificationIdCannotBeDeserializedFromAnEmptyString);
            }

            // @todo: do we check that this string has the right format?
            return new NotificationId(serializedNotificationId);
        }
    }
}
