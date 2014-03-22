//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for strong-typed meta data describing an the registered type of an element.
    /// </summary>
    internal interface ITypeMetaData
    {
        /// <summary>
        /// Gets the registered type.
        /// </summary>
        Type RegisteredType
        {
            get;
        }
    }
}
