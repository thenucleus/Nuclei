//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            void MethodWithoutReturnValue(int someNumber);

            int MethodWithReturnValue(int someNumber);
        }

        [Test]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            SendMessage sendAction = (e, m, r) => { };
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
                    .Verifiable();
            }

            var commandIds = new List<CommandId>
                {
                    CommandId.Create(typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue")),
                };
            var commandSets = new List<CommandDefinition> 
                { 
                    new CommandDefinition(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterDefinition(
                                    typeof(int),
                                    "someNumber",
                                    CommandParameterOrigin.FromCommand), 
                            }, 
                        false,
                        (Action<int>)actionObject.Object.MethodWithoutReturnValue)
                };

            var endpoint = new EndpointId("id");

            ICommunicationMessage storedMsg = null;
            SendMessage sendAction =
                (e, m, r) =>
                {
                    storedMsg = m;
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandToInvoke(It.IsAny<CommandId>()))
                    .Returns(commandSets[0]);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandIds.GetEnumerator());
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterValueMap(
                                    new CommandParameterDefinition(typeof(int), "someNumber", CommandParameterOrigin.FromCommand), 
                                    2), 
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
                    .Returns(1)
                    .Verifiable();
            }

            var commandIds = new List<CommandId>
                {
                    CommandId.Create(typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue")),
                };
            var commandSets = new List<CommandDefinition> 
                { 
                    new CommandDefinition(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterDefinition(
                                    typeof(int),
                                    "someNumber",
                                    CommandParameterOrigin.FromCommand), 
                            }, 
                        true,
                        (Func<int, int>)actionObject.Object.MethodWithReturnValue)
                };

            var endpoint = new EndpointId("id");

            ICommunicationMessage storedMsg = null;
            SendMessage sendAction =
                (e, m, r) =>
                {
                    storedMsg = m;
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandToInvoke(It.IsAny<CommandId>()))
                    .Returns(commandSets[0]);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandIds.GetEnumerator());
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterValueMap(
                                    new CommandParameterDefinition(typeof(int), "someNumber", CommandParameterOrigin.FromCommand), 
                                    2), 
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
                    .Verifiable();
            }

            var commandIds = new List<CommandId>
                {
                    CommandId.Create(typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue")),
                };
            var commandSets = new List<CommandDefinition> 
                { 
                    new CommandDefinition(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterDefinition(
                                    typeof(int),
                                    "someNumber",
                                    CommandParameterOrigin.FromCommand), 
                            }, 
                        false,
                        (Action<int>)actionObject.Object.MethodWithoutReturnValue)
                };

            var endpoint = new EndpointId("id");

            int count = 0;
            ICommunicationMessage storedMsg = null;
            SendMessage sendAction =
                (e, m, r) =>
                {
                    count++;
                    if (count <= 1)
                    {
                        throw new Exception();
                    }
                    
                    storedMsg = m;
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandToInvoke(It.IsAny<CommandId>()))
                    .Returns(commandSets[0]);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandIds.GetEnumerator());
            }

            int loggerCount = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { loggerCount++; }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterValueMap(
                                    new CommandParameterDefinition(typeof(int), "someNumber", CommandParameterOrigin.FromCommand), 
                                    2), 
                            })));

            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());
            Assert.AreEqual(2, count);
            Assert.AreEqual(2, loggerCount);
            Assert.IsInstanceOf<FailureMessage>(storedMsg);
        }

        [Test]
        public void InvokeWithFailedChannel()
        {
            var actionObject = new Mock<IMockCommandSet>();
            {
                actionObject.Setup(a => a.MethodWithoutReturnValue(It.IsAny<int>()))
                    .Verifiable();
            }

            var commandIds = new List<CommandId>
                {
                    CommandId.Create(typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue")),
                };
            var commandSets = new List<CommandDefinition> 
                { 
                    new CommandDefinition(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterDefinition(
                                    typeof(int),
                                    "someNumber",
                                    CommandParameterOrigin.FromCommand), 
                            }, 
                        false,
                        (Action<int>)actionObject.Object.MethodWithoutReturnValue)
                };

            var endpoint = new EndpointId("id");
            SendMessage sendAction =
                (e, m, r) =>
                {
                    throw new Exception();
                };
            var commands = new Mock<ICommandCollection>();
            {
                commands.Setup(c => c.CommandToInvoke(It.IsAny<CommandId>()))
                    .Returns(commandSets[0]);
                commands.Setup(c => c.GetEnumerator())
                    .Returns(commandIds.GetEnumerator());
            }

            int count = 0;
            var systemDiagnostics = new SystemDiagnostics((p, s) => { count++; }, null);

            var action = new CommandInvokedProcessAction(endpoint, sendAction, commands.Object, systemDiagnostics);
            action.Invoke(
                new CommandInvokedMessage(
                    new EndpointId("otherId"),
                    new CommandInvokedData(
                        commandIds[0],
                        new[]
                            {
                                new CommandParameterValueMap(
                                    new CommandParameterDefinition(typeof(int), "someNumber", CommandParameterOrigin.FromCommand), 
                                    2), 
                            })));

            Assert.AreEqual(3, count);
            actionObject.Verify(a => a.MethodWithoutReturnValue(It.IsAny<int>()), Times.Once());
        }
    }
}
