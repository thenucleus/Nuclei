//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines the interface for objects that pipe data from one object to another object.
    /// </summary>
    internal interface IDataPipe : IReceiveInformationFromRemoteEndpoints
    {
        /// <summary>
        /// An event raised when a new data message is available in the pipe.
        /// </summary>
        event EventHandler<DataTransferEventArgs> OnNewData;
    }
}
