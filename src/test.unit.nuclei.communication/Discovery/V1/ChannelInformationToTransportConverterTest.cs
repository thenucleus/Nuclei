//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Discovery.V1
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class ChannelInformationToTransportConverterTest
    {
        [Test]
        public void ToVersioned()
        {
            var input = new ChannelInformation(
                EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                new Version(1, 2, 3, 4),
                new Uri("http://localhost/invalid"));

            var output = ChannelInformationToTransportConverter.ToVersioned(input);

            Assert.AreSame(input.Id, output.Id);
            Assert.AreSame(input.ProtocolVersion, output.ProtocolVersion);
            Assert.AreSame(input.Address, output.Address);
        }

        [Test]
        public void ToVersionedWithNullObject()
        {
            Assert.Throws<ArgumentNullException>(() => ChannelInformationToTransportConverter.ToVersioned(null));
        }

        [Test]
        public void FromVersioned()
        {
            var input = new VersionedChannelInformation
            {
                Id = EndpointIdExtensions.CreateEndpointIdForCurrentProcess(),
                ProtocolVersion = new Version(1, 2, 3, 4),
                Address = new Uri("http://localhost/invalid")
            };

            var output = ChannelInformationToTransportConverter.FromVersioned(input);

            Assert.AreSame(input.Id, output.Id);
            Assert.AreSame(input.ProtocolVersion, output.ProtocolVersion);
            Assert.AreSame(input.Address, output.Address);
        }

        [Test]
        public void FromVersionedWithNullObject()
        {
            Assert.Throws<ArgumentNullException>(() => ChannelInformationToTransportConverter.FromVersioned(null));
        }
    }
}
