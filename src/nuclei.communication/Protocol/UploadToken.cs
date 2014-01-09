//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Nuclei.Communication.Protocol
{
    /// <summary>
    /// Defines an ID number for uploads.
    /// </summary>
    /// <remarks>
    /// This token uses integers internally. We don't expect to
    /// have enough uploads in a single execution of an application
    /// for integer overflow to occur.
    /// </remarks>
    /// <design>
    /// <para>
    /// The <c>UploadToken</c> class stores a token for an upload. The internal
    /// data is an integer which indicates the sequential number of the token. The way
    /// this is implemented means that tokens are only sequential inside a single
    /// application. This means that we should always get the token of the upload
    /// from the same location. 
    /// </para>
    /// </design>
    [Serializable]
    public sealed class UploadToken : Id<UploadToken, int>
    {
        /// <summary>
        /// Defines the ID number for an invalid dataset ID.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private const int InvalidId = -1;

        /// <summary>
        /// The value of the last ID number.
        /// </summary>
        private static int s_LastId = 0;

        /// <summary>
        /// Returns the next integer that can be used for an ID number.
        /// </summary>
        /// <returns>
        /// The next unused ID value.
        /// </returns>
        private static int NextIdValue()
        {
            var current = Interlocked.Increment(ref s_LastId);
            return current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadToken"/> class.
        /// </summary>
        public UploadToken()
            : this(NextIdValue())
        { 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadToken"/> class with the given integer as ID number.
        /// </summary>
        /// <param name="id">The ID number. Must be larger than -1.</param>
        internal UploadToken(int id)
            : base(id)
        {
            Debug.Assert(id > InvalidId, "The ID number should not be invalid"); 
        }

        /// <summary>
        /// Performs the actual act of creating a copy of the current ID number.
        /// </summary>
        /// <param name="value">The internally stored value.</param>
        /// <returns>
        /// A copy of the current ID number.
        /// </returns>
        protected override UploadToken Clone(int value)
        {
            var result = new UploadToken(value);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "UploadToken: [{0}]",
                InternalValue);
        }
    }
}
