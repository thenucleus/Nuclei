//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace Nuclei.Nunit.Extensions
{
    /// <summary>
    /// Stores a number of hashcodes for use in collision calculations.
    /// </summary>
    /// <remarks>
    /// This code is based on, but not exactly the same as, the code of the hashcode contract verifier in the MbUnit 
    /// project which is licensed under the Apache License 2.0. More information can be found at:
    /// https://code.google.com/p/mb-unit/.
    /// </remarks>
    internal sealed class HashStore
    {
        private readonly IDictionary<int, int> m_More = new Dictionary<int, int>();
        private readonly HashSet<int> m_One = new HashSet<int>();
        private readonly HashStoreResult m_Result;
        private readonly HashSet<int> m_Two = new HashSet<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="HashStore"/> class.
        /// </summary>
        /// <param name="hashes">The collection of hashcodes.</param>
        public HashStore(IEnumerable<int> hashes)
        {
            int actual = 0;
            foreach (int num2 in hashes)
            {
                Add(num2);
                actual++;
            }

            if (actual < 2)
            {
                throw new NotEnoughHashesException(2, actual);
            }

            m_Result = CalculateResults(actual);
        }

        private void Add(int hash)
        {
            if (m_One.Contains(hash))
            {
                m_One.Remove(hash);
                m_Two.Add(hash);
            }
            else if (m_Two.Contains(hash))
            {
                m_Two.Remove(hash);
                m_More.Add(hash, 3);
            }
            else
            {
                int num;
                if (m_More.TryGetValue(hash, out num))
                {
                    m_More[hash] = 1 + num;
                }
                else
                {
                    m_One.Add(hash);
                }
            }
        }

        private HashStoreResult CalculateResults(int count)
        {
            int bucketSize = GetBucketSize();
            var actual = new double[bucketSize];
            double collisionProbability = 0.0;
            int num3 = 0;
            for (int i = 0; i < m_One.Count; i++)
            {
                actual[num3++ % bucketSize]++;
            }

            for (int j = 0; j < m_Two.Count; j++)
            {
                actual[num3++ % bucketSize] += 2.0;
                collisionProbability += 2.0 / (count * (count - 1));
            }

            foreach (KeyValuePair<int, int> pair in m_More)
            {
                actual[num3++ % bucketSize] += pair.Value;
                collisionProbability += ((pair.Value / ((double)count)) * (pair.Value - 1)) / (count - 1);
            }

            var test = new ChiSquareTest(count / ((double)bucketSize), actual, 1);
            return new HashStoreResult(collisionProbability, 1.0 - test.TwoTailedpValue);
        }

        private int GetBucketSize()
        {
            var numArray = new[] { 0x39dd, 0xe1d, 0xdf, 0x11 };
            int num = (m_One.Count + this.m_Two.Count) + m_More.Count;
            foreach (int num2 in numArray)
            {
                if (num >= (num2 * 10))
                {
                    return num2;
                }
            }

            return num;
        }

        internal int this[int hash]
        {
            get
            {
                int num;
                if (m_One.Contains(hash))
                {
                    return 1;
                }

                if (m_Two.Contains(hash))
                {
                    return 2;
                }

                if (!m_More.TryGetValue(hash, out num))
                {
                    return 0;
                }

                return num;
            }
        }

        public HashStoreResult Result
        {
            get
            {
                return m_Result;
            }
        }
    }
}
