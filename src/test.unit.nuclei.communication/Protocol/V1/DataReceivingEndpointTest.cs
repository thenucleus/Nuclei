//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;
using Nuclei.Communication.Protocol.V1.DataObjects;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Protocol.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class DataReceivingEndpointTest
    {
        [Test]
        public void AcceptStream()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var msg = new StreamData
                {
                    Data = new MemoryStream(),
                };

            var endpoint = new DataReceivingEndpoint(systemDiagnostics);
            endpoint.OnNewData += (s, e) => Assert.AreSame(msg.Data, e.Data.Data);

            endpoint.AcceptStream(msg);
        }

        [Test]
        public void AcceptStreamThrowingException()
        {
            var systemDiagnostics = new SystemDiagnostics((p, s) => { }, null);
            var msg = new StreamData
                {
                    Data = new MemoryStream(),
                };

            var endpoint = new DataReceivingEndpoint(systemDiagnostics);
            endpoint.OnNewData +=
                (s, e) =>
                {
                    throw new Exception();
                };

            Assert.DoesNotThrow(() => endpoint.AcceptStream(msg));
        }
    }
}
