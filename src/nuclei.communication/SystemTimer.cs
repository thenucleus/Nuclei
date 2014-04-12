//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Timers;

namespace Nuclei.Communication
{
    /// <summary>
    /// An <see cref="ITimer"/> that forms a facade for a <see cref="System.Timers.Timer"/>.
    /// </summary>
    internal sealed class SystemTimer : ITimer, IDisposable
    {
        /// <summary>
        /// The timer object.
        /// </summary>
        private readonly Timer m_Timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemTimer"/> class.
        /// </summary>
        /// <param name="interval">The timer interval.</param>
        public SystemTimer(TimeSpan interval)
        {
            m_Timer = new Timer(interval.TotalMilliseconds);
            m_Timer.AutoReset = true;
            m_Timer.Elapsed += TimerOnElapsed;

            m_Timer.Enabled = true;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            RaiseOnElapsed();
        }

        /// <summary>
        /// An event raised when the timer has elapsed.
        /// </summary>
        public event EventHandler OnElapsed;

        private void RaiseOnElapsed()
        {
            var local = OnElapsed;
            if (local != null)
            {
                local(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            m_Timer.Dispose();
        }
    }
}
