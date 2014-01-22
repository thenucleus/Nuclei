//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for a proxy that handles all message sending. This interface
    /// is only used by the WCF <see cref="ChannelFactory{T}"/>.
    /// </summary>
    internal interface IMessageReceivingEndpointProxy : IMessageReceivingEndpoint, IOutputChannel
    {
    }
}
