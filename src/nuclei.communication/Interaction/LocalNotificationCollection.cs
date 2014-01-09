//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using Nuclei.Communication.Messages;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines a collection that contains notification objects for the local endpoint.
    /// </summary>
    internal sealed class LocalNotificationCollection : INotificationSendersCollection, ISendNotifications
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
        private readonly IDictionary<Type, INotificationSet> m_Notifications
            = new SortedList<Type, INotificationSet>(new TypeComparer());

        /// <summary>
        /// The collection that maps an notification to a collection of registered listeners.
        /// </summary>
        private readonly IDictionary<ISerializedEventRegistration, List<EndpointId>> m_RegisteredListeners
            = new Dictionary<ISerializedEventRegistration, List<EndpointId>>();

        /// <summary>
        /// The communication layer that is used to send out messages about newly
        /// registered commands.
        /// </summary>
        private readonly ISendDataViaChannels m_Layer;

        /// <summary>
        /// The object that stores the communication descriptions for the application.
        /// </summary>
        private readonly IStoreCommunicationDescriptions m_Descriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalNotificationCollection"/> class.
        /// </summary>
        /// <param name="layer">
        ///     The communication layer that is used to send out messages about newly registered notifications.
        /// </param>
        /// <param name="descriptions">The object that stores the communication descriptions for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="layer"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="descriptions"/> is <see langword="null" />.
        /// </exception>
        public LocalNotificationCollection(ISendDataViaChannels layer, IStoreCommunicationDescriptions descriptions)
        {
            {
                Lokad.Enforce.Argument(() => layer);
                Lokad.Enforce.Argument(() => descriptions);
            }

            m_Layer = layer;
            m_Descriptions = descriptions;
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
        /// <param name="notificationType">The interface that defines the notification events.</param>
        /// <param name="notifications">The notification object.</param>
        public void Store(Type notificationType, INotificationSet notifications)
        {
            {
                Lokad.Enforce.Argument(() => notificationType);
                Lokad.Enforce.Argument(() => notifications);
                Lokad.Enforce.With<ArgumentException>(
                    notificationType.IsInstanceOfType(notifications),
                    Resources.Exceptions_Messages_NotificationObjectMustImplementNotificationInterface);
            }

            NotificationProxyBuilder.VerifyThatTypeIsACorrectNotificationSet(notificationType);
            if (m_Notifications.ContainsKey(notificationType))
            {
                throw new CommandAlreadyRegisteredException();
            }
            
            ConnectToEvents(notificationType, notifications);

            m_Notifications.Add(notificationType, notifications);
            m_Descriptions.RegisterNotificationType(notificationType);
        }

        private void ConnectToEvents(Type notificationType, INotificationSet notifications)
        {
            var events = notificationType.GetEvents();
            foreach (var eventInfo in events)
            {
                var serializedInfo = ProxyExtensions.FromEventInfo(eventInfo);
                if (eventInfo.EventHandlerType == typeof(EventHandler))
                {
                    // This one is easy because we know the types ...
                    EventHandler handler = (s, e) => HandleEventAndForwardToListeners(serializedInfo, e);
                    eventInfo.AddEventHandler(notifications, handler);
                }
                else 
                {
                    // This one is not easy. So we need to create an EventHandler of the 
                    // correct type (EventHandler<T> where T is the correct type) and then
                    // attach it to the event.
                    var argsTypes = eventInfo.EventHandlerType.GetGenericArguments();
                    var handlerType = typeof(EventHandler<>).MakeGenericType(argsTypes);
                    EventHandler<EventArgs> handler = (s, e) => HandleEventAndForwardToListeners(serializedInfo, e);

                    // The following works if all the interface / class definitions
                    // are inside the same assembly (?)
                    //   var del = Delegate.CreateDelegate(handlerType, handler.Method);
                    // Unfortunately that seems to fail in this case so we'll do this the
                    // nasty way.
                    var constructors = handlerType.GetConstructors();
                    var del = (Delegate)constructors[0].Invoke(
                        new[] 
                            { 
                                handler.Target, 
                                handler.Method.MethodHandle.GetFunctionPointer() 
                            });

                    eventInfo.AddEventHandler(notifications, del);
                }
            }
        }

        private void HandleEventAndForwardToListeners(ISerializedEventRegistration originatingEvent, EventArgs args)
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
                    m_Layer.SendMessageTo(endpoint, new NotificationRaisedMessage(m_Layer.Id, originatingEvent, args));
                }
            }
        }

        /// <summary>
        /// Returns the notification object that was registered for the given interface type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="INotificationSet"/> derived interface.</typeparam>
        /// <returns>
        /// The desired notification set.
        /// </returns>
        public T NotificationsFor<T>() where T : INotificationSet
        {
            return (T)NotificationsFor(typeof(T));
        }

        /// <summary>
        /// Returns the notification object that was registered for the given interface type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="INotificationSet"/> derived interface type.</param>
        /// <returns>
        /// The desired notification set.
        /// </returns>
        public INotificationSet NotificationsFor(Type interfaceType)
        {
            return !m_Notifications.ContainsKey(interfaceType) ? null : m_Notifications[interfaceType];
        }

        /// <summary>
        ///  Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public IEnumerator<KeyValuePair<Type, INotificationSet>> GetEnumerator()
        {
            return m_Notifications.GetEnumerator();
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
        public void RegisterForNotification(EndpointId endpoint, ISerializedEventRegistration notification)
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
        public void UnregisterFromNotification(EndpointId endpoint, ISerializedEventRegistration notification)
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
