//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Nuclei.Communication;
using Nuclei.Examples.Complete.Models;

namespace Nuclei.Examples.Complete
{
    /// <summary>
    /// Provides the methods necessary for handling incoming messages.
    /// </summary>
    internal sealed class ApplicationCentral : IFormTheApplicationCenter
    {
        /// <summary>
        /// The layer from which the incoming messages are received.
        /// </summary>
        private readonly ICommunicationFacade m_CommunicationLayer;

        /// <summary>
        /// The object that stores information about the current connections and the messages 
        /// that have been received.
        /// </summary>
        private readonly ConnectionViewModel m_ConnectionStateInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationCentral"/> class.
        /// </summary>
        /// <param name="communicationLayer">The layer from which incoming messages are received.</param>
        /// <param name="connectionStateInformation">The object that stores information about the current connections and received messages.</param>
        public ApplicationCentral(
            ICommunicationFacade communicationLayer, 
            ConnectionViewModel connectionStateInformation)
        {
            m_CommunicationLayer = communicationLayer;
            m_CommunicationLayer.OnEndpointConnected += (s, e) => OnEndpointSignedOn(e.Endpoint);
            m_CommunicationLayer.OnEndpointDisconnected += (s, e) => OnEndpointSignedOff(e.Endpoint);

            m_ConnectionStateInformation = connectionStateInformation;
        }

        private void OnEndpointSignedOn(EndpointId endpoint)
        {
            m_ConnectionStateInformation.AddKnownEndpoint(
                new ConnectionInformationViewModel(endpoint));
        }

        private void OnEndpointSignedOff(EndpointId endpointId)
        {
            m_ConnectionStateInformation.RemoveKnownEndpoint(endpointId);
        }
    }
}
