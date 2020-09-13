using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSortFieldOrderer
    {
        [Test]
        public void FieldsAreReturnedInSequence()
        {
            //Arrange
            var sortFields = new List<TableDefinition.SortField>
            {
                new TableDefinition.SortField("C", SortOrder.Ascending, 3),
                new TableDefinition.SortField("B", SortOrder.Ascending, 2),
                new TableDefinition.SortField("A", SortOrder.Ascending, 1),
            };

            //Act
            var ordered = SortFieldOrderer.Order(sortFields).ToList();

            //Assert
            string.Join(", ", ordered.Select(o => o.Field)).Should().Be("A, B, C");
        }

        [Test]
        public void FieldsWithoutIndexAreReturnedFirst()
        {
            //Arrange
            var sortFields = new List<TableDefinition.SortField>
            {
                new TableDefinition.SortField("C", SortOrder.Ascending, 3),
                new TableDefinition.SortField("B", SortOrder.Ascending, 2),
                new TableDefinition.SortField("A", SortOrder.Ascending, 1),
                new TableDefinition.SortField("D", SortOrder.Ascending, null),
                new TableDefinition.SortField("E", SortOrder.Ascending, null),
            };

            //Act
            var ordered = SortFieldOrderer.Order(sortFields).ToList();

            //Assert
            string.Join(", ", ordered.Select(o => o.Field)).Should().Be("D, E, A, B, C");
        }
    }
}