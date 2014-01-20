//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the interface for a proxy that bootstraps the discovery process. This interface
    /// is only used by the WCF <see cref="ChannelFactory{TChannel}"/>.
    /// </summary>
    internal interface IBootstrapEndpointProxy : IBootstrapEndpoint, IOutputChannel
    {
    }
}
