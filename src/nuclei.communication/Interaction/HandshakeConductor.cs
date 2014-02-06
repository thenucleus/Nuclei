using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Nuclei.Communication.Interaction.Transport.Messages;

namespace Nuclei.Communication.Interaction
{
    internal sealed class HandshakeConductor
    {
        /// <summary>
        /// The object that stores information about the known endpoints.
        /// </summary>
        private readonly IStoreInformationAboutEndpoints m_EndpointInformationStorage;

        /// <summary>
        /// The collection that contains all the registered commands.
        /// </summary>
        private readonly ICommandCollection m_Commands;

        /// <summary>
        /// The collection that contains all the registered notifications.
        /// </summary>
        private readonly INotificationCollection m_Notifications;

        /// <summary>
        /// The function which sends the <see cref="EndpointInteractionInformationMessage"/> to the owning endpoint.
        /// </summary>
        private readonly Action<CommandInvokedData> m_TransmitInteractionInformation;

        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakeConductor"/> class.
        /// </summary>
        /// <param name="endpointInformationStorage">The object that stores information about the known endpoints.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="endpointInformationStorage"/> is <see langword="null" />.
        /// </exception>
        public HandshakeConductor(IStoreInformationAboutEndpoints endpointInformationStorage)
        {
            {
                Lokad.Enforce.Argument(() => endpointInformationStorage);
            }

            m_EndpointInformationStorage = endpointInformationStorage;
            m_EndpointInformationStorage.OnEndpointConnected += HandleEndpointSignIn;
        }

        private void HandleEndpointSignIn(object sender, EndpointEventArgs e)
        {
            var commandInformation = new List<OfflineTypeInformation>();
            foreach (var pair in m_Commands)
            {
                commandInformation.Add(
                    new OfflineTypeInformation(
                        pair.Item1.FullName,
                        pair.Item1.Assembly.GetName()));
            }

            var notificationInformation = new List<OfflineTypeInformation>();
            foreach (var pair in m_Notifications)
            {
                notificationInformation.Add(
                    new OfflineTypeInformation(
                        pair.Item1.FullName,
                        pair.Item1.Assembly.GetName()));
            }
        }
    }
}
