//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Nuclei.Communication;
using Nuclei.Communication.Interaction;

namespace Nuclei.Examples.Complete.Views
{
    /// <summary>
    /// Forwards communication commands to the communication layer.
    /// </summary>
    internal sealed class CommunicationPassThrough : IHandleCommunication
    {
        /// <summary>
        /// The communication layer which does the actual communication work.
        /// </summary>
        private readonly ICommunicationLayer m_Layer;

        /// <summary>
        /// The object that sends commands to the remote endpoints.
        /// </summary>
        private readonly ISendCommandsToRemoteEndpoints m_Commands;

        /// <summary>
        /// The object that keeps track of files registered for upload.
        /// </summary>
        private readonly IStoreUploads m_Uploads;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationPassThrough"/> class.
        /// </summary>
        /// <param name="layer">The communication layer which does the actual communication work.</param>
        /// <param name="commands">The object that sends commands to the remote endpoints.</param>
        /// <param name="uploads">The object that tracks files registered for upload.</param>
        public CommunicationPassThrough(ICommunicationLayer layer, ISendCommandsToRemoteEndpoints commands, IStoreUploads uploads)
        {
            m_Layer = layer;
            m_Commands = commands;
            m_Uploads = uploads;
        }

        /// <summary>
        /// Gets a value indicating whether the communication layer has been
        /// activated.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return m_Layer.IsSignedIn;
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
            return m_Layer.IsEndpointContactable(endpoint) && m_Commands.HasCommandsFor(endpoint);
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
                commands.Echo(messageText);
            }
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
                commands.StartDownload(m_Layer.Id, token);
            }
        }

        /// <summary>
        /// Closes the connections to all endpoints.
        /// </summary>
        public void Close()
        {
            m_Layer.SignOut();
        }
    }
}
