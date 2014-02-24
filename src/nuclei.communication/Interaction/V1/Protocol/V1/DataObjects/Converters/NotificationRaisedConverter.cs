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
    /// Converts <see cref="NotificationRaisedMessage"/> objects to <see cref="Protocol.V1.DataObjects.NotificationRaisedData"/> objects and visa versa.
    /// </summary>
    internal sealed class NotificationRaisedConverter : IConvertCommunicationMessages
    {
        /// <summary>
        /// The ordered list of serializers for object data.
        /// </summary>
        private readonly IStoreObjectSerializers m_TypeSerializers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationRaisedConverter"/> class.
        /// </summary>
        /// <param name="typeSerializers">The ordered list of serializers for object data.</param>
        public NotificationRaisedConverter(IStoreObjectSerializers typeSerializers)
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
                return typeof(NotificationRaisedMessage);
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
                return typeof(Protocol.V1.DataObjects.NotificationRaisedData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var msg = data as Protocol.V1.DataObjects.NotificationRaisedData;
            if (msg == null)
            {
                throw new UnknownMessageTypeException();
            }

            try
            {
                var interfaceType = TypeLoader.FromPartialInformation(
                    msg.InterfaceType.FullName,
                    msg.InterfaceType.AssemblyName);

                var eventArgsType = TypeLoader.FromPartialInformation(
                    msg.EventArgumentsType.FullName,
                    msg.EventArgumentsType.AssemblyName);

                var serializedObjectData = msg.EventArguments;
                if (m_TypeSerializers.HasSerializerFor(eventArgsType))
                {
                    throw new MissingObjectDataSerializerException();
                }

                var serializer = m_TypeSerializers.SerializerFor(eventArgsType);
                var eventArgs = serializer.Deserialize(serializedObjectData) as EventArgs;

                return new NotificationRaisedMessage(
                    data.Sender,
                    new Interaction.NotificationRaisedData(
                        new NotificationData(
                            interfaceType,
                            msg.EventName), 
                        eventArgs));
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
            var msg = message as NotificationRaisedMessage;
            if (msg == null)
            {
                throw new UnknownMessageTypeException();
            }

            try
            {
                var eventArgs = msg.Notification.EventArgs;
                var eventArgsType = msg.Notification.EventArgs.GetType();
                var serializedEventArgsType = new SerializedType
                    {
                        FullName = eventArgsType.FullName,
                        AssemblyName = eventArgsType.Assembly.GetName().Name
                    };

                if (m_TypeSerializers.HasSerializerFor(eventArgsType))
                {
                    throw new MissingObjectDataSerializerException();
                }

                var serializer = m_TypeSerializers.SerializerFor(eventArgsType);

                var value = serializer.Serialize(eventArgs);
                
                return new NotificationRaisedData
                    {
                        Id = message.Id,
                        InResponseTo = message.InResponseTo,
                        Sender = message.Sender,
                        InterfaceType = new SerializedType
                            {
                                FullName = msg.Notification.Notification.InterfaceType.FullName,
                                AssemblyName = msg.Notification.Notification.InterfaceType.Assembly.GetName().Name
                            },
                        EventName = msg.Notification.Notification.EventName,
                        EventArgumentsType = serializedEventArgsType,
                        EventArguments = value,
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
