using System;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    public class TestDifferenceEnhancer
    {
        private SnapshotObjectMother _om;

        [SetUp]
        public void SetUp()
        {
            _om = new SnapshotObjectMother();
        }

        [Test]
        public void RowsAreAddedToDifferences()
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
            table2[1].Variable = "edited";
            refTable1[0].Variable = "edited";
            refTable1[1].ParentId = 3; //will required table1 row with key of 3 and also 2 because that is the before
            refTable2[0].Variable = "edited";
            refTable2[1].ParentId = 5; //will required table2 row with key of 5 but not 2 (the before) because it is edited and therefore already present
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);
            var rows = ReferencedRowLocator.GetMissingRows(_om.Collection, diffs);

            //Act
            var result = DifferenceEnhancer.RequireRows(_om.Collection, diffs, rows, before);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void TablesAreAddedToDifferences()
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
            refTable1[0].Variable = "edited";
            refTable1[1].ParentId = 3; //will required table1 row with key of 3 and also 2 because that is the before
            refTable2[0].Variable = "edited";
            refTable2[1].ParentId = 5; //will required table2 row with key of 5 but not 2 (the before) because it is edited and therefore already present
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);
            var rows = ReferencedRowLocator.GetMissingRows(_om.Collection, diffs);

            //Act
            var result = DifferenceEnhancer.RequireRows(_om.Collection, diffs, rows, before);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void TablesWithNoReferencesArePreserved()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var table2 = _om.Make<SnapshotObjectMother.Table2>(1, 2, 3, 4, 5);
            var table3 = _om.Make<SnapshotObjectMother.GuidKeyTable>(Guid.Parse("D04A9F30-DEAA-4E87-B88A-C244D729BBCA"));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                table3.ToSnapshotTable(builder);
            }

            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table3[0].OtherVariable = "edited";
            table3.Add(new SnapshotObjectMother.GuidKeyTable() { GuidKeyId = Guid.Parse("D0E1FDD6-51E5-467C-91FD-E07BFD8D9830"), Variable = "New"});
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);
            var rows = ReferencedRowLocator.GetMissingRows(_om.Collection, diffs);

            //Act
            var result = DifferenceEnhancer.RequireRows(_om.Collection, diffs, rows, before);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void NewRowsInParentPreservedWhenRequiringRows()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var refTable1 = _om.Make<SnapshotObjectMother.ReferencingTable>((1, 1),(2, 1));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
            }

            TakeSnapshot("Before");
            table1.Add(new SnapshotObjectMother.Table { Id = 50, Variable = "Preserve this"});
            table1.Add(new SnapshotObjectMother.Table { Id = 55, Variable = "Preserve this"});
            refTable1[0].Variable = "Change";
            refTable1.Add(new SnapshotObjectMother.ReferencingTable { Id = 30, ParentId = 4});
            refTable1.Add(new SnapshotObjectMother.ReferencingTable { Id = 40, ParentId = 55});
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);
            var rows = ReferencedRowLocator.GetMissingRows(_om.Collection, diffs);

            //Act
            var result = DifferenceEnhancer.RequireRows(_om.Collection, diffs, rows, before);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }
    }
}
