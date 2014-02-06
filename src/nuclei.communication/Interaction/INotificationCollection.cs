//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for collections that store one or more <see cref="INotificationSet"/>
    /// objects.
    /// </summary>
    public interface INotificationCollection : IEnumerable<Tuple<Type, INotificationSet>>
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
    }
}
