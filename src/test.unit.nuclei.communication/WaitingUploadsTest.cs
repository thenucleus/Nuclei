//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol;
using NUnit.Framework;

namespace Nuclei.Communication
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class WaitingUploadsTest
    {
        [Test]
        public void Register()
        {
            var file = @"c:\temp\myfile.txt";

            var uploads = new WaitingUploads();
            var token = uploads.Register(file);
            Assert.IsTrue(uploads.HasRegistration(token));
            Assert.AreEqual(file, uploads.Deregister(token));
        }

        [Test]
        public void RegisterWithEmptyPath()
        {
            var uploads = new WaitingUploads();
            Assert.Throws<ArgumentException>(() => uploads.Register(string.Empty));
        }

        [Test]
        public void Reregister()
        {
            var token = new UploadToken();
            var file = @"c:\temp\myfile.txt";

            var uploads = new WaitingUploads();
            uploads.Reregister(token, file);

            Assert.IsTrue(uploads.HasRegistration(token));
            Assert.AreEqual(file, uploads.Deregister(token));
        }

        [Test]
        public void ReregisterWithEmptyPath()
        {
            var token = new UploadToken();
            var uploads = new WaitingUploads();
            Assert.Throws<ArgumentException>(() => uploads.Reregister(token, string.Empty));
        }

        [Test]
        public void ReregisterWithExistingToken()
        {
            var token = new UploadToken();
            var file = @"c:\temp\myfile.txt";

            var uploads = new WaitingUploads();
            uploads.Reregister(token, file);
            Assert.Throws<UploadNotDeregisteredException>(() => uploads.Reregister(token, @"c:\temp\otherfile.txt"));
        }

        [Test]
        public void DeregisterWithUnknownToken()
        {
            var token = new UploadToken();
            var uploads = new WaitingUploads();

            Assert.Throws<FileRegistrationNotFoundException>(() => uploads.Deregister(token));
        }
    }
}
