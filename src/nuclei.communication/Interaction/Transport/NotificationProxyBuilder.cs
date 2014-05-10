//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using Castle.DynamicProxy;
using Nuclei.Communication.Interaction.Transport.Messages;
using Nuclei.Communication.Properties;
using Nuclei.Communication.Protocol;
using Nuclei.Diagnostics;

namespace Nuclei.Communication.Interaction.Transport
{
    /// <summary>
    /// Builds proxy objects for the <see cref="RemoteNotificationHub"/>.
    /// </summary>
    internal sealed class NotificationProxyBuilder
    {
        /// <summary>
        /// The generator that will create the proxy objects.
        /// </summary>
        private readonly ProxyGenerator m_Generator = new ProxyGenerator();

        /// <summary>
        /// The ID of the local endpoint.
        /// </summary>
        private readonly EndpointId m_Local;

        /// <summary>
        /// The function which sends the message to the owning endpoint.
        /// </summary>
        private readonly SendMessage m_SendWithoutResponse;

        /// <summary>
        /// The object that provides the diagnostic methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationProxyBuilder"/> class.
        /// </summary>
        /// <param name="localEndpoint">The ID number of the local endpoint.</param>
        /// <param name="sendWithoutResponse">
        ///     The function that sends out a message to the given endpoint.
        /// </param>
        /// <param name="systemDiagnostics">The object that provides the diagnostic methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="localEndpoint"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendWithoutResponse"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public NotificationProxyBuilder(
            EndpointId localEndpoint,
            SendMessage sendWithoutResponse,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => localEndpoint);
                Lokad.Enforce.Argument(() => sendWithoutResponse);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Local = localEndpoint;
            m_SendWithoutResponse = sendWithoutResponse;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The interface of the command set for which a proxy must be made.</typeparam>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public T ProxyConnectingTo<T>(EndpointId endpoint) where T : INotificationSet
        {
            object result = ProxyConnectingTo(endpoint, typeof(T));
            return (T)result;
        }

        /// <summary>
        /// Generates a proxy object for the given command set and the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint for which a proxy must be made.</param>
        /// <param name="interfaceType">The interface of the command set for which a proxy must be made.</param>
        /// <returns>
        /// The interfaced proxy.
        /// </returns>
        public INotificationSet ProxyConnectingTo(EndpointId endpoint, Type interfaceType)
        {
            {
                Lokad.Enforce.Argument(() => interfaceType);
                Lokad.Enforce.With<ArgumentException>(
                    typeof(INotificationSet).IsAssignableFrom(interfaceType), 
                    Resources.Exceptions_Messages_ANotificationSetTypeMustDeriveFromINotificationSet);
            }

            // We assume that the interface lives up to the demands we placed on it, i.e.:
            // - Derives from INotificationSet
            // - Has only events, no properties and no methods
            // - Every event is based on either the EventHandler or the EventHandler<T> delegate.
            // All these checks should have been done when the interface was registered
            // at the remote endpoint.
            var selfReference = new ProxySelfReferenceInterceptor();
            var addEventHandler = new NotificationEventAddMethodInterceptor(
                interfaceType,
                eventInfo =>
                {
                    var msg = new RegisterForNotificationMessage(m_Local, eventInfo);
                    m_SendWithoutResponse(endpoint, msg, CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);
                },
                m_Diagnostics);
            var removeEventHandler = new NotificationEventRemoveMethodInterceptor(
                interfaceType,
                eventInfo =>
                {
                    var msg = new UnregisterFromNotificationMessage(m_Local, eventInfo);
                    m_SendWithoutResponse(endpoint, msg, CommunicationConstants.DefaultMaximuNumberOfRetriesForMessageSending);
                },
                m_Diagnostics);

            var options = new ProxyGenerationOptions
                {
                    Selector = new NotificationSetInterceptorSelector(),
                    BaseTypeForInterfaceProxy = typeof(NotificationSetProxy),
                };

            var proxy = m_Generator.CreateInterfaceProxyWithoutTarget(
                interfaceType,
                options,
                new IInterceptor[] { selfReference, addEventHandler, removeEventHandler });

            return (INotificationSet)proxy;
        }
    }
}
