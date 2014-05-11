//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class ConnectionVerificationProcessActionTest
    {
        [Test]
        public void MessageTypeToProcess()
        {
            var endpoint = new EndpointId("id");
            SendMessage sendAction = (e, m, r) => { };
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            KeepAliveResponseCustomDataBuilder customData = o => o;

            var action = new ConnectionVerificationProcessAction(endpoint, sendAction, systemDiagnostics, customData);
            Assert.AreEqual(typeof(ConnectionVerificationMessage), action.MessageTypeToProcess);
        }

        [Test]
        public void Invoke()
        {
            var endpoint = new EndpointId("id");

            ICommunicationMessage storedMsg = null;
            SendMessage sendAction = 
                (e, m, r) =>
                {
                    storedMsg = m;
                };

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var data = "a";
            var responseData = "b";
            KeepAliveResponseCustomDataBuilder customData = 
                o =>
                {
                    Assert.AreSame(data, o);
                    return responseData;
                };

            var action = new ConnectionVerificationProcessAction(endpoint, sendAction, systemDiagnostics, customData);

            var id = new EndpointId("id");
            var msg = new ConnectionVerificationMessage(id, data);
            action.Invoke(msg);

            var responseMessage = storedMsg as ConnectionVerificationResponseMessage;
            Assert.IsNotNull(responseMessage);
            Assert.AreEqual(msg.Id, responseMessage.InResponseTo);
            Assert.AreSame(responseData, responseMessage.ResponseData);
        }
    }
}
