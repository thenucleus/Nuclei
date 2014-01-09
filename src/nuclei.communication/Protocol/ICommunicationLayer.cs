//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nuclei.Communication.Protocol;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the methods for communicating with a remote endpoint.
    /// </summary>
    public interface ICommunicationLayer
    {
        /// <summary>
        /// Gets the endpoint ID of the local endpoint.
        /// </summary>
        EndpointId Id
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has signed on with
        /// the network.
        /// </summary>
        bool IsSignedIn
        { 
            get; 
        }

        /// <summary>
        /// Gets the connection information for the channel of a given type created by the current application.
        /// </summary>
        /// <param name="channelType">The type of channel for which the connection information is required.</param>
        /// <returns>
        /// A tuple containing the <see cref="EndpointId"/>, the <see cref="Uri"/> of the message channel and the 
        /// <see cref="Uri"/> of the data channel; returns <see langword="null" /> if no channel of the given type exists.
        /// </returns>
        Tuple<EndpointId, Uri, Uri> LocalConnectionFor(ChannelType channelType);

        /// <summary>
        /// Returns a collection containing the endpoint IDs of the known remote endpoints.
        /// </summary>
        /// <returns>
        ///     The collection that contains the endpoint IDs of the remote endpoints.
        /// </returns>
        IEnumerable<EndpointId> KnownEndpoints();

        /// <summary>
        /// Connects to the network and broadcasts a sign on message.
        /// </summary>
        void SignIn();

        /// <summary>
        /// Broadcasts a sign off message and disconnects from the network.
        /// </summary>
        void SignOut();

        /// <summary>
        /// An event raised when the layer has signed in.
        /// </summary>
        event EventHandler<EventArgs> OnSignedIn;

        /// <summary>
        /// An event raised when the layer has signed out.
        /// </summary>
        event EventHandler<EventArgs> OnSignedOut;

        /// <summary>
        /// An event raised when an endpoint has joined the network.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointSignedIn;

        /// <summary>
        /// An event raised when an endpoint has left the network.
        /// </summary>
        event EventHandler<EndpointEventArgs> OnEndpointSignedOut;

        /// <summary>
        /// Returns a value indicating if the given endpoint has provided the information required to
        /// contact it if it isn't offline.
        /// </summary>
        /// <param name="endpoint">The ID number of the endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint has provided the information necessary to contact 
        ///     it over the network. Otherwise; <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool IsEndpointContactable(EndpointId endpoint);
    }
}
