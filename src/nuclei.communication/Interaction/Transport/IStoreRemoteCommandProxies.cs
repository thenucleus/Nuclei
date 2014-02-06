//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Defines the interface for objects that store command proxies.
    /// </summary>
    internal interface IStoreRemoteCommandProxies
    {
        /// <summary>
        /// Handles the reception of new command types.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that owns the commands.</param>
        /// <param name="commandTypes">An array containing the command types for a given endpoint.</param>
        void OnReceiptOfEndpointCommands(EndpointId endpoint, OfflineTypeInformation[] commandTypes);
    }
}