//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
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
            Assert.Throws<MissingCommandParameterException>(() => definition.Invoke(parameterValues));
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
            Assert.Throws<InvalidCommandParameterOriginException>(() => definition.Invoke(parameterValues));
        }

        [Test]
        public void Invoke()
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
            var result = definition.Invoke(parameterValues);
            Assert.IsInstanceOf(typeof(string), result);
            Assert.AreEqual(20.0.ToString(CultureInfo.InvariantCulture), result);
        }
    }
}
