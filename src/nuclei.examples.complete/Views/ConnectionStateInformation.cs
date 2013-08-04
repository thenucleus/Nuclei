//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.ObjectModel;

namespace Nuclei.Examples.Complete.Views
{
    /// <summary>
    /// Stores information about the currently known and active connections.
    /// </summary>
    internal sealed class ConnectionStateInformation
    {
        /// <summary>
        /// Gets a collection containing information about all the known endpoints.
        /// </summary>
        public ObservableCollection<object> KnownEndpoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a collection containing information about all the endpoints to which the
        /// current application is connected.
        /// </summary>
        public ObservableCollection<object> ConnectedEndpoints
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a collection containing all the messages that were send from the currently
        /// connected endpoints.
        /// </summary>
        public ObservableCollection<object> EndpointMessages
        {
            get;
            private set;
        }
    }
}
