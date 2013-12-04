//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Defines the interface for objects that store an interval of time.
    /// </summary>
    public interface ITimerInterval : IDisposable
    {
        /// <summary>
        /// Gets the group to which the current interval belongs.
        /// </summary>
        TimingGroup Group
        {
            get;
        }

        /// <summary>
        /// Gets the description for the current interval.
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Gets the tick count at which the interval measurement was started.
        /// </summary>
        long StartedAt
        {
            get;
        }

        /// <summary>
        /// Gets the tick count at which the interval measurement was stopped.
        /// </summary>
        long StoppedAt
        {
            get;
        }

        /// <summary>
        /// Gets the number of ticks for the interval, or 0 if the interval has not been
        /// determined yet.
        /// </summary>
        long TotalTicks
        {
            get;
        }
    }
}
