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
    /// Defines the mapping of the methods of a <see cref="INotificationSet"/> to a set of delegates.
    /// </summary>
    public sealed class NotificationMap
    {
        /// <summary>
        /// The type of the command set.
        /// </summary>
        private readonly Type m_NotificationType;

        /// <summary>
        /// The mappings for each of the notification events.
        /// </summary>
        private readonly NotificationDefinition[] m_Definitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationMap"/> class.
        /// </summary>
        /// <param name="notificationType">The type of the notification set.</param>
        /// <param name="definitions">The mappings of each of the notification events.</param>
        internal NotificationMap(Type notificationType, NotificationDefinition[] definitions)
        {
            {
                Lokad.Enforce.Argument(() => notificationType);
                Lokad.Enforce.Argument(() => definitions);
            }

            m_NotificationType = notificationType;
            m_Definitions = definitions;
        }

        /// <summary>
        /// Gets the type of the notification set.
        /// </summary>
        internal Type NotificationType
        {
            [DebuggerStepThrough]
            get
            {
                return m_NotificationType;
            }
        }

        /// <summary>
        /// Gets the mappings for each of the notification events.
        /// </summary>
        internal NotificationDefinition[] Definitions
        {
            [DebuggerStepThrough]
            get
            {
                return m_Definitions;
            }
        }
    }
}
