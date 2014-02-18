//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Diagnostics.Properties;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Stores a measurement interval.
    /// </summary>
    internal sealed class TimerInterval : ITimerInterval
    {
        /// <summary>
        /// The profiler who owns the current interval.
        /// </summary>
        private readonly IIntervalOwner m_Owner;

        /// <summary>
        /// The group to which the current interval belongs.
        /// </summary>
        private readonly TimingGroup m_Group;

        /// <summary>
        /// The description for the current interval.
        /// </summary>
        private readonly string m_IntervalDescription;

        /// <summary>
        /// The number of ticks as given by the <see cref="IIntervalOwner"/> when the current
        /// interval was started.
        /// </summary>
        private long m_StartTicks;

        /// <summary>
        /// The number of ticks as given by the <see cref="IIntervalOwner"/> when the current interval
        /// was stopped.
        /// </summary>
        private long m_StopTicks;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimerInterval"/> class.
        /// </summary>
        /// <param name="owner">The objects which owns the current interval.</param>
        /// <param name="group">The group to which the current interval belongs.</param>
        /// <param name="intervalDescription">The description for the current interval.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="owner"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="group"/> is <see langword="null" />.
        /// </exception>
        internal TimerInterval(IIntervalOwner owner, TimingGroup group, string intervalDescription)
        {
            {
                Lokad.Enforce.Argument(() => owner);
                Lokad.Enforce.Argument(() => group);
            }

            m_Owner = owner;
            m_Group = group;
            m_IntervalDescription = intervalDescription;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TimerInterval"/> class.
        /// </summary>
        /// <remarks>
        /// This finalizer is really only here because we want to make sure that all intervals are
        /// properly disposed of.
        /// </remarks>
        ~TimerInterval()
        {
            {
                Debug.Assert(false, "The interval has not be disposed of correctly. Measurement will be invalid.");
            }

            TerminateMeasurement();
        }

        /// <summary>
        /// Starts the measurement of the interval.
        /// </summary>
        public void Start()
        {
            {
                Lokad.Enforce.With<CanOnlyMeasureIntervalOnceException>(
                    m_StartTicks == 0,
                    Resources.Exceptions_Messages_CanOnlyMeasureIntervalOnce);
            }

            m_StartTicks = m_Owner.CurrentTicks;
        }

        /// <summary>
        /// Gets the group to which the current interval belongs.
        /// </summary>
        public TimingGroup Group
        {
            [DebuggerStepThrough]
            get
            {
                return m_Group;
            }
        }

        /// <summary>
        /// Gets the description for the current interval.
        /// </summary>
        public string Description
        {
            [DebuggerStepThrough]
            get
            {
                return m_IntervalDescription;
            }
        }

        /// <summary>
        /// Gets the tick count at which the interval measurement was started.
        /// </summary>
        public long StartedAt
        {
            [DebuggerStepThrough]
            get
            {
                return m_StartTicks;
            }
        }

        /// <summary>
        /// Gets the tick count at which the interval measurement was stopped.
        /// </summary>
        public long StoppedAt
        {
            [DebuggerStepThrough]
            get
            {
                return m_StopTicks;
            }
        }

        /// <summary>
        /// Gets the number of ticks for the interval, or 0 if the interval has not been
        /// determined yet.
        /// </summary>
        public long TotalTicks
        {
            [DebuggerStepThrough]
            get
            {
                return (m_StopTicks > 0) ? m_StopTicks - m_StartTicks : 0;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            TerminateMeasurement();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Stops the measurement and notifies the owner that the current interval is complete.
        /// </summary>
        private void TerminateMeasurement()
        {
            {
                Debug.Assert(
                    m_StartTicks != 0,
                    "The measurement was stopped before it was started.");
            }

            m_StopTicks = m_Owner.CurrentTicks;
            m_Owner.StopInterval(this);
        }
    }
}
