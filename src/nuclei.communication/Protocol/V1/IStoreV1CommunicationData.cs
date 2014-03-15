//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Defines the interface for objects that store communication data in accordance with version 1.0 of the
    /// communication protocol.
    /// </summary>
    internal interface IStoreV1CommunicationData
    {
        /// <summary>
        /// Gets a value indicating the ID number of the message.
        /// </summary>
        MessageId Id
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the message to which 
        /// the current message is a response.
        /// </summary>
        MessageId InResponseTo
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating the ID number of the endpoint that 
        /// send the current message.
        /// </summary>
        EndpointId Sender
        {
            get;
        }
    }
}
