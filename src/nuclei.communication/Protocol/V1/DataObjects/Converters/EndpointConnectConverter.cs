﻿//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Protocol.V1.DataObjects.Converters
{
    /// <summary>
    /// Converts <see cref="EndpointConnectMessage"/> objects to <see cref="EndpointConnectData"/> objects and visa versa.
    /// </summary>
    internal sealed class EndpointConnectConverter : IConvertCommunicationMessages
    {
        /// <summary>
        /// Gets the type of <see cref="ICommunicationMessage"/> objects that the current convertor can
        /// convert.
        /// </summary>
        public Type MessageTypeToTranslate
        {
            get
            {
                return typeof(EndpointConnectMessage);
            }
        }

        /// <summary>
        /// Gets the type of <see cref="IStoreV1CommunicationData"/> objects that the current 
        /// converter can convert.
        /// </summary>
        public Type DataTypeToTranslate
        {
            get
            {
                return typeof(EndpointConnectData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var endpointConnectData = data as EndpointConnectData;
            if (endpointConnectData == null)
            {
                throw new UnknownMessageTypeException();
            }

            return new EndpointConnectMessage(
                endpointConnectData.Sender,
                new DiscoveryInformation(endpointConnectData.DiscoveryAddress), 
                new ProtocolInformation(
                    endpointConnectData.ProtocolVersion,
                    endpointConnectData.MessageAddress,
                    endpointConnectData.DataAddress), 
                endpointConnectData.Information);
        }

        /// <summary>
        /// Converts the communication message to a data structure.
        /// </summary>
        /// <param name="message">The communication message.</param>
        /// <returns>The data structure that contains all the information that was stored in the message.</returns>
        public IStoreV1CommunicationData FromMessage(ICommunicationMessage message)
        {
            var endpointConnectMessage = message as EndpointConnectMessage;
            if (endpointConnectMessage == null)
            {
                throw new UnknownMessageTypeException();
            }

            return new EndpointConnectData
                {
                    Id = endpointConnectMessage.Id,
                    InResponseTo = endpointConnectMessage.InResponseTo,
                    Sender = endpointConnectMessage.Sender,
                    DiscoveryAddress = endpointConnectMessage.DiscoveryInformation.Address,
                    ProtocolVersion = endpointConnectMessage.ProtocolInformation.Version,
                    MessageAddress = endpointConnectMessage.ProtocolInformation.MessageAddress,
                    DataAddress = endpointConnectMessage.ProtocolInformation.DataAddress,
                    Information = endpointConnectMessage.Information,
                };
        }
    }
}
