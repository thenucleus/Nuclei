//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Communication.Protocol;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CommandDefinitionTest
    {
        [Test]
        public void Create()
        {
            var id = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            Func<int, double, string> func = (i, d) => (i + d).ToString(CultureInfo.InvariantCulture);
            var parameters = new[]
                {
                    new CommandParameterDefinition(typeof(int), "i", CommandParameterOrigin.FromCommand), 
                    new CommandParameterDefinition(typeof(double), "d", CommandParameterOrigin.FromCommand), 
                };

            Delegate commandToExecute = func;

            var definition = new CommandDefinition(id, parameters, true, commandToExecute);
            
            Assert.AreEqual(id, definition.Id);
            Assert.IsTrue(definition.HasReturnValue);
        }

        [Test]
        public void InvokeWithMissingParameter()
        {
            var id = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            Func<int, double, string> func = (i, d) => (i + d).ToString(CultureInfo.InvariantCulture);
            var parameters = new[]
                {
                    new CommandParameterDefinition(typeof(int), "i", CommandParameterOrigin.FromCommand), 
                    new CommandParameterDefinition(typeof(double), "d", CommandParameterOrigin.FromCommand), 
                };

            Delegate commandToExecute = func;

            var definition = new CommandDefinition(id, parameters, true, commandToExecute);

            var parameterValues = new[]
                {
                    new CommandParameterValueMap(
                        parameters[0],
                        10), 
                };
            Assert.Throws<MissingCommandParameterException>(() => definition.Invoke(new EndpointId("a"), new MessageId(), parameterValues));
        }

        [Test]
        public void InvokeWithInvalidOrigin()
        {
            var id = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            Func<int, double, string> func = (i, d) => (i + d).ToString(CultureInfo.InvariantCulture);
            var parameters = new[]
                {
                    new CommandParameterDefinition(typeof(int), "i", CommandParameterOrigin.FromCommand), 
                    new CommandParameterDefinition(typeof(double), "d", CommandParameterOrigin.Unknown), 
                };

            Delegate commandToExecute = func;

            var definition = new CommandDefinition(id, parameters, true, commandToExecute);

            var parameterValues = new[]
                {
                    new CommandParameterValueMap(
                        parameters[0],
                        10), 
                    new CommandParameterValueMap(
                        parameters[1],
                        10.0), 
                };
            Assert.Throws<InvalidCommandParameterOriginException>(() => definition.Invoke(new EndpointId("a"), new MessageId(), parameterValues));
        }

        [Test]
        public void InvokeWithOnlyInterfaceMethodParameters()
        {
            var id = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            Func<int, double, string> func = (i, d) => (i + d).ToString(CultureInfo.InvariantCulture);
            var parameters = new[]
                {
                    new CommandParameterDefinition(typeof(int), "i", CommandParameterOrigin.FromCommand), 
                    new CommandParameterDefinition(typeof(double), "d", CommandParameterOrigin.FromCommand), 
                };

            Delegate commandToExecute = func;

            var definition = new CommandDefinition(id, parameters, true, commandToExecute);

            var parameterValues = new[]
                {
                    new CommandParameterValueMap(
                        parameters[0],
                        10), 
                    new CommandParameterValueMap(
                        parameters[1],
                        10.0), 
                };
            var result = definition.Invoke(new EndpointId("a"), new MessageId(), parameterValues);
            Assert.IsInstanceOf(typeof(string), result);
            Assert.AreEqual(20.0.ToString(CultureInfo.InvariantCulture), result);
        }

        [Test]
        public void InvokeWithEndpointIdParameter()
        {
            var id = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            int storedValue = -1;
            EndpointId storedEndpoint = null;
            var expectedResult = "20.0";
            Func<int, EndpointId, string> func = 
                (i, e) =>
                {
                    storedValue = i;
                    storedEndpoint = e;
                    return expectedResult;
                };
            var parameters = new[]
                {
                    new CommandParameterDefinition(typeof(int), "i", CommandParameterOrigin.FromCommand), 
                    new CommandParameterDefinition(typeof(EndpointId), "e", CommandParameterOrigin.InvokingEndpointId), 
                };

            Delegate commandToExecute = func;

            var definition = new CommandDefinition(id, parameters, true, commandToExecute);

            var parameterValues = new[]
                {
                    new CommandParameterValueMap(
                        parameters[0],
                        10), 
                };
            var endpoint = new EndpointId("a");
            var msg = new MessageId();
            var result = definition.Invoke(endpoint, msg, parameterValues);
            Assert.IsInstanceOf(typeof(string), result);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(parameterValues[0].Value, storedValue);
            Assert.AreEqual(endpoint, storedEndpoint);
        }

        [Test]
        public void InvokeWithMessageIdParameter()
        {
            var id = CommandId.Create(typeof(int).GetMethod("CompareTo", new[] { typeof(object) }));

            int storedValue = -1;
            MessageId storedMessage = null;
            var expectedResult = "20.0";
            Func<int, MessageId, string> func =
                (i, m) =>
                {
                    storedValue = i;
                    storedMessage = m;
                    return expectedResult;
                };
            var parameters = new[]
                {
                    new CommandParameterDefinition(typeof(int), "i", CommandParameterOrigin.FromCommand), 
                    new CommandParameterDefinition(typeof(MessageId), "m", CommandParameterOrigin.InvokingMessageId), 
                };

            Delegate commandToExecute = func;

            var definition = new CommandDefinition(id, parameters, true, commandToExecute);

            var parameterValues = new[]
                {
                    new CommandParameterValueMap(
                        parameters[0],
                        10), 
                };
            var endpoint = new EndpointId("a");
            var msg = new MessageId();
            var result = definition.Invoke(endpoint, msg, parameterValues);
            Assert.IsInstanceOf(typeof(string), result);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(expectedResult, result);
            Assert.AreEqual(parameterValues[0].Value, storedValue);
            Assert.AreEqual(msg, storedMessage);
        }
    }
}
