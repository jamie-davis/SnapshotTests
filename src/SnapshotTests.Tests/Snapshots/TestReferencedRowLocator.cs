using FluentAssertions;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestReferencedRowLocator
    {
        private SnapshotObjectMother _om;

        [SetUp]
        public void SetUp()
        {
            _om = new SnapshotObjectMother();
        }

        [Test]
        public void IfAllReferencesArePresentAnEmptyResultIsReturned()
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

            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table1[1].Variable = "edited";
            table2[2].Variable = "edited";
            table2[3].Variable = "edited";
            refTable1[0].Variable = "edited";
            refTable1[1].Variable = "edited";
            refTable2[0].Variable = "edited";
            refTable2[1].Variable = "edited";
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);

            //Act
            var result = ReferencedRowLocator.GetMissingRows(_om.Collection, diffs);

            //Assert
            result.Tables.Should().BeEmpty();
        }

        [Test]
        public void UneditedReferencedRowsAreExtracted()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var table2 = _om.Make<SnapshotObjectMother.Table2>(1, 2, 3, 4, 5);
            var refTable1 = _om.Make<SnapshotObjectMother.ReferencingTable>((1, 1),(2, 2));
            var refTable2 = _om.Make<SnapshotObjectMother.ReferencingTable2>((1, 1),(2, 2));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
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
            var result = ReferencedRowLocator.GetMissingRows(_om.Collection, diffs);

            //Assert
            var output = new Output();
            var resultRep = result.Tables.AsReport(rep => rep.RemoveBufferLimit()
                .AddColumn(t => t.TableDefinition.TableName)
                .AddChild(t => t.Keys, trep => trep.RemoveBufferLimit()
                    .AddColumn(c => c.ColumnName)
                    .AddColumn(c => c.RequestedValue)));
            output.FormatTable(resultRep); output.Report.Verify();

        }

        [Test]
        public void RowsCanBeRequiredForTablesWithoutDifferences()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var table2 = _om.Make<SnapshotObjectMother.Table2>(1, 2, 3, 4, 5);
            var refTable1 = _om.Make<SnapshotObjectMother.ReferencingTable>((1, 1),(2, 2));
            var refTable2 = _om.Make<SnapshotObjectMother.ReferencingTable2>((1, 1),(2, 2));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
            }

            TakeSnapshot("Before");
            refTable1[0].Variable = "edited";
            refTable1[1].ParentId = 3; //will required table1 row with key of 3 and also 2 because that is the before
            refTable2[0].Variable = "edited";
            refTable2[1].ParentId = 5; //will required table2 row with key of 5 but not 2 (the before) because it is edited and therefore already present
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);

            //Act
            var result = ReferencedRowLocator.GetMissingRows(_om.Collection, diffs);

            //Assert
            var output = new Output();
            var resultRep = result.Tables.AsReport(rep => rep.RemoveBufferLimit()
                .AddColumn(t => t.TableDefinition.TableName)
                .AddChild(t => t.Keys, trep => trep.RemoveBufferLimit()
                    .AddColumn(c => c.ColumnName)
                    .AddColumn(c => c.RequestedValue)));
            output.FormatTable(resultRep); output.Report.Verify();

        }
    }
}
