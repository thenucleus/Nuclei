//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Defines what the connection owner 'thinks' of the given connection.
    /// </summary>
    internal enum InteractionConnectionState
    {
        /// <summary>
        /// Indicates that the state of the connection is unknown.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the owner considers the connection useful and will maintain it.
        /// </summary>
        Desired,

        /// <summary>
        /// Indicates that the owner considers the connection neither useful nor harmful and will
        /// maintain it if desired by the other side.
        /// </summary>
        Neutral,

        /// <summary>
        /// Indicates that the owner considers the connection harmful and will close it.
        /// </summary>
        Denied,
    }
}
