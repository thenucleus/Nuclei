//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines an attribute that indicates that an parameter on an instance method is supposed to
    /// be the <see cref="EndpointId"/> of the remote endpoint that requested the invocation of 
    /// the current command.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class InvokingEndpointAttribute : CommandInstanceParameterUsageAttribute
    {
        /// <summary>
        /// Gets the type that the parameter on which the current attribute is applied should have.
        /// </summary>
        public override Type AllowedParameterType
        {
            get
            {
                return typeof(EndpointId);
            }
        }
    }
}
