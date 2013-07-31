//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using NUnit.Framework;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// The base class for contract verifiers that test if <c>object.GetHashcode()</c> is
    /// correctly implemented.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    public abstract class HashcodeContractVerifier
    {
        private const double CollisionProbabilityLimit = 0.01;
        private const double UniformDistributionQualityLimit = 0.01;

        /// <summary>
        /// Returns a collection of hashcodes.
        /// </summary>
        /// <returns>The collection containing the hashcodes.</returns>
        protected abstract IEnumerable<int> GetHashcodes();

        /// <summary>
        /// Verifies that the probability of a hashcode collision is lower than the desired limit.
        /// </summary>
        internal void VerifyCollisionProbability()
        {
            HashStoreResult result = null;
            try
            {
                result = GetResult();
            }
            catch (NotEnoughHashesException)
            {
                Assert.Fail("Not enough hash code samples were provided to the hash code acceptance contract.");
            }

            double collisionProbability = result.CollisionProbability;
            Assert.LessOrEqual(collisionProbability, CollisionProbabilityLimit);
        }

        private HashStoreResult GetResult()
        {
            var store = new HashStore(GetHashcodes());
            return store.Result;
        }

        /// <summary>
        /// Verifies that the hashcodes are uniformly distributed.
        /// </summary>
        internal void VerifyUniformDistribution()
        {
            HashStoreResult result = null;
            try
            {
                result = GetResult();
            }
            catch (NotEnoughHashesException)
            {
                Assert.Fail("Not enough hash code samples were provided to the hash code acceptance contract.");
            }

            double uniformDistributionDeviationProbability = result.UniformDistributionDeviationProbability;
            Assert.LessOrEqual(uniformDistributionDeviationProbability, UniformDistributionQualityLimit);
        }
    }
}
