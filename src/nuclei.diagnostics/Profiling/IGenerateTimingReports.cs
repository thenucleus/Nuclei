//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Defines the interface for objects that generate timing reports.
    /// </summary>
    public interface IGenerateTimingReports
    {
        /// <summary>
        /// Creates a new report that contains all the timing intervals from the
        /// moment the profiler was started till the current point in time.
        /// </summary>
        /// <returns>A new report that contains all known timing intervals.</returns>
        TimingReport FromStartTillEnd();

        /// <summary>
        /// Creates a new report that contains all timing intervals starting at the specified
        /// <paramref name="start"/> interval up to the <paramref name="inclusiveEnd"/> interval,
        /// including all child intervals.
        /// </summary>
        /// <param name="start">The start interval.</param>
        /// <param name="inclusiveEnd">The end interval.</param>
        /// <returns>A new report that contains all desired intervals.</returns>
        TimingReport FromIntervalToInterval(ITimerInterval start, ITimerInterval inclusiveEnd);

        /// <summary>
        /// Creates a new report that contains all the timing intervals starting and ending
        /// at the specified <paramref name="interval"/>.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>A new report that contains all the desired intervals.</returns>
        TimingReport ForInterval(ITimerInterval interval);
    }
}
