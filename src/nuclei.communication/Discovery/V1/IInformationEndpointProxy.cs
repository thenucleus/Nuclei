//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nuclei.Communication.Discovery.V1
{
    /// <summary>
    /// Defines the interface for a proxy that handles all requests for discovery. This interface
    /// is only used by the WCF <see cref="ChannelFactory{TChannel}"/>.
    /// </summary>
    internal interface IInformationEndpointProxy : IInformationEndpoint, IOutputChannel
    {
    }
}
