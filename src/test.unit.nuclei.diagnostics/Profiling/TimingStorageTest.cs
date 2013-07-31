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
    public sealed class TimingStorageTest
    {
        private static TimingStorage BuildStorage(ITimerInterval[] parents, ITimerInterval[][] children)
        {
            var result = new TimingStorage();
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

        private static void CheckReport(TimingReport report, ITimerInterval[] parents, ITimerInterval[][] children)
        {
            var list = new List<Tuple<ITimerInterval, int>>();
            report.Traverse((node, level) => list.Add(new Tuple<ITimerInterval, int>(node, level)));

            int index = 0;
            for (int i = 0; i < parents.Length; i++)
            {
                Assert.AreEqual(parents[i], list[index].Item1);
                Assert.AreEqual(0, list[index].Item2);
                index++;

                for (int j = 0; j < children[i].Length; j++)
                {
                    Assert.AreEqual(children[i][j], list[index].Item1);
                    Assert.AreEqual(1, list[index].Item2);

                    index++;
                }
            }
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
        public void FromStartTillEndWithSingleBase()
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

            var storage = BuildStorage(parents, children);
            var report = storage.FromStartTillEnd();
            CheckReport(report, parents, children);

            CleanUpIntervals(parents, children);
        }

        [Test]
        public void FromStartTillEndWithMultipleBases()
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

            var storage = BuildStorage(parents, children);
            var report = storage.FromStartTillEnd();
            CheckReport(report, parents, children);

            CleanUpIntervals(parents, children);
        }

        [Test]
        public void FromIntervalTillEnd()
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

            var storage = BuildStorage(parents, children);
            var report = storage.FromIntervalTillEnd(parents[1]);
            CheckReport(report, new ITimerInterval[] { parents[1], parents[2] }, new ITimerInterval[][] { children[1], children[2] });

            CleanUpIntervals(parents, children);
        }

        [Test]
        public void FromIntervalToInterval()
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

            var storage = BuildStorage(parents, children);
            var report = storage.FromIntervalToInterval(parents[0], parents[1]);
            CheckReport(report, new ITimerInterval[] { parents[0], parents[1] }, new ITimerInterval[][] { children[0], children[1] });

            CleanUpIntervals(parents, children);
        }

        [Test]
        public void ForInterval()
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

            var storage = BuildStorage(parents, children);
            var report = storage.ForInterval(parents[1]);
            CheckReport(report, new ITimerInterval[] { parents[1] }, new ITimerInterval[][] { children[1] });

            CleanUpIntervals(parents, children);
        }
    }
}
