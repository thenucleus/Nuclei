//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Protocol.Messages
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DataDownloadRequestMessageTest
    {
        [Test]
        public void Create()
        {
            var sender = new EndpointId("sendingEndpoint");
            var token = new UploadToken();
            var msg = new DataDownloadRequestMessage(sender, token);

            Assert.IsNotNull(msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(token, msg.Token);
        }

        [Test]
        public void CreateWithId()
        {
            var sender = new EndpointId("sendingEndpoint");
            var id = new MessageId();
            var token = new UploadToken();
            var msg = new DataDownloadRequestMessage(sender, id, token);

            Assert.AreSame(id, msg.Id);
            Assert.AreSame(sender, msg.Sender);
            Assert.AreSame(token, msg.Token);
        }
    }
}
