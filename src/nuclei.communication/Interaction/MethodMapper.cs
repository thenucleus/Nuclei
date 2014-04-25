﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
        internal MethodMapper(
            Action<CommandDefinition> storeDefinition, 
            CommandId commandId, 
            Type expectedReturnType, 
            ParameterInfo[] providedParameters)
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
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To(Expression<Action> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        private void CreateCommandDefinition(LambdaExpression methodCall)
        {
            var pair = ExtractInstanceAndMethod(methodCall);
            if (pair.Item2.ReturnType != m_ExpectedReturnType)
            {
                throw new InvalidCommandMappingException();
            }

            var parameters = CreateParameterDefinitions(pair.Item2, m_ProvidedParameters);

            var commandDelegate = CreateCommandDelegate(pair.Item1, pair.Item2);
            var definition = new CommandDefinition(
                m_CommandId,
                parameters,
                pair.Item2.ReturnType != typeof(void),
                commandDelegate);
            m_StoreDefinition(definition);
        }

        private Tuple<object, MethodInfo> ExtractInstanceAndMethod(LambdaExpression method)
        {
            var methodCall = method.Body as MethodCallExpression;
            if (methodCall == null)
            {
                throw new InvalidCommandMethodExpressionException();
            }

            var fieldExpression = methodCall.Object as MemberExpression;
            if (fieldExpression == null)
            {
                throw new InvalidCommandMethodExpressionException();
            }

            var constantExpression = fieldExpression.Expression as ConstantExpression;
            if (constantExpression == null)
            {
                throw new InvalidCommandMethodExpressionException();
            }

            var instance = constantExpression.Value;
            var methodInfo = methodCall.Method;

            return new Tuple<object, MethodInfo>(instance, methodInfo);
        }

        private CommandParameterDefinition[] CreateParameterDefinitions(MethodInfo methodInfo, ParameterInfo[] providedParameters)
        {
            var instanceParameters = methodInfo.GetParameters();

            // Verify that all the provided parameters have a matching instance parameter
            var hasUnmatchedProvidedParameters = providedParameters.Any(
                p => !instanceParameters.Any(i => (i.ParameterType == p.ParameterType) && string.Equals(i.Name, p.Name, StringComparison.Ordinal)));
            if (hasUnmatchedProvidedParameters)
            {
                throw new NonMappedCommandParameterException();
            }

            // Get all the instance parameters for which there is no provided parameter and check that they have an attribute attached to it
            var nonMappedInstanceParameters = instanceParameters
                .Where(i => !providedParameters.Any(p => (p.ParameterType == i.ParameterType) && string.Equals(p.Name, i.Name)))
                .ToList();
            foreach (var instanceParameter in nonMappedInstanceParameters)
            {
                var attributes = instanceParameter.GetCustomAttributes(true);

                var parameterUsageAttribute = attributes.FirstOrDefault(
                    o => InteractionExtensions.KnownCommandInstanceParameterAttributes.Contains(o.GetType())) 
                    as CommandInstanceParameterUsageAttribute;
                if (parameterUsageAttribute != null)
                {
                    if (parameterUsageAttribute.AllowedParameterType != instanceParameter.ParameterType)
                    {
                        throw new NonMappedCommandParameterException();
                    }
                }
            }

            var commandParameterDefinitions = providedParameters
                .Select(p => new CommandParameterDefinition(p.ParameterType, p.Name, CommandParameterOrigin.FromCommand));
            var instanceParameterDefinitions = nonMappedInstanceParameters
                .Select(p => new CommandParameterDefinition(p.ParameterType, p.Name, CommandParameterOrigin.FromCommand));

            return commandParameterDefinitions.Append(instanceParameterDefinitions).ToArray();
        }

        private Delegate CreateCommandDelegate(object instance, MethodInfo method)
        {
            var args = method.GetParameters().Select(p => p.ParameterType).ToList();
            args.Add(method.ReturnType);
            var delegateType = Expression.GetDelegateType(args.ToArray());

            return Delegate.CreateDelegate(delegateType, instance, method);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1>(Expression<Action<T1>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2>(Expression<Action<T1, T2>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3>(Expression<Action<T1, T2, T3>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7>(Expression<Action<T1, T2, T3, T4, T5, T6, T7>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <typeparam name="T10">The tenth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <typeparam name="T10">The tenth method parameter.</typeparam>
        /// <typeparam name="T11">The eleventh method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <typeparam name="T10">The tenth method parameter.</typeparam>
        /// <typeparam name="T11">The eleventh method parameter.</typeparam>
        /// <typeparam name="T12">The twelfth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <typeparam name="T10">The tenth method parameter.</typeparam>
        /// <typeparam name="T11">The eleventh method parameter.</typeparam>
        /// <typeparam name="T12">The twelfth method parameter.</typeparam>
        /// <typeparam name="T13">The thirteenth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <typeparam name="T10">The tenth method parameter.</typeparam>
        /// <typeparam name="T11">The eleventh method parameter.</typeparam>
        /// <typeparam name="T12">The twelfth method parameter.</typeparam>
        /// <typeparam name="T13">The thirteenth method parameter.</typeparam>
        /// <typeparam name="T14">The fourteenth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <typeparam name="T10">The tenth method parameter.</typeparam>
        /// <typeparam name="T11">The eleventh method parameter.</typeparam>
        /// <typeparam name="T12">The twelfth method parameter.</typeparam>
        /// <typeparam name="T13">The thirteenth method parameter.</typeparam>
        /// <typeparam name="T14">The fourteenth method parameter.</typeparam>
        /// <typeparam name="T15">The fifteenth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }

        /// <summary>
        /// Maps a <see cref="ICommandSet"/> method to a command instance method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <typeparam name="T9">The ninth method parameter.</typeparam>
        /// <typeparam name="T10">The tenth method parameter.</typeparam>
        /// <typeparam name="T11">The eleventh method parameter.</typeparam>
        /// <typeparam name="T12">The twelfth method parameter.</typeparam>
        /// <typeparam name="T13">The thirteenth method parameter.</typeparam>
        /// <typeparam name="T14">The fourteenth method parameter.</typeparam>
        /// <typeparam name="T15">The fifteenth method parameter.</typeparam>
        /// <typeparam name="T16">The sixteenth method parameter.</typeparam>
        /// <param name="methodCall">The instance method to which the command method should be mapped.</param>
        public void To<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            Expression<Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>> methodCall)
        {
            CreateCommandDefinition(methodCall);
        }
    }
}