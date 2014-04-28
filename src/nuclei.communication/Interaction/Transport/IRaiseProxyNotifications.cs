//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Raises notifications on proxy objects.
    /// </summary>
    internal interface IRaiseProxyNotifications
    {
        /// <summary>
        /// Raises the notification with the given notification ID.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that raised the notification.</param>
        /// <param name="id">The ID of the notification.</param>
        /// <param name="args">The event arguments for the notification.</param>
        void RaiseNotification(EndpointId endpoint, NotificationId id, EventArgs args);
    }
}
