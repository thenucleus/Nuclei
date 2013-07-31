//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that carry information, in serialized form, about 
    /// a method call.
    /// </summary>
    internal interface ISerializedMethodInvocation : IEquatable<ISerializedMethodInvocation>
    {
        /// <summary>
        /// Gets the type on which the method was invoked.
        /// </summary>
        ISerializedType Type
        {
            get;
        }

        /// <summary>
        /// Gets the name of the method.
        /// </summary>
        string MemberName
        {
            get;
        }

        /// <summary>
        /// Gets a collection which contains the types and values of the parameters.
        /// </summary>
        List<Tuple<ISerializedType, object>> Parameters
        {
            get;
        }
    }
}
