//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using NUnit.Framework;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// The base class for tests that need to verify that <c>object.GetHashcode()</c> is implemented
    /// correctly.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    public abstract class HashcodeContractVerifierTest
    {
        /// <summary>
        /// Gets an instance of the hashcode contract verifier for use in the tests.
        /// </summary>
        protected abstract HashcodeContractVerifier HashContract
        {
            get;
        }

        /// <summary>
        /// Verifies that the probability of hashcode collisions is less than the 
        /// desired limit.
        /// </summary>
        [Test]
        public void VerifyCollisionProbability()
        {
            HashContract.VerifyCollisionProbability();
        }

        /// <summary>
        /// Verifies that the hashcodes are uniformly distributed.
        /// </summary>
        [Test]
        public void VerifyUniformDistribution()
        {
            HashContract.VerifyUniformDistribution();
        }
    }
}
