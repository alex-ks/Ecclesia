using System;
using System.Linq;
using Ecclesia.Resolver.Storage.Models;
using Xunit;

namespace Ecclesia.Resolver.StorageTest
{
    public class VersionTest
    {
        [Theory]
        [InlineData(null)]
        [InlineData("1")]
        [InlineData("1.0")]
        [InlineData("1.0.52")]
        [InlineData("1-a1")]
        [InlineData("1.0-b2")]
        [InlineData("1.0.52-3c")]
        public void SettingVersion_SetValidVersion_NoException(string version)
        {
        //When
            var atomId = new AtomId("Text", "Hello", version);
        //Then
            Assert.Equal(version, atomId.Version);   
        }

        [Theory]
        [InlineData("a")]
        [InlineData("1.2b")]
        [InlineData("1..1")]
        [InlineData("2.")]
        [InlineData(".3")]
        [InlineData("2.3.a")]
        [InlineData("1.2.3f")]
        [InlineData("1.2.3-@")]
        [InlineData("1.2.3-a'")]
        public void SettingVersion_SetInvalidVersion_GotException(string version)
        {
            Assert.Throws<FormatException>(() => new AtomId("Text", "Hello", version));
        }


        [Theory]
        [InlineData("1", 1, null, null, null)]
        [InlineData("2.1", 2, 1, null, null)]
        [InlineData("1.2.3", 1, 2, 3, null)]
        [InlineData("2.0.4-beta", 2, 0, 4, "beta")]
        [InlineData(null, int.MaxValue, int.MaxValue, int.MaxValue, null)]
        public void SortingAtoms_SetVersion_GotVersionNumbersAsTuple(string version, 
                                                                     int major,
                                                                     int? middle,
                                                                     int? minor,
                                                                     string versionName)
        {
        // Given
            var atomId = new AtomId("Text", "Hello", version);
        // When
            var (a, b, c, d) = atomId.VersionTuple;
        // Then
            Assert.Equal(major, a);
            Assert.Equal(middle, b);
            Assert.Equal(minor, c);
            Assert.Equal(versionName, d);
        }

        void ShuffleArray<T>(T[] array, int seed)
        {
            var rng = new Random(seed);
            for (int i = 0; i < array.Length - 1; ++i)
            {
                var index = rng.Next(i, array.Length);
                var temp = array[index];
                array[index] = array[i];
                array[i] = temp;
            }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(7)]
        [InlineData(42)]
        [InlineData(1024 * 1024)]
        public void SortingAtoms_OrderByAtoms_GetCorrectOrder(int seed)
        {
            var correctVersionsOrder = new string[] 
            { 
                "0.9.4-beta",
                "1.0.0-release",
                "1.0.1",
                "1.1",
                "2",
                "2.0.1",
                null 
            };

            var atoms = correctVersionsOrder.Select(v => new AtomId("Text", "Hello", v)).ToArray();
            ShuffleArray(atoms, seed);
            var sortedAtoms = atoms.OrderBy(a => a);

            var checkers = Enumerable.Range(0, correctVersionsOrder.Length)
                                     .Select(i => 
                                        new Action<AtomId>(a => 
                                            Assert.Equal(correctVersionsOrder[i], a.Version)))
                                     .ToArray();
            
            Assert.Collection(sortedAtoms, checkers);
        }
    }
}