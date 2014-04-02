//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for objects that monitor the state of connections to remote endpoints.
    /// </summary>
    public interface IMonitorConnections
    {
        /// <summary>
        /// Sets the function that is used to provide a keep-alive message with
        /// custom data.
        /// </summary>
        /// <param name="keepAliveCustomDataBuilder">
        /// The function that is used to set the custom data for a keep-alive message.
        /// </param>
        void OnKeepAlive(KeepAliveCustomDataBuilder keepAliveCustomDataBuilder);

        /// <summary>
        /// Sets the function that is used to channel custom data across with each
        /// keep-alive message.
        /// </summary>
        /// <param name="keepAliveCustomDataBuilder">
        /// The function that is used to gather the custom data that should be provided
        /// upon a keep-alive response.
        /// </param>
        void OnKeepAliveRepondWith(KeepAliveResponseCustomDataBuilder keepAliveCustomDataBuilder);
    }
}
