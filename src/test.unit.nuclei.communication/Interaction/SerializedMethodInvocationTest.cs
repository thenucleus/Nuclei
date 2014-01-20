//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Nuclei.Nunit.Extensions;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class SerializedMethodInvocationTest : EqualityContractVerifierTest
    {
        // A fake command set interface to invoke methods on
        public interface IMockCommandSet : ICommandSet
        {
            Task MethodWithoutReturnValue(int someNumber);

            Task OtherMethodWithoutReturnValue(int otherNumber);

            Task<int> MethodWithReturnValue(int someNumber);

            Task<int> OtherMethodWithReturnValue(int otherNumber);
        }

        private sealed class SerializedMethodEqualityContractVerifier : EqualityContractVerifier<SerializedMethodInvocation>
        {
            private readonly SerializedMethodInvocation m_First
                = ProxyExtensions.FromMethodInfo(
                    typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue"), 
                    new object[] { 2 }) as SerializedMethodInvocation;

            private readonly SerializedMethodInvocation m_Second
                 = ProxyExtensions.FromMethodInfo(
                    typeof(IMockCommandSet).GetMethod("OtherMethodWithoutReturnValue"),
                    new object[] { 2 }) as SerializedMethodInvocation;

            protected override SerializedMethodInvocation Copy(SerializedMethodInvocation original)
            {
                return new SerializedMethodInvocation(original.Type, original.MemberName, original.Parameters);
            }

            protected override SerializedMethodInvocation FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override SerializedMethodInvocation SecondInstance
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

        private sealed class SerializedMethodHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<ISerializedMethodInvocation> m_DistinctInstances
                = new List<ISerializedMethodInvocation> 
                     {
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("MethodWithoutReturnValue"), new object[] { 2 }),
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("OtherMethodWithoutReturnValue"), new object[] { 2 }),
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("MethodWithReturnValue"), new object[] { 2 }),
                        ProxyExtensions.FromMethodInfo(
                            typeof(IMockCommandSet).GetMethod("OtherMethodWithReturnValue"), new object[] { 2 }),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly SerializedMethodHashcodeContractVerfier m_HashcodeVerifier = new SerializedMethodHashcodeContractVerfier();

        private readonly SerializedMethodEqualityContractVerifier m_EqualityVerifier = new SerializedMethodEqualityContractVerifier();

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
