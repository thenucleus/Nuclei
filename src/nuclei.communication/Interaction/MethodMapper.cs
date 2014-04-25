//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Documents;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines methods for mapping a <see cref="ICommandSet"/> method to a method on a given object.
    /// </summary>
    public sealed class MethodMapper
    {
        /// <summary>
        /// The action that is used to store the command definition.
        /// </summary>
        private readonly Action<CommandDefinition> m_StoreDefinition;

        /// <summary>
        /// The ID for the command interface method.
        /// </summary>
        private readonly CommandId m_CommandId;

        /// <summary>
        /// The expected return type of the command instance method.
        /// </summary>
        private readonly Type m_ExpectedReturnType;

        /// <summary>
        /// The list of parameters that will be passed to the command instance method.
        /// </summary>
        private readonly ParameterInfo[] m_ProvidedParameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodMapper"/> class.
        /// </summary>
        /// <param name="storeDefinition">The action that is used to store the command definition.</param>
        /// <param name="commandId">The ID for the command interface method.</param>
        /// <param name="expectedReturnType">The expected return type of the command instance method.</param>
        /// <param name="providedParameters">The list of parameters that will be passed to the command instance method.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="storeDefinition"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commandId"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="expectedReturnType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="providedParameters"/> is <see langword="null" />.
        /// </exception>
        internal MethodMapper(Action<CommandDefinition> storeDefinition, CommandId commandId, Type expectedReturnType, ParameterInfo[] providedParameters)
        {
            {
                Lokad.Enforce.Argument(() => storeDefinition);
                Lokad.Enforce.Argument(() => commandId);
                Lokad.Enforce.Argument(() => expectedReturnType);
                Lokad.Enforce.Argument(() => providedParameters);
            }

            m_StoreDefinition = storeDefinition;
            m_CommandId = commandId;
            m_ExpectedReturnType = expectedReturnType;
            m_ProvidedParameters = providedParameters;
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> methods to a parameterless method with no return value.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance that contains the mapped method.</typeparam>
        /// <param name="instance">The instance which contains the method to which the command method should be mapped.</param>
        /// <param name="methodCall">The method to which the command method should be mapped.</param>
        public void To<TInstance>(
            TInstance instance,
            Expression<Action> methodCall)
        {
            // Grab method info
            //
            // Check:
            // - Return type matches expected return type (Task or Task<T>)
            // - Parameters match provided parameters
            // - Remaining parameters are attribute-d and known

            // Store instance
            // Create mapping
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> methods to a parameterless method with no return value.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance that contains the mapped method.</typeparam>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <param name="instance">The instance which contains the method to which the command method should be mapped.</param>
        /// <param name="methodCall">The method to which the command method should be mapped.</param>
        public void To<TInstance, T1>(
            TInstance instance,
            Expression<Action<T1>> methodCall)
        {
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> methods to a parameterless method with no return value.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance that contains the mapped method.</typeparam>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <param name="instance">The instance which contains the method to which the command method should be mapped.</param>
        /// <param name="methodCall">The method to which the command method should be mapped.</param>
        public void To<TInstance, T1, T2>(
            TInstance instance,
            Expression<Action<T1, T2>> methodCall)
        {
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> methods to a parameterless method with no return value.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance that contains the mapped method.</typeparam>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <param name="instance">The instance which contains the method to which the command method should be mapped.</param>
        /// <param name="methodCall">The method to which the command method should be mapped.</param>
        public void To<TInstance, T1, T2, T3>(
            TInstance instance,
            Expression<Action<T1, T2, T3>> methodCall)
        {
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> methods to a parameterless method with no return value.
        /// </summary>
        /// <typeparam name="TInstance">The type of the instance that contains the mapped method.</typeparam>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <param name="instance">The instance which contains the method to which the command method should be mapped.</param>
        /// <param name="methodCall">The method to which the command method should be mapped.</param>
        public void To<TInstance, T1, T2, T3, T4>(
            TInstance instance,
            Expression<Action<T1, T2, T3, T4>> methodCall)
        {
        }
    }
}
