//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nuclei.Diagnostics.Properties;
using QuickGraph;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Stores the timing intervals as a tree.
    /// </summary>
    internal sealed class TimingTree
    {
        /// <summary>
        /// The collection that holds the timing intervals at the root of the timing process.
        /// </summary>
        private readonly BidirectionalGraph<ITimerInterval, Edge<ITimerInterval>> m_Graph
            = new BidirectionalGraph<ITimerInterval, Edge<ITimerInterval>>(false);

        /// <summary>
        /// The intervals that form the top-level intervals.
        /// </summary>
        private readonly List<ITimerInterval> m_Roots
            = new List<ITimerInterval>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingTree"/> class.
        /// </summary>
        public TimingTree()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingTree"/> class and copies all the data 
        /// from the specified tree.
        /// </summary>
        /// <param name="treeToCopy">The tree that should be copied.</param>
        public TimingTree(TimingTree treeToCopy)
            : this(treeToCopy, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingTree"/> class and copies the 
        /// provided tree from the start interval till the end interval.
        /// </summary>
        /// <param name="treeToCopy">The tree that should be copied.</param>
        /// <param name="start">The interval where the copy should be started from. Must be a root interval.</param>
        /// <param name="end">The interval where the copy should be stopped at. Must be a root interval.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="treeToCopy"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownIntervalException">
        ///     Thrown if <paramref name="start"/> is not part of the root of the tree.
        /// </exception>
        /// <exception cref="UnknownIntervalException">
        ///     Thrown if <paramref name="end"/> is not part of the root of the tree.
        /// </exception>
        public TimingTree(TimingTree treeToCopy, ITimerInterval start, ITimerInterval end)
        {
            {
                Lokad.Enforce.Argument(() => treeToCopy);

                Lokad.Enforce.With<UnknownIntervalException>(
                    (start == null) || treeToCopy.m_Roots.Contains(start),
                    Resources.Exceptions_Messages_UnknownInterval);
                Lokad.Enforce.With<UnknownIntervalException>(
                    (end == null) || treeToCopy.m_Roots.Contains(end),
                    Resources.Exceptions_Messages_UnknownInterval);
            }

            int startIndex = (start != null) ? treeToCopy.m_Roots.IndexOf(start) : 0;
            int endIndex = (end != null) ? treeToCopy.m_Roots.IndexOf(end) + 1 : treeToCopy.m_Roots.Count;
            for (int i = startIndex; i < endIndex; i++)
            {
                m_Roots.Add(treeToCopy.m_Roots[i]);
            }

            foreach (var root in m_Roots)
            {
                CopyTreeSection(treeToCopy.m_Graph, root);
            }
        }

        private void CopyTreeSection(BidirectionalGraph<ITimerInterval, Edge<ITimerInterval>> treeToCopy, ITimerInterval root)
        {
            m_Graph.AddVertex(root);

            var newVertices = new Queue<ITimerInterval>();
            newVertices.Enqueue(root);
            while (newVertices.Count > 0)
            {
                var source = newVertices.Dequeue();
                var outEdges = treeToCopy.OutEdges(source);
                foreach (var outEdge in outEdges)
                {
                    var target = outEdge.Target;
                    m_Graph.AddVertex(target);
                    m_Graph.AddEdge(new Edge<ITimerInterval>(source, target));

                    newVertices.Enqueue(target);
                }
            }
        }

        /// <summary>
        /// Adds a top-level interval to the tree.
        /// </summary>
        /// <param name="interval">The interval.</param>
        public void AddBaseInterval(ITimerInterval interval)
        {
            {
                Debug.Assert(interval != null, "Cannot add a null reference to the tree.");
            }

            m_Graph.AddVertex(interval);
            m_Roots.Add(interval);
        }

        /// <summary>
        /// Adds a new interval to the tree.
        /// </summary>
        /// <param name="parent">The parent interval.</param>
        /// <param name="interval">The interval that should be added.</param>
        public void AddChildInterval(ITimerInterval parent, ITimerInterval interval)
        {
            {
                Debug.Assert(parent != null, "Cannot link an interval to a null reference.");
                Debug.Assert(interval != null, "Cannot add a null reference to the tree.");
                Debug.Assert(m_Graph.ContainsVertex(parent), "Cannot add a child interval for a non-existing parent.");
            }

            m_Graph.AddVertex(interval);
            m_Graph.AddEdge(new Edge<ITimerInterval>(parent, interval));
        }

        /// <summary>
        /// Returns the collection of base intervals that have been registered.
        /// </summary>
        /// <returns>
        /// The collection containing all the base intervals.
        /// </returns>
        public IEnumerable<ITimerInterval> BaseIntervals()
        {
            return m_Roots;
        }

        /// <summary>
        /// Returns the intervals that are direct children of the specified interval.
        /// </summary>
        /// <param name="interval">The parent interval.</param>
        /// <returns>
        /// The collection of direct child intervals for the given interval.
        /// </returns>
        public IEnumerable<ITimerInterval> ChildIntervals(ITimerInterval interval)
        {
            return from edge in m_Graph.OutEdges(interval)
                   select edge.Target;
        }

        /// <summary>
        /// Traverses the tree in pre-order (root before sub-trees) format.
        /// </summary>
        /// <param name="nodeAction">The action that is taken for each node.</param>
        public void TraversePreOrder(Action<ITimerInterval, int> nodeAction)
        {
            var toVisit = new Stack<Tuple<ITimerInterval, int>>();
            for (int i = m_Roots.Count - 1; i >= 0; i--)
            {
                toVisit.Push(new Tuple<ITimerInterval, int>(m_Roots[i], 0));
            }

            while (toVisit.Count > 0)
            {
                var node = toVisit.Pop();
                var interval = node.Item1;
                var level = node.Item2;

                nodeAction(interval, level);
                var children = ChildIntervals(interval).ToList();
                for (int i = children.Count - 1; i >= 0; i--)
                {
                    toVisit.Push(new Tuple<ITimerInterval, int>(children[i], level + 1));
                }
            }
        }
    }
}
