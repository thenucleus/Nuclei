//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class MethodMapperTest
    {
        private sealed class CommandInstance
        {
            public static int StaticMethod()
            {
                return -1;
            }

            public int InstanceMethod()
            {
                return 0;
            }

            public int InstanceMethod(int p1)
            {
                return p1;
            }

            public int InstanceMethod(int p1, int p2)
            {
                return p1 + p2;
            }

            public int InstanceMethod(int p1, int p2, int p3)
            {
                return p1 + p2 + p3;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4)
            {
                return p1 + p2 + p3 + p4;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5)
            {
                return p1 + p2 + p3 + p4 + p5;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6)
            {
                return p1 + p2 + p3 + p4 + p5 + p6;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11, int p12)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12;
            }

            public int InstanceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11, int p12, int p13)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13;
            }

            public int InstanceMethod(
                int p1,
                int p2,
                int p3,
                int p4,
                int p5,
                int p6,
                int p7,
                int p8,
                int p9,
                int p10,
                int p11,
                int p12,
                int p13,
                int p14)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14;
            }

            public int InstanceMethod(
                int p1,
                int p2,
                int p3,
                int p4,
                int p5,
                int p6,
                int p7,
                int p8,
                int p9,
                int p10,
                int p11,
                int p12,
                int p13,
                int p14,
                int p15)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15;
            }

            public int InstanceMethod(
                int p1,
                int p2,
                int p3,
                int p4,
                int p5,
                int p6,
                int p7,
                int p8,
                int p9,
                int p10,
                int p11,
                int p12,
                int p13,
                int p14,
                int p15,
                int p16)
            {
                return p1 + p2 + p3 + p4 + p5 + p6 + p7 + p8 + p9 + p10 + p11 + p12 + p13 + p14 + p15 + p16;
            }
        }

        [Test]
        public void ToWithMethodCallExpressionWithMissingInstance()
        {
            Action<CommandDefinition> store = d => { };
            var id = new CommandId("a");
            var mapper = new MethodMapper(store, id, typeof(void), new ParameterInfo[0]);
            Assert.Throws<InvalidCommandMethodExpressionException>(() => mapper.To((Version v) => v.GetHashCode()));
        }

        [Test]
        public void ToWithInvalidReturnType()
        {
            Action<CommandDefinition> store = d => { };
            var id = new CommandId("a");
            var mapper = new MethodMapper(store, id, typeof(void), new ParameterInfo[0]);

            var version = new Version(1, 0);
            Assert.Throws<InvalidCommandMappingException>(() => mapper.To(() => version.GetHashCode()));
        }

        [Test]
        public void ToWithNonMappedCommandParameter()
        {
            Action<CommandDefinition> store = d => { };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(bool),
                typeof(Version).GetMethod("Equals", new[] { typeof(object) }).GetParameters());

            var version = new Version(1, 0);
            Assert.Throws<NonMappedCommandParameterException>(() => mapper.To((Version v) => version.Equals(v)));
        }

        [Test]
        public void ToWithNonMappedInstanceParameter()
        {
            Action<CommandDefinition> store = d => { };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(bool),
                new ParameterInfo[0]);

            var version = new Version(1, 0);
            Assert.Throws<NonMappedCommandParameterException>(() => mapper.To((Version v) => version.Equals(v)));
        }

        [Test]
        public void ToWithStaticParameterlessMethod()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                new ParameterInfo[0]);

            mapper.To(() => CommandInstance.StaticMethod());
            Assert.IsNotNull(definition);
            Assert.AreEqual(-1, definition.Invoke(new CommandParameterValueMap[0]));
        }

        [Test]
        public void ToWithParameterlessMethod()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                new ParameterInfo[0]);

            var instance = new CommandInstance();
            mapper.To(() => instance.InstanceMethod());
            Assert.IsNotNull(definition);
            Assert.AreEqual(0, definition.Invoke(new CommandParameterValueMap[0]));
        }

        [Test]
        public void ToWithMethodWithOneParameter()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int)
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int>(p1 => instance.InstanceMethod(p1));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 1).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithTwoParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int>((p1, p2) => instance.InstanceMethod(p1, p2));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 2).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithThreeParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int>((p1, p2, p3) => instance.InstanceMethod(p1, p2, p3));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 3).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithFourParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int>((p1, p2, p3, p4) => instance.InstanceMethod(p1, p2, p3, p4));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 4).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithFiveParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int>((p1, p2, p3, p4, p5) => instance.InstanceMethod(p1, p2, p3, p4, p5));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 5).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithSixParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int>((p1, p2, p3, p4, p5, p6) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 6).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithSevenParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int>((p1, p2, p3, p4, p5, p6, p7) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 7).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithp8Parameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 8).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithNineParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 9).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithTenParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 10).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p10", CommandParameterOrigin.FromCommand), 
                            values[9]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithElevenParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 11).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p10", CommandParameterOrigin.FromCommand), 
                            values[9]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p11", CommandParameterOrigin.FromCommand), 
                            values[10]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithTwelveParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 12).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p10", CommandParameterOrigin.FromCommand), 
                            values[9]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p11", CommandParameterOrigin.FromCommand), 
                            values[10]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p12", CommandParameterOrigin.FromCommand), 
                            values[11]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithThirteenParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13) => 
                    instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 13).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p10", CommandParameterOrigin.FromCommand), 
                            values[9]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p11", CommandParameterOrigin.FromCommand), 
                            values[10]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p12", CommandParameterOrigin.FromCommand), 
                            values[11]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p13", CommandParameterOrigin.FromCommand), 
                            values[12]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithFourteenParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14) =>
                    instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 14).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p10", CommandParameterOrigin.FromCommand), 
                            values[9]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p11", CommandParameterOrigin.FromCommand), 
                            values[10]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p12", CommandParameterOrigin.FromCommand), 
                            values[11]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p13", CommandParameterOrigin.FromCommand), 
                            values[12]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p14", CommandParameterOrigin.FromCommand), 
                            values[13]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithFifteenParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15) =>
                    instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 15).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p10", CommandParameterOrigin.FromCommand), 
                            values[9]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p11", CommandParameterOrigin.FromCommand), 
                            values[10]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p12", CommandParameterOrigin.FromCommand), 
                            values[11]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p13", CommandParameterOrigin.FromCommand), 
                            values[12]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p14", CommandParameterOrigin.FromCommand), 
                            values[13]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p15", CommandParameterOrigin.FromCommand), 
                            values[14]), 
                    }));
        }

        [Test]
        public void ToWithMethodWithSixteenParameters()
        {
            CommandDefinition definition = null;
            Action<CommandDefinition> store =
                d =>
                {
                    definition = d;
                };
            var id = new CommandId("a");
            var mapper = new MethodMapper(
                store,
                id,
                typeof(int),
                typeof(CommandInstance).GetMethod(
                    "InstanceMethod",
                    new[]
                        {
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                            typeof(int),
                        }).GetParameters());

            var instance = new CommandInstance();
            mapper.To<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16) =>
                    instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16));
            Assert.IsNotNull(definition);

            var values = Enumerable.Range(1, 16).ToArray();
            Assert.AreEqual(
                values.Sum(),
                definition.Invoke(new[]
                    {
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p1", CommandParameterOrigin.FromCommand), 
                            values[0]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p2", CommandParameterOrigin.FromCommand), 
                            values[1]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p3", CommandParameterOrigin.FromCommand), 
                            values[2]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p4", CommandParameterOrigin.FromCommand), 
                            values[3]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p5", CommandParameterOrigin.FromCommand), 
                            values[4]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p6", CommandParameterOrigin.FromCommand), 
                            values[5]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p7", CommandParameterOrigin.FromCommand), 
                            values[6]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p8", CommandParameterOrigin.FromCommand), 
                            values[7]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p9", CommandParameterOrigin.FromCommand), 
                            values[8]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p10", CommandParameterOrigin.FromCommand), 
                            values[9]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p11", CommandParameterOrigin.FromCommand), 
                            values[10]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p12", CommandParameterOrigin.FromCommand), 
                            values[11]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p13", CommandParameterOrigin.FromCommand), 
                            values[12]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p14", CommandParameterOrigin.FromCommand), 
                            values[13]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p15", CommandParameterOrigin.FromCommand), 
                            values[14]), 
                        new CommandParameterValueMap(
                            new CommandParameterDefinition(typeof(int), "p16", CommandParameterOrigin.FromCommand), 
                            values[15]), 
                    }));
        }
    }
}
