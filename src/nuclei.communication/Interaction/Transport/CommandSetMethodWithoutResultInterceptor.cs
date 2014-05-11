//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Configuration;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Defines an <see cref="IInterceptor"/> for <see cref="ICommandSet"/> methods that do not return a value.
    /// </summary>
    internal sealed class CommandSetMethodWithoutResultInterceptor : CommandSetMethodInterceptor
    {
        /// <summary>
        /// Returns a task with a specific return type based on an expected <see cref="CommandInvokedResponseMessage"/> object
        /// which is delivered by another task.
        /// </summary>
        /// <param name="inputTask">The task which will deliver the <see cref="ICommunicationMessage"/> that contains the return value.</param>
        /// <param name="scheduler">The scheduler that is used to run the task.</param>
        /// <returns>
        /// A task returning the desired return type.
        /// </returns>
        private static Task CreateTask(Task<ICommunicationMessage> inputTask, TaskScheduler scheduler)
        {
            Action action = () =>
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

                var successMsg = inputTask.Result as SuccessMessage;
                if (successMsg != null)
                {
                    return;
                }

                // var failureMsg = inputTask.Result as FailureMessage;
                throw new CommandInvocationFailedException();
            };

            return Task.Factory.StartNew(
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
        /// Initializes a new instance of the <see cref="CommandSetMethodWithoutResultInterceptor"/> class.
        /// </summary>
        /// <param name="transmitCommandInvocation">
        ///     The function used to send the information about the method invocation to the owning endpoint.
        /// </param>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
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
        public CommandSetMethodWithoutResultInterceptor(
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
            return CreateTask(result, m_Scheduler);
        }
    }
}
