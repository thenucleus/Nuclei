//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines an attribute that indicates that an <see cref="IMessageProcessAction"/> class
    /// provides a last chance for processing a message before the message is destroyed 
    /// without processing it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class LastChanceMessageHandlerAttribute : Attribute
    {
    }
}
