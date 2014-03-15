//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class EndpointDisconnectProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var endpointStorage = new Mock<IStoreEndpointApprovalState>();
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointDisconnectProcessAction(endpointStorage.Object, systemDiagnostics);
            Assert.AreEqual(typeof(EndpointDisconnectMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var endpoint = new EndpointId("a");
            var endpointStorage = new Mock<IStoreEndpointApprovalState>();
            {
                endpointStorage.Setup(e => e.TryRemoveEndpoint(It.IsAny<EndpointId>()))
                    .Callback<EndpointId>(e => Assert.AreSame(endpoint, e))
                    .Returns(true)
                    .Verifiable();
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new EndpointDisconnectProcessAction(endpointStorage.Object, systemDiagnostics);

            var disconnectMsg = new EndpointDisconnectMessage(endpoint);
            action.Invoke(disconnectMsg);

            endpointStorage.Verify(e => e.TryRemoveEndpoint(It.IsAny<EndpointId>()), Times.Once());
        }
    }
}
