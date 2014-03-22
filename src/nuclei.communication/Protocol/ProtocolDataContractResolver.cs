//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines an <see cref="DataContractResolver"/> for the protocol layer.
    /// </summary>
    internal sealed class ProtocolDataContractResolver : DataContractResolver
    {
        /// <summary>
        /// Override this method to map a data contract type to an type name and namespace during serialization.
        /// </summary>
        /// <returns>
        /// <see langword="true" /> if mapping succeeded; otherwise, <see langword="false" />.
        /// </returns>
        /// <param name="type">The type to map.</param>
        /// <param name="declaredType">The type declared in the data contract.</param>
        /// <param name="knownTypeResolver">The known type resolver.</param>
        /// <param name="typeName">The type name that can be used in the SOAP XML declaration.</param>
        /// <param name="typeNamespace">The type namespace that can be used in the SOAP XML declaration.</param>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public override bool TryResolveType(
            Type type, 
            Type declaredType, 
            DataContractResolver knownTypeResolver, 
            out XmlDictionaryString typeName, 
            out XmlDictionaryString typeNamespace)
        {
            if (!knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace))
            {
                var dictionary = new XmlDictionary();
                typeName = dictionary.Add(type.FullName);
                typeNamespace = dictionary.Add(type.Assembly.GetName().Name);
            }

            return true;
        }

        /// <summary>
        /// Override this method to map the specified type name and namespace to a data contract type during deserialization.
        /// </summary>
        /// <returns>
        /// The type the type name and namespace is mapped to. 
        /// </returns>
        /// <param name="typeName">The type name that is used in the SOAP XML declaration.</param>
        /// <param name="typeNamespace">The type namespace that is used in the SOAP XML declaration.</param>
        /// <param name="declaredType">The type declared in the data contract.</param>
        /// <param name="knownTypeResolver">The known type resolver.</param>
        public override Type ResolveName(
            string typeName, 
            string typeNamespace, 
            Type declaredType, 
            DataContractResolver knownTypeResolver)
        {
            var result = knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null)
                ?? TypeLoader.FromPartialInformation(typeName, typeNamespace, throwOnError: false);

            return result;
        }
    }
}
