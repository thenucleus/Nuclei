//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Autofac;
using Nuclei.Communication.Interaction.V1.Protocol.V1.DataObjects.Converters;
using Nuclei.Communication.Protocol.V1;

namespace Nuclei.Communication
{
    /// <content>
    /// Defines the component registrations for the V1 part of the interaction namespace.
    /// </content>
    public sealed partial class CommunicationModule
    {
        private static void RegisterInteractionV1ProtocolV1MessageConverters(ContainerBuilder builder)
        {
            builder.Register(c => new CommandInvocationConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new CommandInvocationResponseConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new EndpointInteractionInformationConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new EndpointInteractionInformationResponseConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new NotificationRegistrationConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new NotificationUnregistrationConverter())
                .As<IConvertCommunicationMessages>();

            builder.Register(c => new NotificationRaisedConverter())
                .As<IConvertCommunicationMessages>();
        }
    }
}
