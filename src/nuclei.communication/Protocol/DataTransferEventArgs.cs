//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// An <see cref="EventArgs"/> object that carries a data message.
    /// </summary>
    internal sealed class DataTransferEventArgs : EventArgs
    {
        /// <summary>
        /// The object that stores the data.
        /// </summary>
        private readonly DataTransferMessage m_Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferEventArgs"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="data"/> is <see langword="null" />.
        /// </exception>
        public DataTransferEventArgs(DataTransferMessage data)
        {
            {
                Lokad.Enforce.Argument(() => data);
            }

            m_Data = data;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public DataTransferMessage Data
        {
            get
            {
                return m_Data;
            }
        }
    }
}
