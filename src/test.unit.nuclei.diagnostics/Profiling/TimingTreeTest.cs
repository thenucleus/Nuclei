//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace Nuclei.Diagnostics.Profiling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class TimingTreeTest
    {
        private static TimingTree BuildTree(ITimerInterval[] parents, ITimerInterval[][] children)
        {
            var result = new TimingTree();
            for (int i = 0; i < parents.Length; i++)
            {
                result.AddBaseInterval(parents[i]);
                for (int j = 0; j < children[i].Length; j++)
                {
                    result.AddChildInterval(parents[i], children[i][j]);
                }
            }

            return result;
        }

        private static void CleanUpIntervals(ITimerInterval[] parents, ITimerInterval[][] children)
        {
            foreach (var parent in parents)
            {
                ((TimerInterval)parent).Start();
                parent.Dispose();
            }

            foreach (var subChildren in children)
            {
                foreach (var subChild in subChildren)
                {
                    ((TimerInterval)subChild).Start();
                    subChild.Dispose();
                }
            }
        }

        [Test]
        public void CopyTreeCompletely()
        {
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            var parents = new ITimerInterval[] 
                { 
                    new TimerInterval(owner.Object, "a"),
                    new TimerInterval(owner.Object, "b"),
                    new TimerInterval(owner.Object, "c"),
                };

            var children = new ITimerInterval[][] 
                { 
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "aa"),
                            new TimerInterval(owner.Object, "ab"),
                            new TimerInterval(owner.Object, "ac"),
                        },
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "ba"),
                            new TimerInterval(owner.Object, "bb"),
                            new TimerInterval(owner.Object, "bc"),
                        },
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "ca"),
                            new TimerInterval(owner.Object, "cb"),
                            new TimerInterval(owner.Object, "cc"),
                        },
                };

            var tree = BuildTree(parents, children);
            var otherTree = new TimingTree(tree);

            Assert.That(otherTree.BaseIntervals(), Is.EquivalentTo(parents));
            Assert.That(otherTree.ChildIntervals(parents[0]), Is.EquivalentTo(children[0]));
            Assert.That(otherTree.ChildIntervals(parents[1]), Is.EquivalentTo(children[1]));
            Assert.That(otherTree.ChildIntervals(parents[2]), Is.EquivalentTo(children[2]));

            CleanUpIntervals(parents, children);
        }

        [Test]
        public void CopyTreeBetweenPoints()
        {
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            var parents = new ITimerInterval[] 
                { 
                    new TimerInterval(owner.Object, "a"),
                    new TimerInterval(owner.Object, "b"),
                    new TimerInterval(owner.Object, "c"),
                };

            var children = new ITimerInterval[][] 
                { 
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "aa"),
                            new TimerInterval(owner.Object, "ab"),
                            new TimerInterval(owner.Object, "ac"),
                        },
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "ba"),
                            new TimerInterval(owner.Object, "bb"),
                            new TimerInterval(owner.Object, "bc"),
                        },
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "ca"),
                            new TimerInterval(owner.Object, "cb"),
                            new TimerInterval(owner.Object, "cc"),
                        },
                };

            var tree = BuildTree(parents, children);
            var otherTree = new TimingTree(tree, parents[1], parents[1]);

            Assert.That(otherTree.BaseIntervals(), Is.EquivalentTo(new ITimerInterval[] { parents[1] }));
            Assert.That(otherTree.ChildIntervals(parents[1]), Is.EquivalentTo(children[1]));

            CleanUpIntervals(parents, children);
        }

        [Test]
        public void AddBaseInterval()
        {
            var tree = new TimingTree();
            
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            using (var interval = new TimerInterval(owner.Object, "a"))
            {
                interval.Start();
                tree.AddBaseInterval(interval);

                Assert.That(
                    tree.BaseIntervals(),
                    Is.EquivalentTo(
                        new ITimerInterval[]
                            {
                                interval
                            }));
            }
        }

        [Test]
        public void AddChildInterval()
        {
            var tree = new TimingTree();

            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            using (var parent = new TimerInterval(owner.Object, "a"))
            {
                parent.Start();
                tree.AddBaseInterval(parent);
                using (var child = new TimerInterval(owner.Object, "b"))
                {
                    child.Start();
                    tree.AddChildInterval(parent, child);
                    Assert.That(tree.ChildIntervals(parent), Is.EquivalentTo(new ITimerInterval[] { child }));
                }
            }
        }

        [Test]
        public void TraversePreOrderWithSingleRoot()
        { 
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            var parents = new ITimerInterval[] 
                { 
                    new TimerInterval(owner.Object, "a"),
                };

            var children = new ITimerInterval[][] 
                { 
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "aa"),
                            new TimerInterval(owner.Object, "ab"),
                            new TimerInterval(owner.Object, "ac"),
                        },
                };

            var tree = BuildTree(parents, children);

            var list = new List<Tuple<ITimerInterval, int>>();
            tree.TraversePreOrder(
                (node, level) =>
                {
                    list.Add(new Tuple<ITimerInterval, int>(node, level));
                });

            Assert.AreEqual(parents[0], list[0].Item1);
            Assert.AreEqual(0, list[0].Item2);
            Assert.AreEqual(children[0][0], list[1].Item1);
            Assert.AreEqual(1, list[1].Item2);
            Assert.AreEqual(children[0][1], list[2].Item1);
            Assert.AreEqual(1, list[2].Item2);
            Assert.AreEqual(children[0][2], list[3].Item1);
            Assert.AreEqual(1, list[3].Item2);

            CleanUpIntervals(parents, children);
        }

        [Test]
        public void TraversePreOrderWithMultipleRoots()
        {
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            var parents = new ITimerInterval[] 
                { 
                    new TimerInterval(owner.Object, "a"),
                    new TimerInterval(owner.Object, "b"),
                    new TimerInterval(owner.Object, "c"),
                };

            var children = new ITimerInterval[][] 
                { 
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "aa"),
                            new TimerInterval(owner.Object, "ab"),
                            new TimerInterval(owner.Object, "ac"),
                        },
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "ba"),
                            new TimerInterval(owner.Object, "bb"),
                            new TimerInterval(owner.Object, "bc"),
                        },
                    new ITimerInterval[] 
                        {
                            new TimerInterval(owner.Object, "ca"),
                            new TimerInterval(owner.Object, "cb"),
                            new TimerInterval(owner.Object, "cc"),
                        },
                };

            var tree = BuildTree(parents, children);

            var list = new List<Tuple<ITimerInterval, int>>();
            tree.TraversePreOrder(
                (node, level) =>
                {
                    list.Add(new Tuple<ITimerInterval, int>(node, level));
                });

            Assert.AreEqual(parents[0], list[0].Item1);
            Assert.AreEqual(0, list[0].Item2);
            Assert.AreEqual(children[0][0], list[1].Item1);
            Assert.AreEqual(1, list[1].Item2);
            Assert.AreEqual(children[0][1], list[2].Item1);
            Assert.AreEqual(1, list[2].Item2);
            Assert.AreEqual(children[0][2], list[3].Item1);
            Assert.AreEqual(1, list[3].Item2);

            Assert.AreEqual(parents[1], list[4].Item1);
            Assert.AreEqual(0, list[4].Item2);
            Assert.AreEqual(children[1][0], list[5].Item1);
            Assert.AreEqual(1, list[5].Item2);
            Assert.AreEqual(children[1][1], list[6].Item1);
            Assert.AreEqual(1, list[6].Item2);
            Assert.AreEqual(children[1][2], list[7].Item1);
            Assert.AreEqual(1, list[7].Item2);

            Assert.AreEqual(parents[2], list[8].Item1);
            Assert.AreEqual(0, list[8].Item2);
            Assert.AreEqual(children[2][0], list[9].Item1);
            Assert.AreEqual(1, list[9].Item2);
            Assert.AreEqual(children[2][1], list[10].Item1);
            Assert.AreEqual(1, list[10].Item2);
            Assert.AreEqual(children[2][2], list[11].Item1);
            Assert.AreEqual(1, list[11].Item2);

            CleanUpIntervals(parents, children);
        }
    }
}
