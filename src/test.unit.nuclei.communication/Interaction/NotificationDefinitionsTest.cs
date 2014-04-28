//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class NotificationDefinitionsTest
    {
        [Test]
        public void ForwardToListeners()
        {
            var id = new NotificationId("a");
            var definition = new NotificationDefinition(id);

            var args = new EventArgs();
            var wasHandlerInvoked = false;
            Action<NotificationId, EventArgs> handler = 
                (n, e) =>
                {
                    wasHandlerInvoked = true;
                    Assert.AreEqual(id, n);
                    Assert.AreSame(args, e);
                };
            definition.OnNotification(handler);

            var obj = new object();
            definition.ForwardToListeners(obj, args);

            Assert.IsTrue(wasHandlerInvoked);
        }
    }
}
