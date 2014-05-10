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
    /// be the maximum number of times a command invocation message can be resend if the initial delivery fails.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class InvocationRetryCountAttribute : CommandProxyParameterUsageAttribute
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