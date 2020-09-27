using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotComparer
    {
        private const string CompoundKeyTable = "CompoundKeyTable";
        private const string SimpleKeyTable = "SimpleKeyTable";
        private const string IdCol = "Id";
        private const string NameCol = "Name";

        private SnapshotCollection _collection;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
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
                var cpRow = builder.AddNewRow(CompoundKeyTable);
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
        public void ComparerShouldReportDifferences()
        {
            //Arrange
            var beforeData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
                new {Name = "Test 3", A = 5, B = 6, C = 7},
            };
            var afterData = new[]
            {
                new {Name = "Test 1", A = 6, B = 6, C = 6},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
                new {Name = "Test 3", A = 5, B = 7, C = 6},
            };

            var beforeBuilder = _collection.NewSnapshot("before");
            beforeData.ToSnapshotTable(beforeBuilder, "Table", "Name");
            var afterBuilder = _collection.NewSnapshot("after");
            afterData.ToSnapshotTable(afterBuilder, "Table");

            var output = new Output();

            //Act
            SnapshotComparer.ReportDifferences(_collection, _collection.GetSnapshot("before"), _collection.GetSnapshot("after"), output);

            //Assert
            output.Report.Verify();
        }

        [Test]
        public void NewRowsAreReported()
        {
            //Arrange
            var beforeData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
            };
            var afterData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
                new {Name = "Test 3", A = 5, B = 7, C = 6},
            };

            var beforeBuilder = _collection.NewSnapshot("before");
            beforeData.ToSnapshotTable(beforeBuilder, "Table", "Name");
            var afterBuilder = _collection.NewSnapshot("after");
            afterData.ToSnapshotTable(afterBuilder, "Table");

            var output = new Output();

            //Act
            SnapshotComparer.ReportDifferences(_collection, _collection.GetSnapshot("before"), _collection.GetSnapshot("after"), output);

            //Assert
            output.Report.Verify();
        }

        [Test]
        public void DeletedRowsAreReported()
        {
            //Arrange
            var beforeData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
                new {Name = "Test 3", A = 5, B = 7, C = 6},
            };
            var afterData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
            };

            var beforeBuilder = _collection.NewSnapshot("before");
            beforeData.ToSnapshotTable(beforeBuilder, "Table", "Name");
            var afterBuilder = _collection.NewSnapshot("after");
            afterData.ToSnapshotTable(afterBuilder, "Table");

            var output = new Output();

            //Act
            SnapshotComparer.ReportDifferences(_collection, _collection.GetSnapshot("before"), _collection.GetSnapshot("after"), output);

            //Assert
            output.Report.Verify();
        }

        [Test]
        public void MultipleTypesOfEditMakeSense()
        {
            //Arrange
            var beforeData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
                new {Name = "Test 3", A = 3, B = 7, C = 6},
            };
            var afterData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 8, B = 6, C = 3},
                new {Name = "Test 4", A = 10, B = 7, C = 6},
            };

            var beforeBuilder = _collection.NewSnapshot("before");
            beforeData.ToSnapshotTable(beforeBuilder, "Table", "Name");
            var afterBuilder = _collection.NewSnapshot("after");
            afterData.ToSnapshotTable(afterBuilder, "Table");

            var output = new Output();

            //Act
            SnapshotComparer.ReportDifferences(_collection, _collection.GetSnapshot("before"), _collection.GetSnapshot("after"), output);

            //Assert
            output.Report.Verify();
        }

        [Test]
        public void SubstitutionsCanBeDisabled()
        {
            //Arrange
            var beforeData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 5, B = 6, C = 7},
                new {Name = "Test 3", A = 3, B = 7, C = 6},
            };
            var afterData = new[]
            {
                new {Name = "Test 1", A = 5, B = 6, C = 7},
                new {Name = "Test 2", A = 8, B = 6, C = 3},
                new {Name = "Test 4", A = 10, B = 7, C = 6},
            };

            var beforeBuilder = _collection.NewSnapshot("before");
            beforeData.ToSnapshotTable(beforeBuilder, "Table", "Name");
            var afterBuilder = _collection.NewSnapshot("after");
            afterData.ToSnapshotTable(afterBuilder, "Table");
            _collection.DefineTable("Table").IsUnpredictable("A").IsUnpredictable("B").IsUnpredictable("C");

            var output = new Output();

            //Act
            SnapshotComparer.ReportDifferences(_collection, _collection.GetSnapshot("before"), _collection.GetSnapshot("after"), output, ChangeReportOptions.NoSubs);

            //Assert
            output.Report.Verify();
        }
    }
}
