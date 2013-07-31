//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// Defines additional methods useful during testing.
    /// </summary>
    /// <remarks>
    /// This code is based on the code for the RoundTripSerialize method in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    public static class AssertExtensions
    {
        /// <summary>
        /// Serializes and then deserializes the given instance.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance.</param>
        /// <returns>A copy of the instance.</returns>
        public static T RoundTripSerialize<T>(T instance)
        {
            using (var memoryStream = new MemoryStream())
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, instance);
                memoryStream.Position = 0L;
                return (T)binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
