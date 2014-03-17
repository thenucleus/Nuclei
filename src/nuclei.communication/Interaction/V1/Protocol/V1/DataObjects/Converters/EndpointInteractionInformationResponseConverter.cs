//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Communication.Interaction.Transport;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects.Converters
{
    /// <summary>
    /// Converts <see cref="EndpointInteractionInformationResponseMessage"/> objects to 
    /// <see cref="EndpointInteractionInformationResponseData"/> objects and visa versa.
    /// </summary>
    internal sealed class EndpointInteractionInformationResponseConverter : IConvertCommunicationMessages
    {
        /// <summary>
        /// Gets the type of <see cref="ICommunicationMessage"/> objects that the current convertor can
        /// convert.
        /// </summary>
        public Type MessageTypeToTranslate
        {
            [DebuggerStepThrough]
            get
            {
                return typeof(EndpointInteractionInformationResponseMessage);
            }
        }

        /// <summary>
        /// Gets the type of <see cref="IStoreV1CommunicationData"/> objects that the current 
        /// converter can convert.
        /// </summary>
        public Type DataTypeToTranslate
        {
            [DebuggerStepThrough]
            get
            {
                return typeof(EndpointInteractionInformationResponseData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var msg = data as EndpointInteractionInformationResponseData;
            if (msg == null)
            {
                return new UnknownMessageTypeMessage(data.Sender, data.Id, data.InResponseTo);
            }

            try
            {
                var state = (InteractionConnectionState)Enum.Parse(typeof(InteractionConnectionState), msg.State);
                return new EndpointInteractionInformationResponseMessage(
                    data.Sender,
                    data.Id,
                    data.InResponseTo,
                    state);
            }
            catch (Exception)
            {
                return new UnknownMessageTypeMessage(data.Sender, data.Id, data.InResponseTo);
            }
        }

        /// <summary>
        /// Converts the communication message to a data structure.
        /// </summary>
        /// <param name="message">The communication message.</param>
        /// <returns>The data structure that contains all the information that was stored in the message.</returns>
        public IStoreV1CommunicationData FromMessage(ICommunicationMessage message)
        {
            var msg = message as EndpointInteractionInformationResponseMessage;
            if (msg == null)
            {
                return new UnknownMessageTypeData
                    {
                        Id = message.Id,
                        InResponseTo = message.InResponseTo,
                        Sender = message.Sender,
                    };
            }

            try
            {
                return new EndpointInteractionInformationResponseData
                {
                    Id = message.Id,
                    InResponseTo = message.InResponseTo,
                    Sender = message.Sender,
                    State = msg.State.ToString(),
                };
            }
            catch (Exception)
            {
                return new UnknownMessageTypeData
                {
                    Id = message.Id,
                    InResponseTo = message.InResponseTo,
                    Sender = message.Sender,
                };
            }
        }
    }
}
