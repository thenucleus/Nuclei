//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Net.Security;
using System.ServiceModel;
using Nuclei.Communication.Protocol.V1.DataObjects;
using ProtoBuf;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Defines the methods for receiving data on the receiving side of the communication channel.
    /// </summary>
    /// <design>
    /// Note that this interface is NOT an <c>IDataPipe</c> because we only want to share the
    /// members defined below with the remote endpoint.
    /// </design>
    [ServiceContract]
    internal interface IDataReceivingEndpoint : IReceiveInformationFromRemoteEndpoints
    {
        /// <summary>
        /// Accepts the stream.
        /// </summary>
        /// <param name="data">The data message that allows a data stream to be transferred.</param>
        [OperationContract(
            IsOneWay = true,
            IsInitiating = true,
            IsTerminating = false,
            ProtectionLevel = ProtectionLevel.None)]
        void AcceptStream(StreamData data);
    }
}
