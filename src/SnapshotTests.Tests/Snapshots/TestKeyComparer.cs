using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestKeyComparer
    {
        private List<object> Keys(params object[] values)
        {
            return values.ToList();
        }

        [Test] public void EqualListsCompareEqual()
        {
            //Arrange
            var mine = Keys(1, "Two");
            var theirs = Keys(1.0, "Two");
            
            //Act
            var result = KeyComparer.CompareKeySets(mine, theirs);

            //Assert
            result.Should().Be(0);
        }

        [Test] public void ComparisonIsPerformedLeftToRight()
        {
            //Arrange
            var mine = Keys(1.0, "B");
            var theirs = Keys(1.1, "A");
            
            //Act
            var result = KeyComparer.CompareKeySets(mine, theirs);

            //Assert
            result.Should().Be(-1);
        }

        [Test] public void ComparisonComparesAllFields()
        {
            //Arrange
            var mine = Keys(1, 5, 9, "B");
            var theirs = Keys(1, 5, 9, "A");
            
            //Act
            var result = KeyComparer.CompareKeySets(mine, theirs);

            //Assert
            result.Should().Be(1);
        }

        [Test] public void ComparisonStopsOnFirstDifference()
        {
            //Arrange
            var mine = Keys(1, 5, 9, "B", 90, 100, 200);
            var theirs = Keys(1, 5, 9, "A", 91, 101, 201);
            
            //Act
            var result = KeyComparer.CompareKeySets(mine, theirs);

            //Assert
            result.Should().Be(1);
        }

        [Test] 
        public void LongerKeysAreGreater()
        {
            //Arrange
            var mine = Keys(1);
            var theirs = Keys(1, 5);
            
            //Act
            var result = KeyComparer.CompareKeySets(mine, theirs);

            //Assert
            result.Should().Be(-1);
        }

        [Test] 
        public void ShorterKeysAreLesser()
        {
            //Arrange
            var mine = Keys(1, 5);
            var theirs = Keys(1);
            
            //Act
            var result = KeyComparer.CompareKeySets(mine, theirs);

            //Assert
            result.Should().Be(1);
        }
    }
}
