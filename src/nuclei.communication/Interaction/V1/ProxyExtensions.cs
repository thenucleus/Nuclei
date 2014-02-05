//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction.V1
{
    /// <summary>
    /// Defines helper methods for 'serializing' proxy type layout and invocation signatures.
    /// </summary>
    internal static class ProxyExtensions
    {
        /// <summary>
        /// Translates an <see cref="Type"/> type into a serializable form.
        /// </summary>
        /// <param name="type">A type.</param>
        /// <returns>
        ///     An object that stores the <see cref="Type"/> information in a serializable format.
        /// </returns>
        public static SerializedType FromType(Type type)
        {
            return new SerializedType(type.FullName, type.AssemblyQualifiedName);
        }

        /// <summary>
        /// Returns the type of the proxy for which information is stored
        /// in the <paramref name="serializedProxyType"/> parameter.
        /// </summary>
        /// <param name="serializedProxyType">The serialization information for a proxy interface.</param>
        /// <returns>
        /// The proxy type.
        /// </returns>
        public static Type ToType(SerializedType serializedProxyType)
        {
            try
            {
                return Type.GetType(serializedProxyType.AssemblyQualifiedTypeName);
            }
            catch (TargetInvocationException e)
            {
                throw new UnableToLoadProxyTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        Resources.Exceptions_Messages_UnableToLoadProxyType_WithTypeName, 
                        serializedProxyType.AssemblyQualifiedTypeName),
                    e);
            }
            catch (TypeLoadException e)
            {
                throw new UnableToLoadProxyTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadProxyType_WithTypeName,
                        serializedProxyType.AssemblyQualifiedTypeName),
                    e);
            }
            catch (FileNotFoundException e)
            {
                throw new UnableToLoadProxyTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadProxyType_WithTypeName,
                        serializedProxyType.AssemblyQualifiedTypeName),
                    e);
            }
            catch (FileLoadException e)
            {
                throw new UnableToLoadProxyTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadProxyType_WithTypeName,
                        serializedProxyType.AssemblyQualifiedTypeName),
                    e);
            }
            catch (BadImageFormatException e)
            {
                throw new UnableToLoadProxyTypeException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_UnableToLoadProxyType_WithTypeName,
                        serializedProxyType.AssemblyQualifiedTypeName),
                    e);
            }
        }

        /// <summary>
        /// Translates a <see cref="MethodInfo"/> and the related parameter values into a serializable form.
        /// </summary>
        /// <param name="method">The method information that needs to be serialized.</param>
        /// <param name="parameters">
        ///     The collection of parameter values with which the method should be called. Note that the parameter
        ///     values should be in the same order as they are given by the <c>MethodInfo.GetParameters()</c> method.
        /// </param>
        /// <returns>
        ///     An object that stores the method invocation information in a serializable format.
        /// </returns>
        public static SerializedMethod FromMethodInfo(MethodBase method, object[] parameters)
        {
            var methodParameters = method.GetParameters();
            Debug.Assert(methodParameters.Length == parameters.Length, "There are a different number of parameters than there are parameter values.");

            var namedParameters = new List<Tuple<SerializedType, object>>();
            for (int i = 0; i < parameters.Length; i++)
            {
                namedParameters.Add(Tuple.Create(FromType(methodParameters[i].ParameterType), parameters[i]));
            }

            return new SerializedMethod(
                FromType(method.DeclaringType),
                method.Name,
                namedParameters);
        }

        /// <summary>
        /// Translates a <see cref="EventInfo"/> into a serializable form.
        /// </summary>
        /// <param name="eventInfo">The event information that needs to be serialized.</param>
        /// <returns>
        ///     An object that stores the event information in a serializable format.
        /// </returns>
        public static SerializedEvent FromEventInfo(EventInfo eventInfo)
        {
            return new SerializedEvent(
                FromType(eventInfo.DeclaringType),
                eventInfo.Name);
        }
    }
}
