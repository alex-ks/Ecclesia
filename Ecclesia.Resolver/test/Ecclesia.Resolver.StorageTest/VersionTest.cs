using System;
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
            var atomId = new AtomId { Kind = "Text", Name = "Hello", Version = version };
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
            Assert.Throws<FormatException>(() => 
                new AtomId 
                { 
                    Kind = "Text", 
                    Name = "Hello", 
                    Version = version 
                });
        }


        [Theory]
        [InlineData("1", 1, 0, 0)]
        [InlineData("2.1", 2, 1, 0)]
        [InlineData("1.2.3", 1, 2, 3)]
        [InlineData("2.0.4-beta", 2, 0, 4)]
        [InlineData(null, int.MaxValue, int.MaxValue, int.MaxValue)]
        public void SortingAtoms_SetVersion_GotVersionNumbersAsTuple(string version, 
                                                                     int major,
                                                                     int middle,
                                                                     int minor)
        {
        // Given
            var atomId = new AtomId { Kind = "Text", Name = "Hello", Version = version };
        // When
            var (a, b, c) = atomId.SortCriteria().AsTuple();
        // Then
            Assert.Equal(major, a);
            Assert.Equal(middle, b);
            Assert.Equal(minor, c);
        }
    }
}