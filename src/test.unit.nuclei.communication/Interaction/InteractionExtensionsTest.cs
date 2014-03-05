//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class InteractionExtensionsTest
    {
        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonAssignableType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(() => typeof(object).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(() => typeof(MockCommandSetNotAnInterface).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericInterface()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithGenericParameters<>).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithProperties()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithProperties).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithAdditionalEvents()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithEvents).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithoutMethods()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithoutMethods).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithGenericMethod()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithGenericMethod).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithIncorrectReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithMethodWithIncorrectReturnType).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableReturnType()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithMethodWithNonSerializableReturnType).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithOutParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithMethodWithOutParameter).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithRefParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithMethodWithRefParameter).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectCommandSetWithNonSerializableParameter()
        {
            Assert.Throws<TypeIsNotAValidCommandSetException>(
                () => typeof(IMockCommandSetWithMethodWithNonSerializableParameter).VerifyThatTypeIsACorrectCommandSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonAssignableType()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(object).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonInterface()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(MockNotificationSetNotAnInterface).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithGenericInterface()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(IMockNotificationSetWithGenericParameters<>).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithProperties()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(IMockNotificationSetWithProperties).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithMethods()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(IMockNotificationSetWithMethods).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithoutEvents()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(IMockNotificationSetWithoutEvents).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonEventHandlerEvent()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(IMockNotificationSetWithNonEventHandlerEvent).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSetWithNonSerializableEventArgs()
        {
            Assert.Throws<TypeIsNotAValidNotificationSetException>(
                () => typeof(IMockNotificationSetWithNonSerializableEventArgs).VerifyThatTypeIsACorrectNotificationSet());
        }

        [Test]
        public void VerifyThatTypeIsACorrectNotificationSet()
        {
            Assert.DoesNotThrow(
                () => typeof(IMockNotificationSetWithEventHandler).VerifyThatTypeIsACorrectNotificationSet());
            Assert.DoesNotThrow(
                () => typeof(IMockNotificationSetWithTypedEventHandler).VerifyThatTypeIsACorrectNotificationSet());
        }
    }
}
