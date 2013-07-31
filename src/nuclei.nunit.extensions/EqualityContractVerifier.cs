//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using NUnit.Framework;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// The typed-base class for contract verifiers that test if the equality contract
    /// is implemented correctly.
    /// </summary>
    /// <typeparam name="T">The type for which the equality contract should be verified.</typeparam>
    public abstract class EqualityContractVerifier<T> : IEqualityContractVerifier
    {
        private static MethodInfo EqualityOperator(Type type)
        {
            var localType = type;
            MethodInfo method = null;
            while ((method == null) && (localType != null))
            {
                method = localType.GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
                localType = localType.BaseType;
            }

            return method;
        }

        private static MethodInfo InequalityOperator(Type type)
        {
            var localType = type;
            MethodInfo method = null;
            while ((method == null) && (localType != null))
            {
                method = localType.GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);
                localType = localType.BaseType;
            }

            return method;
        }

        /// <summary>
        /// Gets a value indicating whether operator overloads are defined for the type.
        /// </summary>
        protected abstract bool HasOperatorOverloads
        {
            get;
        }

        /// <summary>
        /// Gets the first object that should be used in equality comparisons.
        /// </summary>
        protected abstract T FirstInstance
        {
            get;
        }

        /// <summary>
        /// Gets the second object that should be used in equality comparisons.
        /// </summary>
        protected abstract T SecondInstance
        {
            get;
        }

        /// <summary>
        /// Creates a deep copy of the given object.
        /// </summary>
        /// <remarks>
        /// This method is used to create identical objects that are not referentially identical.
        /// </remarks>
        /// <param name="original">The original object that should be copied.</param>
        /// <returns>A new instance that contains the same values as the original object.</returns>
        protected abstract T Copy(T original);

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value.
        /// </summary>
        public void ObjectEquals()
        {
            object left = FirstInstance;
            object right = Copy(FirstInstance);

            Assert.IsTrue(left.Equals(right));
        }

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if the passed object is <see langword="null" />.
        /// </summary>
        public void ObjectEqualsWithNullObject()
        {
            object left = FirstInstance;
            Assert.IsFalse(left.Equals(null));
        }

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if a non-equal object of the same type is passed as the parameter.
        /// </summary>
        public void ObjectEqualsWithNonEqualObjectOfSameType()
        {
            object left = FirstInstance;
            object right = SecondInstance;

            Assert.IsFalse(left.Equals(right));
        }

        /// <summary>
        /// Tests whether <c>Object.Equals(object)</c> gives the correct return value
        /// if an object of a different type is passed as the parameter.
        /// </summary>
        public void ObjectEqualsWithNonEqualObjectOfDifferentType()
        {
            object left = FirstInstance;
            Assert.IsFalse(left.Equals(new object()));
        }

        /// <summary>
        /// Tests whether <c>IEquatable{T}.Equals(T)</c> gives the correct return value.
        /// </summary>
        public void EquatableEquals()
        {
            if (!typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
            {
                Assert.Ignore("IEquatable<T> not implemented.");
                return;
            }

            var left = (IEquatable<T>)FirstInstance;
            var right = Copy(FirstInstance);

            Assert.IsTrue(left.Equals(right));
        }

        /// <summary>
        /// Tests whether <c>IEquatable{T}.Equals(T)</c> gives the correct return value
        /// if a non-equal object is passed in as the parameter.
        /// </summary>
        public void EquatableEqualsWithNonEqualObject()
        {
            if (!typeof(IEquatable<T>).IsAssignableFrom(typeof(T)))
            {
                Assert.Ignore("IEquatable<T> not implemented.");
                return;
            }

            var left = (IEquatable<T>)FirstInstance;
            var right = SecondInstance;

            Assert.IsFalse(left.Equals(right));
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if two equal objects are compared.
        /// </summary>
        public void EqualsOperatorWithEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = Copy(FirstInstance);

            var method = EqualityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsTrue((bool)result);
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if two non-equal objects are compared.
        /// </summary>
        public void EqualsOperatorWithNonEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = SecondInstance;

            var method = EqualityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsFalse((bool)result);
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if a null reference is compared to an object.
        /// </summary>
        public void EqualsOperatorWithLeftNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var instance = FirstInstance;

            var method = EqualityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        null,
                        instance
                    });

            Assert.IsFalse((bool)result);
        }

        /// <summary>
        /// Tests whether the equals operator gives the correct return value
        /// if an object is compared to a null reference.
        /// </summary>
        public void EqualsOperatorWithRightNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var instance = SecondInstance;

            var method = EqualityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        instance,
                        null
                    });

            Assert.IsFalse((bool)result);
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if two equal objects are compared.
        /// </summary>
        public void NotEqualsOperatorWithEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = Copy(FirstInstance);

            var method = InequalityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsFalse((bool)result);
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if two non-equal objects are compared.
        /// </summary>
        public void NotEqualsOperatorWithNonEqualObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;
            var right = SecondInstance;

            var method = InequalityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        right
                    });

            Assert.IsTrue((bool)result);
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if a null reference is compared to an object.
        /// </summary>
        public void NotEqualsOperatorWithLeftNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var left = FirstInstance;

            var method = InequalityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        left,
                        null
                    });

            Assert.IsTrue((bool)result);
        }

        /// <summary>
        /// Tests whether the not-equals operator gives the correct return value
        /// if an object is compared to a null reference.
        /// </summary>
        public void NotEqualsOperatorWithRightNullObject()
        {
            if (!HasOperatorOverloads)
            {
                Assert.Ignore("Equality operators not overloaded.");
                return;
            }

            var right = SecondInstance;

            var method = InequalityOperator(typeof(T));
            Assert.IsNotNull(method);

            var result = method.Invoke(
                null,
                new object[]
                    {
                        null,
                        right
                    });

            Assert.IsTrue((bool)result);
        }

        /// <summary>
        /// Tests whether <c>object.GetHashcode()</c> returns the same value
        /// for two equal objects.
        /// </summary>
        public void HashcodeComparisonForEqualObjects()
        {
            object left = FirstInstance;
            object right = Copy(FirstInstance);

            Assert.AreEqual(left.GetHashCode(), right.GetHashCode());
        }
    }
}
