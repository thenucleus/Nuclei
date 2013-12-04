//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Moq;
using NUnit.Framework;

namespace Nuclei.Diagnostics.Profiling
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class TimerIntervalTest
    {
        [Test]
        public void Description()
        {
            var description = "Description";
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            using (var interval = new TimerInterval(owner.Object, new TimingGroup(), description))
            {
                interval.Start();
                Assert.AreSame(description, interval.Description);
            }
        }

        [Test]
        public void Start()
        {
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            var interval = new TimerInterval(owner.Object, new TimingGroup(), string.Empty);
            using (interval)
            {
                interval.Start();
                Assert.AreEqual(10L, interval.StartedAt);
            }
        }

        [Test]
        public void Stop()
        {
            var owner = new Mock<IIntervalOwner>();
            {
                owner.Setup(o => o.CurrentTicks)
                    .Returns(10L);
            }

            var interval = new TimerInterval(owner.Object, new TimingGroup(), string.Empty);
            using (interval)
            {
                interval.Start();
                owner.Setup(o => o.CurrentTicks)
                    .Returns(50L);
            }

            Assert.AreEqual(10L, interval.StartedAt);
            Assert.AreEqual(50L, interval.StoppedAt);
            Assert.AreEqual(40L, interval.TotalTicks);
        }
    }
}
