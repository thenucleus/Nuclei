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
    /// Maps an endpoint to a set of registered commands.
    /// </summary>
    public sealed class CommandInformationPerEndpoint
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandInformationPerEndpoint"/> class.
        /// </summary>
        /// <param name="endpoint">The ID of the endpoint.</param>
        /// <param name="commands">The collection that describes all the registered commands.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="commands"/> is <see langword="null" />.
        /// </exception>
        public CommandInformationPerEndpoint(EndpointId endpoint, IEnumerable<Type> commands)
        {
            {
                Lokad.Enforce.Argument(() => endpoint);
                Lokad.Enforce.Argument(() => commands);
            }

            Endpoint = endpoint;
            RegisteredCommands = commands;
        }

        /// <summary>
        /// Gets the ID number of the endpoint.
        /// </summary>
        public EndpointId Endpoint
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection that describes all the 
        /// registered commands for the given endpoint.
        /// </summary>
        public IEnumerable<Type> RegisteredCommands
        {
            get;
            private set;
        }
    }
}
