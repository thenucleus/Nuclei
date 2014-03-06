//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;
using Nuclei.Nunit.Extensions;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationDataTest : EqualityContractVerifierTest
    {
        private sealed class NotificationDataEqualityContractVerifier : EqualityContractVerifier<NotificationData>
        {
            private readonly NotificationData m_First = new NotificationData(typeof(Process), "Exited");

            private readonly NotificationData m_Second = new NotificationData(typeof(EventLog), "EntryWritten");

            protected override NotificationData Copy(NotificationData original)
            {
                return new NotificationData(original.InterfaceType, original.EventName);
            }

            protected override NotificationData FirstInstance
            {
                get
                {
                    return m_First;
                }
            }

            protected override NotificationData SecondInstance
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

        private sealed class NotificationDataHashcodeContractVerfier : HashcodeContractVerifier
        {
            private readonly IEnumerable<NotificationData> m_DistinctInstances
                = new List<NotificationData> 
                     {
                        new NotificationData(typeof(Process), "Exited"),
                        new NotificationData(typeof(Process), "ErrorDataReceived"),
                        new NotificationData(typeof(EventLog), "EntryWritten"),
                        new NotificationData(typeof(EventLog), "Disposed"),
                     };

            protected override IEnumerable<int> GetHashcodes()
            {
                return m_DistinctInstances.Select(i => i.GetHashCode());
            }
        }

        private readonly NotificationDataHashcodeContractVerfier m_HashcodeVerifier = new NotificationDataHashcodeContractVerfier();

        private readonly NotificationDataEqualityContractVerifier m_EqualityVerifier = new NotificationDataEqualityContractVerifier();

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
            var type = typeof(Process);
            var method = "Exited";
            var data = new NotificationData(type, method);

            Assert.AreSame(type, data.InterfaceType);
            Assert.AreSame(method, data.EventName);
        }
    }
}
