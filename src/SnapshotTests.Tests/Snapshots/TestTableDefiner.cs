using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Exceptions;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestTableDefiner
    {
        private const string TestTableName = "Test";
        private SnapshotCollection _collection;
        private TableDefiner _definer;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _definer = _collection.DefineTable(TestTableName);
        }

        [Test]
        public void PrimaryKeyCanBeSet()
        {
            //Act
            _definer.PrimaryKey("Key1");

            //Assert
            _collection.GetTableDefinition(TestTableName)
                .PrimaryKeys
                .Single()
                .Should().Be("Key1");
        }

        [Test]
        public void CompoundPrimaryKeyCanBeSet()
        {
            //Act
            _definer.PrimaryKey("Key1").PrimaryKey("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).PrimaryKeys)
                .Should().Be("Key1, Key2");
        }

        [Test]
        public void DuplicatesAreNotCreatedInPrimaryKeys()
        {
            //Arrange
            _definer.PrimaryKey("Key1").PrimaryKey("Key2");

            //Act
            _definer.PrimaryKey("Key1").PrimaryKey("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).PrimaryKeys)
                .Should().Be("Key1, Key2");
        }

        [Test]
        public void CompareKeyCanBeSet()
        {
            //Act
            _definer.CompareKey("Key1");

            //Assert
            _collection.GetTableDefinition(TestTableName)
                .CompareKeys
                .Single()
                .Should().Be("Key1");
        }

        [Test]
        public void DuplicatesAreNotCreatedInCompareKey()
        {
            //Arrange
            _definer.CompareKey("Key1");

            //Act
            _definer.CompareKey("Key1");

            //Assert
            _collection.GetTableDefinition(TestTableName)
                .CompareKeys
                .Single()
                .Should().Be("Key1");
        }

        [Test]
        public void CompoundCompareKeyCanBeSet()
        {
            //Act
            _definer.CompareKey("Key1").CompareKey("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).CompareKeys)
                .Should().Be("Key1, Key2");
        }

        [Test]
        public void FieldCanBeFlaggedAsUnpredictable()
        {
            //Act
            _definer.IsUnpredictable("Key1").IsUnpredictable("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).Unpredictable)
                .Should().Be("Key1, Key2");
        }

        [Test]
        public void FieldCanBeFlaggedAsPredictable()
        {
            //Act
            _definer.IsPredictable("Key1").IsPredictable("Key2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).Predictable)
                .Should().Be("Key1, Key2");
        }

        [Test]
        public void FieldCanBeFlaggedAsRequired()
        {
            //Act
            _definer.IsRequired("Field1").IsRequired("Field2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).RequiredColumns)
                .Should().Be("Field1, Field2");
        }

        [Test]
        public void FieldsCanBeFlaggedAsReferences()
        {
            //Act
            _definer.IsReference("Field1", "Table", "Id");

            //Assert
            var output = new Output();
            output.FormatTable(_collection.GetTableDefinition(TestTableName).References);
            output.Report.Verify();
        }

        [Test]
        public void FieldsReferenceFlaggedTwiceThrow()
        {
            //Act
            _definer.IsReference("Field1", "Table", "Id");

            //Assert
            Action action = () => _definer.IsReference("Field1", "Table2", "Id2"); //Same field references two different tables
            action.Should().Throw<AmbiguosReferenceInSnapshotColumn>()
                .Where(x => x.FieldName == "Field1"
                            && x.ConflictingPropertyName == "Id2" && x.ConflictingTableName == "Table2"
                            && x.ReferencedPropertyName == "Id" && x.ReferencedTableName == "Table");
        }

        [Test]
        public void DuplicateReferenceDefinitionsAreOnlyRecordedOnce()
        {
            //Act
            _definer.IsReference("Field1", "Table", "Id");
            _definer.IsReference("Field1", "Table", "Id");

            //Assert
            var output = new Output();
            output.FormatTable(_collection.GetTableDefinition(TestTableName).References);
            output.Report.Verify();
        }

        [Test]
        public void IncludeInComparisonCanBeSet()
        {
            //Act
            _definer.IncludeInComparison();

            //Assert
            _collection.GetTableDefinition(TestTableName).IncludeInComparison.Should().BeTrue();
        }

        [Test]
        public void ExcludeFromComparisonCanBeSet()
        {
            //Act
            _definer.ExcludeFromComparison();

            //Assert
            _collection.GetTableDefinition(TestTableName).ExcludeFromComparison.Should().BeTrue();
        }

        [Test]
        public void IncludeInComparisonResetsExcludeFromComparison()
        {
            //Arrange
            _definer.ExcludeFromComparison();
            
            //Act
            _definer.IncludeInComparison();

            //Assert
            _collection.GetTableDefinition(TestTableName).ExcludeFromComparison.Should().BeFalse();
        }

        [Test]
        public void ExcludeFromComparisonResetsIncludeInComparison()
        {
            //Arrange
            _definer.IncludeInComparison();
            
            //Act
            _definer.ExcludeFromComparison();

            //Assert
            _collection.GetTableDefinition(TestTableName).IncludeInComparison.Should().BeFalse();
        }

        [Test]
        public void SortFieldCanBeSet()
        {
            //Act
            _definer.Sort("Field1");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).SortFields.Select(sf => $"{sf.Field} {sf.SortOrder}"))
                .Should().Be("Field1 Ascending");
        }

        [Test]
        public void MultipleSortFieldsCanBeSet()
        {
            //Act
            _definer.Sort("Field1");
            _definer.Sort("Field2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).SortFields.Select(sf => $"{sf.Field} {sf.SortOrder}"))
                .Should().Be("Field1 Ascending, Field2 Ascending");
        }

        [Test]
        public void DescendingSortFieldCanBeSet()
        {
            //Act
            _definer.SortDescending("Field1");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).SortFields.Select(sf => $"{sf.Field} {sf.SortOrder}"))
                .Should().Be("Field1 Descending");
        }

        [Test]
        public void MultipleDescendingSortFieldsCanBeSet()
        {
            //Act
            _definer.SortDescending("Field1");
            _definer.SortDescending("Field2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).SortFields.Select(sf => $"{sf.Field} {sf.SortOrder}"))
                .Should().Be("Field1 Descending, Field2 Descending");
        }

        [Test]
        public void MixedOrderSortFieldsCanBeSet()
        {
            //Act
            _definer.Sort("Field1");
            _definer.SortDescending("Field2");

            //Assert
            string.Join(", ", _collection.GetTableDefinition(TestTableName).SortFields.Select(sf => $"{sf.Field} {sf.SortOrder}"))
                .Should().Be("Field1 Ascending, Field2 Descending");
        }
    }
}