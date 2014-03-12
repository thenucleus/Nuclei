//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class LocalCommandCollectionTest
    {
        [Test]
        public void Register()
        {
            var collection = new LocalCommandCollection();

            var commands = new Mock<InteractionExtensionsTest.IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn), commands.Object);

            Assert.IsTrue(collection.Any(pair => pair.Item1 == typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
        }

        [Test]
        public void RegisterWithExistingType()
        {
            var collection = new LocalCommandCollection();
            
            var commands = new Mock<InteractionExtensionsTest.IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn), commands.Object);
            Assert.AreEqual(1, collection.Count(pair => pair.Item1 == typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));

            Assert.Throws<CommandAlreadyRegisteredException>(() => collection.Register(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn), commands.Object));
            Assert.AreEqual(1, collection.Count(pair => pair.Item1 == typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
        }

        [Test]
        public void CommandsForWithUnknownType()
        {
            var collection = new LocalCommandCollection();
            Assert.IsNull(collection.CommandsFor(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn)));
        }

        [Test]
        public void CommandsFor()
        {
            var collection = new LocalCommandCollection();

            var commands = new Mock<InteractionExtensionsTest.IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn), commands.Object);

            var commandSet = collection.CommandsFor(typeof(InteractionExtensionsTest.IMockCommandSetWithTaskReturn));
            Assert.AreSame(commands.Object, commandSet);
        }
    }
}
