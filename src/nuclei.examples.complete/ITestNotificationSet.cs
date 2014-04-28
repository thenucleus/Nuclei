//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Interaction;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Defines a set of test notifications.
    /// </summary>
    public interface ITestNotificationSet : INotificationSet
    {
        /// <summary>
        /// An event raised to test notifications without event data.
        /// </summary>
        event EventHandler OnNotify;
    }
}
