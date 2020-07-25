using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    public class TestSnapshotRowKey
    {
        private const string TableName = "CompareKeyTable";
        private const string PrimaryKeyTableName = "PrimaryKeyTable";
        private const string IdCol = "Id";
        private const string NameCol = "Name";

        class TestData
        {
            private SnapshotCollection _collection;
            internal Snapshot Snapshot { get; private set; }

            internal TableDefiner Definer { get; }
            internal TableDefinition Table => _collection.GetTableDefinition(TableName);
            internal RowBuilder RowFields { get; set; }
            internal SnapshotRow Row => RowFields.Row;
            internal SnapshotBuilder Builder { get; set; }

            internal TestData()
            {
                _collection = new SnapshotCollection();
                Definer = _collection.DefineTable(TableName);
                Builder = _collection.NewSnapshot("Test");
                Snapshot = _collection.GetSnapshot("Test");
                NewRow();
            }

            internal void NewRow()
            {
                RowFields = Builder.AddNewRow(TableName);
            }
        }

        [Test]
        public void CompareKeyIsExtracted()
        {
            //Arrange
            var td = new TestData();
            td.Definer.CompareKey(NameCol).PrimaryKey(IdCol);
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;

            //Act
            var key = new SnapshotRowKey(td.Row, td.Table);

            //Assert
            key[0].Should().Be("Name");
        }

        [Test]
        public void CompoundCompareKeyIsExtracted()
        {
            //Arrange
            var td = new TestData();
            td.Definer.CompareKey(NameCol).PrimaryKey(IdCol).CompareKey(IdCol);
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;

            //Act
            var key = new SnapshotRowKey(td.Row, td.Table);

            //Assert
            key.AllKeys.Should().Equal("Name", 100);
        }

        [Test]
        public void PrimaryKeyIsExtracted()
        {
            //Arrange
            var td = new TestData();
            td.Definer.PrimaryKey(IdCol); //Primary key only
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;

            //Act
            var key = new SnapshotRowKey(td.Row, td.Table);

            //Assert
            key[0].Should().Be(100);
        }

        [Test]
        public void CompoundPrimaryKeyIsExtracted()
        {
            //Arrange
            var td = new TestData();
            td.Definer.PrimaryKey(IdCol).PrimaryKey(NameCol);
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;

            //Act
            var key = new SnapshotRowKey(td.Row, td.Table);

            //Assert
            key.AllKeys.Should().Equal(100, "Name");
        }

        [Test]
        public void AllFieldsExtractedForUnkeyedTable()
        {
            //Arrange
            var td = new TestData();
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;

            //Act
            var key = new SnapshotRowKey(td.Row, td.Table);

            //Assert
            key.AllKeys.Should().Equal("Name", 100);
        }

        [Test]
        public void IdenticalKeysCompareEqual()
        {
            //Arrange
            var td = new TestData();
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;

            var key = new SnapshotRowKey(td.Row, td.Table);
            var key2 = new SnapshotRowKey(td.Row, td.Table);
            
            //Act
            var result = key.CompareTo(key2);

            //Assert
            result.Should().Be(0);
        }

        [Test]
        public void KeysAreSortable()
        {
            //Arrange
            var td = new TestData();
            td.Definer.PrimaryKey(IdCol).PrimaryKey(NameCol);
            for (var n = 1; n <= 10; n++)
            {
                if (n > 1) td.NewRow();
                td.RowFields[IdCol] = n;
                td.RowFields[NameCol] = $"Name {n}";
                td.RowFields["Details"] = $"Row details {n}";
            }

            var rowIndex = SnapshotKeyExtractor.GetKeys(td.Snapshot, td.Table);
            var keys = rowIndex.Keys.ToList();
            keys.Reverse(); //The keys are now out of order

            //Act
            keys.Sort(); //Restore order

            //Assert
            var output = new Output();
            var rep = keys.MakeKeyReport();
            output.FormatTable(rep);
            output.Report.Verify();
        }

        [Test]
        public void ComparisonCanIdentifyEqualKeys()
        {
            //Arrange
            var td = new TestData();
            td.Definer.PrimaryKey(IdCol).PrimaryKey(NameCol);
            for (var n = 1; n <= 10; n++)
            {
                if (n > 1) td.NewRow();
                td.RowFields[IdCol] = n;
                td.RowFields[NameCol] = $"Name {n}";
                td.RowFields["Details"] = $"Row details {n}";
            }
            var keys = SnapshotKeyExtractor.GetKeys(td.Snapshot, td.Table).Keys;

            //Act/Assert
            keys.Select(k => ReferenceEquals(k, keys.Single(other => other.CompareTo(k) == 0))).Should().AllBeEquivalentTo(true);
            //Checks that for every key there is only one match in the set and the match is itself
        }

        [Test]
        public void KeyIsNotEqualToNull()
        {
            var td = new TestData();
            td.Definer.CompareKey(NameCol).PrimaryKey(IdCol);
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;
            var key = new SnapshotRowKey(td.Row, td.Table);

            //Act
            var result = key.Equals(null);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void KeyIsNotEqualToNonKey()
        {
            var td = new TestData();
            td.Definer.CompareKey(NameCol).PrimaryKey(IdCol);
            td.RowFields[NameCol] = "Name";
            td.RowFields[IdCol] = 100;
            var key = new SnapshotRowKey(td.Row, td.Table);

            //Act
            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = key.Equals(1);

            //Assert
            result.Should().BeFalse();
        }
    }
}