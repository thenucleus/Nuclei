//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ObjectSerializerStorageTest
    {
        private interface IMockBase
        {
        }

        private interface IMockDerived : IMockBase
        {
        }

        private sealed class MockBase : IMockBase
        {
        }

        private class MockDerived : IMockDerived
        {
        }

        private sealed class MockMoreDerived : MockDerived
        {
        }

        private sealed class MockSerializer : ISerializeObjectData
        {
            private readonly Type m_Type;

            public MockSerializer(Type type)
            {
                m_Type = type;
            }

            public Type TypeToSerialize
            {
                get
                {
                    return m_Type;
                }
            }

            public object Serialize(object input)
            {
                return null;
            }

            public object Deserialize(object data)
            {
                return null;
            }
        }

        [Test]
        public void Add()
        {
            var storage = new ObjectSerializerStorage();
            
            var serializer = new MockSerializer(typeof(MockDerived));
            storage.Add(serializer);

            Assert.IsTrue(storage.HasSerializerFor(typeof(MockDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockMoreDerived)));
            Assert.IsFalse(storage.HasSerializerFor(typeof(MockBase)));
            Assert.IsFalse(storage.HasSerializerFor(typeof(IMockBase)));
            Assert.IsFalse(storage.HasSerializerFor(typeof(IMockDerived)));
            Assert.AreSame(serializer, storage.SerializerFor(typeof(MockDerived)));
            Assert.AreSame(serializer, storage.SerializerFor(typeof(MockMoreDerived)));
        }

        [Test]
        public void AddWithExistingSerializerForSameType()
        {
            var storage = new ObjectSerializerStorage();

            var serializer = new MockSerializer(typeof(MockDerived));
            storage.Add(serializer);

            var otherSerializer = new MockSerializer(typeof(MockDerived));
            Assert.Throws<DuplicateObjectSerializerException>(() => storage.Add(otherSerializer));
        }

        [Test]
        public void AddClassSerializerWithMoreDerivedClassSerializersExisting()
        {
            var storage = new ObjectSerializerStorage();

            var derivedSerializer = new MockSerializer(typeof(MockDerived));
            storage.Add(derivedSerializer);

            var baseSerializer = new MockSerializer(typeof(MockBase));
            storage.Add(baseSerializer);

            Assert.IsTrue(storage.HasSerializerFor(typeof(MockDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockMoreDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockBase)));
            Assert.IsFalse(storage.HasSerializerFor(typeof(IMockBase)));
            Assert.IsFalse(storage.HasSerializerFor(typeof(IMockDerived)));
            Assert.AreSame(baseSerializer, storage.SerializerFor(typeof(MockBase)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockDerived)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockMoreDerived)));
        }

        [Test]
        public void AddClassSerializerWithLessDerivedClassSerializersExisting()
        {
            var storage = new ObjectSerializerStorage();

            var baseSerializer = new MockSerializer(typeof(MockBase));
            storage.Add(baseSerializer);

            var derivedSerializer = new MockSerializer(typeof(MockDerived));
            storage.Add(derivedSerializer);

            Assert.IsTrue(storage.HasSerializerFor(typeof(MockDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockMoreDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockBase)));
            Assert.IsFalse(storage.HasSerializerFor(typeof(IMockBase)));
            Assert.IsFalse(storage.HasSerializerFor(typeof(IMockDerived)));
            Assert.AreSame(baseSerializer, storage.SerializerFor(typeof(MockBase)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockDerived)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockMoreDerived)));
        }

        [Test]
        public void AddInterfaceSerializerWithMoreDerivedInterfaceSerializersExisting()
        {
            var storage = new ObjectSerializerStorage();

            var derivedSerializer = new MockSerializer(typeof(IMockDerived));
            storage.Add(derivedSerializer);

            var baseSerializer = new MockSerializer(typeof(IMockBase));
            storage.Add(baseSerializer);

            Assert.IsTrue(storage.HasSerializerFor(typeof(MockDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockMoreDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockDerived)));
            Assert.AreSame(baseSerializer, storage.SerializerFor(typeof(MockBase)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockDerived)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockMoreDerived)));
        }

        [Test]
        public void AddInterfaceSerializerWithLessDerivedInterfaceSerializersExisting()
        {
            var storage = new ObjectSerializerStorage();

            var baseSerializer = new MockSerializer(typeof(IMockBase));
            storage.Add(baseSerializer);

            var derivedSerializer = new MockSerializer(typeof(IMockDerived));
            storage.Add(derivedSerializer);

            Assert.IsTrue(storage.HasSerializerFor(typeof(MockDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockMoreDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockDerived)));
            Assert.AreSame(baseSerializer, storage.SerializerFor(typeof(MockBase)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockDerived)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockMoreDerived)));
        }

        [Test]
        public void AddInterfaceSerializerWithMoreDerivedClassSerializersExisting()
        {
            var storage = new ObjectSerializerStorage();

            var derivedSerializer = new MockSerializer(typeof(MockDerived));
            storage.Add(derivedSerializer);

            var baseSerializer = new MockSerializer(typeof(IMockBase));
            storage.Add(baseSerializer);

            Assert.IsTrue(storage.HasSerializerFor(typeof(MockDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockMoreDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockDerived)));
            Assert.AreSame(baseSerializer, storage.SerializerFor(typeof(MockBase)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockDerived)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockMoreDerived)));
        }

        [Test]
        public void AddClassSerializerWithLessDerivedInterfaceSerializersExisting()
        {
            var storage = new ObjectSerializerStorage();

            var baseSerializer = new MockSerializer(typeof(IMockBase));
            storage.Add(baseSerializer);

            var derivedSerializer = new MockSerializer(typeof(MockDerived));
            storage.Add(derivedSerializer);

            Assert.IsTrue(storage.HasSerializerFor(typeof(MockDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockMoreDerived)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(MockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockBase)));
            Assert.IsTrue(storage.HasSerializerFor(typeof(IMockDerived)));
            Assert.AreSame(baseSerializer, storage.SerializerFor(typeof(MockBase)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockDerived)));
            Assert.AreSame(derivedSerializer, storage.SerializerFor(typeof(MockMoreDerived)));
        }
    }
}
