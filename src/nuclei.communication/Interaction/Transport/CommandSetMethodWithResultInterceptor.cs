//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Lokad.Quality;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Defines an <see cref="IInterceptor"/> for <see cref="ICommandSet"/> methods that return a value.
    /// </summary>
    internal sealed class CommandSetMethodWithResultInterceptor : CommandSetMethodInterceptor
    {
        /// <summary>
        /// Returns a task with a specific return type based on an expected <see cref="CommandInvokedResponseMessage"/> object
        /// which is delivered by another task.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the object carried by the <see cref="CommandInvokedResponseMessage"/> object in the input task.
        /// </typeparam>
        /// <param name="inputTask">
        ///     The task which will deliver the <see cref="ICommunicationMessage"/> that contains the return value.
        /// </param>
        /// <param name="scheduler">The scheduler that runs the return task.</param>
        /// <returns>
        /// A task returning the desired return type.
        /// </returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method is called via reflection in order to generate the correct return value for a command method.")]
        [UsedImplicitly]
        private static Task<T> CreateTask<T>(Task<ICommunicationMessage> inputTask, TaskScheduler scheduler)
        {
            Func<T> action = () =>
            {
                try
                {
                    inputTask.Wait();
                }
                catch (AggregateException e)
                {
                    throw new CommandInvocationFailedException(
                        Resources.Exceptions_Messages_CommandInvocationFailed,
                        e);
                }

                var resultMsg = inputTask.Result as CommandInvokedResponseMessage;
                if (resultMsg != null)
                {
                    return (T)resultMsg.Result;
                }

                throw new CommandInvocationFailedException();
            };

            return Task<T>.Factory.StartNew(
                action,
                new CancellationToken(),
                TaskCreationOptions.LongRunning,
                scheduler);
        }

        /// <summary>
        /// The scheduler that will be used to schedule tasks.
        /// </summary>
        private readonly TaskScheduler m_Scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSetMethodWithResultInterceptor"/> class.
        /// </summary>
        /// <param name="transmitCommandInvocation">
        ///     The function used to send the information about the method invocation to the owning endpoint.
        /// </param>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <param name="systemDiagnostics">The function that is used to log messages.</param>
        /// <param name="scheduler">The scheduler that is used to run the tasks.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="transmitCommandInvocation"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public CommandSetMethodWithResultInterceptor(
            SendCommandData transmitCommandInvocation,
            IConfiguration configuration,
            SystemDiagnostics systemDiagnostics,
            TaskScheduler scheduler = null) 
            : base(transmitCommandInvocation, configuration, systemDiagnostics)
        {
            m_Scheduler = scheduler ?? TaskScheduler.Default;
        }

        /// <summary>
        /// Extracts the return value of the method from the response message.
        /// </summary>
        /// <param name="invocation">The information about the method invocation.</param>
        /// <param name="result">The task that will eventually return the response message.</param>
        /// <returns>The <see cref="Task"/> that will complete when the response message is received.</returns>
        protected override Task ExtractMethodReturnFrom(IInvocation invocation, Task<ICommunicationMessage> result)
        {
            // Now that we have the result we need to create the return value of the correct type
            // The only catch is that we don't know (at compile time) what the return value must
            // be so we'll have to do this the nasty way
            //
            // First get the return value for the proxied method
            // This should be Task<T> for some value of T
            Type invocationReturn = invocation.Method.ReturnType;
            Debug.Assert(!invocationReturn.ContainsGenericParameters, "The return type should be a closed constructed type.");

            var genericArguments = invocationReturn.GetGenericArguments();
            Debug.Assert(genericArguments.Length == 1, "There should be exactly one generic argument.");

            // Now 'build' a method that can create the Task<T> object. We'll have to do this
            // through reflection because we don't know the typeof(T) at compile time.
            // Unfortunatly we can't do this with the 'dynamic
            // keyword because the generic parameters for the method are NOT determined by the input parameters
            // so if we create a 'dynamic' object and call the method with the desired name then the runtime
            // won't be able to figure out what the typed parameter has to be. So we'll do it the 
            // old-fashioned way, with reflection.
            var taskBuilder = GetType()
                .GetMethod(
                    "CreateTask",
                    BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic,
                    null,
                    new[] { typeof(Task<ICommunicationMessage>), typeof(TaskScheduler) },
                    null)
                .MakeGenericMethod(genericArguments[0]);

            // Create the return value. This is invoking a MethodInfo object which is
            // slow but we don't expect it to cause too much trouble given that we're getting
            // the result from another application which lives on the other side of a named pipe
            // (best case) or TCP connection (worst case)
            return taskBuilder.Invoke(null, new object[] { result, m_Scheduler }) as Task;
        }
    }
}
