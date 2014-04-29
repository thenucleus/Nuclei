//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines the origin of a command parameter.
    /// </summary>
    internal enum CommandParameterOrigin
    {
        /// <summary>
        /// The command parameter has no origin.
        /// </summary>
        None,

        /// <summary>
        /// The command parameter is provided by the original command interface method.
        /// </summary>
        FromCommand,

        /// <summary>
        /// The command parameter is the endpoint ID of the endpoint that requested the 
        /// invocation of the command.
        /// </summary>
        InvokingEndpointId,

        /// <summary>
        /// The command parameter is the message ID of the message that requested the 
        /// invocation of the command.
        /// </summary>
        InvokingMessageId,

        /// <summary>
        /// The command parameter has an unknown origin.
        /// </summary>
        Unknown,
    }
}
