//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Stores all the timing intervals in a hierarchical fashion.
    /// </summary>
    public sealed class TimingStorage : IStoreIntervals, IGenerateTimingReports
    {
        /// <summary>
        /// The tree that contains all the intervals that have been provided
        /// since the last report was build.
        /// </summary>
        private readonly TimingTree m_Tree = new TimingTree();

        /// <summary>
        /// Adds a top-level interval to the tree.
        /// </summary>
        /// <param name="interval">The interval.</param>
        public void AddBaseInterval(ITimerInterval interval)
        {
            m_Tree.AddBaseInterval(interval);
        }

        /// <summary>
        /// Adds a new interval to the tree.
        /// </summary>
        /// <param name="parent">The parent interval.</param>
        /// <param name="interval">The interval that should be added.</param>
        public void AddChildInterval(ITimerInterval parent, ITimerInterval interval)
        {
            m_Tree.AddChildInterval(parent, interval);
        }

        /// <summary>
        /// Creates a new report that contains all the timing intervals from the
        /// moment the profiler was started till the current point in time.
        /// </summary>
        /// <returns>A new report that contains all known timing intervals.</returns>
        public TimingReport FromStartTillEnd()
        {
            var tree = CopyTreeSection(null, null);
            return new TimingReport(tree);
        }

        private TimingTree CopyTreeSection(ITimerInterval start, ITimerInterval end)
        {
            return new TimingTree(m_Tree, start, end);
        }

        /// <summary>
        /// Creates a new report that contains all timing intervals starting at the
        /// specified interval till the current point in time.
        /// </summary>
        /// <param name="interval">The interval from which the report should start.</param>
        /// <returns>A new report that contains all intervals since the specified interval.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="interval"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownIntervalException">
        ///     Thrown if <paramref name="interval"/> is not part of the root of the tree.
        /// </exception>
        public TimingReport FromIntervalTillEnd(ITimerInterval interval)
        {
            {
                Lokad.Enforce.Argument(() => interval);
            }

            var tree = CopyTreeSection(interval, null);
            return new TimingReport(tree);
        }

        /// <summary>
        /// Creates a new report that contains all timing intervals starting at the specified
        /// <paramref name="start"/> interval up to the <paramref name="inclusiveEnd"/> interval,
        /// including all child intervals.
        /// </summary>
        /// <param name="start">The start interval.</param>
        /// <param name="inclusiveEnd">The end interval.</param>
        /// <returns>A new report that contains all desired intervals.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="start"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="inclusiveEnd"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownIntervalException">
        ///     Thrown if <paramref name="start"/> is not part of the root of the tree.
        /// </exception>
        /// <exception cref="UnknownIntervalException">
        ///     Thrown if <paramref name="inclusiveEnd"/> is not part of the root of the tree.
        /// </exception>
        public TimingReport FromIntervalToInterval(ITimerInterval start, ITimerInterval inclusiveEnd)
        {
            {
                Lokad.Enforce.Argument(() => start);
                Lokad.Enforce.Argument(() => inclusiveEnd);
            }

            var tree = CopyTreeSection(start, inclusiveEnd);
            return new TimingReport(tree);
        }

        /// <summary>
        /// Creates a new report that contains all the timing intervals starting and ending
        /// at the specified <paramref name="interval"/>.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns>A new report that contains all the desired intervals.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="interval"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownIntervalException">
        ///     Thrown if <paramref name="interval"/> is not part of the root of the tree.
        /// </exception>
        public TimingReport ForInterval(ITimerInterval interval)
        {
            {
                Lokad.Enforce.Argument(() => interval);
            }

            var tree = CopyTreeSection(interval, interval);
            return new TimingReport(tree);
        }
    }
}
