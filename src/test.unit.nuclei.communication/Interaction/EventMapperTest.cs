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
    public sealed class EventMapperTest
    {
        [Test]
        public void GenerateHandler()
        {
            NotificationDefinition storedDefinition = null;
            Action<NotificationDefinition> storeDefinition = d => storedDefinition = d;
            var id = new NotificationId("a");
            var mapper = new EventMapper(storeDefinition, id);

            var handler = mapper.GenerateHandler();

            Assert.IsNotNull(storedDefinition);
            Assert.AreEqual(id, storedDefinition.Id);

            var args = new EventArgs();
            var wasActionInvoked = false;
            Action<NotificationId, EventArgs> action = 
                (n, a) =>
                {
                    wasActionInvoked = true;
                    Assert.AreEqual(id, n);
                    Assert.AreSame(args, a);
                };
            storedDefinition.OnNotification(action);

            var obj = new object();
            handler(obj, args);

            Assert.IsTrue(wasActionInvoked);
        }

        [Test]
        public void GenerateTypedHandler()
        {
            NotificationDefinition storedDefinition = null;
            Action<NotificationDefinition> storeDefinition = d => storedDefinition = d;
            var id = new NotificationId("a");
            var mapper = new EventMapper(storeDefinition, id);

            var handler = mapper.GenerateHandler<EventArgs>();

            Assert.IsNotNull(storedDefinition);
            Assert.AreEqual(id, storedDefinition.Id);

            var args = new EventArgs();
            var wasActionInvoked = false;
            Action<NotificationId, EventArgs> action =
                (n, a) =>
                {
                    wasActionInvoked = true;
                    Assert.AreEqual(id, n);
                    Assert.AreSame(args, a);
                };
            storedDefinition.OnNotification(action);

            var obj = new object();
            handler(obj, args);

            Assert.IsTrue(wasActionInvoked);
        }
    }
}
