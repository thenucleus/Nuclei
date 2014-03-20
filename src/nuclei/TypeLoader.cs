//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Nuclei.Properties;

namespace Nuclei
{
    /// <summary>
    /// Defines methods for loading a <see cref="Type"/> from serialized type information.
    /// </summary>
    public static class TypeLoader
    {
        /// <summary>
        /// Tries to load a based on a partially qualified name.
        /// </summary>
        /// <param name="typeName">The full name of the type.</param>
        /// <param name="assemblyName">The assembly name of the assembly that contains the type.</param>
        /// <param name="assemblyVersion">The version of the assembly that contains the type.</param>
        /// <param name="throwOnError">
        /// <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> or <see langword="null" /> if the type could not be loaded 
        /// and <paramref name="throwOnError"/> was set to <see langword="false" />.
        /// </returns>
        /// <exception cref="UnableToLoadTypeException">
        /// Thrown when the <see cref="Type"/> could not be loaded and <paramref name="throwOnError"/> was set to <see langword="true" />.
        /// </exception>
        public static Type FromPartialInformation(
            string typeName, 
            string assemblyName = null, 
            Version assemblyVersion = null, 
            bool throwOnError = true)
        {
            {
                Lokad.Enforce.Argument(() => typeName);
                Lokad.Enforce.Argument(() => typeName, Lokad.Rules.StringIs.NotEmpty);
            }

            // Generate the non-strong named assembly qualified name for the type, based
            // on the remarks from here: http://msdn.microsoft.com/en-us/library/system.type.assemblyqualifiedname(v=vs.110).aspx
            var assemblyFullName = string.Empty;
            if (!string.IsNullOrEmpty(assemblyName))
            {
                // MyAssembly, Version=1.3.0.0, Culture=neutral, PublicKeyToken=b17a5c561934e089
                assemblyFullName = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}{1}{2}{3}",
                    assemblyName,
                    (assemblyVersion != null) 
                        ? string.Format(
                            CultureInfo.InvariantCulture,
                            ", Version={0}",
                            assemblyVersion.ToString(4))
                        : string.Empty,
                    string.Empty,
                    string.Empty);
            }

            var assemblyQualifiedName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}{2}",
                typeName,
                !string.IsNullOrEmpty(assemblyFullName) ? ", " : string.Empty,
                assemblyFullName);

            return FromFullyQualifiedName(assemblyQualifiedName, throwOnError);
        }

        /// <summary>
        /// Tries to load a type based on a fully qualified assembly name.
        /// </summary>
        /// <param name="assemblyQualifiedName">The full type name combined with the assembly name.</param>
        /// <param name="throwOnError">
        /// <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />.
        /// </param>
        /// <returns>
        /// The <see cref="Type"/> or <see langword="null" /> if the type could not be loaded 
        /// and <paramref name="throwOnError"/> was set to <see langword="false" />.
        /// </returns>
        /// <exception cref="UnableToLoadTypeException">
        /// Thrown when the <see cref="Type"/> could not be loaded and <paramref name="throwOnError"/> was set to <see langword="true" />.
        /// </exception>
        public static Type FromFullyQualifiedName(string assemblyQualifiedName, bool throwOnError = true)
        {
            {
                Lokad.Enforce.Argument(() => assemblyQualifiedName);
                Lokad.Enforce.Argument(() => assemblyQualifiedName, Lokad.Rules.StringIs.NotEmpty);
            }

            try
            {
                return Type.GetType(assemblyQualifiedName, throwOnError);
            }
            catch (TargetInvocationException e)
            {
                if (throwOnError)
                {
                    throw new UnableToLoadTypeException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_UnableToLoadType_WithTypeName,
                            assemblyQualifiedName),
                        e);
                }
            }
            catch (TypeLoadException e)
            {
                if (throwOnError)
                {
                    throw new UnableToLoadTypeException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_UnableToLoadType_WithTypeName,
                            assemblyQualifiedName),
                        e);
                }
            }
            catch (FileNotFoundException e)
            {
                if (throwOnError)
                {
                    throw new UnableToLoadTypeException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_UnableToLoadType_WithTypeName,
                            assemblyQualifiedName),
                        e);
                }
            }
            catch (FileLoadException e)
            {
                if (throwOnError)
                {
                    throw new UnableToLoadTypeException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_UnableToLoadType_WithTypeName,
                            assemblyQualifiedName),
                        e);
                }
            }
            catch (BadImageFormatException e)
            {
                if (throwOnError)
                {
                    throw new UnableToLoadTypeException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_UnableToLoadType_WithTypeName,
                            assemblyQualifiedName),
                        e);
                }
            }

            return null;
        }
    }
}
