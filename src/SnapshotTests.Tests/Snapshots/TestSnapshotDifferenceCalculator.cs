using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotDifferenceCalculator
    {
        private const string TableName = "CompoundKeyTable";
        private const string IdCol = "Id";
        private const string NameCol = "Name";

        private SnapshotCollection _collection;
        private Snapshot _snapshot;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable(TableName).PrimaryKey(IdCol).PrimaryKey(NameCol);
        }

        private Snapshot MakeSnapshot(string name, int startRow, int numRows, params object[][] rowValues)
        {
            return MakeSnapshot(name, startRow, numRows, (IEnumerable<object[]>)rowValues);
        }

        private Snapshot MakeSnapshot(string name, int startRow = 1, int numRows = 10, IEnumerable<object[]> rowValues = null)
        {
            var rowValuesList = rowValues?.ToList() ?? new List<object[]>();
            var builder = _collection.NewSnapshot(name);
            for (var n = startRow; n < startRow + numRows; n++)
            {
                var cpRow = builder.AddNewRow(TableName);
                var values = n - startRow < rowValuesList.Count ? rowValuesList[n - startRow] : null;
                PopulateRow(n, cpRow, values);
            }

            return _collection.GetSnapshot(name);
        }

        private object[] MakeValueSet(params object[] values) => values;

        private void PopulateRow(int n, RowBuilder rowBuilder, params object[] additional)
        {
            if (additional == null || additional.Length == 0)
                additional = new object[] {n, (double) n / 4, (decimal) n / 5};

            rowBuilder[IdCol] = n;
            rowBuilder[NameCol] = $"Name {n}";
            rowBuilder["Details"] = $"Row details {n}";

            foreach (var additionalValue in additional.Where(a => a != null))
            {
                var name = additionalValue.GetType().Name + "Col";
                rowBuilder[name] = additionalValue;
            }
        }

        [Test]
        public void ShouldCalculateDifferences()
        {
            //Arrange
            var before = MakeSnapshot("Test", 1, 3, MakeValueSet(5), MakeValueSet(6), MakeValueSet(7));
            var after = MakeSnapshot("Test2", 1, 3, MakeValueSet(6), MakeValueSet(6), MakeValueSet(6));

            //Act
            var result = SnapshotDifferenceCalculator.GetDifferences(_collection, before, after);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void TablesNotInFirstSnapshotShouldShowAsInserts()
        {
            //Arrange
            var before = MakeSnapshot("Test2", 1, 0);
            var after = MakeSnapshot("Test", 1, 3, MakeValueSet(5), MakeValueSet(6), MakeValueSet(7));

            //Act
            var result = SnapshotDifferenceCalculator.GetDifferences(_collection, before, after);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void TablesNotInSecondSnapshotShouldShowAsDeletes()
        {
            //Arrange
            var before = MakeSnapshot("Test", 1, 3, MakeValueSet(5), MakeValueSet(6), MakeValueSet(7));
            var after = MakeSnapshot("Test2", 1, 0);

            //Act
            var result = SnapshotDifferenceCalculator.GetDifferences(_collection, before, after);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }
    }
}