//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects.Converters
{
    /// <summary>
    /// Converts <see cref="CommandInvokedMessage"/> objects to <see cref="CommandInvocationData"/> objects and visa versa.
    /// </summary>
    internal sealed class CommandInvocationConverter : IConvertCommunicationMessages
    {
        /// <summary>
        /// The ordered list of serializers for object data.
        /// </summary>
        private readonly IStoreObjectSerializers m_TypeSerializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvocationConverter"/> class.
        /// </summary>
        /// <param name="typeSerializers">The ordered list of serializers for object data.</param>
        public CommandInvocationConverter(IStoreObjectSerializers typeSerializers)
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
                return typeof(CommandInvokedMessage);
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
                return typeof(CommandInvocationData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var msg = data as CommandInvocationData;
            if (msg == null)
            {
                return new UnknownMessageTypeMessage(data.Sender, data.Id, data.InResponseTo);
            }

            try
            {
                var id = CommandIdExtensions.Deserialize(msg.CommandId);
                var parameterValues = new CommandParameterValueMap[msg.ParameterTypes.Length];
                for (int i = 0; i < msg.ParameterTypes.Length; i++)
                {
                    var typeInfo = msg.ParameterTypes[i];
                    var type = TypeLoader.FromPartialInformation(typeInfo.FullName, typeInfo.AssemblyName);

                    var name = msg.ParameterNames[i];

                    var serializedObjectData = msg.ParameterValues[i];
                    if (!m_TypeSerializers.HasSerializerFor(type))
                    {
                        throw new MissingObjectDataSerializerException();
                    }

                    var serializer = m_TypeSerializers.SerializerFor(type);
                    var value = serializer.Deserialize(serializedObjectData);
                    parameterValues[i] = new CommandParameterValueMap(
                        new CommandParameterDefinition(type, name, CommandParameterOrigin.FromCommand), 
                        value);
                }

                return new CommandInvokedMessage(
                    data.Sender,
                    data.Id,
                    new CommandInvokedData(
                        id,
                        parameterValues));
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
            var msg = message as CommandInvokedMessage;
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
                var parameters = msg.Invocation.Parameters;
                var parameterTypes = new SerializedType[parameters.Length];
                var parameterNames = new string[parameters.Length];
                var parameterValues = new object[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    parameterTypes[i] = new SerializedType
                        {
                            FullName = parameter.Parameter.Type.FullName,
                            AssemblyName = parameter.Parameter.Type.Assembly.GetName().Name
                        };

                    parameterNames[i] = parameter.Parameter.Name;

                    if (!m_TypeSerializers.HasSerializerFor(parameter.Parameter.Type))
                    {
                        throw new MissingObjectDataSerializerException();
                    }

                    var serializer = m_TypeSerializers.SerializerFor(parameter.Parameter.Type);
                    var value = serializer.Serialize(parameter.Value);
                    parameterValues[i] = value;
                }

                return new CommandInvocationData
                    {
                        Id = message.Id,
                        InResponseTo = message.InResponseTo,
                        Sender = message.Sender,
                        CommandId = CommandIdExtensions.Serialize(msg.Invocation.Command),
                        ParameterTypes = parameterTypes,
                        ParameterNames = parameterNames,
                        ParameterValues = parameterValues,
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
