//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Tracks the time between the starting of a measurement and the stopping of that same measurement.
    /// </summary>
    public sealed class Profiler : IIntervalOwner
    {
        /// <summary>
        /// The currently active timers.
        /// </summary>
        private readonly Stack<ITimerInterval> m_ActiveIntervals = new Stack<ITimerInterval>();

        /// <summary>
        /// The stopwatch that is used to track the time. Each timing interval will use
        /// the tick count for its start and stop.
        /// </summary>
        private readonly Stopwatch m_Timer = new Stopwatch();

        /// <summary>
        /// The object that stores the timing results.
        /// </summary>
        private readonly IStoreIntervals m_Storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Profiler"/> class.
        /// </summary>
        /// <param name="storage">The object that stores the timing results.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storage"/> is <see langword="null" />.
        /// </exception>
        public Profiler(IStoreIntervals storage)
        {
            {
                Lokad.Enforce.Argument(() => storage);
            }

            m_Storage = storage;
        }

        /// <summary>
        /// Starts the measurement of a time interval.
        /// </summary>
        /// <param name="stepDescription">The description for the new interval.</param>
        /// <returns>
        /// The object that describes the interval.
        /// </returns>
        /// <example>
        /// <code>
        /// using(var interval = profiler.Measure("A"))
        /// {
        ///     // Do stuff here ...
        /// }
        /// </code>
        /// </example>
        internal ITimerInterval MeasureInterval(string stepDescription)
        {
            if (!m_Timer.IsRunning)
            {
                m_Timer.Start();
            }

            var interval = new TimerInterval(this, stepDescription);
            StoreIntervalInTree(interval);

            interval.Start();
            return interval;
        }

        private void StoreIntervalInTree(TimerInterval interval)
        {
            if (m_ActiveIntervals.Count > 0)
            {
                var parent = m_ActiveIntervals.Peek();
                m_Storage.AddChildInterval(parent, interval);
            }
            else
            {
                m_Storage.AddBaseInterval(interval);
            }

            m_ActiveIntervals.Push(interval);
        }

        /// <summary>
        /// Gets the current tick count.
        /// </summary>
        public long CurrentTicks
        {
            get
            {
                return m_Timer.ElapsedTicks;
            }
        }

        /// <summary>
        /// Indicates to the owner that the interval has been closed and is
        /// no longer measuring time.
        /// </summary>
        /// <param name="timerInterval">The interval that has been closed.</param>
        public void StopInterval(ITimerInterval timerInterval)
        {
            {
                Debug.Assert(
                    ReferenceEquals(m_ActiveIntervals.Peek(), timerInterval),
                    "Parent interval stopped before child intervals have stopped.");
            }

            m_ActiveIntervals.Pop();
            if (m_ActiveIntervals.Count == 0)
            {
                m_Timer.Stop();
                m_Timer.Reset();
            }
        }
    }
}
