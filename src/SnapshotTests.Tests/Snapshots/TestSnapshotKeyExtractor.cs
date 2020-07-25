using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    public class TestSnapshotKeyExtractor
    {
        private const string CompareKeyTableName = "CompareKeyTable";
        private const string PrimaryKeyTableName = "PrimaryKeyTable";
        private const string CompoundKeyTableName = "CompoundKeyTable";
        private const string IdCol = "Id";
        private const string NameCol = "Name";
        private SnapshotCollection _collection;
        private Snapshot _snapshot;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable(CompareKeyTableName).CompareKey(NameCol).PrimaryKey(IdCol);
            _collection.DefineTable(PrimaryKeyTableName).PrimaryKey(IdCol);
            _collection.DefineTable(CompoundKeyTableName).PrimaryKey(IdCol).PrimaryKey(NameCol);
            var builder = _collection.NewSnapshot("Test");
            for (var n = 1; n <= 10; n++)
            {
                var ckRow = builder.AddNewRow(CompareKeyTableName);
                var pkRow = builder.AddNewRow(PrimaryKeyTableName);
                var cpRow = builder.AddNewRow(CompoundKeyTableName);
                PopulateRows(n, ckRow, pkRow, cpRow);
            }

            _snapshot = _collection.GetSnapshot("Test");
        }

        private void PopulateRows(int n, params RowBuilder[] rows)
        {
            foreach (var rowBuilder in rows)
            {
                rowBuilder[IdCol] = n;
                rowBuilder[NameCol] = $"Name {n}";
                rowBuilder["Details"] = $"Row details {n}";
            }
        }

        [Test]
        public void CompareKeyTableKeysAreExtracted()
        {
            //Act
            var keys = SnapshotKeyExtractor.GetKeys(_snapshot, _collection.GetTableDefinition(CompareKeyTableName));

            //Assert
            var output = new Output();
            var rep = keys.MakeKeyReport();
            output.FormatTable(rep);
            output.Report.Verify();
        }

        [Test]
        public void PrimaryKeyTableKeysAreExtracted()
        {
            //Act
            var keys = SnapshotKeyExtractor.GetKeys(_snapshot, _collection.GetTableDefinition(PrimaryKeyTableName));

            //Assert
            var output = new Output();
            var rep = keys.MakeKeyReport();
            output.FormatTable(rep);
            output.Report.Verify();
        }

        [Test]
        public void CompoundKeyTableKeysAreExtracted()
        {
            //Act
            var keys = SnapshotKeyExtractor.GetKeys(_snapshot, _collection.GetTableDefinition(CompoundKeyTableName));

            //Assert
            var output = new Output();
            var rep = keys.MakeKeyReport();
            output.FormatTable(rep);
            output.Report.Verify();
        }
    }
}
