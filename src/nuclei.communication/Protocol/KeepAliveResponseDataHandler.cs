//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// A delegate used to return the custom data from a keep-alive response message.
    /// </summary>
    /// <param name="state">The state information that was contained in the keep-alive response message.</param>
    public delegate void KeepAliveResponseDataHandler(object state);
}
