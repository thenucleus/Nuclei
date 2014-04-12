//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Communication.Protocol.Messages;

namespace Nuclei.Communication.Protocol.V1.DataObjects.Converters
{
    /// <summary>
    /// Converts <see cref="ConnectionVerificationMessage"/> objects to <see cref="ConnectionVerificationData"/> objects and visa versa.
    /// </summary>
    internal sealed class ConnectionVerificationConverter : IConvertCommunicationMessages
    {
        /// <summary>
        /// The ordered list of serializers for object data.
        /// </summary>
        private readonly IStoreObjectSerializers m_TypeSerializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionVerificationConverter"/> class.
        /// </summary>
        /// <param name="typeSerializers">The ordered list of serializers for object data.</param>
        public ConnectionVerificationConverter(IStoreObjectSerializers typeSerializers)
        {
            {
                Lokad.Enforce.Argument(() => typeSerializers);
            }

            m_TypeSerializers = typeSerializers;
        }

        /// <summary>
        /// Gets the type of <see cref="ICommunicationMessage"/> objects that the current convertor can
        /// convert.
        /// </summary>
        public Type MessageTypeToTranslate
        {
            [DebuggerStepThrough]
            get
            {
                return typeof(ConnectionVerificationMessage);
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
                return typeof(ConnectionVerificationData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var endpointConnectData = data as ConnectionVerificationData;
            if (endpointConnectData == null)
            {
                return new UnknownMessageTypeMessage(data.Sender, data.Id, data.InResponseTo);
            }

            var dataType = TypeLoader.FromPartialInformation(
                endpointConnectData.DataType.FullName,
                endpointConnectData.DataType.AssemblyName);

            if (!m_TypeSerializers.HasSerializerFor(dataType))
            {
                throw new MissingObjectDataSerializerException();
            }

            var serializer = m_TypeSerializers.SerializerFor(dataType);
            var value = serializer.Deserialize(endpointConnectData.CustomData);

            return new ConnectionVerificationMessage(
                endpointConnectData.Sender,
                data.Id,
                value);
        }

        /// <summary>
        /// Converts the communication message to a data structure.
        /// </summary>
        /// <param name="message">The communication message.</param>
        /// <returns>The data structure that contains all the information that was stored in the message.</returns>
        public IStoreV1CommunicationData FromMessage(ICommunicationMessage message)
        {
            var endpointConnectMessage = message as ConnectionVerificationMessage;
            if (endpointConnectMessage == null)
            {
                return new UnknownMessageTypeData
                {
                    Id = message.Id,
                    InResponseTo = message.InResponseTo,
                    Sender = message.Sender,
                };
            }

            var type = endpointConnectMessage.CustomData.GetType();
            if (!m_TypeSerializers.HasSerializerFor(type))
            {
                throw new MissingObjectDataSerializerException();
            }

            var serializer = m_TypeSerializers.SerializerFor(type);
            var value = serializer.Serialize(endpointConnectMessage.CustomData);

            return new ConnectionVerificationData
                {
                    Id = endpointConnectMessage.Id,
                    InResponseTo = endpointConnectMessage.InResponseTo,
                    Sender = endpointConnectMessage.Sender,
                    DataType = new SerializedType
                        {
                            FullName = type.FullName,
                            AssemblyName = type.Assembly.GetName().Name
                        },
                    CustomData = value,
                };
        }
    }
}
