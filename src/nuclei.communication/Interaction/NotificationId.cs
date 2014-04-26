//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Uniquely identifies a single command on an <see cref="ICommandSet"/> interface.
    /// </summary>
    [Serializable]
    public sealed class NotificationId : Id<NotificationId, string>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NotificationId"/> class based on the given method info identifying the
        /// command.
        /// </summary>
        /// <param name="eventInfo">The method that is invoked when the command is executed.</param>
        /// <returns>The ID of the command.</returns>
        public static NotificationId Create(EventInfo eventInfo)
        {
            var id = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}.{2})",
                eventInfo.EventHandlerType.FullName,
                eventInfo.DeclaringType.FullName,
                eventInfo.Name);

            return new NotificationId(id);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationId"/> class.
        /// </summary>
        /// <param name="id">The value.</param>
        internal NotificationId(string id)
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
        protected override NotificationId Clone(string value)
        {
            return new NotificationId(value);
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
