//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CommandInvokedDataTest
    {
        [Test]
        public void Create()
        {
            var id = CommandId.Create(typeof(string).GetMethod("CompareTo"));
            var parameters = new[]
                {
                    new CommandParameterValueMap(
                        new CommandParameterDefinition(typeof(string), "a", CommandParameterOrigin.FromCommand), 
                        "b"), 
                };

            var invocationData = new CommandInvokedData(id, parameters);
            Assert.AreSame(id, invocationData.Command);
            Assert.AreSame(parameters, invocationData.Parameters);
        }
    }
}
