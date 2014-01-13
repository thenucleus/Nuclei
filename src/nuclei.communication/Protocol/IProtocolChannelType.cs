﻿//-----------------------------------------------------------------------
// <copyright company="P. van der Velde">
//     Copyright (c) P. van der Velde. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that provide information about a specific type of
    /// WCF channel for the protocol level, e.g. TCP.
    /// </summary>
    internal interface IProtocolChannelType : IChannelType
    {
        /// <summary>
        /// Gets the type of the channel.
        /// </summary>
        ChannelType ChannelType
        {
            get;
        }

        /// <summary>
        /// Generates a new binding object used to send messages across the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        Binding GenerateMessageBinding();

        /// <summary>
        /// Generates a new binding object used to transfer data across the channel.
        /// </summary>
        /// <returns>
        /// The newly generated binding.
        /// </returns>
        Binding GenerateDataBinding();

        /// <summary>
        /// Attaches a new message endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <param name="localEndpoint">The ID of the local endpoint, to be used in the endpoint metadata.</param>
        /// <returns>The newly attached endpoint.</returns>
        ServiceEndpoint AttachMessageEndpoint(ServiceHost host, Type implementedContract, EndpointId localEndpoint);

        /// <summary>
        /// Attaches a new endpoint to the given host.
        /// </summary>
        /// <param name="host">The host to which the endpoint should be attached.</param>
        /// <param name="implementedContract">The contract implemented by the endpoint.</param>
        /// <returns>The newly attached endpoint.</returns>
        ServiceEndpoint AttachDataEndpoint(ServiceHost host, Type implementedContract);
    }
}