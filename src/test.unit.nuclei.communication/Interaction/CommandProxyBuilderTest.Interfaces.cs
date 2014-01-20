//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Nuclei.Communication.Interaction
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1601:PartialElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation. Especially not in partial classes.")]
    public sealed partial class CommandProxyBuilderTest
    {
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit tests do not need documentation.")]
        public sealed class MockCommandSetNotAnInterface : ICommandSet
        {
        }

        public interface IMockCommandSetWithGenericParameters<T> : ICommandSet
        { 
        }

        public interface IMockCommandSetWithProperties : ICommandSet
        {
            int MyProperty { get; set; }
        }

        public interface IMockCommandSetWithEvents : ICommandSet
        {
            event EventHandler<EventArgs> OnMyEvent;
        }

        public interface IMockCommandSetWithoutMethods : ICommandSet
        { 
        }

        public interface IMockCommandSetWithGenericMethod : ICommandSet
        {
            void MyMethod<T>(T input);
        }

        public interface IMockCommandSetWithMethodWithIncorrectReturnType : ICommandSet
        {
            object MyMethod(object input);
        }

        public class MyNonSerializableType
        { 
        }

        public interface IMockCommandSetWithMethodWithNonSerializableReturnType : ICommandSet
        {
            Task<MyNonSerializableType> MyMethod(object input);
        }

        public interface IMockCommandSetWithMethodWithOutParameter : ICommandSet
        {
            Task MyMethod(object input, out int output);
        }

        public interface IMockCommandSetWithMethodWithRefParameter : ICommandSet
        {
            Task MyMethod(object input, ref int inputAndOutput);
        }

        public interface IMockCommandSetWithMethodWithNonSerializableParameter : ICommandSet
        {
            Task MyMethod(MyNonSerializableType input);
        }

        public interface IMockCommandSetWithTaskReturn : ICommandSet
        {
            Task MyMethod(int input);
        }

        public interface IMockCommandSetWithTypedTaskReturn : ICommandSet
        {
            Task<int> MyMethod(int input);
        }

        [InternalCommand]
        public interface IMockCommandSetForInternalUse : ICommandSet
        {
            Task MyInternalMethod();
        }
    }
}
