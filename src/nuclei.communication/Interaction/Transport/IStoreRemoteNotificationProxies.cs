//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Defines the interface for objects that store notification proxies.
    /// </summary>
    internal interface IStoreRemoteNotificationProxies
    {
        /// <summary>
        /// Handles the reception of new notification types.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that owns the notifications.</param>
        /// <param name="notificationTypes">An array containing the notification types for a given endpoint.</param>
        void OnReceiptOfEndpointNotifications(EndpointId endpoint, IEnumerable<OfflineTypeInformation> notificationTypes);
    }
}