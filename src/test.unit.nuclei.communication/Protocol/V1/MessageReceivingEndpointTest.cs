//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1.DataObjects;
using Nuclei.Communication.Protocol.V1.DataObjects.Converters;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class MessageReceivingEndpointTest
    {
        [Test]
        public void AcceptMessage()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectData
                {
                    Sender = endpointId,
                };

            var endpoint = new MessageReceivingEndpoint(
                new IConvertCommunicationMessages[]
                    {
                        new EndpointDisconnectConverter(), 
                    }, 
                systemDiagnostics);
            endpoint.OnNewMessage += (s, e) => Assert.IsInstanceOf(typeof(EndpointDisconnectMessage), e.Message);

            endpoint.AcceptMessage(msg);
        }

        [Test]
        public void AcceptMessageThrowingException()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var endpointId = new EndpointId("id");
            var msg = new EndpointDisconnectData
            {
                Sender = endpointId,
            };

            var endpoint = new MessageReceivingEndpoint(
                new IConvertCommunicationMessages[]
                    {
                        new EndpointDisconnectConverter(), 
                    },
                systemDiagnostics);
            endpoint.OnNewMessage += 
                (s, e) => 
                { 
                    throw new Exception(); 
                };

            Assert.DoesNotThrow(() => endpoint.AcceptMessage(msg));
        }
    }
}
