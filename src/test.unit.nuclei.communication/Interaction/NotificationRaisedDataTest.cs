//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationRaisedDataTest
    {
        [Test]
        public void Create()
        {
            var data = new NotificationData(typeof(Process), "Exited");
            var args = new EventArgs();

            var eventRaisedData = new NotificationRaisedData(data, args);
            Assert.AreSame(data, eventRaisedData.Notification);
            Assert.AreSame(args, eventRaisedData.EventArgs);
        }
    }
}
