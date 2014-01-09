//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Moq;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DataHandlerTest
    {
        private static string GenerateNewTestFileName()
        {
            var path = Assembly.GetExecutingAssembly().LocalDirectoryPath();
            return Path.Combine(path, Path.GetRandomFileName());
        }

        [Test]
        public void ForwardResponse()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new DataHandler(systemDiagnostics);

            var sendingEndpoint = new EndpointId("sendingEndpoint");
            var filePath = GenerateNewTestFileName();
            var task = handler.ForwardData(sendingEndpoint, filePath);
            Assert.IsFalse(task.IsCompleted);

            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            writer.Flush();
            data.Position = 0;

            var receivingEndpoint = new EndpointId("receivingEndpoint");
            var msg = new DataTransferMessage 
                {
                    SendingEndpoint = sendingEndpoint,
                    ReceivingEndpoint = receivingEndpoint,
                    Data = data,
                };
            handler.ProcessData(msg);

            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(text, new StreamReader(task.Result.FullName).ReadToEnd());
        }

        [Test]
        public void ForwardResponseWithDisconnectingEndpoint()
        {
            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new DataHandler(systemDiagnostics);

            var sendingEndpoint = new EndpointId("sendingEndpoint");
            var messageId = new MessageId();
            var task = handler.ForwardData(sendingEndpoint, GenerateNewTestFileName());
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            handler.OnEndpointSignedOff(sendingEndpoint);

            Assert.Throws<AggregateException>(task.Wait);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCanceled);
        }
    }
}
