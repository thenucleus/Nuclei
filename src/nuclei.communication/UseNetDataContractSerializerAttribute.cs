//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Nuclei.Communication
{
    /// <summary>
    /// The data contract attribute that should be applied when using the 
    /// <see cref="NetDataContractSerializer"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal sealed class UseNetDataContractSerializerAttribute : Attribute, IOperationBehavior
    {
        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="operationDescription">
        ///     The operation being examined. Use for examination only. If the operation description is modified, the results are undefined.
        /// </param>
        /// <param name="bindingParameters">The collection of objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {
        }

        /// <summary>
        /// Implements a modification or extension of the client across an operation.
        /// </summary>
        /// <param name="operationDescription">
        ///     The operation being examined. Use for examination only. If the operation description is modified,
        ///     the results are undefined.
        /// </param>
        /// <param name="clientOperation">
        ///     The run-time object that exposes customization properties for the operation described
        ///     by <paramref name="operationDescription"/>.
        /// </param>
        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
            ReplaceDataContractSerializerOperationBehavior(operationDescription);
        }

        /// <summary>
        /// Implements a modification or extension of the service across an operation.
        /// </summary>
        /// <param name="operationDescription">
        ///     The operation being examined. Use for examination only. If the operation description is modified,
        ///     the results are undefined.
        /// </param>
        /// <param name="dispatchOperation">
        ///     The run-time object that exposes customization properties for the operation described
        ///     by <paramref name="operationDescription"/>.
        /// </param>
        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            ReplaceDataContractSerializerOperationBehavior(operationDescription);
        }

        /// <summary>
        /// Implement to confirm that the operation meets some intended criteria.
        /// </summary>
        /// <param name="operationDescription">
        ///     The operation being examined. Use for examination only. If the operation description is modified,
        ///     the results are undefined.
        /// </param>
        public void Validate(OperationDescription operationDescription)
        {
        }

        /// <summary>
        /// Replaces the data contract serializer operation behavior.
        /// </summary>
        /// <param name="description">The description.</param>
        private static void ReplaceDataContractSerializerOperationBehavior(OperationDescription description)
        {
            DataContractSerializerOperationBehavior dcsOperationBehavior =
               description.Behaviors.Find<DataContractSerializerOperationBehavior>();

            if (dcsOperationBehavior != null)
            {
                description.Behaviors.Remove(dcsOperationBehavior);
                description.Behaviors.Add(new NetDataContractOperationBehavior(description));
            }
        }
    }
}
