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
    /// Converts <see cref="RegisterForNotificationMessage"/> objects to <see cref="NotificationRegistrationData"/> objects and visa versa.
    /// </summary>
    internal sealed class NotificationRegistrationConverter : IConvertCommunicationMessages
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
                return typeof(RegisterForNotificationMessage);
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
                return typeof(NotificationRegistrationData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var unregistrationMessage = data as NotificationRegistrationData;
            if (unregistrationMessage == null)
            {
                return new UnknownMessageTypeMessage(data.Sender, data.Id, data.InResponseTo);
            }

            try
            {
                var interfaceType = TypeLoader.FromPartialInformation(
                    unregistrationMessage.InterfaceType.FullName,
                    unregistrationMessage.InterfaceType.AssemblyName);

                return new RegisterForNotificationMessage(
                    data.Sender,
                    data.Id,
                    new NotificationData(
                        interfaceType,
                        unregistrationMessage.EventName));
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
            var unregistrationMessage = message as RegisterForNotificationMessage;
            if (unregistrationMessage == null)
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
                return new NotificationRegistrationData
                {
                    Id = message.Id,
                    InResponseTo = message.InResponseTo,
                    Sender = message.Sender,
                    InterfaceType = new SerializedType
                    {
                        FullName = unregistrationMessage.Notification.InterfaceType.FullName,
                        AssemblyName = unregistrationMessage.Notification.InterfaceType.Assembly.GetName().Name
                    },
                    EventName = unregistrationMessage.Notification.EventName,
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
