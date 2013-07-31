//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Nuclei.Communication.Properties;

namespace Nuclei.Communication
{
    /// <summary>
    /// Provides extension methods for the <see cref="EndpointId"/> class.
    /// </summary>
    public static class EndpointIdExtensions
    {
        /// <summary>
        /// Creates a new <see cref="EndpointId"/> for the current process.
        /// </summary>
        /// <returns>The newly created <see cref="EndpointId"/>.</returns>
        public static EndpointId CreateEndpointIdForCurrentProcess()
        {
            return Process.GetCurrentProcess().CreateEndpointIdForProcess();
        }

        /// <summary>
        /// Creates a new <see cref="EndpointId"/> for the given process.
        /// </summary>
        /// <param name="process">The process for which the ID should be created.</param>
        /// <returns>The newly created <see cref="EndpointId"/>.</returns>
        public static EndpointId CreateEndpointIdForProcess(this Process process)
        {
            return new EndpointId(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}:{1}",
                    Environment.MachineName,
                    process.Id));
        }

        /// <summary>
        /// Deserializes an <see cref="EndpointId"/> from a string.
        /// </summary>
        /// <param name="serializedEndpoint">The string containing the serialized <see cref="EndpointId"/> information.</param>
        /// <returns>A new <see cref="EndpointId"/> based on the given <paramref name="serializedEndpoint"/>.</returns>
        public static EndpointId Deserialize(string serializedEndpoint)
        {
            {
                Lokad.Enforce.Argument(() => serializedEndpoint);
                Lokad.Enforce.With<ArgumentException>(
                    !string.IsNullOrWhiteSpace(serializedEndpoint),
                    Resources.Exceptions_Messages_EndpointIdCannotBeDeserializedFromAnEmptyString);
            }

            // @todo: do we check that this string has the right format?
            return new EndpointId(serializedEndpoint);
        }

        /// <summary>
        /// Returns a value indicating if the <see cref="EndpointId"/> comes from an
        /// application that lives on a machine with the given name.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="machineName">The name of the machine on which the endpoint generating application may be present.</param>
        /// <returns>
        ///     <see langword="true"/> if the endpoint comes from an application on the machine with the given name;
        ///     otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsOnMachine(this EndpointId endpoint, string machineName)
        {
            var endpointMachineName = endpoint.OriginatesOnMachine();
            return string.Equals(machineName, endpointMachineName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a value indicating if the <see cref="EndpointId"/> comes from the local machine.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>
        ///     <see langword="true" /> if the endpoint comes from the local machine; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public static bool IsOnLocalMachine(this EndpointId endpoint)
        {
            var endpointMachineName = endpoint.OriginatesOnMachine();
            return string.Equals(Environment.MachineName, endpointMachineName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns the name of the machine on which the application lives that generated
        /// the given endpoint.
        /// </summary>
        /// <remarks>
        /// This method assumes that the endpoint ID was generated from the <see cref="CreateEndpointIdForCurrentProcess"/>
        /// method.
        /// </remarks>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>The name of the machine on which the endpoint was generated.</returns>
        public static string OriginatesOnMachine(this EndpointId endpoint)
        {
            var stringRepresentation = endpoint.ToString();
            var index = stringRepresentation.LastIndexOf(":", StringComparison.Ordinal);
            if (index == -1)
            {
                throw new UnknownEndpointIdFormatException();
            }

            return stringRepresentation.Substring(0, index);
        }
    }
}
