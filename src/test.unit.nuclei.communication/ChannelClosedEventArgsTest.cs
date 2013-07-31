//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ChannelClosedEventArgsTest
    {
        [Test]
        public void Create()
        {
            var id = new EndpointId("a");
            var args = new ChannelClosedEventArgs(id);

            Assert.AreSame(id, args.ClosedChannel);
        }
    }
}
