//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Protocol.V1.DataObjects.Converters
{
    /// <summary>
    /// Converts <see cref="DataDownloadRequestMessage"/> objects to <see cref="DownloadRequestData"/> objects and visa versa.
    /// </summary>
    internal sealed class DownloadRequestConverter : IConvertCommunicationMessages
    {
        /// <summary>
        /// Gets the type of <see cref="ICommunicationMessage"/> objects that the current convertor can
        /// convert.
        /// </summary>
        public Type MessageTypeToTranslate
        {
            get
            {
                return typeof(DataDownloadRequestMessage);
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
                return typeof(DownloadRequestData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var downloadData = data as DownloadRequestData;
            if (downloadData == null)
            {
                throw new UnknownMessageTypeException();
            }

            return new DataDownloadRequestMessage(downloadData.Sender, downloadData.Token);
        }

        /// <summary>
        /// Converts the communication message to a data structure.
        /// </summary>
        /// <param name="message">The communication message.</param>
        /// <returns>The data structure that contains all the information that was stored in the message.</returns>
        public IStoreV1CommunicationData FromMessage(ICommunicationMessage message)
        {
            var downloadMessage = message as DataDownloadRequestMessage;
            if (downloadMessage == null)
            {
                throw new UnknownMessageTypeException();
            }

            return new DownloadRequestData
                {
                    Id = downloadMessage.Id,
                    InResponseTo = downloadMessage.InResponseTo,
                    Sender = downloadMessage.Sender,
                    Token = downloadMessage.Token,
                };
        }
    }
}
