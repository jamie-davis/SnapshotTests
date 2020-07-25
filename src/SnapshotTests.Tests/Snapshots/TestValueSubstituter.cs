using System;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestValueSubstituter
    {
        private SnapshotObjectMother _om;
        private int _guidIx;

        [SetUp]
        public void SetUp()
        {
            _om = new SnapshotObjectMother();
            _guidIx = 1;
        }

        private Guid[] GuidArray(int length)
        {
            return Enumerable.Range(1, length).Select(n => Guid.Parse($"3E3B99D2-E581-4B00-8FB2-{_guidIx++:000000000000}")).ToArray();
        }

        [Test]
        public void SubstitutionsReplaceDifferences()
        {
            //Arrange
            var refGuids = GuidArray(3);
            var table1 = _om.Make<SnapshotObjectMother.GuidKeyTable>(GuidArray(4));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
            }

            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table1[1].Variable = "edited";
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var allDiffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);
            var diffs = DifferenceRegulator.ExpandDifferences(_om.Collection, allDiffs, before);

            var tables = diffs.Select(d => d.TableDefinition).ToList();
            var unpredictableCols = UnpredictableColumnLocator.Locate(tables).Single(t => t.Table.TableName == nameof(SnapshotObjectMother.GuidKeyTable));
            var columnValueSets = UnpredictableValueScanner.Scan(unpredictableCols, diffs);
            
            //Act
            var result = ValueSubstituter.Substitute(columnValueSets, diffs);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }

        [Test]
        public void SubstitutionsReplaceReferencedKeys()
        {
            //Arrange
            var refGuids = GuidArray(3);
            var table1 = _om.Make<SnapshotObjectMother.GuidKeyTable>(GuidArray(4));
            var refTable1 = _om.Make<SnapshotObjectMother.GuidRefTable>((refGuids[0], table1[0].GuidKeyId),(refGuids[1], table1[1].GuidKeyId));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
                refTable1.ToSnapshotTable(builder);
            }

            TakeSnapshot("Before");
            table1[0].Variable = "edited";
            table1[1].Variable = "edited";
            refTable1[0].GuidKeyParentId = table1[1].GuidKeyId;
            refTable1[1].GuidKeyParentId = table1[2].GuidKeyId;
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = DifferenceRegulator.ExpandDifferences(_om.Collection, SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after), before);

            var tables = diffs.Select(d => d.TableDefinition).ToList();
            var unpredictableCols = UnpredictableColumnLocator.Locate(tables);
            var columnValueSets = unpredictableCols.Select(u => UnpredictableValueScanner.Scan(u, diffs)).ToList();
            
            //Act
            var result = diffs;
            foreach (var columnValueSet in columnValueSets)
            {
                result = ValueSubstituter.Substitute(columnValueSet, result);
            }

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }
    }
}
