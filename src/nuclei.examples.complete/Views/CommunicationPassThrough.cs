//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;
using Nuclei.Communication.Protocol;

namespace Nuclei.Examples.Complete.Views
{
    /// <summary>
    /// Forwards communication commands to the communication layer.
    /// </summary>
    internal sealed class CommunicationPassThrough : IHandleCommunication
    {
        /// <summary>
        /// The object that sends commands to the remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Commands;

        /// <summary>
        /// The object that keeps track of files registered for upload.
        /// </summary>
        private readonly IStoreUploads m_Uploads;

        /// <summary>
        /// The object that holds the notifications.
        /// </summary>
        private readonly TestNotifications m_LocalNotifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationPassThrough"/> class.
        /// </summary>
        /// <param name="commands">The object that sends commands to the remote endpoints.</param>
        /// <param name="uploads">The object that tracks files registered for upload.</param>
        /// <param name="localNotifications">The object that holds the notifications.</param>
        public CommunicationPassThrough(
            ISendCommandsToRemoteEndpoints commands, 
            IStoreUploads uploads,
            TestNotifications localNotifications)
        {
            m_Commands = commands;
            m_Uploads = uploads;
            m_LocalNotifications = localNotifications;
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has been
        /// activated.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Returns a value indicating if connection information is available for
        /// the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>
        /// <see langword="true" /> if connection information is available for the given endpoint;
        /// otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool CanContactEndpoint(EndpointId endpoint)
        {
            return m_Commands.HasCommandFor(endpoint, typeof(ITestCommandSet));
        }

        /// <summary>
        /// Sends an echo message to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="messageText">The text of the message.</param>
        public void SendEchoMessageTo(EndpointId endpoint, string messageText)
        {
            if (m_Commands.HasCommandFor(endpoint, typeof(ITestCommandSet)))
            {
                var commands = m_Commands.CommandsFor<ITestCommandSet>(endpoint);
                commands.Echo(messageText, 10, 15 * 1000);
            }
        }

        /// <summary>
        /// Sends a message to the given endpoint with the request to add the numbers.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the message should be send.</param>
        /// <param name="first">The first number.</param>
        /// <param name="second">The second number.</param>
        /// <returns>The result of the addition.</returns>
        public Task<int> AddNumbers(EndpointId endpoint, int first, int second)
        {
            if (!m_Commands.HasCommandFor(endpoint, typeof(ITestCommandSet)))
            {
                return Task<int>.Factory.StartNew(() => int.MinValue);
            }

            var commands = m_Commands.CommandsFor<ITestCommandSet>(endpoint);
            var result = commands.Calculate(first, second);

            return result;
        }

        /// <summary>
        /// Sends a data stream to the given endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint to which the data should be send.</param>
        /// <param name="dataText">The text.</param>
        public void SendDataTo(EndpointId endpoint, string dataText)
        {
            if (m_Commands.HasCommandFor(endpoint, typeof(ITestCommandSet)))
            {
                var path = Path.Combine(Assembly.GetExecutingAssembly().LocalDirectoryPath(), Path.GetRandomFileName());
                using (var writer = new StreamWriter(path, false))
                {
                    writer.Write(dataText);
                }

                var token = m_Uploads.Register(path);

                var commands = m_Commands.CommandsFor<ITestCommandSet>(endpoint);
                commands.StartDownload(token);
            }
        }

        /// <summary>
        /// Sends a notification to the given endpoint.
        /// </summary>
        public void Notify()
        {
            m_LocalNotifications.RaiseOnNotify();
        }
    }
}
