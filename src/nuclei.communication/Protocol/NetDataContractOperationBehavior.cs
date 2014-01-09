//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Description;
using System.Xml;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines a new data contract serialization approach. 
    /// </summary>
    /// <design>
    /// Normally WCF uses the standard <see cref="DataContractSerializer"/>. Unfortunately
    /// this also means that .NET type information is not send over the wire. While this allows
    /// for a more loosely coupled system (and better security) in our case this is not what
    /// we want though. We want all the .NET type information because we are in control of 
    /// all of the contracts etc.
    /// </design>
    /// <source>
    /// http://lunaverse.wordpress.com/2007/05/09/remoting-using-wcf-and-nhibernate/
    /// </source>
    internal sealed class NetDataContractOperationBehavior : DataContractSerializerOperationBehavior
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetDataContractOperationBehavior"/> class.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public NetDataContractOperationBehavior(OperationDescription operation)
            : base(operation)
        {
        }

        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> for 
        /// serialization and deserialization operations.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type"/> to create the serializer for.</param>
        /// <param name="name">The name of the generated type.</param>
        /// <param name="ns">The namespace of the generated type.</param>
        /// <param name="knownTypes">
        ///     An <see cref="T:System.Collections.Generic.IList`1"/> of <see cref="T:System.Type"/> that contains known types.
        /// </param>
        /// <returns>
        /// An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> class.
        /// </returns>
        public override XmlObjectSerializer CreateSerializer(
           Type type,
           string name,
           string ns,
           IList<Type> knownTypes)
        {
            return new NetDataContractSerializer(name, ns);
        }

        /// <summary>
        /// Creates an instance of a class that inherits from <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> for
        /// serialization and deserialization operations.
        /// </summary>
        /// <param name="type">The <see cref="T:System.Type"/> to create the serializer for.</param>
        /// <param name="name">The name of the generated type.</param>
        /// <param name="ns">The namespace of the generated type.</param>
        /// <param name="knownTypes">
        ///     An <see cref="T:System.Collections.Generic.IList`1"/> of <see cref="T:System.Type"/> that contains known types.
        /// </param>
        /// <returns>
        /// An instance of a class that inherits from the <see cref="T:System.Runtime.Serialization.XmlObjectSerializer"/> class.
        /// </returns>
        public override XmlObjectSerializer CreateSerializer(
           Type type,
           XmlDictionaryString name,
           XmlDictionaryString ns,
           IList<Type> knownTypes)
        {
            return new NetDataContractSerializer(name, ns);
        }
    }
}
