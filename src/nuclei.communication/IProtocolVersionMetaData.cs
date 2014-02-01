//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the interface for strong-typed meta data describing an versioned protocol element.
    /// </summary>
    internal interface IProtocolVersionMetaData
    {
        /// <summary>
        /// Gets the version of the discovery protocol that the attached
        /// endpoint can handle.
        /// </summary>
        Version Version
        {
            get;
        }
    }
}