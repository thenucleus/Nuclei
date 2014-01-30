//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Profiling;

namespace Nuclei.Communication.Protocol.Messages.Processors
{
    /// <summary>
    /// Defines the action that processes an <see cref="EndpointDisconnectMessage"/>.
    /// </summary>
    internal sealed class EndpointDisconnectProcessAction : IMessageProcessAction
    {
        /// <summary>
        /// The object that stores the endpoint information for the application.
        /// </summary>
        private readonly IStoreEndpointApprovalState m_EndpointStorage;

        /// <summary>
        /// The object that provides the diagnostics methods.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointDisconnectProcessAction"/> class.
        /// </summary>
        /// <param name="endpointStorage">The object that stores information about all the known endpoints.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointStorage"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public EndpointDisconnectProcessAction(
            IStoreEndpointApprovalState endpointStorage,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => endpointStorage);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_EndpointStorage = endpointStorage;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Gets the message type that can be processed by this filter action.
        /// </summary>
        /// <value>The message type to process.</value>
        public Type MessageTypeToProcess
        {
            get
            {
                return typeof(EndpointDisconnectMessage);
            }
        }

        /// <summary>
        /// Invokes the current action based on the provided message.
        /// </summary>
        /// <param name="message">The message upon which the action acts.</param>
        public void Invoke(ICommunicationMessage message)
        {
            var msg = message as EndpointDisconnectMessage;
            if (msg == null)
            {
                Debug.Assert(false, "The message is of the incorrect type.");
                return;
            }

            using (m_Diagnostics.Profiler.Measure(CommunicationConstants.TimingGroup, "Endpoint disconnecting"))
            {
                m_EndpointStorage.TryRemoveEndpoint(message.Sender);
            }
        }
    }
}
