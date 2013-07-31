//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Stores information about one or more timer intervals in reportable form.
    /// </summary>
    public sealed class TimingReport
    {
        /// <summary>
        /// The tree that contains all the intervals that have been provided
        /// since the last report was build.
        /// </summary>
        private readonly TimingTree m_Tree;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingReport"/> class.
        /// </summary>
        /// <param name="tree">The timing tree for the current report.</param>
        internal TimingReport(TimingTree tree)
        {
            {
                Debug.Assert(tree != null, "Cannot create a report with a null tree reference.");
            }

            m_Tree = tree;
        }

        /// <summary>
        /// Traverses the tree in order of element creation.
        /// </summary>
        /// <param name="action">
        /// The action that is taken for each element in the report, providing the interval and the level of the interval,
        /// where the level starts at 0 for the top level element and increases monotonically for sub-elements.
        /// </param>
        public void Traverse(Action<ITimerInterval, int> action)
        {
            m_Tree.TraversePreOrder(action);
        }
    }
}
