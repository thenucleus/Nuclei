//-----------------------------------------------------------------------
// <copyright company="TheNucleus">
// Copyright (c) TheNucleus. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nuclei
{
    /// <summary>
    /// Defines helper and extension methods for reflection of types and methods.
    /// </summary>
    public static class ReflectionExtensions
    {
        /// <summary>
        /// Returns the name of the member which is called inside the expression.
        /// </summary>
        /// <param name="expression">The expression that is used to call the member for which the name must be determined.</param>
        /// <example>
        /// <code>
        /// var result = MemberName(() => x.Bar())
        /// </code>
        /// </example>
        /// <returns>
        /// The name of the member in the expression or <see langword="null"/> if no member was called in the expression.
        /// </returns>
        public static string MemberName(LambdaExpression expression)
        {
            if (expression == null)
            {
                return null;
            }

            var method = expression.Body as MemberExpression;
            if (method != null)
            {
                return method.Member.Name;
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the member which is called inside the expression.
        /// </summary>
        /// <example>
        /// <code>
        /// var result = MemberName(x => x.Bar())
        /// </code>
        /// </example>
        /// <typeparam name="T">The type on which the member is called.</typeparam>
        /// <param name="expression">The expression that is used to call the member for which the name must be determined.</param>
        /// <returns>
        /// The name of the member in the expression or <see langword="null"/> if no member was called in the expression.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "By typing it as Expression<T> it becomes possible to use the lambda syntax at the caller site.")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The generics are necessar to get the name of the member via a lambda expression.")]
        public static string MemberName<T>(Expression<Action<T>> expression)
        {
            if (expression == null)
            {
                return null;
            }

            var method = expression.Body as MemberExpression;
            if (method != null)
            {
                return method.Member.Name;
            }

            return null;
        }

        /// <summary>
        /// Returns the name of the member which is called inside the expression.
        /// </summary>
        /// <example>
        /// <code>
        /// var result = MemberName(x => x.Bar())
        /// </code>
        /// </example>
        /// <typeparam name="T">The type on which the member is called.</typeparam>
        /// <typeparam name="TResult">The result of the member call.</typeparam>
        /// <param name="expression">The expression that is used to call the member for which the name must be determined.</param>
        /// <returns>
        /// The name of the member in the expression or <see langword="null"/> if no member was called in the expression.
        /// </returns>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "By typing it as Expression<T> it becomes possible to use the lambda syntax at the caller site.")]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "The generics are necessar to get the name of the member via a lambda expression.")]
        public static string MemberName<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            if (expression == null)
            {
                return null;
            }

            var method = expression.Body as MemberExpression;
            if (method != null)
            {
                return method.Member.Name;
            }

            return null;
        }

        /// <summary>
        /// Builds a comma separated string containing all the method names and parameters for each of the method information
        /// objects in the collection.
        /// </summary>
        /// <param name="methods">The collection containing the method information.</param>
        /// <returns>A string containing all the method signatures.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="methods"/> is <see langword="null"/>.
        /// </exception>
        public static string MethodInfoToString(IEnumerable<MethodInfo> methods)
        {
            if (methods == null)
            {
                throw new ArgumentNullException("methods");
            }

            var methodsText = new StringBuilder();
            foreach (var methodInfo in methods)
            {
                if (methodsText.Length > 0)
                {
                    methodsText.Append("; ");
                }

                var parametersText = new StringBuilder();
                foreach (var parameterInfo in methodInfo.GetParameters())
                {
                    if (parametersText.Length > 0)
                    {
                        parametersText.Append(", ");
                    }

                    parametersText.Append(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}{1}{2} {3}",
                            parameterInfo.IsOut ? "out " : string.Empty,
                            parameterInfo.IsRetval ? "ref " : string.Empty,
                            parameterInfo.ParameterType.Name,
                            parameterInfo.Name));
                }

                methodsText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}({2})",
                        methodInfo.DeclaringType.Name,
                        methodInfo.Name,
                        parametersText.ToString()));
            }

            return methodsText.ToString();
        }

        /// <summary>
        /// Builds a comma separated string containing all the property signatures of the property information in the collection.
        /// </summary>
        /// <param name="properties">The collection containing the property information.</param>
        /// <returns>A string containing the property information.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="properties"/> is <see langword="null"/>.
        /// </exception>
        public static string PropertyInfoToString(IEnumerable<PropertyInfo> properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            var propertiesText = new StringBuilder();
            foreach (var propertyInfo in properties)
            {
                if (propertiesText.Length > 0)
                {
                    propertiesText.Append("; ");
                }

                propertiesText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        propertyInfo.DeclaringType.Name,
                        propertyInfo.Name));
            }

            return propertiesText.ToString();
        }

        /// <summary>
        /// Builds a comma separated string containing the event signatures of all the events in the
        /// collection.
        /// </summary>
        /// <param name="events">The collection containing the events.</param>
        /// <returns>A string containing the event signatures.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="events"/> is <see langword="null"/>.
        /// </exception>
        public static string EventInfoToString(IEnumerable<EventInfo> events)
        {
            if (events == null)
            {
                throw new ArgumentNullException("events");
            }

            var eventText = new StringBuilder();
            foreach (var eventInfo in events)
            {
                if (eventText.Length > 0)
                {
                    eventText.Append("; ");
                }

                eventText.Append(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}.{1}",
                        eventInfo.DeclaringType.Name,
                        eventInfo.Name));
            }

            return eventText.ToString();
        }

        /// <summary>
        /// Returns a value indicating if the given assembly name is an exact match for the current assembly name.
        /// </summary>
        /// <param name="current">The current assembly name.</param>
        /// <param name="other">The assembly name that should be compared to the current assembly name.</param>
        /// <returns>
        /// <see langword="true" /> if the given assembly name is an exact match for the current assembly name;
        /// otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsSame(this AssemblyName current, AssemblyName other)
        {
            if ((current == null) || (other == null))
            {
                return false;
            }

            var isMatch = string.Equals(current.Name, other.Name, StringComparison.Ordinal);
            isMatch = isMatch
                && (((current.CultureInfo != null) && current.CultureInfo.Equals(other.CultureInfo))
                || ((current.CultureInfo == null) && other.CultureInfo == null));

            isMatch = isMatch && current.Version.Equals(other.Version);

            var currentPublicKey = current.GetPublicKey();
            var namePublicKey = other.GetPublicKey();
            isMatch = isMatch
                && (((currentPublicKey != null) && (namePublicKey != null) && current.GetPublicKey().SequenceEqual(other.GetPublicKey()))
                || ((currentPublicKey == null) && (namePublicKey == null)));

            return isMatch;
        }

        /// <summary>
        /// Returns a value indicating if the current assembly name belongs to the same assembly as the assembly with the other name, or
        /// a later version of that assembly.
        /// </summary>
        /// <param name="current">The current assembly name.</param>
        /// <param name="other">The assembly name that should be compared to the current assembly name.</param>
        /// <returns>
        /// <see langword="true" /> if the current assembly name belongs to the same assembly as the assembly with the other name, or
        /// a later version of that assembly; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsMatch(this AssemblyName current, AssemblyName other)
        {
            if ((current == null) || (other == null))
            {
                return false;
            }

            var isMatch = string.Equals(current.Name, other.Name, StringComparison.Ordinal);
            isMatch = isMatch
                && (((current.CultureInfo != null) && current.CultureInfo.Equals(other.CultureInfo))
                || ((current.CultureInfo == null) && other.CultureInfo == null));

            var currentPublicKey = current.GetPublicKey();
            var namePublicKey = other.GetPublicKey();
            isMatch = isMatch
                && (((currentPublicKey != null) && (namePublicKey != null))
                || ((currentPublicKey == null) && (namePublicKey == null)));

            if ((current.GetPublicKey() != null) && (current.GetPublicKey().Length > 0))
            {
                isMatch = isMatch
                    && current.GetPublicKey().SequenceEqual(other.GetPublicKey())
                    && current.Version.Equals(other.Version);
            }
            else
            {
                isMatch = isMatch && (current.Version >= other.Version);
            }

            return isMatch;
        }
    }
}
