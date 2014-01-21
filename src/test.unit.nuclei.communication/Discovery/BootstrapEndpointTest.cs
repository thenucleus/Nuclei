//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NUnit.Framework;

namespace Nuclei.Communication.Discovery
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
                Justification = "Unit tests do not need documentation.")]
    public sealed class BootstrapEndpointTest
    {
        [Test]
        public void DiscoveryVersions()
        {
            var versionedEndpoints = new List<Tuple<Version, Uri>>
                {
                    new Tuple<Version, Uri>(
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/v2")),
                    new Tuple<Version, Uri>(
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/v1")),
                    new Tuple<Version, Uri>(
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/v3")),
                };

            var endpoint = new BootstrapEndpoint(versionedEndpoints);
            var versions = endpoint.DiscoveryVersions();

            Assert.That(
                versions,
                Is.EquivalentTo(
                    versionedEndpoints
                        .Select(t => t.Item1)
                        .OrderBy(v => v)));
        }

        [Test]
        public void UriForVersionWithNullVersion()
        {
            var versionedEndpoints = new List<Tuple<Version, Uri>>
                {
                    new Tuple<Version, Uri>(
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/v2")),
                    new Tuple<Version, Uri>(
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/v1")),
                    new Tuple<Version, Uri>(
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/v3")),
                };

            var endpoint = new BootstrapEndpoint(versionedEndpoints);
            Assert.IsNull(endpoint.UriForVersion(null));
        }

        [Test]
        public void UriForVersionWithNonExistingVersion()
        {
            var versionedEndpoints = new List<Tuple<Version, Uri>>
                {
                    new Tuple<Version, Uri>(
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/v2")),
                    new Tuple<Version, Uri>(
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/v1")),
                    new Tuple<Version, Uri>(
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/v3")),
                };

            var endpoint = new BootstrapEndpoint(versionedEndpoints);
            Assert.IsNull(endpoint.UriForVersion(new Version(4, 0, 0, 0)));
            Assert.IsNull(endpoint.UriForVersion(new Version(1, 1, 0, 0)));
            Assert.IsNull(endpoint.UriForVersion(new Version(1, 0, 1, 0)));
            Assert.IsNull(endpoint.UriForVersion(new Version(1, 0, 0, 1)));
        }

        [Test]
        public void UriForVersion()
        {
            var versionedEndpoints = new List<Tuple<Version, Uri>>
                {
                    new Tuple<Version, Uri>(
                        new Version(2, 0, 0, 0), 
                        new Uri("http://localhost/v2")),
                    new Tuple<Version, Uri>(
                        new Version(1, 0, 0, 0), 
                        new Uri("http://localhost/v1")),
                    new Tuple<Version, Uri>(
                        new Version(3, 0, 0, 0), 
                        new Uri("http://localhost/v3")),
                };

            var endpoint = new BootstrapEndpoint(versionedEndpoints);
            var address = endpoint.UriForVersion(new Version(2, 0, 0, 0));
            Assert.AreEqual(versionedEndpoints[0].Item2, address);
        }
    }
}
