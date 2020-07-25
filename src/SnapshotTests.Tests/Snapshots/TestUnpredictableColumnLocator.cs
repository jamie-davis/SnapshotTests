using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsole.OutputFormatting;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestUnpredictableColumnLocator
    {
        private SnapshotObjectMother _om;

        public TestUnpredictableColumnLocator()
        {
            _om = new SnapshotObjectMother();
        }

        [Test]
        public void UnpredictableColumnsAreExtracted()
        {
            //Arrange
            var tables = _om.Collection.TablesInDefinitionOrder.ToList();

            //Act
            var result = UnpredictableColumnLocator.Locate(tables);

            //Assert
            var output = new Output();
            var report = result.AsReport(rep => rep.Title("Extracted Unpredictable Columns")
                .RemoveBufferLimit()
                .AddColumn(t => t.Table.TableName)
                .AddChild(t => t.Columns, colRep => colRep.RemoveBufferLimit()
                    .AddColumn(c => c.Column.Name, cc => cc.Heading("Unpredictable Column"))
                    .AddChild(c => c.ReferencingColumns, refRep => refRep.Title("Referenced By")
                        .RemoveBufferLimit()
                        .AddColumn(r => r.SourceTable.TableName, cc => cc.Heading("Table"))
                        .AddColumn(rc => string.Join(", ", rc.References.Select(r => r.Name)), cc => cc.Heading("Columns")))));
            output.FormatTable(report);
            output.Report.Verify();

        }
    }
}