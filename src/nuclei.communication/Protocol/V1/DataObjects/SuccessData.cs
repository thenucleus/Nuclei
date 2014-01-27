//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Nuclei.Communication.Protocol.V1.DataObjects
{
    /// <summary>
    /// A data structure that indicates that a certain action has succeeded.
    /// </summary>
    [DataContract]
    internal sealed class SuccessData : DataObjectBase
    {
    }
}
