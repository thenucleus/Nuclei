//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the base for classes that implement a set of notifications 
    /// that can be watched remotely through a <see cref="INotifyOfRemoteEndpointEvents"/>.
    /// </summary>
    /// <design>
    /// The <see cref="INotifyOfRemoteEndpointEvents"/> implementations will generate a proxy object for all the notification sets
    /// available on a given endpoint.
    /// </design>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "This interface is used as marker interface for sets of notifications.")]
    public interface INotificationSet
    {
    }
}
