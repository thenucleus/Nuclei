//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Defines extensions for the <see cref="Profiler"/>.
    /// </summary>
    public static class ProfilerExtensions
    {
        /// <summary>
        /// Returns an <see cref="ITimerInterval"/> object reference.
        /// </summary>
        /// <param name="profiler">The profiler which should provide the timer interval. May be <see langword="null" />.</param>
        /// <param name="description">The description for the interval.</param>
        /// <returns>
        /// A new <see cref="ITimerInterval"/> if the <paramref name="profiler"/> reference is not <see langword="null" />;
        /// otherwise, <see langword="null" />.
        /// </returns>
        /// <example>
        /// <code>
        /// var profiler = GetProfiler(); // Ok if this returns null.
        /// using(profiler.Measure("A"))
        /// {
        ///     // Do stuff here ...
        /// }
        /// </code>
        /// </example>
        public static IDisposable Measure(this Profiler profiler, string description)
        {
            return (profiler != null) ? profiler.MeasureInterval(description) : null;
        }
    }
}
