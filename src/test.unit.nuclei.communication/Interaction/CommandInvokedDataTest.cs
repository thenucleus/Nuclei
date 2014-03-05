//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
            var data = new CommandData(typeof(string), "CompareTo");
            var parameters = new[]
                {
                    new Tuple<Type, object>(typeof(string), "a"), 
                };

            var invocationData = new CommandInvokedData(data, parameters);
            Assert.AreSame(data, invocationData.Command);
            Assert.AreSame(parameters, invocationData.ParameterValues);
        }
    }
}
