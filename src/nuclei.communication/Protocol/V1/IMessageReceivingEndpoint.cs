//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Net.Security;
using System.ServiceModel;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Defines the methods for receiving messages on the receiving side of the communication channel.
    /// </summary>
    /// <design>
    /// Note that this interface is NOT an <c>IMessagePipe</c> because we only want to share the
    /// members defined below with the remote endpoint.
    /// </design>
    [ServiceContract]
    internal interface IMessageReceivingEndpoint : IReceiveInformationFromRemoteEndpoints
    {
        /// <summary>
        /// Accepts the messages.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <design>
        /// At the moment we use a binary serializer. At some point we should switch this over to 
        /// use the 'Protocol Buffers' approach provided here: http://code.google.com/p/protobuf-net/
        /// Using the Protocol Buffers should provide us with a better way of providing versioning
        /// etc. of messages and data.
        /// </design>
        [UseNetDataContractSerializer]
        [OperationContract(
            IsOneWay = true, 
            IsInitiating = true, 
            IsTerminating = false, 
            ProtectionLevel = ProtectionLevel.None)]
        void AcceptMessage(ICommunicationMessage message);
    }
}
