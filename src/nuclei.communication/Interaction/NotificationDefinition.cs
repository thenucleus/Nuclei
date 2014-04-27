//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nuclei.Communication.Interaction
{
    internal class NotificationDefinition
    {
        /// <summary>
        /// The ID of the notification.
        /// </summary>
        private readonly NotificationId m_Id;

        private readonly List<Action<NotificationId, EventArgs>> m_EventHandlers
            = new List<Action<NotificationId, EventArgs>>();

        private void ConnectToEvents(Type notificationType, INotificationSet notifications)
        {
            var events = notificationType.GetEvents();
            foreach (var eventInfo in events)
            {
                if (eventInfo.EventHandlerType == typeof(EventHandler))
                {
                    // This one is easy because we know the types ...
                    eventInfo.AddEventHandler(notifications, (EventHandler)HandleEventAndForwardToListeners);
                }
                else
                {
                    // This one is not easy. So we need to create an EventHandler of the 
                    // correct type (EventHandler<T> where T is the correct type) and then
                    // attach it to the event.
                    var argsTypes = eventInfo.EventHandlerType.GetGenericArguments();
                    var handlerType = typeof(EventHandler<>).MakeGenericType(argsTypes);
                    EventHandler<EventArgs> handler = HandleEventAndForwardToListeners;

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

        private void HandleEventAndForwardToListeners(object sender, EventArgs args)
        {
            foreach (var handler in m_EventHandlers)
            {
                handler(Id, args);
            }
        }

        /// <summary>
        /// Gets the ID of the notification.
        /// </summary>
        public NotificationId Id
        {
            [DebuggerStepThrough]
            get
            {
                return m_Id;
            }
        }

        public void OnNotification(Action<NotificationId, EventArgs> notificationHandler)
        {
        }
    }
}