//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines extension methods for the interaction namespace.
    /// </summary>
    internal static class InteractionExtensions
    {
        /// <summary>
        /// The collection containing the types of all the attributes that can 
        /// be applied to a command interface method parameter.
        /// </summary>
        private static readonly HashSet<Type> s_KnownCommandSetParameterAttributes
            = new HashSet<Type>
                {
                    typeof(InvocationRetryCountAttribute),
                    typeof(InvocationTimeoutAttribute),
                };

        /// <summary>
        /// The collection containing the types of all the attributes that can be
        /// applied to a command instance method parameter.
        /// </summary>
        private static readonly HashSet<Type> s_KnownCommandInstanceParameterAttributes
            = new HashSet<Type>
                {
                    typeof(InvokingEndpointAttribute),
                    typeof(InvocationMessageAttribute),
                };

        /// <summary>
        /// Gets the collection containing the types of all the attributes that can 
        /// be applied to a command interface method parameter.
        /// </summary>
        public static ISet<Type> KnownCommandSetParameterAttributes
        {
            [DebuggerStepThrough]
            get
            {
                return s_KnownCommandSetParameterAttributes;
            }
        }

        /// <summary>
        /// Gets the collection containing the types of all the attributes that can be
        /// applied to a command instance method parameter.
        /// </summary>
        public static ISet<Type> KnownCommandInstanceParameterAttributes
        {
            [DebuggerStepThrough]
            get
            {
                return s_KnownCommandInstanceParameterAttributes;
            }
        }

        /// <summary>
        /// Verifies that an interface type will be a correct command set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A proper command set class has the following characteristics:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The interface must derive from <see cref="ICommandSet"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>The interface must only have methods, no properties or events.</description>
        ///     </item>
        ///     <item>
        ///         <description>Each method must return either <see cref="Task"/> or <see cref="Task{T}"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>If a method returns a <see cref="Task{T}"/> then <c>T</c> must be a closed constructed type.</description>
        ///     </item>
        ///     <item>
        ///         <description>If a method returns a <see cref="Task{T}"/> then <c>T</c> must be serializable.</description>
        ///     </item>
        ///     <item>
        ///         <description>All method parameters must be serializable.</description>
        ///     </item>
        ///     <item>
        ///         <description>None of the method parameters may be <c>ref</c> or <c>out</c> parameters.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="commandSet">The type that has implemented the command set interface.</param>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     If the given type is not a valid <see cref="ICommandSet"/> interface.
        /// </exception>
        public static void VerifyThatTypeIsACorrectCommandSet(this Type commandSet)
        {
            if (!typeof(ICommandSet).IsAssignableFrom(commandSet))
            {
                throw new TypeIsNotAValidCommandSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnICommandSet,
                        commandSet));
            }

            if (!commandSet.IsInterface)
            {
                throw new TypeIsNotAValidCommandSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_TypeIsNotAnInterface,
                        commandSet));
            }

            if (commandSet.ContainsGenericParameters)
            {
                throw new TypeIsNotAValidCommandSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_TypeMustBeClosedConstructed,
                        commandSet));
            }

            if (commandSet.GetProperties().Length > typeof(ICommandSet).GetProperties().Length)
            {
                var propertiesText = ReflectionExtensions.PropertyInfoToString(commandSet.GetProperties());
                throw new TypeIsNotAValidCommandSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveProperties,
                        commandSet,
                        propertiesText));
            }

            if (commandSet.GetEvents().Length > typeof(ICommandSet).GetEvents().Length)
            {
                var eventsText = ReflectionExtensions.EventInfoToString(commandSet.GetEvents());
                throw new TypeIsNotAValidCommandSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetCannotHaveEvents,
                        commandSet,
                        eventsText));
            }

            var methods = commandSet.GetMethods();
            if (methods.Length == 0)
            {
                throw new TypeIsNotAValidCommandSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMustHaveMethods,
                        commandSet));
            }

            foreach (var method in methods)
            {
                if (method.IsGenericMethodDefinition)
                {
                    throw new TypeIsNotAValidCommandSetException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsCannotBeGeneric,
                            commandSet,
                            method));
                }

                if (!HasCorrectReturnType(method.ReturnType))
                {
                    throw new TypeIsNotAValidCommandSetException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsMustHaveCorrectReturnType,
                            commandSet,
                            method));
                }

                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                {
                    if (!IsParameterValid(parameter))
                    {
                        throw new TypeIsNotAValidCommandSetException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetParametersMustBeValid,
                                commandSet,
                                method));
                    }
                }
            }
        }

        private static bool HasCorrectReturnType(Type type)
        {
            if (type == typeof(Task))
            {
                return true;
            }

            if (type.IsGenericType)
            {
                var baseType = type.GetGenericTypeDefinition();
                if (baseType == typeof(Task<>))
                {
                    var genericParameters = type.GetGenericArguments();
                    if (genericParameters[0].ContainsGenericParameters)
                    {
                        return false;
                    }

                    if (IsTypeSerializable(genericParameters[0]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool IsTypeSerializable(Type type)
        {
            return Attribute.IsDefined(type, typeof(DataContractAttribute)) || typeof(ISerializable).IsAssignableFrom(type) || type.IsSerializable;
        }

        private static bool IsParameterValid(ParameterInfo parameter)
        {
            if (parameter.ParameterType.ContainsGenericParameters)
            {
                return false;
            }

            if (parameter.IsOut || parameter.ParameterType.IsByRef)
            {
                return false;
            }

            // If the parameter has an attribute then it should be one of the recognized ones
            var attributes = parameter.GetCustomAttributes(true);
            var parameterUsageAttribute = attributes.FirstOrDefault(
                    o => s_KnownCommandSetParameterAttributes.Contains(o.GetType())) as CommandProxyParameterUsageAttribute;
            if (parameterUsageAttribute != null)
            {
                return parameterUsageAttribute.AllowedParameterType == parameter.ParameterType;
            }

            return IsTypeSerializable(parameter.ParameterType);
        }

        /// <summary>
        /// Verifies that an interface type will be a correct command set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A proper notification set class has the following characteristics:
        /// <list type="bullet">
        ///     <item>
        ///         <description>The interface must derive from <see cref="INotificationSet"/>.</description>
        ///     </item>
        ///     <item>
        ///         <description>The interface must only have events, no properties or methods.</description>
        ///     </item>
        ///     <item>
        ///         <description>Each event be based on <see cref="EventHandler{T}"/> delegate.</description>
        ///     </item>
        ///     <item>
        ///         <description>The event must be based on a closed constructed type.</description>
        ///     </item>
        ///     <item>
        ///         <description>The <see cref="EventArgs"/> of <see cref="EventHandler{T}"/> must be serializable.</description>
        ///     </item>
        /// </list>
        /// </para>
        /// </remarks>
        /// <param name="notificationSet">The type that has implemented the command set interface.</param>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     If the given type is not a valid <see cref="ICommandSet"/> interface.
        /// </exception>
        public static void VerifyThatTypeIsACorrectNotificationSet(this Type notificationSet)
        {
            if (!typeof(INotificationSet).IsAssignableFrom(notificationSet))
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnINotificationSet,
                        notificationSet));
            }

            if (!notificationSet.IsInterface)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeIsNotAnInterface,
                        notificationSet));
            }

            if (notificationSet.ContainsGenericParameters)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_TypeMustBeClosedConstructed,
                        notificationSet));
            }

            if (notificationSet.GetProperties().Length > 0)
            {
                var propertiesText = ReflectionExtensions.PropertyInfoToString(notificationSet.GetProperties());
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveProperties,
                        notificationSet,
                        propertiesText));
            }

            // Event methods get to stay, anything else is evil ...
            var invalidMethods = from methodInfo in notificationSet.GetMethods()
                                 where (!methodInfo.Name.StartsWith("add_", StringComparison.Ordinal)
                                        && !methodInfo.Name.StartsWith("remove_", StringComparison.Ordinal))
                                 select methodInfo;
            if (invalidMethods.Any())
            {
                var methodsText = ReflectionExtensions.MethodInfoToString(invalidMethods);
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetCannotHaveMethods,
                        notificationSet,
                        methodsText));
            }

            var events = notificationSet.GetEvents();
            if (events.Length == 0)
            {
                throw new TypeIsNotAValidNotificationSetException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetMustHaveEvents,
                        notificationSet));
            }

            foreach (var eventInfo in events)
            {
                if (!HasCorrectDelegateType(eventInfo.EventHandlerType))
                {
                    throw new TypeIsNotAValidNotificationSetException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_TypeIsNotAValidNotificationSet_NotificationSetEventsMustUseEventHandler,
                            notificationSet,
                            eventInfo));
                }
            }
        }

        private static bool HasCorrectDelegateType(Type type)
        {
            if (type == typeof(EventHandler))
            {
                return true;
            }

            if (type.IsGenericType)
            {
                var baseType = type.GetGenericTypeDefinition();
                if (baseType == typeof(EventHandler<>))
                {
                    var genericParameters = type.GetGenericArguments();
                    if (genericParameters[0].ContainsGenericParameters)
                    {
                        return false;
                    }

                    if (IsTypeSerializable(genericParameters[0]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
