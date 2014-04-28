//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Defines several test events.
    /// </summary>
    internal sealed class TestNotifications
    {
        /// <summary>
        /// An event.
        /// </summary>
        public event EventHandler OnNotify;

        /// <summary>
        /// Raises the <see cref="OnNotify"/> event.
        /// </summary>
        public void RaiseOnNotify()
        {
            var local = OnNotify;
            if (local != null)
            {
                local(this, new EventArgs());
            }
        }
    }
}
