using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    public class TestMandatoryColumnFinder
    {
        private SnapshotObjectMother _om;

        class LocalTable
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public string Variable { get; set; }
        }

        [SetUp]
        public void SetUp()
        {
            _om = new SnapshotObjectMother();
        }

        [Test]
        public void MandatoryColumnsAreReturnedForCollection()
        {
            //Arrange
            var definer = _om.Collection.DefineTable("LocalTable")
                .CompareKey("CompareKey")
                .CompareKey("CompareKey2")
                .IsReference("ParentId", "ReferencingTable", "OtherId")
                .IsRequired("Required");
            
            var table1 = _om.Make<SnapshotObjectMother.Table>(1, 2, 3, 4, 5);
            var table2 = _om.Make<SnapshotObjectMother.Table2>(1, 2, 3, 4, 5);
            var refTable1 = _om.Make<SnapshotObjectMother.ReferencingTable>((1, 1),(2, 1));
            var refTable2 = _om.Make<SnapshotObjectMother.ReferencingTable2>((1, 3),(2, 4));
            var localTable = _om.Make<LocalTable>((1, 2));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                table2.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
                refTable2.ToSnapshotTable(builder);
                localTable.ToSnapshotTable(builder);
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
            localTable[0].ParentId = 1;
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);

            //Act
            var result = MandatoryColumnFinder.Find(_om.Collection, diffs);

            //Assert
            var output = new Output();
            var report = result.AsReport(rep => rep.RemoveBufferLimit()
                .AddColumn(t => t.TableDefinition.TableName, cc => cc.Heading("Table"))
                .AddChild(t => t.Columns, crep => crep.RemoveBufferLimit()
                    .AddColumn(c => c, cc => cc.Heading("Column")))
            );
            output.FormatTable(report);
            output.Report.Verify();
        }
    }
}