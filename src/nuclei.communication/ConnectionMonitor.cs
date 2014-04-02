//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Nuclei.Communication.Protocol;
using Nuclei.Configuration;
using Timer = System.Timers.Timer;

namespace Nuclei.Communication
{
    internal sealed class ConnectionMonitor : IRegisterConnectionsForMonitoring, IMonitorConnections
    {
        /// <summary>
        /// Stores the connection data for a given connection.
        /// </summary>
        private sealed class ConnectionMap
        {
            /// <summary>
            /// Gets or sets the next point in time that the connection should be tested.
            /// </summary>
            public DateTimeOffset NextConnectionTime
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the number of missed keep-alive signals.
            /// </summary>
            public int NumberOfMissedKeepAliveSignals
            {
                get;
                set;
            }
        }

        /// <summary>
        /// The object used to lock on.
        /// </summary>
        private readonly object m_Lock = new object();

        /// <summary>
        /// The collection of connections that are being monitored.
        /// </summary>
        private readonly Dictionary<IHandleConnections, ConnectionMap> m_RegisteredConnections
            = new Dictionary<IHandleConnections, ConnectionMap>();

        /// <summary>
        /// The collection that contains all the known endpoints.
        /// </summary>
        private readonly IStoreInformationAboutEndpoints m_Endpoints;

        /// <summary>
        /// The function that is used to get the current time and date.
        /// </summary>
        private readonly Func<DateTimeOffset> m_Now;

        /// <summary>
        /// The timer which is used to signal the next keep-alive moment.
        /// </summary>
        private readonly Timer m_Timer;

        /// <summary>
        /// The maximum number of keep-alive signals that any connection is allowed to miss
        /// in sequence.
        /// </summary>
        private readonly int m_MaximumNumberOfMissedKeepAliveSignals;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionMonitor"/> class.
        /// </summary>
        /// <param name="endpoints">The collection that contains all the known endpoints.</param>
        /// <param name="now">The function that is used to get the current time and date.</param>
        /// <param name="configuration">The object that stores the configuration for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpoints"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="now"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        public ConnectionMonitor(IStoreInformationAboutEndpoints endpoints, Func<DateTimeOffset> now, IConfiguration configuration)
        {
            {
                Lokad.Enforce.Argument(() => endpoints);
                Lokad.Enforce.Argument(() => now);
                Lokad.Enforce.Argument(() => configuration);
            }

            m_Endpoints = endpoints;
            m_Now = now;

            m_MaximumNumberOfMissedKeepAliveSignals = configuration.HasValueFor(CommunicationConfigurationKeys.MaximumNumberOfMissedKeepAliveSignals)
                ? configuration.Value<int>(CommunicationConfigurationKeys.MaximumNumberOfMissedKeepAliveSignals)
                : CommunicationConstants.DefaultMaximumNumberOfMissedKeepAliveSignals;

            var keepAliveIntervalInMilliseconds = configuration.HasValueFor(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds)
                ? configuration.Value<int>(CommunicationConfigurationKeys.KeepAliveIntervalInMilliseconds)
                : CommunicationConstants.DefaultKeepAliveIntervalInMilliseconds;

            m_Timer = new Timer(keepAliveIntervalInMilliseconds);
            m_Timer.Elapsed += HandleKeepAliveIntervalElapsed;
            m_Timer.AutoReset = true;
        }

        private void HandleKeepAliveIntervalElapsed(object sender, ElapsedEventArgs e)
        {
            var now = m_Now();
            if (Monitor.TryEnter(m_Lock))
            {
                try
                {
                    var maps = m_RegisteredConnections
                        .Where(p => p.Value.NextConnectionTime < now)
                        .Select(p => p.Value)
                        .ToList();

                    foreach (var map in maps)
                    {
                        // Send ping message with custom data
                        // Wait for response, time-out after x time (requires #7?)
                        throw new NotImplementedException();
                    }
                }
                finally
                {
                    Monitor.Exit(m_Lock);
                }
            }
        }

        /// <summary>
        /// Registers a new connection for monitoring.
        /// </summary>
        /// <param name="connectionHandler">The object that handles the connections.</param>
        public void Register(IHandleConnections connectionHandler)
        {
            // For every received message / send message reset the clock
            //   Hook up to the IProtocolChannel.OnMessageReception and the
            //     IProtocolChannel.OnDataReception events to be notified each time
            //     something happens there. 
            //   Also need to be notified on send
            //
            // NOTE: we do not want to block the receive or send, so we should just
            // grab the endpoint ID and queue it with a time, if a newer value comes in
            // then we toss the last one and replace it with the new one
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unregisters a connection from monitoring.
        /// </summary>
        /// <param name="connectionHandler">The object that handles the connections.</param>
        public void Unregister(IHandleConnections connectionHandler)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the function that is used to provide a keep-alive message with
        /// custom data.
        /// </summary>
        /// <param name="keepAliveCustomDataBuilder">
        /// The function that is used to set the custom data for a keep-alive message.
        /// </param>
        public void OnKeepAlive(KeepAliveCustomDataBuilder keepAliveCustomDataBuilder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the function that is used to channel custom data across with each
        /// keep-alive message.
        /// </summary>
        /// <param name="keepAliveCustomDataBuilder">
        /// The function that is used to gather the custom data that should be provided
        /// upon a keep-alive response.
        /// </param>
        public void OnKeepAliveRepondWith(KeepAliveResponseCustomDataBuilder keepAliveCustomDataBuilder)
        {
            throw new NotImplementedException();
        }
    }
}
