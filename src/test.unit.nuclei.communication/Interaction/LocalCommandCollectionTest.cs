//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

            var map = new[]
                {
                    new CommandDefinition(
                        CommandId.Create(typeof(int).GetMethod("CompareTo")),
                        new[]
                            {
                                new CommandParameterDefinition(typeof(int), "other", CommandParameterOrigin.FromCommand), 
                            }, 
                        false,
                        (Action)delegate { }), 
                };
            collection.Register(map);

            Assert.IsTrue(collection.Any(id => id == map[0].Id));
        }

        [Test]
        public void RegisterWithExistingType()
        {
            var collection = new LocalCommandCollection();

            var map = new[]
                {
                    new CommandDefinition(
                        CommandId.Create(typeof(int).GetMethod("CompareTo")),
                        new[]
                            {
                                new CommandParameterDefinition(typeof(int), "other", CommandParameterOrigin.FromCommand), 
                            }, 
                        false,
                        (Action)delegate { }), 
                };
            collection.Register(map);
            Assert.AreEqual(1, collection.Count(id => id == map[0].Id));

            Assert.Throws<CommandAlreadyRegisteredException>(
                () => collection.Register(map));
            Assert.AreEqual(1, collection.Count(id => id == map[0].Id));
        }

        [Test]
        public void CommandsForWithUnknownType()
        {
            var collection = new LocalCommandCollection();
            var id = CommandId.Create(typeof(int).GetMethod("CompareTo"));
            Assert.Throws<UnknownCommandException>(() => collection.CommandToInvoke(id));
        }

        [Test]
        public void CommandsFor()
        {
            var collection = new LocalCommandCollection();

            var map = new[]
                {
                    new CommandDefinition(
                        CommandId.Create(typeof(int).GetMethod("CompareTo")),
                        new[]
                            {
                                new CommandParameterDefinition(typeof(int), "other", CommandParameterOrigin.FromCommand), 
                            }, 
                        false,
                        (Action)delegate { }), 
                };
            collection.Register(map);

            var commandSet = collection.CommandToInvoke(map[0].Id);
            Assert.AreSame(map[0], commandSet);
        }
    }
}
