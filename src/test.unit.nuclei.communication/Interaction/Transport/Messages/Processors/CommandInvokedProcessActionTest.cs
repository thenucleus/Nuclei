//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInvokedProcessActionTest
    {
        // A fake command set interface to invoke methods on
        public interface IMockCommandSet : ICommandSet
        {
            Task MethodWithoutReturnValue(int someNumber);

            Task<int> MethodWithReturnValue(int someNumber);
        }

        [Test]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) => { };
            var commands = new Mock<ICommandCollection>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            Assert.AreEqual(typeof(CommandInvokedMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void InvokeWithTaskReturn()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithoutReturnValue(It.IsAny<int>()))
                    .Returns(Task.Factory.StartNew(
                        () => { },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var commandSets = new List<Tuple<Type, ICommandSet>> 
                { 
                    new Tuple<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");

            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
                {
                    storedMsg = m;
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Item2);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        new CommandData(typeof(IMockCommandSet), "MethodWithReturnValue"),
                        new[]
                            {
                                new Tuple<Type, object>(typeof(int), 2), 
                            })));

            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());
            Assert.IsInstanceOf<SuccessMessage>(storedMsg);
        }

        [Test]
        public void InvokeWithTypedTaskReturn()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithReturnValue(It.IsAny<int>()))
                    .Returns(() => Task<int>.Factory.StartNew(
                        () => 1,
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var commandSets = new List<Tuple<Type, ICommandSet>> 
                { 
                    new Tuple<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
                {
                    storedMsg = m;
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Item2);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        new CommandData(typeof(IMockCommandSet), "MethodWithReturnValue"),
                        new[]
                            {
                                new Tuple<Type, object>(typeof(int), 2), 
                            })));

            actionObject.Verify(a => a.MethodWithReturnValue(It.IsAny<int>()), Times.Once());
            Assert.IsInstanceOf<CommandInvokedResponseMessage>(storedMsg);

            var responseMsg = storedMsg as CommandInvokedResponseMessage;
            Assert.IsInstanceOf<int>(responseMsg.Result);
            Assert.AreEqual(1, (int)responseMsg.Result);
        }

        [Test]
        public void InvokeWithFailingResponse()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithoutReturnValue(It.IsAny<int>()))
                    .Returns(Task.Factory.StartNew(
                        () => { },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var commandSets = new List<Tuple<Type, ICommandSet>> 
                { 
                    new Tuple<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");

            int count = 0;
            ICommunicationMessage storedMsg = null;
            Action<EndpointId, ICommunicationMessage> sendAction = (e, m) =>
                {
                    count++;
                    if (count <= 1)
                    {
                        throw new Exception();
                    }
                    else
                    {
                        storedMsg = m;
                    }
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Item2);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            int loggerCount = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { loggerCount++; }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        new CommandData(typeof(IMockCommandSet), "MethodWithReturnValue"),
                        new[]
                            {
                                new Tuple<Type, object>(typeof(int), 2), 
                            })));

            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());
            Assert.AreEqual(2, count);
            Assert.AreEqual(3, loggerCount);
            Assert.IsInstanceOf<FailureMessage>(storedMsg);
        }

        [Test]
        public void InvokeWithFailedChannel()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithoutReturnValue(It.IsAny<int>()))
                    .Returns(Task.Factory.StartNew(
                        () => { },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var commandSets = new List<Tuple<Type, ICommandSet>> 
                { 
                    new Tuple<Type, ICommandSet>(typeof(IMockCommandSet), actionObject.Object)
                };

            var endpoint = new EndpointId("id");
            Action<EndpointId, ICommunicationMessage> sendAction =
                (e, m) =>
                {
                    throw new Exception();
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandsFor(It.IsAny<Type>()))
                    .Returns(commandSets[0].Item2);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandSets.GetEnumerator());
            }

            int count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { count++; }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        new CommandData(typeof(IMockCommandSet), "MethodWithReturnValue"),
                        new[]
                            {
                                new Tuple<Type, object>(typeof(int), 2), 
                            })));

            // This is obviously pure evil but we need to wait for the tasks that get created by the Invoke method
            // Unfortunately we can't get to those tasks so we'll have to sleep the thread.
            // And because we are throwing exceptions we can't really define a good place to put a reset event either :(
            // SpinWait.SpinUntil(() => { }, 100);
            Assert.AreEqual(4, count);
            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());
        }
    }
}
