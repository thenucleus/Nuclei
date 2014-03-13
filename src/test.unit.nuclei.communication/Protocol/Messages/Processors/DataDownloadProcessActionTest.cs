//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Moq;
using Nuclei.Diagnostics;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DataDownloadProcessActionTest
    {
        [Test]
        public void Invoke()
        {
            var uploads = new WaitingUploads();
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("other"));
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) => 
                        {
                            Assert.IsInstanceOf<SuccessMessage>(m);
                        })
                    .Verifiable();
                layer.Setup(l => l.UploadData(
                        It.IsAny<EndpointId>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<TaskScheduler>()))
                    .Returns(Task.Factory.StartNew(
                        () => { },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new DataDownloadProcessAction(
                uploads,
                layer.Object,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            var path = @"c:\temp\myfile.txt";
            var token = uploads.Register(path);

            var msg = new DataDownloadRequestMessage(
                EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                token);
            action.Invoke(msg);

            Assert.IsFalse(uploads.HasRegistration(token));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            layer.Verify(
                l => l.UploadData(
                    It.IsAny<EndpointId>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TaskScheduler>()),
                Times.Once());
        }

        [Test]
        public void InvokeWithoutFileRegistration()
        {
            var uploads = new WaitingUploads();
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("other"));
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            Assert.IsInstanceOf<FailureMessage>(m);
                        })
                    .Verifiable();
                layer.Setup(l => l.UploadData(
                        It.IsAny<EndpointId>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<TaskScheduler>()))
                    .Returns(Task.Factory.StartNew(
                        () => { },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new DataDownloadProcessAction(
                uploads,
                layer.Object,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            var token = new UploadToken();

            var msg = new DataDownloadRequestMessage(
                EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                token);
            action.Invoke(msg);

            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            layer.Verify(
                l => l.UploadData(
                    It.IsAny<EndpointId>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TaskScheduler>()),
                Times.Never());
        }

        [Test]
        public void InvokeWithFailedUpload()
        {
            var uploads = new WaitingUploads();
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("other"));
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            Assert.IsInstanceOf<FailureMessage>(m);
                        })
                    .Verifiable();
                layer.Setup(l => l.UploadData(
                        It.IsAny<EndpointId>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<TaskScheduler>()))
                    .Returns(Task.Factory.StartNew(
                        () => { throw new Exception(); },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new DataDownloadProcessAction(
                uploads,
                layer.Object,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            var path = @"c:\temp\myfile.txt";
            var token = uploads.Register(path);

            var msg = new DataDownloadRequestMessage(
                EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                token);
            action.Invoke(msg);

            Assert.IsTrue(uploads.HasRegistration(token));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Once());
            layer.Verify(
                l => l.UploadData(
                    It.IsAny<EndpointId>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TaskScheduler>()),
                Times.Once());
        }

        [Test]
        public void InvokeWithFailingMessageChannel()
        {
            var uploads = new WaitingUploads();
            var layer = new Mock<IProtocolLayer>();
            {
                layer.Setup(l => l.Id)
                    .Returns(new EndpointId("other"));
                layer.Setup(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()))
                    .Callback<EndpointId, ICommunicationMessage>(
                        (e, m) =>
                        {
                            throw new Exception();
                        })
                    .Verifiable();
                layer.Setup(l => l.UploadData(
                        It.IsAny<EndpointId>(),
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<TaskScheduler>()))
                    .Returns(Task.Factory.StartNew(
                        () => { },
                        new CancellationToken(),
                        TaskCreationOptions.None,
                        new CurrentThreadTaskScheduler()))
                    .Verifiable();
            }

            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);

            var action = new DataDownloadProcessAction(
                uploads,
                layer.Object,
                systemDiagnostics,
                new CurrentThreadTaskScheduler());

            var path = @"c:\temp\myfile.txt";
            var token = uploads.Register(path);

            var msg = new DataDownloadRequestMessage(
                EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                token);
            action.Invoke(msg);

            Assert.IsFalse(uploads.HasRegistration(token));
            layer.Verify(l => l.SendMessageTo(It.IsAny<EndpointId>(), It.IsAny<ICommunicationMessage>()), Times.Exactly(2));
            layer.Verify(
                l => l.UploadData(
                    It.IsAny<EndpointId>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<TaskScheduler>()),
                Times.Once());
        }
    }
}
