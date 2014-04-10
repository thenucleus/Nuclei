// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for timer objects.
    /// </summary>
    internal interface ITimer
    {
        /// <summary>
        /// An event raised when the timer has elapsed.
        /// </summary>
        event EventHandler Elapsed;
    }
}
