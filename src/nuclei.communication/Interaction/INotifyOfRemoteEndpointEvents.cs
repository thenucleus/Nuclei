//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for objects that handle receiving notifications from remote endpoints.
    /// </summary>
    public interface INotifyOfRemoteEndpointEvents
    {
        /// <summary>
        /// Returns a collection describing all the known endpoints and the notifications they
        /// provide.
        /// </summary>
        /// <returns>
        /// The collection describing all the known endpoints and the notifications they describe.
        /// </returns>
        IEnumerable<NotificationInformationPerEndpoint> AvailableNotifications();

        /// <summary>
        /// Returns a collection describing all the known notifications for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     The collection describing all the known notifications for the given endpoint.
        /// </returns>
        IEnumerable<Type> AvailableNotificationsFor(EndpointId endpoint);

        /// <summary>
        /// An event raised when an endpoint signs on and provides a set of notifications.
        /// </summary>
        event EventHandler<NotificationSetAvailabilityEventArgs> OnEndpointSignedIn;

        /// <summary>
        /// An event raised when an endpoint signs off.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointSignedOff;

        /// <summary>
        /// Returns a value indicating if there are any known notifications for a given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if there are known notifications for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasNotificationsFor(EndpointId endpoint);

        /// <summary>
        /// Returns a value indicating if a specific set of notifications is available for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <param name="notificationInterfaceType">The type of the notification that should be available.</param>
        /// <returns>
        ///     <see langword="true" /> if there are the specific notifications exist for the given endpoint; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasNotificationFor(EndpointId endpoint, Type notificationInterfaceType);

        /// <summary>
        /// Returns the notification proxy for the given endpoint.
        /// </summary>
        /// <typeparam name="TNotification">The typeof notification set that should be returned.</typeparam>
        /// <param name="endpoint">The ID number of the endpoint for which the notifications should be returned.</param>
        /// <returns>The requested notification set.</returns>
        TNotification NotificationsFor<TNotification>(EndpointId endpoint) where TNotification : class, INotificationSet;

        /// <summary>
        /// Returns the notification proxy for the given endpoint.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint for which the notification should be returned.</param>
        /// <param name="notificationType">The type of the notification.</param>
        /// <returns>The requested notification set.</returns>
        INotificationSet NotificationsFor(EndpointId endpoint, Type notificationType);
    }
}
