//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nuclei.Communication.Interaction.Transport;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.DataObjects.Converters
{
    /// <summary>
    /// Converts <see cref="CommandInvokedResponseMessage"/> objects to <see cref="CommandInvocationResponseData"/> objects and visa versa.
    /// </summary>
    internal sealed class CommandInvocationResponseConverter : IConvertCommunicationMessages
    {
        /// <summary>
        /// The ordered list of serializers for object data.
        /// </summary>
        private readonly IList<ISerializeObjectData> m_TypeSerializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvocationResponseConverter"/> class.
        /// </summary>
        /// <param name="typeSerializers">The ordered list of serializers for object data.</param>
        public CommandInvocationResponseConverter(IList<ISerializeObjectData> typeSerializers)
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
                return typeof(CommandInvokedResponseMessage);
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
                return typeof(CommandInvocationResponseData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var invocationData = data as CommandInvocationResponseData;
            if (invocationData == null)
            {
                throw new UnknownMessageTypeException();
            }

            try
            {
                var typeInfo = invocationData.ReturnedType;
                var type = TypeLoader.FromPartialInformation(typeInfo.FullName, typeInfo.AssemblyName);

                var serializedObjectData = invocationData.Result;
                var serializer = m_TypeSerializers.FirstOrDefault(t => t.TypeToSerialize.IsAssignableFrom(type));
                if (serializer == null)
                {
                    throw new MissingObjectDataSerializerException();
                }

                var returnValue = serializer.Deserialize(serializedObjectData);

                return new CommandInvokedResponseMessage(
                    data.Sender,
                    data.InResponseTo,
                    returnValue);
            }
            catch (Exception)
            {
                return new UnknownMessageTypeMessage(data.Sender, data.InResponseTo);
            }
        }

        /// <summary>
        /// Converts the communication message to a data structure.
        /// </summary>
        /// <param name="message">The communication message.</param>
        /// <returns>The data structure that contains all the information that was stored in the message.</returns>
        public IStoreV1CommunicationData FromMessage(ICommunicationMessage message)
        {
            var invocationMessage = message as CommandInvokedResponseMessage;
            if (invocationMessage == null)
            {
                throw new UnknownMessageTypeException();
            }

            try
            {
                var type = invocationMessage.Result.GetType();
                var returnedType = new SerializedType
                    {
                        FullName = type.FullName,
                        AssemblyName = type.Assembly.GetName().Name
                    };

                var serializer = m_TypeSerializers.FirstOrDefault(t => t.TypeToSerialize.IsInstanceOfType(type));
                if (serializer == null)
                {
                    throw new MissingObjectDataSerializerException();
                }

                var returnValue = serializer.Serialize(invocationMessage.Result);
                
                return new CommandInvocationResponseData
                    {
                        Id = message.Id,
                        InResponseTo = message.InResponseTo,
                        Sender = message.Sender,
                        ReturnedType = returnedType,
                        Result = returnValue,
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
