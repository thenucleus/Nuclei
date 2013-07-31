//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Diagnostics.Profiling.Reporting
{
    /// <summary>
    /// Defines the interface for objects that transform <see cref="TimingReport"/> objects into
    /// another form.
    /// </summary>
    public interface ITransformReports
    {
        /// <summary>
        /// Transforms the report.
        /// </summary>
        /// <param name="report">The report that should be transformed.</param>
        void Transform(TimingReport report);
    }
}
