//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Stores data about a raised notification.
    /// </summary>
    internal sealed class NotificationRaisedData
    {
        /// <summary>
        /// The notification that was raised.
        /// </summary>
        private readonly NotificationData m_Notification;

        /// <summary>
        /// The event arguments for the notification.
        /// </summary>
        private readonly EventArgs m_EventArgs;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRaisedData"/> class.
        /// </summary>
        /// <param name="notification">The notification that was raised.</param>
        /// <param name="eventArgs">The event arguments for the notification.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notification"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="eventArgs"/> is <see langword="null" />.
        /// </exception>
        public NotificationRaisedData(NotificationData notification, EventArgs eventArgs)
        {
            {
                Lokad.Enforce.Argument(() => notification);
                Lokad.Enforce.Argument(() => eventArgs);
            }

            m_Notification = notification;
            m_EventArgs = eventArgs;
        }

        /// <summary>
        /// Gets the notification that was raised.
        /// </summary>
        public NotificationData Notification
        {
            [DebuggerStepThrough]
            get
            {
                return m_Notification;
            }
        }

        /// <summary>
        /// Gets the event arguments for the notification.
        /// </summary>
        public EventArgs EventArgs
        {
            [DebuggerStepThrough]
            get
            {
                return m_EventArgs;
            }
        }
    }
}
