//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// Defines the interface for tests that need to verify that the equality contract
    /// is implemented correctly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The interface is defined so that the <see cref="EqualityContractVerifierTest"/> can
    /// call methods on a non-generic object, while the user can provide a generic class. 
    /// </para>
    /// <para>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/
    /// </para>
    /// </remarks>
    public interface IEqualityContractVerifier
    {
        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value.
        /// </summary>
        void ObjectEquals();

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if the passed object is <see langword="null" />.
        /// </summary>
        void ObjectEqualsWithNullObject();

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if a non-equal object of the same type is passed as the parameter.
        /// </summary>
        void ObjectEqualsWithNonEqualObjectOfSameType();

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if an object of a different type is passed as the parameter.
        /// </summary>
        void ObjectEqualsWithNonEqualObjectOfDifferentType();

        /// <summary>
        /// Tests whether <c>IEquatable{T}.Equals(T)</c> gives the correct return value.
        /// </summary>
        void EquatableEquals();

        /// <summary>
        /// Tests whether <c>IEquatable{T}.Equals(T)</c> gives the correct return value
        /// if a non-equal object is passed in as the parameter.
        /// </summary>
        void EquatableEqualsWithNonEqualObject();

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if two equal objects are compared.
        /// </summary>
        void EqualsOperatorWithEqualObject();

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if two non-equal objects are compared.
        /// </summary>
        void EqualsOperatorWithNonEqualObject();

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if a null reference is compared to an object.
        /// </summary>
        void EqualsOperatorWithLeftNullObject();

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if an object is compared to a null reference.
        /// </summary>
        void EqualsOperatorWithRightNullObject();

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if two equal objects are compared.
        /// </summary>
        void NotEqualsOperatorWithEqualObject();

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if two non-equal objects are compared.
        /// </summary>
        void NotEqualsOperatorWithNonEqualObject();

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if a null reference is compared to an object.
        /// </summary>
        void NotEqualsOperatorWithLeftNullObject();

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if an object is compared to a null reference.
        /// </summary>
        void NotEqualsOperatorWithRightNullObject();

        /// <summary>
        /// Tests whether <c>object.GetHashcode()</c> returns the same value
        /// for two equal objects.
        /// </summary>
        void HashcodeComparisonForEqualObjects();
    }
}
