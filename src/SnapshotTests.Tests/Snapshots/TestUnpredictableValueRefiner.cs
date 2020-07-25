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
    public class TestUnpredictableValueRefiner
    {
        private SnapshotObjectMother _om;

        [SetUp]
        public void SetUp()
        {
            _om = new SnapshotObjectMother();
        }

        private Guid[] GuidArray(int length)
        {
            return Enumerable.Range(1, length).Select(n => Guid.NewGuid()).ToArray();
        }

        [Test]
        public void GuidKeysAreSubstituted()
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
            refTable1[0].Variable = "edited";
            refTable1[1].Variable = "edited";
            refTable1[0].ParentId = table1[3].Id;
            refTable1[1].ParentId = table1[4].Id;
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = DifferenceRegulator.ExpandDifferences(_om.Collection, SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after), before);

            var tables = diffs.Select(d => d.TableDefinition).ToList();
            
            //Act
            var result = UnpredictableValueRefiner.Refine(_om.Collection, diffs);

            //Assert
            var output = new Output();
            result.Report(output);
            output.Report.Verify();
        }
    }
}
