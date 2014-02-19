//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Protocol;
using Nuclei.Communication.Protocol.Messages;
using Nuclei.Communication.Protocol.V1;
using Nuclei.Communication.Protocol.V1.DataObjects;

namespace Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects.Converters
{
    /// <summary>
    /// Converts <see cref="EndpointInteractionInformationMessage"/> objects to 
    /// <see cref="EndpointInteractionInformationData"/> objects and visa versa.
    /// </summary>
    internal sealed class EndpointInteractionInformationConverter : IConvertCommunicationMessages
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
                return typeof(EndpointInteractionInformationMessage);
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
                return typeof(EndpointInteractionInformationData);
            }
        }

        /// <summary>
        /// Converts the data structure to a communication message.
        /// </summary>
        /// <param name="data">The data structure.</param>
        /// <returns>The communication message containing all the information that was stored in the data structure.</returns>
        public ICommunicationMessage ToMessage(IStoreV1CommunicationData data)
        {
            var msg = data as EndpointInteractionInformationData;
            if (msg == null)
            {
                throw new UnknownMessageTypeException();
            }

            try
            {
                var groups = new CommunicationSubjectGroup[msg.Groups.Length];
                for (int i = 0; i < msg.Groups.Length; i++)
                {
                    var serializedGroup = msg.Groups[i];

                    var commands = new VersionedTypeFallback[serializedGroup.Commands.Length];
                    for (int j = 0; j < serializedGroup.Commands.Length; j++)
                    {
                        var typeFallback = serializedGroup.Commands[j];
                        var types = typeFallback.Types.Select(
                                t => new Tuple<OfflineTypeInformation, Version>(
                                    new OfflineTypeInformation(t.Type.FullName, new AssemblyName(t.Type.AssemblyName)), 
                                    t.Version))
                            .ToArray();
                        commands[j] = new VersionedTypeFallback(types);
                    }

                    var notifications = new VersionedTypeFallback[serializedGroup.Notifications.Length];
                    for (int j = 0; j < serializedGroup.Notifications.Length; j++)
                    {
                        var typeFallBack = serializedGroup.Notifications[j];
                        var types = typeFallBack.Types.Select(
                                t => new Tuple<OfflineTypeInformation, Version>(
                                    new OfflineTypeInformation(t.Type.FullName, new AssemblyName(t.Type.AssemblyName)), 
                                    t.Version))
                            .ToArray();
                        notifications[j] = new VersionedTypeFallback(types);
                    }

                    groups[i] = new CommunicationSubjectGroup(
                        new CommunicationSubject(serializedGroup.Subject), 
                        commands,
                        notifications);
                }

                return new EndpointInteractionInformationMessage(data.Sender, groups);
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
            var msg = message as EndpointInteractionInformationMessage;
            if (msg == null)
            {
                throw new UnknownMessageTypeException();
            }

            try
            {
                var groups = new SerializedSubjectInformation[msg.SubjectGroups.Length];
                for (int i = 0; i < msg.SubjectGroups.Length; i++)
                {
                    var serializedGroup = msg.SubjectGroups[i];

                    var commands = new SerializedTypeFallback[serializedGroup.Commands.Length];
                    for (int j = 0; j < serializedGroup.Commands.Length; j++)
                    {
                        var typeFallback = serializedGroup.Commands[j];
                        var types = typeFallback.Select(
                                t => new SerializedVersionedType
                                    {
                                        Type = new SerializedType
                                            {
                                                FullName = t.Item1.TypeFullName,
                                                AssemblyName = t.Item1.AssemblyName.Name
                                            },
                                        Version = t.Item2,
                                    })
                            .ToArray();
                        commands[j] = new SerializedTypeFallback
                            {
                                Types = types,
                            };
                    }

                    var notifications = new SerializedTypeFallback[serializedGroup.Notifications.Length];
                    for (int j = 0; j < serializedGroup.Notifications.Length; j++)
                    {
                        var typeFallback = serializedGroup.Notifications[j];
                        var types = typeFallback.Select(
                                t => new SerializedVersionedType
                                    {
                                        Type = new SerializedType
                                            {
                                                FullName = t.Item1.TypeFullName,
                                                AssemblyName = t.Item1.AssemblyName.Name
                                            },
                                        Version = t.Item2,
                                    })
                            .ToArray();
                        notifications[j] = new SerializedTypeFallback
                            {
                                Types = types,
                            };
                    }

                    groups[i] = new SerializedSubjectInformation
                        {
                            Subject = serializedGroup.Subject.ToString(), 
                            Commands = commands,
                            Notifications = notifications
                        };
                }

                return new EndpointInteractionInformationData
                    {
                        Id = message.Id,
                        InResponseTo = message.InResponseTo,
                        Sender = message.Sender,
                        Groups = groups,
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
