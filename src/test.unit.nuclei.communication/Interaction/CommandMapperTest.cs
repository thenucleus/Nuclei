//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CommandMapperTest
    {
        private interface ICommandInterface : ICommandSet
        {
            Task<int> InterfaceMethod();

            Task<int> InterfaceMethod(int p1);

            Task<int> InterfaceMethod(int p1, int p2);

            Task<int> InterfaceMethod(int p1, int p2, int p3);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11, int p12);

            Task<int> InterfaceMethod(int p1, int p2, int p3, int p4, int p5, int p6, int p7, int p8, int p9, int p10, int p11, int p12, int p13);

            Task<int> InterfaceMethod(
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
                int p14);

            Task<int> InterfaceMethod(
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
                int p15);
        }

        private sealed class CommandInstance
        {
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
        }

        private static CommandMap CreateCommandMap()
        {
            var instance = new CommandInstance();

            var mapper = CommandMapper<ICommandInterface>.Create();
            mapper.From(c => c.InterfaceMethod())
                .To(() => instance.InstanceMethod());
            mapper.From<int>((c, p1) => c.InterfaceMethod(p1))
                .To<int>(p1 => instance.InstanceMethod(p1));
            mapper.From<int, int>((c, p1, p2) => c.InterfaceMethod(p1, p2))
                .To<int, int>((p1, p2) => instance.InstanceMethod(p1, p2));
            mapper.From<int, int, int>((c, p1, p2, p3) => c.InterfaceMethod(p1, p2, p3))
                .To<int, int, int>((p1, p2, p3) => instance.InstanceMethod(p1, p2, p3));
            mapper.From<int, int, int, int>((c, p1, p2, p3, p4) => c.InterfaceMethod(p1, p2, p3, p4))
                .To<int, int, int, int>((p1, p2, p3, p4) => instance.InstanceMethod(p1, p2, p3, p4));
            mapper.From<int, int, int, int, int>((c, p1, p2, p3, p4, p5) => c.InterfaceMethod(p1, p2, p3, p4, p5))
                .To<int, int, int, int, int>((p1, p2, p3, p4, p5) => instance.InstanceMethod(p1, p2, p3, p4, p5));
            mapper.From<int, int, int, int, int, int>((c, p1, p2, p3, p4, p5, p6) => c.InterfaceMethod(p1, p2, p3, p4, p5, p6))
                .To<int, int, int, int, int, int>((p1, p2, p3, p4, p5, p6) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6));
            mapper.From<int, int, int, int, int, int, int>((c, p1, p2, p3, p4, p5, p6, p7) => c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7))
                .To<int, int, int, int, int, int, int>((p1, p2, p3, p4, p5, p6, p7) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7));
            mapper.From<int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8) => c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8))
                .To<int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8));
            mapper.From<int, int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8, p9) => c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9))
                .To<int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9));
            mapper.From<int, int, int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10) => c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10))
                .To<int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10));
            mapper.From<int, int, int, int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11) => c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11))
                .To<int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11));
            mapper.From<int, int, int, int, int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12) => c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12))
                .To<int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12) => instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12));
            mapper.From<int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13) =>
                    c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13))
                .To<int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13) =>
                    instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13));
            mapper.From<int, int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14) =>
                    c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14))
                .To<int, int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14) =>
                    instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14));
            mapper.From<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (c, p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15) =>
                    c.InterfaceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15))
                .To<int, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(
                (p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15) =>
                    instance.InstanceMethod(p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15));

            return mapper.ToMap();
        }

        [Test]
        public void CreateWithNonCommandInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => CommandMapper<InteractionExtensionsTest.IMockCommandSetWithMethodWithNonSerializableParameter>.Create());
        }

        [Test]
        public void ToMapWithMissingMethods()
        {
            var instance = new CommandInstance();

            var mapper = CommandMapper<ICommandInterface>.Create();
            mapper.From(c => c.InterfaceMethod())
                .To(() => instance.InstanceMethod());

            Assert.Throws<CommandMethodNotMappedException>(() => mapper.ToMap());
        }

        [Test]
        public void From()
        {
            var map = CreateCommandMap();

            foreach (var method in typeof(ICommandInterface).GetMethods())
            {
                var id = CommandId.Create(method);
                Assert.AreEqual(1, map.Definitions.Count(m => m.Id.Equals(id)));
            }
        }
    }
}
