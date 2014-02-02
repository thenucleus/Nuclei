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

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class CommandProxyBuilderTest
    {
        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonAssignableType()
        { 
            Assert.Throws<TypeIsNotAValidCommandSetException>(() => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(object)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(MockCommandSetNotAnInterface)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithGenericParameters<>)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithProperties()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithProperties)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithAdditionalEvents()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithEvents)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithoutMethods()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithoutMethods)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericMethod()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithGenericMethod)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithIncorrectReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithIncorrectReturnType)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithNonSerializableReturnType)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithOutParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithOutParameter)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithRefParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithRefParameter)));
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandProxyBuilder.VerifyThatTypeIsACorrectCommandSet(typeof(IMockCommandSetWithMethodWithNonSerializableParameter)));
        }

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
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            result.Wait();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsFalse(result.IsFaulted);

            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn)), intermediateMsg.Invocation.Type);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(ProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
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
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            Assert.Throws<AggregateException>(result.Wait);

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsTrue(result.IsFaulted);
            Assert.IsAssignableFrom(typeof(CommandInvocationFailedException), result.Exception.InnerExceptions[0]);

            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockCommandSetWithTaskReturn)), intermediateMsg.Invocation.Type);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(ProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
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
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTypedTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            result.Wait();

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsFalse(result.IsFaulted);
            Assert.AreEqual(20, result.Result);

            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn)), intermediateMsg.Invocation.Type);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(ProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
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
            var proxy = builder.ProxyConnectingTo<IMockCommandSetWithTypedTaskReturn>(remoteEndpoint);

            var result = proxy.MyMethod(10);
            Assert.Throws<AggregateException>(result.Wait);

            Assert.IsTrue(result.IsCompleted);
            Assert.IsFalse(result.IsCanceled);
            Assert.IsTrue(result.IsFaulted);
            Assert.IsAssignableFrom(typeof(CommandInvocationFailedException), result.Exception.InnerExceptions[0]);

            Assert.AreEqual(ProxyExtensions.FromType(typeof(IMockCommandSetWithTypedTaskReturn)), intermediateMsg.Invocation.Type);
            Assert.AreEqual(1, intermediateMsg.Invocation.Parameters.Count);
            Assert.AreEqual(ProxyExtensions.FromType(typeof(int)), intermediateMsg.Invocation.Parameters[0].Item1);
            Assert.AreEqual(10, intermediateMsg.Invocation.Parameters[0].Item2);
        }
    }
}
