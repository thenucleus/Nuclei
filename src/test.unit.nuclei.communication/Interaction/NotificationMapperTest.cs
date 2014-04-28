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
    public sealed class NotificationMapperTest
    {
        public interface IMockNotificationSet : INotificationSet
        {
            event EventHandler OnMyEvent;
        }

        public sealed class MockNotificationSet
        {
            public event EventHandler OnMyEvent;

            public void RaiseOnMyEvent(EventArgs eventArgs)
            {
                var local = OnMyEvent;
                if (local != null)
                {
                    local(this, eventArgs);
                }
            }
        }

        [Test]
        public void FromWithNonEventAction()
        {
            var mapper = NotificationMapper<IMockNotificationSet>.Create();
            Assert.Throws<InvalidNotificationMethodExpressionException>(() => mapper.From(s => s.GetType()));
        }

        [Test]
        public void FromWithEventRegistration()
        {
            var mapper = NotificationMapper<IMockNotificationSet>.Create();
            mapper.From(s => s.OnMyEvent += null)
                .GenerateHandler();

            var map = mapper.ToMap();
            Assert.AreEqual(1, map.Definitions.Length);

            var def = map.Definitions[0];

            var id = NotificationId.Create(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            Assert.AreEqual(id, def.Id);
        }

        [Test]
        public void FromWithEventUnregistration()
        {
            var mapper = NotificationMapper<IMockNotificationSet>.Create();
            mapper.From(s => s.OnMyEvent -= null)
                .GenerateHandler();

            var map = mapper.ToMap();
            Assert.AreEqual(1, map.Definitions.Length);

            var def = map.Definitions[0];

            var id = NotificationId.Create(typeof(IMockNotificationSet).GetEvent("OnMyEvent"));
            Assert.AreEqual(id, def.Id);
        }

        public void ToMapWithMissingRegistration()
        {
            var mapper = NotificationMapper<IMockNotificationSet>.Create();
            Assert.Throws<NotificationEventNotMappedException>(() => mapper.ToMap());
        }
    }
}
