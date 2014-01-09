//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using Castle.DynamicProxy;
using Nuclei.Communication.Messages;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Interaction
{
    /// <summary>
    /// Defines an <see cref="IInterceptor"/> for the 'remove' method of an <see cref="INotificationSet"/> event.
    /// </summary>
    internal sealed class NotificationEventRemoveMethodInterceptor : IInterceptor
    {
        /// <summary>
        /// The prefix for the method that removes event handlers from the event.
        /// </summary>
        private const string MethodPrefix = "remove_";

        private static string MethodToText(MethodInfo method)
        {
            return method.ToString();
        }

        /// <summary>
        /// The type of the interface for which a proxy is being provided.
        /// </summary>
        private readonly Type m_InterfaceType;

        /// <summary>
        /// The function which sends the <see cref="RegisterForNotificationMessage"/> to the owning endpoint.
        /// </summary>
        private readonly Action<ISerializedEventRegistration> m_SendMessageWithoutResponse;

        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationEventRemoveMethodInterceptor"/> class.
        /// </summary>
        /// <param name="proxyInterfaceType">The type of the interface for which a proxy is being provided.</param>
        /// <param name="sendMessageWithoutResponse">
        ///     The function used to send the information about the event registration to the owning endpoint.
        /// </param>
        /// <param name="systemDiagnostics">The object that provides the diagnostic methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="proxyInterfaceType"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="sendMessageWithoutResponse"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public NotificationEventRemoveMethodInterceptor(
            Type proxyInterfaceType,
            Action<ISerializedEventRegistration> sendMessageWithoutResponse,
            SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => proxyInterfaceType);
                Lokad.Enforce.Argument(() => sendMessageWithoutResponse);
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_InterfaceType = proxyInterfaceType;
            m_SendMessageWithoutResponse = sendMessageWithoutResponse;
            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Called when a method or property call is intercepted.
        /// </summary>
        /// <param name="invocation">Information about the call that was intercepted.</param>
        public void Intercept(IInvocation invocation)
        {
            {
                Debug.Assert(invocation.Method.Name.StartsWith(MethodPrefix, StringComparison.Ordinal), "Intercepted an incorrect method.");
                Debug.Assert(invocation.Arguments.Length == 1, "There should only be one argument.");
                Debug.Assert(invocation.Arguments[0] is Delegate, "The argument should be a delegate.");
            }

            m_Diagnostics.Log(
                LevelToLog.Trace,
                CommunicationConstants.DefaultLogTextPrefix,
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Invoking {0}",
                    MethodToText(invocation.Method)));
            
            var methodToInvoke = invocation.Method.Name;
            var eventName = methodToInvoke.Substring(MethodPrefix.Length);

            var handler = invocation.Arguments[0] as Delegate;
            var proxy = invocation.Proxy as NotificationSetProxy;
            proxy.RemoveFromEvent(eventName, handler);

            if (!proxy.HasSubscribers(eventName))
            {
                m_SendMessageWithoutResponse(new SerializedEvent(ProxyExtensions.FromType(m_InterfaceType), eventName));
            }
        }
    }
}
