//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// Defines a gamma distribution.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    internal static class Gamma
    {
        private static double GammaLogarithm(double a)
        {
            var coefficients = new[]
                {
                    76.180091729471457, 
                    -86.505320329416776, 
                    24.014098240830911, 
                    -1.231739572450155, 
                    0.001208650973866179, 
                    -5.395239384953E-06
                };
            var num2 = a;
            double d = a + 5.5;
            d -= (a + 0.5) * Math.Log(d);
            double num4 = 1.0000000001900149;
            for (int i = 0; i < coefficients.Length; i++)
            {
                num4 += coefficients[i] / ++num2;
            }

            return -d + Math.Log((2.5066282746310007 * num4) / a);
        }

        public static double IncompleteGamma(double a, double x)
        {
            if (x < 0.0)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            if (a <= 0.0)
            {
                throw new ArgumentOutOfRangeException("a");
            }

            if (x < (a + 1.0))
            {
                return 1.0 - IncompleteGammaSeries(a, x);
            }

            return IncompleteGammaContinuedFraction(a, x);
        }

        private static double IncompleteGammaContinuedFraction(double a, double x)
        {
            double num = (x + 1.0) - a;
            double num2 = 9.9999999999999988E+29;
            double num3 = 1.0 / num;
            double num4 = num3;
            for (int i = 1; i < 0x3e8; i++)
            {
                double num6 = -i * (i - a);
                num += 2.0;
                num3 = 1.0 / Math.Max(1E-30, (num6 * num3) + num);
                num2 = Math.Max(1E-30, num + (num6 / num2));
                double num7 = num3 * num2;
                num4 *= num7;
                if (Math.Abs(num7 - 1.0) < 3E-07)
                {
                    return Math.Exp((-x + (a * Math.Log(x))) - GammaLogarithm(a)) * num4;
                }
            }

            throw new ArgumentOutOfRangeException("a", "Value too large.");
        }

        private static double IncompleteGammaSeries(double a, double x)
        {
            if (x < 0.0)
            {
                throw new ArgumentOutOfRangeException("x");
            }

            double num = a;
            double num2 = 1.0 / a;
            double num3 = num2;
            for (int i = 1; i < 0x3e8; i++)
            {
                num++;
                num2 *= x / num;
                num3 += num2;
                if (Math.Abs(num2) < (Math.Abs(num3) * 3E-07))
                {
                    return num3 * Math.Exp((-x + (a * Math.Log(x))) - GammaLogarithm(a));
                }
            }

            throw new ArgumentOutOfRangeException("a", "Value too large.");
        }
    }
}
