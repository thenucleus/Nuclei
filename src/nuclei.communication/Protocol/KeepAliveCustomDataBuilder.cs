//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// A delegate used to provide custom data for a keep-alive message.
    /// </summary>
    /// <returns>The state information for the keep-alive message.</returns>
    public delegate object KeepAliveCustomDataBuilder();
}
