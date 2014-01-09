//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines a <see cref="EventArgs"/> class that stores information about the availability of commands for
    /// a communication endpoint.
    /// </summary>
    public sealed class CommandSetAvailabilityEventArgs : EventArgs
    {
        /// <summary>
        /// The endpoint ID for the endpoint that has provided a set of commands.
        /// </summary>
        private readonly EndpointId m_Endpoint;

        /// <summary>
        /// The commands that were provided by the endpoint.
        /// </summary>
        private readonly IEnumerable<Type> m_Commands;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandSetAvailabilityEventArgs"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint that has provided a set of commands.</param>
        /// <param name="commands">The commands that were provided.</param>
        public CommandSetAvailabilityEventArgs(EndpointId endpoint, IEnumerable<Type> commands)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => commands);
            }

            m_Endpoint = endpoint;
            m_Commands = commands;
        }

        /// <summary>
        /// Gets the ID of the endpoint that provided the commands.
        /// </summary>
        public EndpointId Endpoint
        {
            get
            {
                return m_Endpoint;
            }
        }

        /// <summary>
        /// Gets the collection containing the command types that the endpoint provided.
        /// </summary>
        public IEnumerable<Type> Commands
        {
            get
            {
                return m_Commands;
            }
        }
    }
}
