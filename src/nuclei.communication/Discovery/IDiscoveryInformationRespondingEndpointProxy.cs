//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nuclei.Communication.Discovery
{
    /// <summary>
    /// Defines the interface for a proxy that handles all requests for discovery. This interface
    /// is only used by the WCF <see cref="ChannelFactory{TChannel}"/>.
    /// </summary>
    internal interface IDiscoveryInformationRespondingEndpointProxy : IDiscoveryInformationRespondingEndpoint, IOutputChannel
    {
    }
}
