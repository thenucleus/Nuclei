using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nuclei.Communication.Protocol
{
    internal static class ProtocolVersions
    {
        /// <summary>
        /// Gets the current version of the discovery layer.
        /// </summary>
        public static Version Current
        {
            get
            {
                return V1;
            }
        }

        /// <summary>
        /// Gets the version of the base discovery layer.
        /// </summary>
        public static Version V1
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        /// <summary>
        /// Returns a collection containing all the supported versions of the discovery protocol.
        /// </summary>
        /// <returns>A collection containing all the supported versions of the discovery protocol.</returns>
        public static IEnumerable<Version> SupportedVersions()
        {
            return new[]
                {
                    V1,
                };
        }
    }
}
