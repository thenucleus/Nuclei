//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Threading.Tasks;
using Nuclei.Communication.Protocol.V1.DataObjects;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Nuclei.Communication.Protocol.V1
{
    /// <summary>
    /// Defines the methods for processing messages from the network.
    /// </summary>
    /// <design>
    /// This class is meant to be able to handle many messages being send at the same time, 
    /// however there should only be one instance of this class so that we can create it
    /// ourselves when we want to.
    /// </design>
    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    internal sealed class DataReceivingEndpoint : IDataPipe, IDataReceivingEndpoint
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataReceivingEndpoint"/> class.
        /// </summary>
        /// <param name="systemDiagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="systemDiagnostics"/> is <see langword="null" />.
        /// </exception>
        public DataReceivingEndpoint(SystemDiagnostics systemDiagnostics)
        {
            {
                Lokad.Enforce.Argument(() => systemDiagnostics);
            }

            m_Diagnostics = systemDiagnostics;
        }

        /// <summary>
        /// Accepts the stream.
        /// </summary>
        /// <param name="data">The data message that allows a data stream to be transferred.</param>
        /// <returns>An object indicating that the data was received successfully.</returns>
        public StreamReceptionConfirmation AcceptStream(StreamData data)
        {
            ProcessStream(data);
            return new StreamReceptionConfirmation
                {
                    WasDataReceived = true,
                };
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "We don't really want the channel to die just because the other side didn't behave properly.")]
        private void ProcessStream(StreamData data)
        {
            Debug.Assert(data != null, "The object should be a StreamData instance.");

            try
            {
                m_Diagnostics.Log(
                    LevelToLog.Trace,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Received data from {0}.",
                        data.SendingEndpoint));

                var translatedData = TranslateMessage(data);

                // Raise the event on another thread AFTER we copied the stream so that the 
                // WCF auto-dispose doesn't stuff us up.
                Task.Factory.StartNew(() => RaiseOnNewData(translatedData));
            }
            catch (Exception e)
            {
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    CommunicationConstants.DefaultLogTextPrefix,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Exception occurred during the handling of a data from {0}. Exception was: {1}",
                        data.SendingEndpoint,
                        e));
            }
        }

        private DataTransferMessage TranslateMessage(StreamData data)
        {
            // copy the stream here because WCF automatically closes streams
            // see here: http://stackoverflow.com/a/6681478/539846
            var stream = new MemoryStream();
            data.Data.CopyTo(stream);
            stream.Position = 0;

            var result = new DataTransferMessage
                {
                    SendingEndpoint = data.SendingEndpoint,
                    Data = stream,
                };

            return result;
        }

        /// <summary>
        /// An event raised when a new data message is available in the pipe.
        /// </summary>
        public event EventHandler<DataTransferEventArgs> OnNewData;

        private void RaiseOnNewData(DataTransferMessage data)
        {
            var local = OnNewData;
            if (local != null)
            {
                local(this, new DataTransferEventArgs(data));
            }
        }
    }
}
