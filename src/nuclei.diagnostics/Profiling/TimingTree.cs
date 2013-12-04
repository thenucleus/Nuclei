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
        private readonly Dictionary<TimingGroup, List<ITimerInterval>> m_Roots
            = new Dictionary<TimingGroup, List<ITimerInterval>>();

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
        {
            {
                Lokad.Enforce.Argument(() => treeToCopy);
            }

            foreach (var pair in treeToCopy.m_Roots)
            {
                var localTimings = new List<ITimerInterval>();
                m_Roots.Add(pair.Key, localTimings);
                for (int i = 0; i < pair.Value.Count; i++)
                {
                    localTimings.Add(pair.Value[i]);
                }

                foreach (var root in localTimings)
                {
                    CopyTreeSection(treeToCopy.m_Graph, root);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimingTree"/> class and copies the 
        /// provided tree from the start interval till the end interval.
        /// </summary>
        /// <param name="treeToCopy">The tree that should be copied.</param>
        /// <param name="root">The interval which should be copied. Must be a root interval.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="treeToCopy"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="UnknownIntervalException">
        ///     Thrown if <paramref name="root"/> is not part of the root of the tree.
        /// </exception>
        public TimingTree(TimingTree treeToCopy, ITimerInterval root)
        {
            {
                Lokad.Enforce.Argument(() => treeToCopy);
                Lokad.Enforce.Argument(() => root);

                Lokad.Enforce.With<UnknownIntervalException>(
                    treeToCopy.m_Roots.ContainsKey(root.Group),
                    Resources.Exceptions_Messages_UnknownInterval);
                Lokad.Enforce.With<UnknownIntervalException>(
                    treeToCopy.m_Roots[root.Group].Contains(root),
                    Resources.Exceptions_Messages_UnknownInterval);
            }

            var timings = treeToCopy.m_Roots[root.Group];
            int index = timings.IndexOf(root);

            var localTimings = new List<ITimerInterval>();
            m_Roots.Add(root.Group, localTimings);
            localTimings.Add(timings[index]);

            foreach (var interval in localTimings)
            {
                CopyTreeSection(treeToCopy.m_Graph, interval);
            }
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
        /// <exception cref="NonMatchingTimingGroupsException">
        ///     Thrown if <paramref name="start"/> and <paramref name="end"/> belong to a different timing group.
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
                Lokad.Enforce.Argument(() => start);
                Lokad.Enforce.Argument(() => end);
                
                Lokad.Enforce.With<UnknownIntervalException>(
                    treeToCopy.m_Roots.ContainsKey(start.Group),
                    Resources.Exceptions_Messages_UnknownInterval);
                Lokad.Enforce.With<UnknownIntervalException>(
                    treeToCopy.m_Roots.ContainsKey(end.Group),
                    Resources.Exceptions_Messages_UnknownInterval);

                Lokad.Enforce.With<NonMatchingTimingGroupsException>(
                    start.Group.Equals(end.Group),
                    Resources.Exceptions_Messages_NonMatchingTimingGroups);
                Lokad.Enforce.With<UnknownIntervalException>(
                    treeToCopy.m_Roots[start.Group].Contains(start),
                    Resources.Exceptions_Messages_UnknownInterval);
                Lokad.Enforce.With<UnknownIntervalException>(
                    treeToCopy.m_Roots[end.Group].Contains(end),
                    Resources.Exceptions_Messages_UnknownInterval);
            }

            var timings = treeToCopy.m_Roots[start.Group];
            int startIndex = (start != null) ? timings.IndexOf(start) : 0;
            int endIndex = (end != null) ? timings.IndexOf(end) + 1 : timings.Count;

            var localTimings = new List<ITimerInterval>();
            m_Roots.Add(start.Group, localTimings);
            for (int i = startIndex; i < endIndex; i++)
            {
                localTimings.Add(timings[i]);
            }

            foreach (var root in localTimings)
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

            if (!m_Roots.ContainsKey(interval.Group))
            {
                m_Roots.Add(interval.Group, new List<ITimerInterval>());
            }

            var list = m_Roots[interval.Group];
            list.Add(interval);
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
        /// <param name="group">The group for which the base intervals should be obtained.</param>
        /// <returns>
        /// The collection containing all the base intervals.
        /// </returns>
        public IEnumerable<ITimerInterval> BaseIntervals(TimingGroup group)
        {
            {
                Debug.Assert(group != null, "The timing group should not be a null reference.");
            }

            return m_Roots[group];
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
        /// Traverses the entire tree in pre-order (root before sub-trees) format.
        /// </summary>
        /// <param name="nodeAction">The action that is taken for each node.</param>
        public void TraversePreOrder(Action<ITimerInterval, int> nodeAction)
        {
            foreach (var group in m_Roots.Keys)
            {
                TraversePreOrder(group, nodeAction);
            }
        }

        /// <summary>
        /// Traverses the section of the tree with the specific group in pre-order (root before sub-trees) format.
        /// </summary>
        /// <param name="group">The group for which the traverse should take place.</param>
        /// <param name="nodeAction">The action that is taken for each node.</param>
        public void TraversePreOrder(TimingGroup group, Action<ITimerInterval, int> nodeAction)
        {
            if ((group == null) || !m_Roots.ContainsKey(group))
            {
                return;
            }

            var toVisit = new Stack<Tuple<ITimerInterval, int>>();
            var list = m_Roots[group];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                toVisit.Push(new Tuple<ITimerInterval, int>(list[i], 0));
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
