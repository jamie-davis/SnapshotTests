using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    public class TestDifferenceColumnAdder
    {
        private SnapshotObjectMother _om;

        [SetUp]
        public void SetUp()
        {
            _om = new SnapshotObjectMother();
        }

        [Test]
        public void IfThereAreNoMissingColumnsTheOriginalDiffsAreReturned()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var table2 = _om.Make<SnapshotObjectMother.Table2>(1, 2, 3, 4, 5);
            var refTable1 = _om.Make<SnapshotObjectMother.ReferencingTable>((1, 1),(2, 1));
            var refTable2 = _om.Make<SnapshotObjectMother.ReferencingTable2>((1, 3),(2, 4));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
            }

            foreach (var item in table2)
            {
                item.OtherVariable = $"other {item.Id}";
            }
            
            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table2[1].Variable = "edited";
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);

            //Act
            var tableDefinition = _om.Collection.GetTableDefinition(nameof(SnapshotObjectMother.Table2));
            var requiredColumns = new [] {nameof(SnapshotObjectMother.Table2.OtherVariable)};
            var result = DifferenceColumnAdder.RequireColumns(_om.Collection, diffs, tableDefinition, new List<string>(), before);

            //Assert
            var zipped = result.SelectMany(r => r.RowDifferences).Zip(diffs.SelectMany(d => d.RowDifferences));
            zipped.All(z => ReferenceEquals(z.First, z.Second))
                .Should().BeTrue();
        }

        [Test]
        public void MissingColumnsAreAddedToDifferences()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var table2 = _om.Make<SnapshotObjectMother.Table2>(1, 2, 3, 4, 5);
            var refTable1 = _om.Make<SnapshotObjectMother.ReferencingTable>((1, 1),(2, 1));
            var refTable2 = _om.Make<SnapshotObjectMother.ReferencingTable2>((1, 3),(2, 4));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
            }

            foreach (var item in table2)
            {
                item.OtherVariable = $"other {item.Id}";
            }
            
            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table2[1].Variable = "edited";
            refTable1[0].Variable = "edited";
            refTable1[1].ParentId = 3; //will required table1 row with key of 3 and also 2 because that is the before
            refTable2[0].Variable = "edited";
            refTable2[1].ParentId = 5; //will required table2 row with key of 5 but not 2 (the before) because it is edited and therefore already present
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);

            //Act
            var tableDefinition = _om.Collection.GetTableDefinition(nameof(SnapshotObjectMother.Table2));
            var requiredColumns = new [] {nameof(SnapshotObjectMother.Table2.OtherVariable)};
            var result = DifferenceColumnAdder.RequireColumns(_om.Collection, diffs, tableDefinition, requiredColumns, before);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void MissingColumnsForTablesNotPresentAreIgnored()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var table2 = _om.Make<SnapshotObjectMother.Table2>(1, 2, 3, 4, 5);
            var refTable1 = _om.Make<SnapshotObjectMother.ReferencingTable>((1, 1),(2, 1));
            var refTable2 = _om.Make<SnapshotObjectMother.ReferencingTable2>((1, 3),(2, 4));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
            }

            foreach (var item in table2)
            {
                item.OtherVariable = $"other {item.Id}";
            }
            
            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            refTable1[0].Variable = "edited";
            refTable1[1].ParentId = 3; //will required table1 row with key of 3 and also 2 because that is the before
            refTable2[0].Variable = "edited";
            refTable2[1].ParentId = 5; //will required table2 row with key of 5 but not 2 (the before) because it is edited and therefore already present
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);

            //Act
            var tableDefinition = _om.Collection.GetTableDefinition(nameof(SnapshotObjectMother.Table2));
            var requiredColumns = new [] {nameof(SnapshotObjectMother.Table2.OtherVariable)};
            var result = DifferenceColumnAdder.RequireColumns(_om.Collection, diffs, tableDefinition, requiredColumns, before);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }
    }
}