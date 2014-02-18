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
        private readonly IDictionary<TimingGroup, Stack<ITimerInterval>> m_ActiveIntervals 
            = new Dictionary<TimingGroup, Stack<ITimerInterval>>();

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
        /// <param name="group">The group to which the current timing belongs.</param>
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
        internal ITimerInterval MeasureInterval(TimingGroup group, string stepDescription)
        {
            if (!m_Timer.IsRunning)
            {
                m_Timer.Start();
            }

            var interval = new TimerInterval(this, group, stepDescription);
            StoreIntervalInTree(interval);

            interval.Start();
            return interval;
        }

        private void StoreIntervalInTree(TimerInterval interval)
        {
            Stack<ITimerInterval> stack;
            if (m_ActiveIntervals.ContainsKey(interval.Group) && (m_ActiveIntervals[interval.Group].Count > 0))
            {
                stack = m_ActiveIntervals[interval.Group];
                var parent = stack.Peek();
                m_Storage.AddChildInterval(parent, interval);
            }
            else
            {
                if (!m_ActiveIntervals.ContainsKey(interval.Group))
                {
                    m_ActiveIntervals.Add(interval.Group, new Stack<ITimerInterval>());
                }

                stack = m_ActiveIntervals[interval.Group];
                m_Storage.AddBaseInterval(interval);
            }

            stack.Push(interval);
        }

        /// <summary>
        /// Gets the current tick count.
        /// </summary>
        public long CurrentTicks
        {
            [DebuggerStepThrough]
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
                Debug.Assert(timerInterval != null, "The interval that should be stopped should not be a null reference.");
                Debug.Assert(
                    m_ActiveIntervals.ContainsKey(timerInterval.Group),
                    "The timer interval does not belong to a known timing group.");
                Debug.Assert(
                    ReferenceEquals(m_ActiveIntervals[timerInterval.Group].Peek(), timerInterval),
                    "Parent interval stopped before child intervals have stopped.");
            }

            var stack = m_ActiveIntervals[timerInterval.Group];
            stack.Pop();
            if (stack.Count == 0)
            {
                m_ActiveIntervals.Remove(timerInterval.Group);
            }

            if (m_ActiveIntervals.Count == 0)
            {
                m_Timer.Stop();
                m_Timer.Reset();
            }
        }
    }
}
