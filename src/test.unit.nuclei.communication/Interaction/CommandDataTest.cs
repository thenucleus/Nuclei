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

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class CommandDataTest : EqualityContractVerifierTest
    {
        private sealed class CommandDataEqualityContractVerifier : EqualityContractVerifier<CommandData>
        {
            private readonly CommandData m_First = new CommandData(typeof(string), "CompareTo");

            private readonly CommandData m_Second = new CommandData(typeof(int), "CompareTo");

            protected override CommandData Copy(CommandData original)
            {
                return new CommandData(original.InterfaceType, original.MethodName);
            }

            protected override CommandData FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override CommandData SecondInstance
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

        private sealed class CommandDataHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<CommandData> m_DistinctInstances
                = new List<CommandData> 
                     {
                        new CommandData(typeof(double), "CompareTo"),
                        new CommandData(typeof(int), "CompareTo"),
                        new CommandData(typeof(string), "CompareTo"),
                        new CommandData(typeof(string), "Equals"),
                        new CommandData(typeof(object), "Equals"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly CommandDataHashcodeContractVerfier m_HashcodeVerifier = new CommandDataHashcodeContractVerfier();

        private readonly CommandDataEqualityContractVerifier m_EqualityVerifier = new CommandDataEqualityContractVerifier();

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

        [Test]
        public void Create()
        {
            var type = typeof(string);
            var method = "CompareTo";
            var data = new CommandData(type, method);

            Assert.AreSame(type, data.InterfaceType);
            Assert.AreSame(method, data.MethodName);
        }
    }
}
