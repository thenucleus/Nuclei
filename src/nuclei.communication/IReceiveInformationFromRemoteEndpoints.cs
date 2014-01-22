//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceModel;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the base interface for objects that act as message or data receiving proxies for a <see cref="ServiceHost"/>.
    /// </summary>
    /// <design>
    /// This interface forms the base interface for all interfaces / classes that define a receiving endpoint,
    /// for both the discovery and the protocol layers.
    /// </design>
    internal interface IReceiveInformationFromRemoteEndpoints
    {
    }
}
