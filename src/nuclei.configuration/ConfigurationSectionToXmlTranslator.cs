//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Nuclei.Configuration
{
    /// <summary>
    /// Defines an <see cref="IConfigurationSectionHandler"/> that collects all the configuration sections
    /// and gathers the child nodes for later processing.
    /// </summary>
    public sealed class ConfigurationSectionToXmlTranslator : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var result = new List<XmlNode>();
            foreach (XmlNode subNode in section)
            {
                result.Add(subNode);
            }

            return result;
        }
    }
}
