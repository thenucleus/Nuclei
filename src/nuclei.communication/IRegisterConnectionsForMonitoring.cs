//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication
{
    /// <summary>
    /// The interface for objects that register connections for monitoring.
    /// </summary>
    internal interface IRegisterConnectionsForMonitoring
    {
        /// <summary>
        /// Registers a new connection for monitoring.
        /// </summary>
        /// <param name="connectionHandler">The object that handles the connections.</param>
        void Register(IHandleConnections connectionHandler);

        /// <summary>
        /// Unregisters a connection from monitoring.
        /// </summary>
        /// <param name="connectionHandler">The object that handles the connections.</param>
        void Unregister(IHandleConnections connectionHandler);
    }
}
