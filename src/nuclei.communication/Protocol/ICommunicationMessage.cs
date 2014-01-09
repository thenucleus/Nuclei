//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the base methods for communication methods.
    /// </summary>
    public interface ICommunicationMessage
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
        EndpointId OriginatingEndpoint
        {
            get;
        }
    }
}
