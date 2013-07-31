//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Text;

namespace Nuclei.Diagnostics.Profiling
{
    /// <summary>
    /// Defines extension methods for the <see cref="string"/> class.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Creates a new string which contains the <paramref name="source"/> string
        /// <paramref name="multiplier"/> times.
        /// </summary>
        /// <param name="source">The string that should be copied.</param>
        /// <param name="multiplier">The number of times the string should be copied.</param>
        /// <returns>
        /// A new string that contains <paramref name="multiplier"/> copies of the <paramref name="source"/>.
        /// </returns>
        public static string Multiply(this string source, int multiplier)
        {
            var sb = new StringBuilder(multiplier * source.Length);
            for (int i = 0; i < multiplier; i++)
            {
                sb.Append(source);
            }

            return sb.ToString();
        }
    }
}
