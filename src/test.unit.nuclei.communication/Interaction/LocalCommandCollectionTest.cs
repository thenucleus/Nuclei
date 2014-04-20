//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

            var mapping = new List<Tuple<CommandId, Delegate>>
                {
                    new Tuple<CommandId, Delegate>(
                        CommandId.Create(typeof(int).GetMethod("CompareTo")),
                        (Action)delegate { }),
                };
            var map = new CommandMap(mapping);
            collection.Register(map);

            Assert.IsTrue(collection.Any(pair => pair.Item1 == mapping[0].Item1));
        }

        [Test]
        public void RegisterWithExistingType()
        {
            var collection = new LocalCommandCollection();

            var mapping = new List<Tuple<CommandId, Delegate>>
                {
                    new Tuple<CommandId, Delegate>(
                        CommandId.Create(typeof(int).GetMethod("CompareTo")),
                        (Action)delegate { }),
                };
            var map = new CommandMap(mapping);
            collection.Register(map);
            Assert.AreEqual(1, collection.Count(pair => pair.Item1 == mapping[0].Item1));

            Assert.Throws<CommandAlreadyRegisteredException>(
                () => collection.Register(map));
            Assert.AreEqual(1, collection.Count(pair => pair.Item1 == mapping[0].Item1));
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

            var mapping = new List<Tuple<CommandId, Delegate>>
                {
                    new Tuple<CommandId, Delegate>(
                        CommandId.Create(typeof(int).GetMethod("CompareTo")),
                        (Action)delegate { }),
                };
            var map = new CommandMap(mapping);
            collection.Register(map);

            var commandSet = collection.CommandToInvoke(mapping[0].Item1);
            Assert.AreSame(mapping[0].Item2, commandSet);
        }
    }
}
