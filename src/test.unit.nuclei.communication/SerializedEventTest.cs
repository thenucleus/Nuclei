//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedEventTest : EqualityContractVerifierTest
    {
        private sealed class SerializedEventEqualityContractVerifier : EqualityContractVerifier<SerializedEvent>
        {
            private readonly SerializedEvent m_First = new SerializedEvent(new SerializedType("a", "a"), "a");

            private readonly SerializedEvent m_Second = new SerializedEvent(new SerializedType("b", "b"), "b");

            protected override SerializedEvent Copy(SerializedEvent original)
            {
                return new SerializedEvent(original.Type, original.MemberName);
            }

            protected override SerializedEvent FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override SerializedEvent SecondInstance
            {
                get
                {
                    return m_Second;
                }
            }

            protected override bool HasOperatorOverloads
            {
                get
                {
                    return true;
                }
            }
        }

        private sealed class SerializedEventHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<SerializedEvent> m_DistinctInstances
                = new List<SerializedEvent> 
                     {
                        new SerializedEvent(new SerializedType("a", "a"), "a"),
                        new SerializedEvent(new SerializedType("b", "b"), "b"),
                        new SerializedEvent(new SerializedType("c", "c"), "c"),
                        new SerializedEvent(new SerializedType("a", "a"), "d"),
                        new SerializedEvent(new SerializedType("b", "b"), "e"),
                        new SerializedEvent(new SerializedType("c", "c"), "f"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SerializedEventHashcodeContractVerfier m_HashcodeVerifier = new SerializedEventHashcodeContractVerfier();

        private readonly SerializedEventEqualityContractVerifier m_EqualityVerifier = new SerializedEventEqualityContractVerifier();

        protected override HashcodeContractVerifier HashContract
        {
            get
            {
                return m_HashcodeVerifier;
            }
        }

        protected override IEqualityContractVerifier EqualityContract
        {
            get
            {
                return m_EqualityVerifier;
            }
        }
    }
}
