//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// Stores the result of a hashcode collision calculation.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    internal sealed class HashStoreResult
    {
        private readonly double m_CollisionProbability;
        private readonly double m_UniformDistributionDeviationProbability;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HashStoreResult"/> class.
        /// </summary>
        /// <param name="collisionProbability">The probability of a hashcode collision.</param>
        /// <param name="uniformDistributionDeviationProbability">The probability a hashcode deviates from the uniform distribution.</param>
        public HashStoreResult(double collisionProbability, double uniformDistributionDeviationProbability)
        {
            m_CollisionProbability = collisionProbability;
            m_UniformDistributionDeviationProbability = uniformDistributionDeviationProbability;
        }

        /// <summary>
        /// Gets the probability of a hashcode collision.
        /// </summary>
        public double CollisionProbability
        {
            get
            {
                return m_CollisionProbability;
            }
        }

        /// <summary>
        /// Gets the probability a hashcode deviates from the uniform distribution.
        /// </summary>
        public double UniformDistributionDeviationProbability
        {
            get
            {
                return m_UniformDistributionDeviationProbability;
            }
        }
    }
}
