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
    /// Defines the delegate that is used to return the mapping of required notifications to their subject groups.
    /// </summary>
    /// <returns>The collection containing the mapping between the required notifications and their subject groups.</returns>
    public delegate IEnumerable<Tuple<Type, SubjectGroupIdentifier[]>> RequiredNotificationsMappedBySubject();
}
