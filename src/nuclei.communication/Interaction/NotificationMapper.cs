//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using System.Security.Permissions;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines methods to map the events on a <see cref="INotificationSet"/> to events on one or more objects
    /// which may or may not implement the interface.
    /// </summary>
    /// <typeparam name="TNotification">The interface type for which the notification events should be mapped.</typeparam>
    public sealed class NotificationMapper<TNotification> where TNotification : INotificationSet
    {
        /// <summary>
        /// Defines a class that implements the methods on an <see cref="INotificationSet"/> as a proxy 
        /// so that it is possible to get specific events on the interface.
        /// </summary>
        /// <remarks>
        /// Taken from here: http://stackoverflow.com/a/11084822/539846.
        /// </remarks>
        private sealed class NotificationProxy : RealProxy
        {
            private const string EventSubscribeMethodPrefix = "add_";
            private const string EventUnsubscribeMethodPrefix = "remove_";

            private static string ReplaceAddRemovePrefixes(string method)
            {
                if (method.Contains(EventSubscribeMethodPrefix))
                {
                    return method.Replace(EventSubscribeMethodPrefix, string.Empty);
                }

                return method.Contains(EventUnsubscribeMethodPrefix) 
                    ? method.Replace(EventUnsubscribeMethodPrefix, string.Empty) 
                    : method;
            }

            private readonly List<string> m_Invocations = new List<string>();

            /// <summary>
            /// Initializes a new instance of the <see cref="NotificationProxy"/> class.
            /// </summary>
            public NotificationProxy()
                : base(typeof(TNotification))
            {
            }

            /// <summary>
            /// Gets the collection containing latest method invocations on the proxy.
            /// </summary>
            public IEnumerable<string> Invocations
            {
                get
                {
                    return m_Invocations;
                }
            }

            /// <summary>
            /// When overridden in a derived class, invokes the method that is specified in the provided 
            /// <see cref="T:System.Runtime.Remoting.Messaging.IMessage"/> on the remote object that is 
            /// represented by the current instance.
            /// </summary>
            /// <returns>
            /// The message returned by the invoked method, containing the return value and any out or ref parameters.
            /// </returns>
            /// <param name="msg">
            ///     A <see cref="T:System.Runtime.Remoting.Messaging.IMessage"/> that contains a 
            /// <see cref="T:System.Collections.IDictionary"/> of information about the method call.
            /// </param>
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.Infrastructure)]
            [DebuggerStepThrough]
            public override IMessage Invoke(IMessage msg)
            {
                var methodName = (string)msg.Properties["__MethodName"];
                var parameterTypes = (Type[])msg.Properties["__MethodSignature"];
                var method = typeof(TNotification).GetMethod(methodName, parameterTypes);

                if ((method != null) && (method.Name != null) 
                    && (method.Name.StartsWith(EventSubscribeMethodPrefix, StringComparison.Ordinal)
                        || method.Name.StartsWith(EventUnsubscribeMethodPrefix, StringComparison.Ordinal)))
                {
                    m_Invocations.Add(ReplaceAddRemovePrefixes(method.Name));
                }

                var message = msg as IMethodCallMessage;
                object response = null;
                var responseMessage = new ReturnMessage(response, null, 0, null, message);
                return responseMessage;
            }
        }

        /// <summary>
        /// Creates a new <see cref="NotificationMapper{TNotification}"/> instance.
        /// </summary>
        /// <returns>The notification mapper instance.</returns>
        public static NotificationMapper<TNotification> Create()
        {
            var type = typeof(TNotification);

            // Verify needs to be updated with the correct attributes 
            type.VerifyThatTypeIsACorrectNotificationSet();

            return new NotificationMapper<TNotification>();
        }

        /// <summary>
        /// The collection that contains all the created notification definitions.
        /// </summary>
        private readonly Dictionary<NotificationId, NotificationDefinition> m_Definitions
            = new Dictionary<NotificationId, NotificationDefinition>();

        /// <summary>
        /// Creates a <see cref="EventMapper"/> for a notification interface event.
        /// </summary>
        /// <param name="eventRegistration">The expression registering for the mapped event.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="eventRegistration"/> expression does not contain a event registration call on the 
        ///     notification interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the delegate type of the event is not a <see cref="EventHandler"/> or a <see cref="EventHandler{T}"/>.
        /// </exception>
        public EventMapper From(Action<TNotification> eventRegistration)
        {
            var proxy = new NotificationProxy();
            var tester = (TNotification)proxy.GetTransparentProxy();
            eventRegistration(tester);

            var eventName = proxy.Invocations.FirstOrDefault();
            if (eventName == null)
            {
                throw new InvalidNotificationMethodExpressionException();
            }

            var eventInfo = typeof(TNotification).GetEvent(eventName);
            if (eventInfo == null)
            {
                throw new InvalidNotificationMethodExpressionException();
            }

            return new EventMapper(
                StoreDefinition,
                NotificationId.Create(eventInfo));
        }

        private void StoreDefinition(NotificationDefinition definition)
        {
            if (m_Definitions.ContainsKey(definition.Id))
            {
                throw new NotificationAlreadyRegisteredException();
            }

            m_Definitions.Add(definition.Id, definition);
        }

        /// <summary>
        /// Creates the notification map for the given notification interface.
        /// </summary>
        /// <returns>The notification map.</returns>
        /// <exception cref="NotificationEventNotMappedException">
        ///     Thrown when one or more of the events on the notification interface are not mapped to instance events.
        /// </exception>
        public NotificationMap ToMap()
        {
            var type = typeof(TNotification);
            var events = type.GetEvents();
            foreach (var eventInfo in events)
            {
                var id = NotificationId.Create(eventInfo);
                if (!m_Definitions.ContainsKey(id))
                {
                    throw new NotificationEventNotMappedException();
                }
            }

            return new NotificationMap(type, m_Definitions.Values.ToArray());
        }
    }
}
