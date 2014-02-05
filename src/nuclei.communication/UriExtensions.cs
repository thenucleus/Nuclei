//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication
{
    /// <summary>
    /// Defines a set of extension methods for the <see cref="Uri"/> class.
    /// </summary>
    internal static class UriExtensions
    {
        /// <summary>
        /// Gets the <see cref="ChannelTemplate"/> from a <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">The URI from which the template should be obtained.</param>
        /// <returns>The channel template that matches the given URI.</returns>
        public static ChannelTemplate ToChannelTemplate(this Uri uri)
        {
            switch (uri.Scheme)
            {
                case "net.pipe":
                    return ChannelTemplate.NamedPipe;
                case "net.tcp":
                    return ChannelTemplate.TcpIP;
                case "http":
                    return ChannelTemplate.Http;
                case "https":
                    return ChannelTemplate.Https;
                default:
                    return ChannelTemplate.Unknown;
            }
        }
    }
}
