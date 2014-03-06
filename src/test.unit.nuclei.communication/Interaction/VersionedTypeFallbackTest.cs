//-----------------------------------------------------------------------
// <copyright company="Nuclei">
//     Copyright 2013 Nuclei. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Nuclei.Communication.Interaction
{
    [TestFixture]
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
        Justification = "Unit tests do not need documentation.")]
    public sealed class VersionedTypeFallbackTest
    {
        [Test]
        public void IsPartialMatchWithNullReference()
        {
            var fallback = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 0)),
                    });

            Assert.IsFalse(fallback.IsPartialMatch(null));
        }

        [Test]
        public void IsPartialMatchWithNoMatch()
        {
            var first = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 0)),
                    });
            var second = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(double).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 0)),
                    });

            Assert.IsFalse(first.IsPartialMatch(second));
        }

        [Test]
        public void IsPartialMatchWithLowestOverlap()
        {
            var first = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(2, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(long).FullName, typeof(long).Assembly.GetName()), 
                            new Version(2, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                            new Version(2, 2)),
                    });
            var second = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(sbyte).FullName, typeof(sbyte).Assembly.GetName()), 
                            new Version(1, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(short).FullName, typeof(short).Assembly.GetName()), 
                            new Version(1, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 2)),
                    });

            Assert.IsTrue(first.IsPartialMatch(second));
        }
        
        [Test]
        public void IsPartialMatchWithHighestOverlap()
        {
            var first = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(2, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(long).FullName, typeof(long).Assembly.GetName()), 
                            new Version(2, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                            new Version(2, 2)),
                    });
            var second = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(sbyte).FullName, typeof(sbyte).Assembly.GetName()), 
                            new Version(1, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(short).FullName, typeof(short).Assembly.GetName()), 
                            new Version(1, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 2)),
                    });

            Assert.IsTrue(second.IsPartialMatch(first));
        }

        [Test]
        public void HighestVersionMatchWithNullReference()
        {
            var fallback = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 0)),
                    });

            Assert.IsNull(fallback.HighestVersionMatch(null));
        }

        [Test]
        public void HighestVersionMatchWithNoMatch()
        {
            var first = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 0)),
                    });
            var second = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(double).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 0)),
                    });

            Assert.IsNull(first.HighestVersionMatch(second));
        }

        [Test]
        public void HighestVersionMatchWithLowestOverlap()
        {
            var firstType = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var first = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            firstType, 
                            new Version(2, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(long).FullName, typeof(long).Assembly.GetName()), 
                            new Version(2, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                            new Version(2, 2)),
                    });

            var secondType = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var second = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(sbyte).FullName, typeof(sbyte).Assembly.GetName()), 
                            new Version(1, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(short).FullName, typeof(short).Assembly.GetName()), 
                            new Version(1, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            secondType, 
                            new Version(1, 2)),
                    });

            var type = first.HighestVersionMatch(second);
            Assert.AreSame(firstType, type);
        }

        [Test]
        public void HighestVersionMatchWithHighestOverlap()
        {
            var firstType = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var first = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            firstType, 
                            new Version(2, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(long).FullName, typeof(long).Assembly.GetName()), 
                            new Version(2, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                            new Version(2, 2)),
                    });

            var secondType = new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName());
            var second = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(sbyte).FullName, typeof(sbyte).Assembly.GetName()), 
                            new Version(1, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(short).FullName, typeof(short).Assembly.GetName()), 
                            new Version(1, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            secondType, 
                            new Version(1, 2)),
                    });

            var type = second.HighestVersionMatch(first);
            Assert.AreSame(secondType, type);
        }

        [Test]
        public void HighestVersionMatchWithOverlap()
        {
            var firstType = new OfflineTypeInformation(typeof(long).FullName, typeof(long).Assembly.GetName());
            var first = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()),
                            new Version(2, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            firstType, 
                            new Version(2, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(double).FullName, typeof(double).Assembly.GetName()), 
                            new Version(2, 2)),
                    });

            var secondType = new OfflineTypeInformation(typeof(long).FullName, typeof(long).Assembly.GetName());
            var second = new VersionedTypeFallback(
                new[]
                    {
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(sbyte).FullName, typeof(sbyte).Assembly.GetName()), 
                            new Version(1, 0)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(short).FullName, typeof(short).Assembly.GetName()), 
                            new Version(1, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            new OfflineTypeInformation(typeof(int).FullName, typeof(int).Assembly.GetName()), 
                            new Version(1, 1)),
                        new Tuple<OfflineTypeInformation, Version>(
                            secondType, 
                            new Version(1, 2)),
                    });

            var type = first.HighestVersionMatch(second);
            Assert.AreSame(firstType, type);
        }
    }
}
