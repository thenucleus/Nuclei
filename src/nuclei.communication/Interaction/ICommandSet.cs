//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines the base methods for classes that implement a set of commands 
    /// that can be invoked remotely through a <see cref="RemoteCommandHub"/>.
    /// </summary>
    /// <design>
    /// The <see cref="RemoteCommandHub"/> will generate a proxy object for all the command sets
    /// available on a given endpoint.
    /// </design>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces",
        Justification = "This interface is used as marker interface for sets of commands.")]
    public interface ICommandSet
    {
    }
}
