using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotDifferenceAnalyser
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
        public void EqualSnapshotsReturnEqualStatus()
        {
            //Arrange
            var before = MakeSnapshot("Test");
            var after = MakeSnapshot("Test2");

            //Act
            var result = SnapshotDifferenceAnalyser.Match(_collection, before, after);

            //Assert
            result.Should().BeTrue();
        }

        [Test]
        public void DeletedRowMakesSnapshotUnequal()
        {
            //Arrange
            var before = MakeSnapshot("Test", 1, 5);
            var after = MakeSnapshot("Test2", 2, 4);

            //Act
            var result = SnapshotDifferenceAnalyser.Match(_collection, before, after);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void AdditionalRowMakesSnapshotUnequal()
        {
            //Arrange
            var before = MakeSnapshot("Test", 1, 5);
            var after = MakeSnapshot("Test2", 1, 6);

            //Act
            var result = SnapshotDifferenceAnalyser.Match(_collection, before, after);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void NonKeyValueDifferenceMakesSnapshotUnequal()
        {
            //Arrange
            var before = MakeSnapshot("Test", 1, 1, MakeValueSet(5));
            var after = MakeSnapshot("Test2", 1, 1, MakeValueSet(6));

            //Act
            var result = SnapshotDifferenceAnalyser.Match(_collection, before, after);

            //Assert
            result.Should().BeFalse();
        }

        [Test]
        public void SnapshotShouldNotConsiderExcludedTables()
        {
            //Arrange
            var before = MakeSnapshot("Test", 1, 1, MakeValueSet(5));
            var after = MakeSnapshot("Test2", 1, 1, MakeValueSet(6));
            _collection.DefineTable(TableName).ExcludeFromComparison();

            //Act
            var result = SnapshotDifferenceAnalyser.Match(_collection, before, after);

            //Assert
            result.Should().BeTrue(); //it can only be a match if the table is ignored
        }

        [Test]
        public void MatchResultShouldContainDifferences()
        {
            //Arrange
            var before = MakeSnapshot("Test", 1, 3, MakeValueSet(5), MakeValueSet(6), MakeValueSet(7));
            var after = MakeSnapshot("Test2", 1, 3, MakeValueSet(6), MakeValueSet(6), MakeValueSet(6));

            //Act
            var result = SnapshotDifferenceAnalyser.ExtractDifferences(_collection, before, after);

            //Assert
            var output = new Output();
            foreach (var tableDiffs in result.TableDifferences)
            {
                var report = tableDiffs
                    .RowDifferences
                    .AsReport(rep => AttachDiffs(rep, tableDiffs));

                output.FormatTable(report);
            }


            output.Report.Verify();

        }

        private void AttachDiffs(ReportParameters<RowDifference> rep, SnapshotTableDifferences tableDiffs)
        {
            var allCols = tableDiffs.RowDifferences.SelectMany(r => r.Differences.Differences.Select(d => d.Name)).Distinct();
            var key = tableDiffs.RowDifferences.FirstOrDefault()?.Key;
            
            rep.Title(tableDiffs.TableDefinition.TableName);

            rep.AddColumn(rd => rd.DifferenceType.ToString(), cc => cc.Heading("Difference"));
            
            if (key != null)
            {
                var keyIndex = 0;
                foreach (var keyField in key.GetFieldNames())
                    rep.AddColumn(rd => rd.Key.AllKeys.Skip(keyIndex).First(), cc => cc.Heading(keyField));
            }

            foreach (var col in allCols)
            {
                rep.AddColumn(rep.Lambda(rd => rd.Differences.Differences.FirstOrDefault(fd => fd.Name == col)?.Before), cc => cc.Heading($"{col} before"));
                rep.AddColumn(rep.Lambda(rd => rd.Differences.Differences.FirstOrDefault(fd => fd.Name == col)?.After), cc => cc.Heading($"{col} after"));
            }

        }
    }
}