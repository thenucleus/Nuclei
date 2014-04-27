//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines methods for mapping a <see cref="INotificationSet"/> event to an event on a given object.
    /// </summary>
    public sealed class EventMapper
    {
        /// <summary>
        /// The action that is used to store the notification definition.
        /// </summary>
        private readonly Action<NotificationDefinition> m_StoreDefinition;

        /// <summary>
        /// The ID for the notification interface event.
        /// </summary>
        private readonly NotificationId m_NotificationId;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventMapper"/> class.
        /// </summary>
        /// <param name="storeDefinition">The action that is used to store the notification definition.</param>
        /// <param name="notificationId">The ID for the notification interface event.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storeDefinition"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="notificationId"/> is <see langword="null" />.
        /// </exception>
        internal EventMapper(Action<NotificationDefinition> storeDefinition, NotificationId notificationId)
        {
            {
                Lokad.Enforce.Argument(() => storeDefinition);
                Lokad.Enforce.Argument(() => notificationId);
            }

            m_StoreDefinition = storeDefinition;
            m_NotificationId = notificationId;
        }

        /// <summary>
        /// Generates an event handler that can be attached to the notification instance event.
        /// </summary>
        /// <returns>The event handler that can be attached to the notification instance event.</returns>
        public EventHandler GenerateHandler()
        {
            var definition = new NotificationDefinition(m_NotificationId);

            m_StoreDefinition(definition);
            return definition.HandleEventAndForwardToListeners;
        }

        /// <summary>
        /// Generates an event handler that can be attached to the notification instance event.
        /// </summary>
        /// <typeparam name="T">The type of the event handler.</typeparam>
        /// <returns>The event handler that can be attached to the notification instance event.</returns>
        public EventHandler<T> GenerateHandler<T>() where T : EventArgs
        {
            var definition = new NotificationDefinition(m_NotificationId);

            m_StoreDefinition(definition);
            return definition.HandleEventAndForwardToListeners;
        }
    }
}
