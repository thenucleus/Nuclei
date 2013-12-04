//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace Nuclei.Diagnostics.Profiling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ProfilerTest
    {
        [Test]
        public void Measure()
        {
            ITimerInterval storedInterval = null;
            var storage = new Mock<IStoreIntervals>();
            {
                storage.Setup(r => r.AddBaseInterval(It.IsAny<ITimerInterval>()))
                    .Callback<ITimerInterval>(i => storedInterval = i);
            }

            var profiler = new Profiler(storage.Object);

            var description = "description";
            var group = new TimingGroup();
            var interval = profiler.MeasureInterval(group, description);
            using (interval)
            {
                Thread.Sleep(10);
            }

            Assert.AreSame(interval, storedInterval);
            storage.Verify(r => r.AddChildInterval(It.IsAny<ITimerInterval>(), It.IsAny<ITimerInterval>()), Times.Never());
        }

        [Test]
        public void MeasureNested()
        {
            var timers = new List<Tuple<ITimerInterval, int>>();
            var storage = new Mock<IStoreIntervals>();
            {
                storage.Setup(r => r.AddBaseInterval(It.IsAny<ITimerInterval>()))
                    .Callback<ITimerInterval>(i => timers.Add(new Tuple<ITimerInterval, int>(i, 0)));
                storage.Setup(r => r.AddChildInterval(It.IsAny<ITimerInterval>(), It.IsAny<ITimerInterval>()))
                    .Callback<ITimerInterval, ITimerInterval>(
                        (parent, child) =>
                        {
                            var storedParent = timers.Find(t => ReferenceEquals(t.Item1, parent));
                            timers.Add(new Tuple<ITimerInterval, int>(child, storedParent.Item2 + 1));
                        });
            }

            var profiler = new Profiler(storage.Object);

            var description = "description";
            var group = new TimingGroup();
            ITimerInterval topLevelInterval;
            ITimerInterval firstChild;
            ITimerInterval secondChild;
            using (topLevelInterval = profiler.MeasureInterval(group, description))
            {
                using (firstChild = profiler.MeasureInterval(group, string.Empty))
                {
                    Thread.Sleep(10);
                }

                using (secondChild = profiler.MeasureInterval(group, string.Empty))
                {
                    Thread.Sleep(10);
                }
            }

            Assert.That(
                timers,
                Is.EquivalentTo(
                    new[] 
                        {
                            new Tuple<ITimerInterval, int>(topLevelInterval, 0),
                            new Tuple<ITimerInterval, int>(firstChild, 1),
                            new Tuple<ITimerInterval, int>(secondChild, 1),
                        }));
        }

        [Test]
        public void MeasureWithMultipleGroups()
        {
            ITimerInterval storedInterval = null;
            var storage = new Mock<IStoreIntervals>();
            {
                storage.Setup(r => r.AddBaseInterval(It.IsAny<ITimerInterval>()))
                    .Callback<ITimerInterval>(i => storedInterval = i);
            }

            var profiler = new Profiler(storage.Object);

            var description = "description";
            for (int i = 0; i < 10; i++)
            {
                var group = new TimingGroup();
                var interval = profiler.MeasureInterval(group, description);
                using (interval)
                {
                    Thread.Sleep(10);
                }

                Assert.AreSame(interval, storedInterval);
                storage.Verify(r => r.AddChildInterval(It.IsAny<ITimerInterval>(), It.IsAny<ITimerInterval>()), Times.Never());
            }
        }
    }
}
