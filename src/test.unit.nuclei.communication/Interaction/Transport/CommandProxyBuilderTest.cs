//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction.Transport
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed class CommandProxyBuilderTest
    {
        [Test]
        public void ProxyConnectingToMethodWithTaskReturnWithSuccessFullExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
                {
                    intermediateMsg = m as CommandInvokedMessage;
                    return Task<ICommunicationMessage>.Factory.StartNew(
                        () => new SuccessMessage(remoteEndpoint, new MessageId()),
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler());
                };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var builder = new CommandProxyBuilder(local, sender, systemDiagnostics);
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockCommandSetWithTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            result.Wait();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsFalse(result.IsFaulted);

            Assert.AreEqual(
                CommandId.Create(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).GetMethod("MyMethod")), 
                intermediateMsg.Invocation.Command);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Length);
            Assert.AreEqual(typeof(int), intermediateMsg.Invocation.Parameters[0].Parameter.Type);
            Assert.AreEqual("input", intermediateMsg.Invocation.Parameters[0].Parameter.Name);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Value);
        }

        [Test]
        public void ProxyConnectingToMethodWithTaskReturnWithFailedExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new FailureMessage(remoteEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var builder = new CommandProxyBuilder(local, sender, systemDiagnostics);
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockCommandSetWithTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            Assert.Throws<AggregateException>(result.Wait);

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsTrue(result.IsFaulted);
            Assert.IsAssignableFrom(typeof(CommandInvocationFailedException), result.Exception.InnerExceptions[0]);

            Assert.AreEqual(
                CommandId.Create(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn).GetMethod("MyMethod")),
                intermediateMsg.Invocation.Command);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Length);
            Assert.AreEqual(typeof(int), intermediateMsg.Invocation.Parameters[0].Parameter.Type);
            Assert.AreEqual("input", intermediateMsg.Invocation.Parameters[0].Parameter.Name);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Value);
        }

        [Test]
        public void ProxyConnectingToMethodWithTypedTaskReturnWithSuccessfullExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new CommandInvokedResponseMessage(remoteEndpoint, new MessageId(), 20),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var builder = new CommandProxyBuilder(local, sender, systemDiagnostics);
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            result.Wait();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(20, result.Result);

            Assert.AreEqual(
                CommandId.Create(typeof(InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn).GetMethod("MyMethod")),
                intermediateMsg.Invocation.Command);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Length);
            Assert.AreEqual(typeof(int), intermediateMsg.Invocation.Parameters[0].Parameter.Type);
            Assert.AreEqual("input", intermediateMsg.Invocation.Parameters[0].Parameter.Name);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Value);
        }

        [Test]
        public void ProxyConnectingToMethodWithTypedTaskReturnWithFailedExecution()
        {
            var remoteEndpoint = new EndpointId("other");

            var local = new EndpointId("local");
            CommandInvokedMessage intermediateMsg = null;
            Func<EndpointId, ICommunicationMessage, Task<ICommunicationMessage>> sender = (e, m) =>
            {
                intermediateMsg = m as CommandInvokedMessage;
                return Task<ICommunicationMessage>.Factory.StartNew(
                    () => new FailureMessage(remoteEndpoint, new MessageId()),
                    new CancellationToken(),
                    TaskCreationOptions.None,
                    new CurrentThreadTaskScheduler());
            };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var builder = new CommandProxyBuilder(local, sender, systemDiagnostics);
            var proxy = builder.ProxyConnectingTo<InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            Assert.Throws<AggregateException>(result.Wait);

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsTrue(result.IsFaulted);
            Assert.IsAssignableFrom(typeof(CommandInvocationFailedException), result.Exception.InnerExceptions[0]);

            Assert.AreEqual(
                CommandId.Create(typeof(InteractionExtensionsTest.IMockCommandSetWithTypedTaskReturn).GetMethod("MyMethod")),
                intermediateMsg.Invocation.Command);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Length);
            Assert.AreEqual(typeof(int), intermediateMsg.Invocation.Parameters[0].Parameter.Type);
            Assert.AreEqual("input", intermediateMsg.Invocation.Parameters[0].Parameter.Name);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Value);
        }
    }
}
