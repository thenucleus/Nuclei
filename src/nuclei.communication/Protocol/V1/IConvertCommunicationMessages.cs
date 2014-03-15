//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Defines the interface for objects that convert <see cref="ICommunicationMessage"/> objects
    /// to protocol version 1.0 data structures that can be send to a remote endpoint.
    /// </summary>
    internal interface IConvertCommunicationMessages
    {
        /// <summary>
        /// Gets the type of <see cref="ICommunicationMessage"/> objects that the current convertor can
        /// convert.
        /// </summary>
        Type MessageTypeToTranslate
        {
            get;
        }

        /// <summary>
        /// Gets the type of <see cref="IStoreV1CommunicationData"/> objects that the current 
        /// converter can convert.
        /// </summary>
        Type DataTypeToTranslate
        {
            get;
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        ICommunicationMessage ToMessage(IStoreV1CommunicationData data);

        /// <summary>
        /// Converts the communication message to a data structure.
        /// </summary>
        /// <param name="message">The communication message.</param>
        /// <returns>The data structure that contains all the information that was stored in the message.</returns>
        IStoreV1CommunicationData FromMessage(ICommunicationMessage message);
    }
}
