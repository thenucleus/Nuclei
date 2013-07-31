//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Provides the interface for objects that store a set of timing intervals in a 
    /// hierarchical order.
    /// </summary>
    public interface IStoreIntervals
    {
        /// <summary>
        /// Adds a top-level interval to the tree.
        /// </summary>
        /// <param name="interval">The interval.</param>
        void AddBaseInterval(ITimerInterval interval);

        /// <summary>
        /// Adds a new interval to the tree.
        /// </summary>
        /// <param name="parent">The parent interval.</param>
        /// <param name="interval">The interval that should be added.</param>
        void AddChildInterval(ITimerInterval parent, ITimerInterval interval);
    }
}
