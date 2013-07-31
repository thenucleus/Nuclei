//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Moq;
using Nuclei.Configuration;
using NUnit.Framework;

namespace Nuclei.Diagnostics.Logging
{
    // There is little point in adding a contract verifier for IEquatable<DebugLogTemplate> here because there
    // is really only one implementation of that type and they're pretty much all the same
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
    public sealed class DebugLogTemplateTest
    {
        private static DateTimeOffset GetDefaultDateTime()
        {
            return new DateTimeOffset(2000, 1, 1, 1, 1, 1, new TimeSpan(0));
        }

        [Test]
        public void Translate()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            var msg = new LogMessage(LevelToLog.Info, "blabla");
            var text = template.Translate(msg);

            var expectedText = string.Format(
                CultureInfo.CurrentCulture, 
                DebugLogTemplate.DebugLogFormat,
                GetDefaultDateTime().ToString("yyyy/MM/ddTHH:mm:ss.fffff zzz", CultureInfo.CurrentCulture), 
                msg.Level, 
                msg.Text());
            Assert.AreEqual(expectedText, text);
        }

        [Test]
        public void EqualsWithNullObject()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsFalse(template.Equals((object)null));
        }

        [Test]
        public void EqualsWithNonEqualType()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsFalse(template.Equals(new object()));
        }

        [Test]
        public void EqualsWithEqualObject()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsTrue(template.Equals((object)new DebugLogTemplate(configuration.Object, GetDefaultDateTime)));
        }

        [Test]
        public void EqualsWithSameObject()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsTrue(template.Equals((object)template));
        }

        [Test]
        public void EqualsWithNullLogTemplate()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsFalse(template.Equals((ILogTemplate)null));
        }

        [Test]
        public void EqualsWithEqualLogTemplate()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsTrue(template.Equals((ILogTemplate)new DebugLogTemplate(configuration.Object, GetDefaultDateTime)));
        }

        [Test]
        public void EqualsWithSameLogTemplate()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsTrue(template.Equals((ILogTemplate)template));
        }

        [Test]
        public void EqualsWithNullDebugLogTemplate()
        {
            var configuration = new Mock<IConfiguration>();
            {
                configuration.Setup(c => c.HasValueFor(It.IsAny<ConfigurationKey>()))
                    .Returns(false);
            }

            var template = new DebugLogTemplate(configuration.Object, GetDefaultDateTime);
            Assert.IsFalse(template.Equals(null));
        }
    }
}
