//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics;
using Nuclei.Diagnostics.Profiling;

namespace Nuclei.Communication
{
    /// <summary>
    /// Stores the constant values related to communication.
    /// </summary>
    internal static class CommunicationConstants
    {
        /// <summary>
        /// The default prefix text for a log message.
        /// </summary>
        public const string DefaultLogTextPrefix = "Communication";

        /// <summary>
        /// The default post-fix string template used to indicate that a channel is a data channel.
        /// </summary>
        public const string DefaultDataAddressPostfixTemplate = @"{0}/Data";

        /// <summary>
        /// The default time-out in milliseconds for a binding to receive any messages.
        /// </summary>
        public const int DefaultBindingReceiveTimeoutInMilliSeconds = 30 * 60 * 1000;

        /// <summary>
        /// The default time-out in milliseconds that a binding will wait for a receipt confirmation
        /// message to be received by the message sending side.
        /// </summary>
        public const int DefaultBindingReceiveConfirmationTimeoutInMilliseconds = 1 * 1000;

        /// <summary>
        /// The default time-out that will be used while waiting for a response message from a remote endpoint.
        /// </summary>
        public const int DefaultWaitForResponseTimeoutInMilliSeconds = 10 * 1000;

        /// <summary>
        /// The default maximum size, in bytes, of the buffer used to store messages.
        /// </summary>
        public const int DefaultBindingMaxBufferSizeForMessagesInBytes = 65536;

        /// <summary>
        /// The default maximum size, in bytes, for a received message that is processed by the binding.
        /// </summary>
        public const long DefaultBindingMaxReceivedSizeForMessagesInBytes = 65536;

        /// <summary>
        /// The default string template for a named pipe channel URI.
        /// </summary>
        public const string DefaultNamedPipeChannelUriTemplate = @"net.pipe://localhost/nuclei.communication/pipe_{0}";

        /// <summary>
        /// The default string template for a named pipe address used for message reception.
        /// </summary>
        public const string DefaultNamedPipeAddressTemplate = "NamedPipe.Nuclei.Communication_{0}";

        /// <summary>
        /// The default maximum number of connections that a named pipe binding can handle.
        /// </summary>
        public const int DefaultMaximumNumberOfConnectionsForNamedPipes = 25;

        /// <summary>
        /// The default string template for a TCP/IP channel URI.
        /// </summary>
        public const string DefaultTcpIpChannelUriTemplate = @"net.tcp://{0}:{1}";

        /// <summary>
        /// The default string template for a TCP/IP address used for message reception.
        /// </summary>
        public const string DefaultTcpIpAddressTemplate = "Tcp.Nuclei.Communication_message_{0}";

        /// <summary>
        /// The default string template for the discovery entry address.
        /// </summary>
        public const string DefaultDiscoveryEntryAddressTemplate = "Discovery";

        /// <summary>
        /// The default maximum number of connections that a TCP/IP binding can handle.
        /// </summary>
        public const int DefaultMaximumNumberOfConnectionsForTcpIp = 25;

        /// <summary>
        /// The default interval for the maximum time between two (connection confirmation) messages.
        /// </summary>
        public const int DefaultKeepAliveIntervalInMilliseconds = 15000;

        /// <summary>
        /// The default value for the maximum number of missed connection confirmations before
        /// a connection to a remote endpoint is considered disconnected.
        /// </summary>
        public const int DefaultMaximumNumberOfMissedKeepAliveSignals = 10;

        /// <summary>
        /// The default value for the maximum time that is allowed to expire between
        /// to connection confirmations.
        /// </summary>
        public const int DefaultMaximumTimeInMillisecondsBetweenConnectionConfirmations = 60 * 1000;

        /// <summary>
        /// The default value for the maximum number of times a message is send if previous attempts fail.
        /// </summary>
        public const int DefaultMaximuNumberOfRetriesForMessageSending = 3;

        /// <summary>
        /// The timing group used for the profiling of the communication system.
        /// </summary>
        private static readonly TimingGroup s_TimingGroup = new TimingGroup();

        /// <summary>
        /// Gets the timing group used for the profiling of the communication system.
        /// </summary>
        internal static TimingGroup TimingGroup
        {
            [DebuggerStepThrough]
            get
            {
                return s_TimingGroup;
            }
        }
    }
}
