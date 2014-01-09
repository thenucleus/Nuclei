//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedTypeTest : EqualityContractVerifierTest
    {
        private sealed class SerializedTypeEqualityContractVerifier : EqualityContractVerifier<SerializedType>
        {
            private readonly SerializedType m_First = ProxyExtensions.FromType(typeof(object)) as SerializedType;

            private readonly SerializedType m_Second = ProxyExtensions.FromType(typeof(ICommandSet)) as SerializedType;

            protected override SerializedType Copy(SerializedType original)
            {
                return new SerializedType(original.FullName, original.AssemblyQualifiedTypeName);
            }

            protected override SerializedType FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override SerializedType SecondInstance
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

        private sealed class SerializedTypeHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ISerializedType> m_DistinctInstances
                = new List<ISerializedType> 
                     {
                        ProxyExtensions.FromType(typeof(object)),
                        ProxyExtensions.FromType(typeof(string)),
                        ProxyExtensions.FromType(typeof(ICommandSet)),
                        ProxyExtensions.FromType(typeof(EndpointId)),
                        ProxyExtensions.FromType(typeof(ICommunicationLayer)),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SerializedTypeHashcodeContractVerfier m_HashcodeVerifier = new SerializedTypeHashcodeContractVerfier();

        private readonly SerializedTypeEqualityContractVerifier m_EqualityVerifier = new SerializedTypeEqualityContractVerifier();

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
