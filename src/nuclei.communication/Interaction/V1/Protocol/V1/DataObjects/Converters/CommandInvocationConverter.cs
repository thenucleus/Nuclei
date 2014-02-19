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
        private readonly IList<ISerializeObjectData> m_TypeSerializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInvocationConverter"/> class.
        /// </summary>
        /// <param name="typeSerializers">The ordered list of serializers for object data.</param>
        public CommandInvocationConverter(IList<ISerializeObjectData> typeSerializers)
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
                throw new UnknownMessageTypeException();
            }

            try
            {
                var interfaceType = TypeLoader.FromPartialInformation(
                    msg.InterfaceType.FullName,
                    msg.InterfaceType.AssemblyName);

                var parameterValues = new Tuple<Type, object>[msg.ParameterTypes.Length];
                for (int i = 0; i < msg.ParameterTypes.Length; i++)
                {
                    var typeInfo = msg.ParameterTypes[i];
                    var type = TypeLoader.FromPartialInformation(typeInfo.FullName, typeInfo.AssemblyName);

                    var serializedObjectData = msg.ParameterValues[i];
                    var serializer = m_TypeSerializers.FirstOrDefault(t => t.TypeToSerialize.IsAssignableFrom(type));
                    if (serializer == null)
                    {
                        throw new MissingObjectDataSerializerException();
                    }

                    var value = serializer.Deserialize(serializedObjectData);
                    parameterValues[i] = new Tuple<Type, object>(type, value);
                }

                return new CommandInvokedMessage(
                    data.Sender,
                    new CommandInvokedData(
                        new CommandData(
                            interfaceType,
                            msg.MethodName),
                        parameterValues));
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
            var msg = message as CommandInvokedMessage;
            if (msg == null)
            {
                throw new UnknownMessageTypeException();
            }

            try
            {
                var valuePairs = msg.Invocation.ParameterValues;
                var parameterTypes = new SerializedType[valuePairs.Length];
                var parameterValues = new object[valuePairs.Length];
                for (int i = 0; i < valuePairs.Length; i++)
                {
                    var pair = valuePairs[i];
                    parameterTypes[i] = new SerializedType
                        {
                            FullName = pair.Item1.FullName,
                            AssemblyName = pair.Item1.Assembly.GetName().Name
                        };

                    var serializer = m_TypeSerializers.FirstOrDefault(t => t.TypeToSerialize.IsInstanceOfType(pair.Item2));
                    if (serializer == null)
                    {
                        throw new MissingObjectDataSerializerException();
                    }

                    var value = serializer.Serialize(pair.Item2);
                    parameterValues[i] = value;
                }

                return new CommandInvocationData
                    {
                        Id = message.Id,
                        InResponseTo = message.InResponseTo,
                        Sender = message.Sender,
                        InterfaceType = new SerializedType
                            {
                                FullName = msg.Invocation.Command.InterfaceType.FullName,
                                AssemblyName = msg.Invocation.Command.InterfaceType.Assembly.GetName().Name
                            },
                        MethodName = msg.Invocation.Command.MethodName,
                        ParameterTypes = parameterTypes,
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
