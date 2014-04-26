//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines methods to map the methods on a <see cref="ICommandSet"/> to methods on one or more objects
    /// which may or may not implement the interface..
    /// </summary>
    /// <typeparam name="TCommand">The interface type for which the command methods should be mapped.</typeparam>
    public sealed class CommandMapper<TCommand> where TCommand : ICommandSet
    {
        /// <summary>
        /// Creates a new <see cref="CommandMapper{TCommand}"/> instance.
        /// </summary>
        /// <returns>The command mapper instance.</returns>
        public static CommandMapper<TCommand> Create()
        {
            var type = typeof(TCommand);
            
            // Verify needs to be updated with the correct attributes 
            type.VerifyThatTypeIsACorrectCommandSet();

            return new CommandMapper<TCommand>();
        }

        /// <summary>
        /// The collection that contains all the created command definitions.
        /// </summary>
        private readonly Dictionary<CommandId, CommandDefinition> m_Definitions
            = new Dictionary<CommandId, CommandDefinition>();

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From(
            Expression<Action<TCommand>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        private MethodMapper CreateMethodMapper(LambdaExpression methodCall)
        {
            var methodInfo = ExtractMethod(methodCall);
            var returnType = ExtractInstanceMethodReturnType(methodInfo);
            var parameters = ExtractInstanceMethodParameters(methodInfo);

            return new MethodMapper(
                StoreDefinition,
                CommandId.Create(methodInfo), 
                returnType, 
                parameters);
        }

        private MethodInfo ExtractMethod(LambdaExpression method)
        {
            var methodCall = method.Body as MethodCallExpression;
            if (methodCall == null)
            {
                throw new InvalidCommandMethodExpressionException();
            }

            return methodCall.Method;
        }

        private Type ExtractInstanceMethodReturnType(MethodInfo method)
        {
            var type = method.ReturnType;
            if (type == typeof(Task))
            {
                return typeof(void);
            }

            if (type.IsGenericType)
            {
                var baseType = type.GetGenericTypeDefinition();
                if (baseType == typeof(Task<>))
                {
                    var genericParameters = type.GetGenericArguments();
                    return genericParameters[0];
                }
            }

            throw new TypeIsNotAValidCommandSetException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetMethodsMustHaveCorrectReturnType,
                    typeof(TCommand),
                    method));
        }

        private ParameterInfo[] ExtractInstanceMethodParameters(MethodInfo method)
        {
            var result = new List<ParameterInfo>();

            var parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var attributes = parameter.GetCustomAttributes(true);

                var parameterUsageAttribute = attributes.FirstOrDefault(
                    o => InteractionExtensions.KnownCommandSetParameterAttributes.Contains(o.GetType())) as CommandProxyParameterUsageAttribute;
                if (parameterUsageAttribute != null)
                {
                    if (parameterUsageAttribute.AllowedParameterType != parameter.ParameterType)
                    {
                        throw new TypeIsNotAValidCommandSetException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.Exceptions_Messages_TypeIsNotAValidCommandSet_CommandSetParametersMustBeValid,
                            typeof(TCommand),
                            method));
                    }

                    continue;
                }

                result.Add(parameter);
            }

            return result.ToArray();
        }

        private void StoreDefinition(CommandDefinition definition)
        {
            if (!m_Definitions.ContainsKey(definition.Id))
            {
                m_Definitions.Add(definition.Id, definition);
            }
            else
            {
                m_Definitions[definition.Id] = definition;
            }
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1>(
            Expression<Action<TCommand, T1>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2>(
            Expression<Action<TCommand, T1, T2>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3>(
            Expression<Action<TCommand, T1, T2, T3>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4>(
            Expression<Action<TCommand, T1, T2, T3, T4>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
        /// </summary>
        /// <typeparam name="T1">The first method parameter.</typeparam>
        /// <typeparam name="T2">The second method parameter.</typeparam>
        /// <typeparam name="T3">The third method parameter.</typeparam>
        /// <typeparam name="T4">The fourth method parameter.</typeparam>
        /// <typeparam name="T5">The fifth method parameter.</typeparam>
        /// <typeparam name="T6">The sixth method parameter.</typeparam>
        /// <typeparam name="T7">The seventh method parameter.</typeparam>
        /// <typeparam name="T8">The eight method parameter.</typeparam>
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
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
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8, T9>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
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
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
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
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
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
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
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
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
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
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates a <see cref="MethodMapper"/> for a command interface method.
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
        /// <param name="methodCall">The expression calling the mapped method.</param>
        /// <returns>The method mapper.</returns>
        /// <exception cref="InvalidCommandMethodExpressionException">
        ///     Thrown when the <paramref name="methodCall"/> expression does not contain a method call on the command interface.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when the return type of the command method is not a <see cref="Task"/> or a <see cref="Task{T}"/> instance.
        /// </exception>
        /// <exception cref="TypeIsNotAValidCommandSetException">
        ///     Thrown when one of the parameters is decorated with a <see cref="CommandProxyParameterUsageAttribute"/> but the
        ///     type of the parameter does not match the type allowed by the attribute.
        /// </exception>
        public MethodMapper From<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Expression<Action<TCommand, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>> methodCall)
        {
            return CreateMethodMapper(methodCall);
        }

        /// <summary>
        /// Creates the command map for the given command interface.
        /// </summary>
        /// <returns>The command map.</returns>
        /// <exception cref="CommandMethodNotMappedException">
        ///     Thrown when one or more of the methods on the command interface are not mapped to instance methods.
        /// </exception>
        public CommandMap ToMap()
        {
            var type = typeof(TCommand);
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                var id = CommandId.Create(method);
                if (!m_Definitions.ContainsKey(id))
                {
                    throw new CommandMethodNotMappedException();
                }
            }

            return new CommandMap(type, m_Definitions.Values.ToArray());
        }
    }
}
