//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Nuclei.Diagnostics.Profiling.Reporting
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class TextReporterTest
    {
        private string TempFile()
        {
            var fileName = Path.GetRandomFileName();

            // Get the location of the assembly before it was shadow-copied
            // Note that Assembly.Codebase gets the path to the manifest-containing
            // file, not necessarily the path to the file that contains a
            // specific type.
            var uncPath = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            return Path.Combine(Path.GetDirectoryName(uncPath.LocalPath), fileName);
        }

        private string FromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        [Test]
        public void ToReport()
        {
            var group = new TimingGroup();
            var description = "description";
            var ticks = 10L;

            var interval = new Mock<ITimerInterval>();
            {
                interval.Setup(i => i.Group)
                    .Returns(group);
                interval.Setup(i => i.Description)
                    .Returns(description);
                interval.Setup(i => i.TotalTicks)
                    .Returns(ticks);
            }

            var path = TempFile();
            Func<Stream> builder = () => new FileStream(path, FileMode.Create, FileAccess.Write);

            var reporter = new TextReporter(builder);

            var tree = new TimingTree();
            tree.AddBaseInterval(interval.Object);

            var report = new TimingReport(tree);
            reporter.Transform(report);

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var result = FromStream(stream);
                Assert.AreEqual(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Description    Total time (ms)" + Environment.NewLine + "{0}    {1}" + Environment.NewLine,
                        description,
                        ticks / 10000),
                    result);
            }
        }

        [Test]
        public void ToReportWithMultiLevelIntervals()
        {
            var ticks = 10L;

            var group = new TimingGroup();
            var parent = new Mock<ITimerInterval>();
            {
                parent.Setup(i => i.Group)
                    .Returns(group);
                parent.Setup(i => i.Description)
                    .Returns("parent");
                parent.Setup(i => i.TotalTicks)
                    .Returns(ticks);
            }

            var child1 = new Mock<ITimerInterval>();
            {
                child1.Setup(i => i.Group)
                    .Returns(group);
                child1.Setup(i => i.Description)
                    .Returns("child1");
                child1.Setup(i => i.TotalTicks)
                    .Returns(ticks);
            }

            var child2 = new Mock<ITimerInterval>();
            {
                child2.Setup(i => i.Group)
                    .Returns(group);
                child2.Setup(i => i.Description)
                    .Returns("child2");
                child2.Setup(i => i.TotalTicks)
                    .Returns(ticks);
            }

            var path = TempFile();
            Func<Stream> builder = () => new FileStream(path, FileMode.Create, FileAccess.Write);

            var reporter = new TextReporter(builder);
            
            var tree = new TimingTree();
            tree.AddBaseInterval(parent.Object);
            tree.AddChildInterval(parent.Object, child1.Object);
            tree.AddChildInterval(parent.Object, child2.Object);

            var report = new TimingReport(tree);
            reporter.Transform(report);

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var result = FromStream(stream);
                var textBuilder = new StringBuilder();
                {
                    textBuilder.AppendLine("Description   Total time (ms)");
                    textBuilder.AppendLine(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "{0}        {1}",
                            parent.Object.Description,
                            ticks / 10000));
                    textBuilder.AppendLine(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "    {0}      {1}",
                            child1.Object.Description,
                            ticks / 10000));
                    textBuilder.AppendLine(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "    {0}      {1}",
                            child2.Object.Description,
                            ticks / 10000));
                }

                Assert.AreEqual(textBuilder.ToString(), result);
            }
        }
    }
}
