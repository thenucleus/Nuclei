//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the interface for objects that track registrations of notifications
    /// and handle sending the incoming notifications to the correct recipients.
    /// </summary>
    internal interface ISendNotifications
    {
        /// <summary>
        /// Registers a specific endpoint so that it may be notified when the specified event
        /// is raised.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="notification">The object that describes to which event the endpoint wants to be subscribed.</param>
        void RegisterForNotification(EndpointId endpoint, ISerializedEventRegistration notification);

        /// <summary>
        /// Deregisters a specific endpoint so that it will no longer be notified when the specified event
        /// is raised.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="notification">The object that describes from which event the endpoint wants to be unsubscribed.</param>
        void UnregisterFromNotification(EndpointId endpoint, ISerializedEventRegistration notification);
    }
}
