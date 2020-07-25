using System;
using System.Linq;
using System.Linq.Expressions;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSnapshotColumnFormatter
    {
        private const string TestTableName = "Test";
        private SnapshotCollection _collection;
        private SnapshotBuilder _snapshot;
        private RowBuilder _row;
        private TableDefinition _table;
        private ColumnConfig _cc;

        [SetUp]
        public void SetUp()
        {
            _collection = new SnapshotCollection();
            _collection.DefineTable(TestTableName).CompareKey("Id");
            _table = _collection.GetTableDefinition(TestTableName);
            _snapshot = _collection.NewSnapshot("Test");

            //Borrow a column config so that we can test with it.
            void ExtractColumnDefForTest(ColumnConfig config)
            {
                _cc = config;
            }

            var report = new[] {"T"}.AsReport(rep => rep.AddColumn(t => t, ExtractColumnDefForTest));
        }

        [Test]
        public void HeadingIsSet()
        {
            //Arrange
            var col = new SnapshotColumnInfo("ColName");
            
            //Act
            var report = new[] {"T"}.AsReport(rep => rep.AddColumn(t => t, cc => SnapshotColumnFormatter.Format(cc, _table, col)));

            //Assert
            var output = new Output();
            output.WrapLine("Here we expect the report column to have the heading \"Colname\":");
            output.WriteLine();
            output.FormatTable(report);
            output.Report.Verify();
        }

        [Test]
        public void AlignmentIsSetForColumn()
        {
            //Arrange
            var testValues = new object[]
            {
                1.5,
                1,
                (short) 1,
                (byte) 1,
                "Text"
            };
            var builder = _snapshot.AddNewRow(TestTableName);

            //Act
            foreach (var testValue in testValues)
            {
                builder[$"Col{testValue.GetType().Name}"] = testValue;
            }

            //Assert
            var output = new Output();
            _snapshot.ReportContents(output);
            output.Report.Verify();
        }

        [Test]
        public void AlignmentIsSetForMixedColumn()
        {
            //Arrange
            var rowData = new[]
            {
                new object[] {1.5, 1},
                new object[] {1, "Text"},
            };

            //Act
            foreach (var colData in rowData)
            {
                var row = _snapshot.AddNewRow(TestTableName);
                row["Col0"] = colData[0];
                row["Col1"] = colData[1];
            }

            //Assert
            var output = new Output();
            output.WrapLine("Columns with all numeric data should be right aligned, columns including non-numeric data should be left aligned. Col0 contains a double and an int, Col1 contains an int and a string.");
            output.WriteLine();
            _snapshot.ReportContents(output);
            output.Report.Verify();
        }
    }
}
