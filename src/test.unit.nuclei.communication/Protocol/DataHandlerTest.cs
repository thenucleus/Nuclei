//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Microsoft.Reactive.Testing;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol
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
        public void ForwardData()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new DataHandler(systemDiagnostics);

            var sendingEndpoint = new EndpointId("sendingEndpoint");
            var filePath = GenerateNewTestFileName();
            var timeout = TimeSpan.FromSeconds(30);
            var task = handler.ForwardData(sendingEndpoint, filePath, timeout);
            Assert.IsFalse(task.IsCompleted);

            var text = "Hello world.";
            var data = new MemoryStream();
            var writer = new StreamWriter(data);
            writer.Write(text);
            writer.Flush();
            data.Position = 0;

            var msg = new DataTransferMessage 
                {
                    SendingEndpoint = sendingEndpoint,
                    Data = data,
                };
            handler.ProcessData(msg);

            task.Wait();
            Assert.IsTrue(task.IsCompleted);
            Assert.AreEqual(text, new StreamReader(task.Result.FullName).ReadToEnd());
        }

        [Test]
        public void ForwardDataWithDataReceiveTimeout()
        {
            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var scheduler = new TestScheduler();
            var handler = new DataHandler(systemDiagnostics, scheduler);

            var sendingEndpoint = new EndpointId("sendingEndpoint");
            var timeout = TimeSpan.FromSeconds(30);
            var task = handler.ForwardData(sendingEndpoint, GenerateNewTestFileName(), timeout);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            scheduler.Start();

            Assert.Throws<AggregateException>(task.Wait);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);
            Assert.IsTrue(task.IsFaulted);
        }

        [Test]
        public void ForwardDataWithDisconnectingEndpoint()
        {
            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new DataHandler(systemDiagnostics);

            var sendingEndpoint = new EndpointId("sendingEndpoint");
            var timeout = TimeSpan.FromSeconds(30);
            var task = handler.ForwardData(sendingEndpoint, GenerateNewTestFileName(), timeout);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            handler.OnEndpointSignedOff(sendingEndpoint);

            Assert.Throws<AggregateException>(task.Wait);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCanceled);
        }

        [Test]
        public void OnLocalChannelClosed()
        {
            var store = new Mock<IStoreInformationAboutEndpoints>();
            {
                store.Setup(s => s.CanCommunicateWithEndpoint(It.IsAny<EndpointId>()))
                    .Returns(false);
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var handler = new DataHandler(systemDiagnostics);

            var sendingEndpoint = new EndpointId("sendingEndpoint");
            var timeout = TimeSpan.FromSeconds(30);
            var task = handler.ForwardData(sendingEndpoint, GenerateNewTestFileName(), timeout);
            Assert.IsFalse(task.IsCompleted);
            Assert.IsFalse(task.IsCanceled);

            handler.OnLocalChannelClosed();

            Assert.Throws<AggregateException>(task.Wait);
            Assert.IsTrue(task.IsCompleted);
            Assert.IsTrue(task.IsCanceled);
        }
    }
}
