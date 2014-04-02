//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication
{
    /// <summary>
    /// A delegate used to provide custom data for a keep-alive response message.
    /// </summary>
    /// <param name="state">The state information that was contained in the keep-alive message.</param>
    /// <returns>The response to the keep-alive state.</returns>
    public delegate object KeepAliveResponseCustomDataBuilder(object state);
}
