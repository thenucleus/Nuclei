//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Nuclei.Communication.Protocol;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class LocalCommandCollectionTest
    {
        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        [Test]
        public void Register()
        {
            Type registeredType = null;
            var store = new Mock<IStoreCommunicationDescriptions>();
            {
                store.Setup(l => l.RegisterCommandType(It.IsAny<Type>()))
                    .Callback<Type>(t => registeredType = t)
                    .Verifiable();
            }

            var collection = new LocalCommandCollection(store.Object);

            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);

            Assert.IsTrue(collection.Any(pair => pair.Key == typeof(IMockCommandSetWithTaskReturn)));
            Assert.AreEqual(typeof(IMockCommandSetWithTaskReturn), registeredType);
            store.Verify(l => l.RegisterCommandType(It.IsAny<Type>()), Times.Once());
        }

        [Test]
        public void RegisterWithExistingType()
        {
            Type registeredType = null;
            var store = new Mock<IStoreCommunicationDescriptions>();
            {
                store.Setup(l => l.RegisterCommandType(It.IsAny<Type>()))
                    .Callback<Type>(t => registeredType = t)
                    .Verifiable();
            }

            var collection = new LocalCommandCollection(store.Object);
            
            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);

            Assert.AreEqual(typeof(IMockCommandSetWithTaskReturn), registeredType);
            store.Verify(l => l.RegisterCommandType(It.IsAny<Type>()), Times.Once());

            Assert.Throws<CommandAlreadyRegisteredException>(() => collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object));
            store.Verify(l => l.RegisterCommandType(It.IsAny<Type>()), Times.Once());
        }

        [Test]
        public void CommandsForWithUnknownType()
        {
            var layer = new Mock<IStoreCommunicationDescriptions>();
            var collection = new LocalCommandCollection(layer.Object);
            Assert.IsNull(collection.CommandsFor(typeof(IMockCommandSetWithTaskReturn)));
        }

        [Test]
        public void CommandsFor()
        {
            var layer = new Mock<IStoreCommunicationDescriptions>();
            var collection = new LocalCommandCollection(layer.Object);

            var commands = new Mock<IMockCommandSetWithTaskReturn>();
            collection.Register(typeof(IMockCommandSetWithTaskReturn), commands.Object);

            var commandSet = collection.CommandsFor(typeof(IMockCommandSetWithTaskReturn));
            Assert.AreSame(commands.Object, commandSet);
        }
    }
}
