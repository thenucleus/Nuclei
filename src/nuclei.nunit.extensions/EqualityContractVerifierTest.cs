//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using NUnit.Framework;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// The base class for tests that verify that <c>object.Equals(object)</c>, <c>IEquatable{T}.Equals(T)</c>
    /// and the <c>==</c> and <c>!=</c> operators are implemented correctly.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the equality contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    public abstract class EqualityContractVerifierTest : HashcodeContractVerifierTest
    {
        /// <summary>
        /// Gets the object that provides the objects to be compared.
        /// </summary>
        protected abstract IEqualityContractVerifier EqualityContract
        {
            get;
        }

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value.
        /// </summary>
        [Test]
        public void ObjectEquals()
        {
            EqualityContract.ObjectEquals();
        }

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if the passed object is <see langword="null" />.
        /// </summary>
        [Test]
        public void ObjectEqualsWithNullObject()
        {
            EqualityContract.ObjectEqualsWithNullObject();
        }

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if a non-equal object of the same type is passed as the parameter.
        /// </summary>
        [Test]
        public void ObjectEqualsWithNonEqualObjectOfSameType()
        {
            EqualityContract.ObjectEqualsWithNonEqualObjectOfSameType();
        }

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if an object of a different type is passed as the parameter.
        /// </summary>
        [Test]
        public void ObjectEqualsWithNonEqualObjectOfDifferentType()
        {
            EqualityContract.ObjectEqualsWithNonEqualObjectOfDifferentType();
        }

        /// <summary>
        /// Tests whether <c>IEquatable{T}.Equals(T)</c> gives the correct return value.
        /// </summary>
        [Test]
        public void EquatableEquals()
        {
            EqualityContract.EquatableEquals();
        }

        /// <summary>
        /// Tests whether <c>IEquatable{T}.Equals(T)</c> gives the correct return value
        /// if a non-equal object is passed in as the parameter.
        /// </summary>
        [Test]
        public void EquatableEqualsWithNonEqualObject()
        {
            EqualityContract.EquatableEqualsWithNonEqualObject();
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if two equal objects are compared.
        /// </summary>
        [Test]
        public void EqualsOperatorWithEqualObject()
        {
            EqualityContract.EqualsOperatorWithEqualObject();
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if two non-equal objects are compared.
        /// </summary>
        [Test]
        public void EqualsOperatorWithNonEqualObject()
        {
            EqualityContract.EqualsOperatorWithNonEqualObject();
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if a null reference is compared to an object.
        /// </summary>
        [Test]
        public void EqualsOperatorWithLeftNullObject()
        {
            EqualityContract.EqualsOperatorWithLeftNullObject();
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if an object is compared to a null reference.
        /// </summary>
        [Test]
        public void EqualsOperatorWithRightNullObject()
        {
            EqualityContract.EqualsOperatorWithRightNullObject();
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if two equal objects are compared.
        /// </summary>
        [Test]
        public void NotEqualsOperatorWithEqualObject()
        {
            EqualityContract.NotEqualsOperatorWithEqualObject();
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if two non-equal objects are compared.
        /// </summary>
        [Test]
        public void NotEqualsOperatorWithNonEqualObject()
        {
            EqualityContract.NotEqualsOperatorWithNonEqualObject();
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if a null reference is compared to an object.
        /// </summary>
        [Test]
        public void NotEqualsOperatorWithLeftNullObject()
        {
            EqualityContract.NotEqualsOperatorWithLeftNullObject();
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if an object is compared to a null reference.
        /// </summary>
        [Test]
        public void NotEqualsOperatorWithRightNullObject()
        {
            EqualityContract.NotEqualsOperatorWithRightNullObject();
        }

        /// <summary>
        /// Tests whether <c>object.GetHashcode()</c> returns the same value
        /// for two equal objects.
        /// </summary>
        [Test]
        public void HashcodeComparisonForEqualObjects()
        {
            EqualityContract.HashcodeComparisonForEqualObjects();
        }
    }
}
