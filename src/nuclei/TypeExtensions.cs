//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Nuclei
{
    /// <summary>
    /// Defines extension methods for the <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Indicates if the <paramref name="derivedType"/> derives from or implements the given
        /// <paramref name="baseType"/>.
        /// </summary>
        /// <param name="baseType">The base type which may be an open generic type.</param>
        /// <param name="derivedType">The derived type.</param>
        /// <returns>
        ///     <see langword="true" /> if the <paramref name="derivedType"/> derives from or implements the
        ///     <paramref name="baseType"/>; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsAssignableToOpenGenericType(this Type baseType, Type derivedType)
        {
            baseType = ResolveGenericTypeDefinition(baseType);

            var currentChild = derivedType.IsGenericType
                                   ? derivedType.GetGenericTypeDefinition()
                                   : derivedType;

            while (currentChild != typeof(object))
            {
                if (baseType == currentChild || HasAnyInterfaces(baseType, currentChild))
                {
                    return true;
                }

                currentChild = currentChild.BaseType != null
                               && currentChild.BaseType.IsGenericType
                                   ? currentChild.BaseType.GetGenericTypeDefinition()
                                   : currentChild.BaseType;

                if (currentChild == null)
                {
                    return false;
                }
            }

            return false;
        }

        private static bool HasAnyInterfaces(Type baseType, Type derivedType)
        {
            return derivedType.GetInterfaces()
                .Any(childInterface =>
                {
                    var currentInterface = childInterface.IsGenericType
                        ? childInterface.GetGenericTypeDefinition()
                        : childInterface;

                    return currentInterface == baseType;
                });
        }

        private static Type ResolveGenericTypeDefinition(Type type)
        {
            bool shouldUseGenericType = !(type.IsGenericType && type.GetGenericTypeDefinition() != type);
            if (type.IsGenericType && shouldUseGenericType)
            {
                type = type.GetGenericTypeDefinition();
            }

            return type;
        }
    }
}
