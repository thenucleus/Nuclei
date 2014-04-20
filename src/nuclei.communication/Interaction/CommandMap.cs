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
    /// Maps the methods of a <see cref="ICommandSet"/> to one or more delegates.
    /// </summary>
    public sealed class CommandMap
    {
        /// <summary>
        /// Returns the IDs for the command methods on a <see cref="ICommandSet"/> interface.
        /// </summary>
        /// <returns>The IDs for the command methods.</returns>
        public IEnumerable<CommandId> Commands()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the delegate that should be invoked if the command with the given ID is
        /// executed.
        /// </summary>
        /// <param name="id">The ID of the command.</param>
        /// <returns>The delegate that should be invoked if the command with the given ID is executed.</returns>
        public Delegate ToExecute(CommandId id)
        {
            throw new NotImplementedException();
        }
    }
}