//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines a collection that contains notification objects for the local endpoint.
    /// </summary>
    internal sealed class LocalNotificationCollection : INotificationCollection, ISendNotifications
    {
        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection of registered notification.
        /// </summary>
        /// <remarks>
        /// We only ever add to this collection so there's no need to protect it against multiple threads
        /// accessing it.
        /// </remarks>
        private readonly IDictionary<NotificationId, NotificationDefinition> m_Notifications
            = new Dictionary<NotificationId, NotificationDefinition>();

        /// <summary>
        /// The collection that maps an notification to a collection of registered listeners.
        /// </summary>
        private readonly IDictionary<NotificationId, List<EndpointId>> m_RegisteredListeners
            = new Dictionary<NotificationId, List<EndpointId>>();

        /// <summary>
        /// The action that is used to send a message to a remote endpoint.
        /// </summary>
        private readonly SendMessage m_SendMessage;

        /// <summary>
        /// The endpoint ID of the current endpoint.
        /// </summary>
        private readonly EndpointId m_Current;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalNotificationCollection"/> class.
        /// </summary>
        /// <param name="localEndpoint">The endpoint ID of the local endpoint.</param>
        /// <param name="sendMessage">The action that is used to send messages.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessage"/> is <see langword="null" />.
        /// </exception>
        public LocalNotificationCollection(EndpointId localEndpoint, SendMessage sendMessage)
        {
            {
                Lokad.Enforce.Argument(() => localEndpoint);
                Lokad.Enforce.Argument(() => sendMessage);
            }

            m_Current = localEndpoint;
            m_SendMessage = sendMessage;
        }

        /// <summary>
        /// Stores a <see cref="INotificationSet"/> object.
        /// </summary>
        /// <para>
        /// A proper notification set class has the following characteristics:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The interface must derrive from <see cref="INotificationSet"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>The interface must only have events, no properties or methods.</description>
        ///     </item>
        ///     <item>
        ///         <description>Each event be based on <see cref="EventHandler{T}"/> delegate.</description>
        ///     </item>
        ///     <item>
        ///         <description>The event must be based on a closed constructed type.</description>
        ///     </item>
        ///     <item>
        ///         <description>The <see cref="EventArgs"/> of <see cref="EventHandler{T}"/> must be serializable.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// <param name="definitions">The definitions that map the notification interface events to the object events.</param>
        public void Register(NotificationDefinition[] definitions)
        {
            {
                Lokad.Enforce.Argument(() => definitions);
            }

            foreach (var definition in definitions)
            {
                if (m_Notifications.ContainsKey(definition.Id))
                {
                    throw new NotificationAlreadyRegisteredException();
                }

                definition.OnNotification(HandleEventAndForwardToListeners);
                m_Notifications.Add(definition.Id, definition);
            }
        }

        private void HandleEventAndForwardToListeners(NotificationId originatingEvent, EventArgs args)
        {
            List<EndpointId> endpoints = null;
            lock (m_Lock)
            {
                if (m_RegisteredListeners.ContainsKey(originatingEvent))
                {
                    endpoints = new List<EndpointId>(m_RegisteredListeners[originatingEvent]);
                }
            }

            if (endpoints != null)
            {
                foreach (var endpoint in endpoints)
                {
                    m_SendMessage(
                        endpoint, 
                        new NotificationRaisedMessage(m_Current, new NotificationRaisedData(originatingEvent, args)),
                        CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);
                }
            }
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<NotificationId> GetEnumerator()
        {
            foreach (var pair in m_Notifications)
            {
                yield return pair.Key;
            }
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

        /// <summary>
        /// Registers a specific endpoint so that it may be notified when the specified event
        /// is raised.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="notification">The object that describes to which event the endpoint wants to be subscribed.</param>
        public void RegisterForNotification(EndpointId endpoint, NotificationId notification)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => notification);
            }
            
            lock (m_Lock)
            {
                if (!m_RegisteredListeners.ContainsKey(notification))
                {
                    m_RegisteredListeners.Add(notification, new List<EndpointId>());
                }

                var list = m_RegisteredListeners[notification];
                if (!list.Contains(endpoint))
                {
                    list.Add(endpoint);
                }
            }
        }

        /// <summary>
        /// Deregisters a specific endpoint so that it will no longer be notified when the specified event
        /// is raised.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="notification">The object that describes from which event the endpoint wants to be unsubscribed.</param>
        public void UnregisterFromNotification(EndpointId endpoint, NotificationId notification)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => notification);
            }

            lock (m_Lock)
            {
                if (m_RegisteredListeners.ContainsKey(notification))
                {
                    var list = m_RegisteredListeners[notification];
                    if (list.Remove(endpoint))
                    {
                        if (list.Count == 0)
                        {
                            m_RegisteredListeners.Remove(notification);
                        }
                    }
                }
            }
        }
    }
}
