//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines the interface for the configuration settings object for the application.
    /// </summary>
    public interface IConfiguration
    {
        /// <summary>
        /// Returns a value indicating if there is a value for the given key or not.
        /// </summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// <see langword="true" /> if there is a value for the given key; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        bool HasValueFor(ConfigurationKey key);

        /// <summary>
        /// Returns the value for the given configuration key.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="key">The configuration key.</param>
        /// <returns>
        /// The desired value.
        /// </returns>
        T Value<T>(ConfigurationKey key);
    }
}
