//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Defines the interface for an object that owns one or more timing intervals.
    /// </summary>
    public interface IIntervalOwner
    {
        /// <summary>
        /// Gets the current tick count.
        /// </summary>
        long CurrentTicks 
        { 
            get;
        }

        /// <summary>
        /// Indicates to the owner that the interval has been closed and is
        /// no longer measuring time.
        /// </summary>
        /// <param name="timerInterval">The interval that has been closed.</param>
        void StopInterval(ITimerInterval timerInterval);
    }
}
