//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Nuclei.Diagnostics.Profiling.Reporting
{
    /// <summary>
    /// Reports timing results in text format.
    /// </summary>
    public sealed class TextReporter : ITransformReports
    {
        /// <summary>
        /// Defines the indentation object.
        /// </summary>
        private const string IndentationPrimitive = "  ";

        /// <summary>
        /// The stream builder which provides the stream to which the report should be written.
        /// </summary>
        private readonly Func<Stream> m_StreamBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReporter"/> class.
        /// </summary>
        /// <param name="streamBuilder">The stream builder which provides the stream to which the report should be written.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="streamBuilder"/> is <see langword="null" />.
        /// </exception>
        public TextReporter(Func<Stream> streamBuilder)
        {
            {
                Lokad.Enforce.Argument(() => streamBuilder);
            }

            m_StreamBuilder = streamBuilder;
        }

        /// <summary>
        /// Transforms the report.
        /// </summary>
        /// <param name="report">The report that should be transformed.</param>
        public void Transform(TimingReport report)
        {
            // Number of spaces we want to offset the timing values from the text
            // values
            const int offset = 4;

            // The collection that holds the description + time strings in the order they should
            // be printed.
            var textList = new List<Tuple<string, string>>();

            // The length of the longest description string.
            int longestDescriptionLength = 0;

            // Get all texts and count the longest item
            report.Traverse(
                (interval, level) =>
                {
                    // Write the description with double the indentation
                    var description = string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}{1}",
                        IndentationPrimitive.Multiply(2 * level),
                        interval.Description);

                    // Write the timing result with the indentation level, i.e.
                    // half of what the description does
                    // Note also that we want to write the timing in milli-seconds, not ticks
                    var time = string.Format(
                        CultureInfo.CurrentCulture,
                        "{0}{1}",
                        IndentationPrimitive.Multiply(level),
                        interval.TotalTicks / 10000);

                    textList.Add(new Tuple<string, string>(description, time));
                    longestDescriptionLength = (description.Length > longestDescriptionLength) ? description.Length : longestDescriptionLength;
                });

            // Create the format string that looks like: {0,-longestDescriptionLength + offset}{1}
            // Doing this the hard way because we can't really escape curly braces. See here:
            // http://msdn.microsoft.com/en-us/library/txafckwd%28v=VS.90%29.aspx
            var format = string.Format(
                CultureInfo.InvariantCulture,
                "{0}0,-{2}{1}{0}1{1}",
                "{",
                "}",
                longestDescriptionLength + offset);

            var stream = m_StreamBuilder();
            using (var writer = new StreamWriter(stream, Encoding.Unicode))
            {
                writer.WriteLine(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        format,
                        "Description",
                        "Total time (ms)"));

                foreach (var texts in textList)
                {
                    var line = string.Format(
                        CultureInfo.CurrentCulture,
                        format,
                        texts.Item1,
                        texts.Item2);
                    writer.WriteLine(line);
                }
            }
        }
    }
}
