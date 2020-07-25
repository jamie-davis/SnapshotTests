using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestRowDataComparer
    {
        private const string TableName = "CompoundKeyTable";
        private const string IdCol = "Id";
        private const string NameCol = "Name";

        private SnapshotCollection _collection;
        private Snapshot _snapshot;
        private TableDefinition _tableDef;
        private SnapshotBuilder _snapshotBuilder;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable(TableName).PrimaryKey(IdCol).PrimaryKey(NameCol);
            _tableDef = _collection.GetTableDefinition(TableName);
            _snapshotBuilder = _collection.NewSnapshot("Test");

        }

        private RowBuilder GetRow(int n)
        {
            var newRow = _snapshotBuilder.AddNewRow(TableName);
            newRow[IdCol] = n;
            newRow[NameCol] = $"Name {n}";
            return newRow;
        }

        [Test]
        public void IdenticalRowsCompareEqual()
        {
            //Arrange
            var first = GetRow(1);
            var second = GetRow(2);

            first["Variable"] = 1;
            second["Variable"] = 1;

            var key = new SnapshotRowKey(first.Row, _tableDef);
            
            //Act
            var result = RowDataComparer.Compare(key, first.Row, second.Row); //note: Keys are different but they are not compared

            //Assert
            result.Matched.Should().BeTrue();
        }

        [Test]
        public void DifferentRowsCompareUnequal()
        {
            //Arrange
            var first = GetRow(1);
            var second = GetRow(2);

            first["Variable"] = 1;
            second["Variable"] = 2;

            var key = new SnapshotRowKey(first.Row, _tableDef);
            
            //Act
            var result = RowDataComparer.Compare(key, first.Row, second.Row); //note: Keys are different but they are not compared

            //Assert
            result.Matched.Should().BeFalse();
        }

        [Test]
        public void DifferencesAreEnumerated()
        {
            //Arrange
            var first = GetRow(1);
            var second = GetRow(2);

            first["Variable"] = 1;
            second["Variable"] = 2;
            first["Variable2"] = "before";
            second["Variable2"] = "after";
            first["Match"] = "same";    //no difference, so not in the output
            second["Match"] = "same";   //no difference, so not in the output

            var key = new SnapshotRowKey(first.Row, _tableDef);
            
            //Act
            var result = RowDataComparer.Compare(key, first.Row, second.Row); //note: Keys are different but they are not compared

            //Assert
            var output = new Output();
            output.WrapLine($"Matched = {result.Matched}");
            output.WriteLine();
            output.FormatTable(result.Differences);
            output.Report.Verify();
        }
    }
}
