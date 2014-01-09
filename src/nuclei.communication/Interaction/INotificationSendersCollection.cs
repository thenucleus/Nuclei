//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for collections that store one or more <see cref="INotificationSet"/>
    /// objects.
    /// </summary>
    public interface INotificationSendersCollection : IEnumerable<KeyValuePair<Type, INotificationSet>>
    {
        /// <summary>
        /// Registers a <see cref="INotificationSet"/> object.
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
        void Store(Type notificationType, INotificationSet notifications);

        /// <summary>
        /// Returns the notification object that was registered for the given interface type.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="INotificationSet"/> derived interface.</typeparam>
        /// <returns>
        /// The desired notification set.
        /// </returns>
        T NotificationsFor<T>() where T : INotificationSet;

        /// <summary>
        /// Returns the notification object that was registered for the given interface type.
        /// </summary>
        /// <param name="interfaceType">The <see cref="INotificationSet"/> derived interface type.</param>
        /// <returns>
        /// The desired notification set.
        /// </returns>
        INotificationSet NotificationsFor(Type interfaceType);
    }
}
