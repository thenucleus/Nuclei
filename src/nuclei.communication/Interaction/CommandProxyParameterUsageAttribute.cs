//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the base attribute that is used to indicate that a command method parameter has 
    /// special meaning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public abstract class CommandProxyParameterUsageAttribute : Attribute
    {
        /// <summary>
        /// Gets the type that the parameter on which the current attribute is applied should have.
        /// </summary>
        public abstract Type AllowedParameterType
        {
            get;
        }
    }
}
