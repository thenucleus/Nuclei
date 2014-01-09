//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Net.Security;
using System.ServiceModel;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the methods for receiving data on the receiving side of the communication channel.
    /// </summary>
    [ServiceContract]
    internal interface IDataReceivingEndpoint : IReceiveInformationFromRemoteEndpoints
    {
        /// <summary>
        /// Accepts the stream.
        /// </summary>
        /// <param name="data">The data message that allows a data stream to be transferred.</param>
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
        void AcceptStream(DataTransferMessage data);
    }
}
