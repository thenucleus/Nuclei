//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines an attribute that indicates that an parameter on an interface method is supposed to
    /// be the maximum time in milliseconds a command invocation may take before timing out.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class InvocationTimeoutAttribute : CommandProxyParameterUsageAttribute
    {
        /// <summary>
        /// Gets the type that the parameter on which the current attribute is applied should have.
        /// </summary>
        public override Type AllowedParameterType
        {
            get
            {
                return typeof(int);
            }
        }
    }
}