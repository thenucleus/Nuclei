//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Description;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication
{
    /// <summary>
    /// Handles the creation and keeping alive of a <see cref="ServiceHost"/> connection.
    /// </summary>
    internal sealed class ServiceConnectionHolder : IHoldServiceConnections
    {
        /// <summary>
        /// The maximum number of faults that are allowed to occur inside a given time span.
        /// </summary>
        private const int MaximumNumberOfSequentialFaults = 10;

        /// <summary>
        /// The number of minutes over which the number of faults is not allowed
        /// to exceed the <see cref="MaximumNumberOfSequentialFaults"/>.
        /// </summary>
        private const int FaultingTimeSpanInMinutes = 1;

        /// <summary>
        /// The collection that contains the times at which faults occurred.
        /// </summary>
        /// <remarks>
        /// We always clear out the times that are further away than the <see cref="FaultingTimeSpanInMinutes"/>
        /// so there should never be more than <see cref="MaximumNumberOfSequentialFaults"/> entries in the 
        /// storage. Except when we push the one fault time in that will push us over the limit. Hence we
        /// reserve space for that one more entry.
        /// </remarks>
        private readonly List<DateTimeOffset> m_LatestFaultingTimes =
            new List<DateTimeOffset>(MaximumNumberOfSequentialFaults + 1);

        /// <summary>
        /// Indicates the type of channel that we're dealing with and provides
        /// utility methods for the channel.
        /// </summary>
        private readonly IChannelTemplate m_Template;

        /// <summary>
        /// The function that returns the current time.
        /// </summary>
        private readonly Func<DateTimeOffset> m_CurrentTime;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// The host that maintains the network connection.
        /// </summary>
        private ServiceHost m_Host;

        /// <summary>
        /// The event handler used when the host faults.
        /// </summary>
        private EventHandler m_HostFaultingHandler;

        /// <summary>
        /// The event handler used when the host closes.
        /// </summary>
        private EventHandler m_HostClosedHandler;

        /// <summary>
        /// The object used to receive messages over the network.
        /// </summary>
        private IReceiveInformationFromRemoteEndpoints m_Receiver;

        /// <summary>
        /// The function that attaches an endpoint to the service host.
        /// </summary>
        private Func<ServiceHost, ServiceEndpoint> m_AttachEndpoint;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConnectionHolder"/> class.
        /// </summary>
        /// <param name="channelTemplate">The type of channel, e.g. TCP.</param>
        /// <param name="currentTime">The function that returns the current time.</param>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="channelTemplate"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="currentTime"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public ServiceConnectionHolder(
            IChannelTemplate channelTemplate,
            Func<DateTimeOffset> currentTime,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => channelTemplate);
                Lokad.Enforce.Argument(() => currentTime);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Template = channelTemplate;
            m_CurrentTime = currentTime;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Opens the channel and provides information on how to connect to the given channel.
        /// </summary>
        /// <param name="receiver">The object that receives the transmissions from the remote endpoint.</param>
        /// <param name="attachEndpoint">The function that attaches an endpoint to the service host.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="receiver"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="attachEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <returns>The URL of the newly opened channel.</returns>
        public Uri OpenChannel(IReceiveInformationFromRemoteEndpoints receiver, Func<ServiceHost, ServiceEndpoint> attachEndpoint)
        {
            {
                Lokad.Enforce.Argument(() => receiver);
                Lokad.Enforce.Argument(() => attachEndpoint);
            }

            if (m_Receiver != null)
            {
                CloseConnection();
            }

            m_Receiver = receiver;
            m_AttachEndpoint = attachEndpoint;

            var uri = m_Template.GenerateNewChannelUri();
            return ReopenChannel(uri);
        }

        private Uri ReopenChannel(Uri uri)
        {
            // Clear the old host
            CleanupHost();

            // Create the new host
            m_HostFaultingHandler = (s, e) =>
            {
                m_Diagnostics.Log(
                    LevelToLog.Warn,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Channel for address: {0} has faulted.",
                        uri));

                StoreCurrentTimeAsFaultingTime();
                ReopenChannel(uri);
            };

            m_HostClosedHandler = (s, e) =>
            {
                m_Diagnostics.Log(
                    LevelToLog.Warn,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Channel for address: {0} has closed prematurely.",
                        uri));

                ReopenChannel(uri);
            };

            m_Host = new ServiceHost(m_Receiver, uri);
            m_Host.Faulted += m_HostFaultingHandler;
            m_Host.Closed += m_HostClosedHandler;

            var endpoint = m_AttachEndpoint(m_Host);
            m_Host.Open();

            m_Diagnostics.Log(
                LevelToLog.Trace,
                CommunicationConstants.DefaultLogTextPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Opened channel on address: {0} on: {1}",
                    uri,
                    endpoint.Address.Uri));

            return endpoint.Address.Uri;
        }

        private void StoreCurrentTimeAsFaultingTime()
        {
            var now = m_CurrentTime();
            m_LatestFaultingTimes.Add(now);

            var timespan = new TimeSpan(0, FaultingTimeSpanInMinutes, 0);
            while (m_LatestFaultingTimes[0] < now - timespan)
            {
                m_LatestFaultingTimes.RemoveAt(0);
            }

            if (m_LatestFaultingTimes.Count > MaximumNumberOfSequentialFaults)
            {
                throw new MaximumNumberOfChannelRestartsExceededException();
            }
        }

        private void CleanupHost()
        {
            if (m_Host != null)
            {
                m_Host.Faulted -= m_HostFaultingHandler;
                m_HostFaultingHandler = null;

                m_Host.Closed -= m_HostClosedHandler;
                m_HostClosedHandler = null;

                try
                {
                    m_Host.Close();
                }
                catch (TimeoutException)
                {
                    // The default interval of time that was allotted for the operation was exceeded
                    // before the operation was completed.
                    m_Host.Abort();
                }
                catch (InvalidOperationException)
                {
                    // EITHER:
                    // The communication object is in a System.ServiceModel.CommunicationState.Closing
                    // or System.ServiceModel.CommunicationState.Closed state and cannot be modified.
                    //
                    // OR
                    //
                    // The communication object is not in a System.ServiceModel.CommunicationState.Opened
                    // or System.ServiceModel.CommunicationState.Opening state and cannot be modified.
                    m_Host.Abort();
                }
                catch (CommunicationObjectFaultedException)
                {
                    // The communication object is in a System.ServiceModel.CommunicationState.Faulted
                    // state and cannot be modified.
                    m_Host.Abort();
                }

                var disposable = m_Host as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                m_Host = null;
            }
        }

        /// <summary>
        /// Closes the current connection.
        /// </summary>
        public void CloseConnection()
        {
            CleanupHost();

            if (m_Receiver != null)
            {
                m_Receiver = null;
                m_AttachEndpoint = null;
            }

            m_Diagnostics.Log(
                LevelToLog.Trace, 
                CommunicationConstants.DefaultLogTextPrefix, 
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Closed channel of type: {0}",
                    m_Template.GetType().Name));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CleanupHost();
        }
    }
}
