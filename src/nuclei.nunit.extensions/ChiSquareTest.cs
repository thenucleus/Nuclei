//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// Defines a chi-squared test.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    internal sealed class ChiSquareTest
    {
        // Fields
        private readonly double m_ChiSquareValue;
        private readonly int m_DegreesOfFreedom;
        private readonly double m_TwoTailedpValue;

        // Methods
        public ChiSquareTest(double expected, ICollection<double> actual, int numberOfConstraints)
        {
            if (expected <= 0.0)
            {
                throw new ArgumentOutOfRangeException("expected", "The expected value is negative.");
            }

            if (actual == null)
            {
                throw new ArgumentNullException("actual");
            }

            m_DegreesOfFreedom = actual.Count - numberOfConstraints;
            foreach (double num in actual)
            {
                double num2 = num - expected;
                m_ChiSquareValue += (num2 * num2) / expected;
            }

            m_TwoTailedpValue = Gamma.IncompleteGamma(m_DegreesOfFreedom / 2.0, m_ChiSquareValue / 2.0);
        }

        public double TwoTailedpValue
        {
            get
            {
                return m_TwoTailedpValue;
            }
        }
    }
}
