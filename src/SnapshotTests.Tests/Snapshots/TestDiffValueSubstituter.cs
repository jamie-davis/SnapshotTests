using System;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Tests.Snapshots.TestSupportUtils;
using SnapshotTests.Tests.TestDoubles;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    public class TestDiffValueSubstituter
    {
        private SnapshotObjectMother _om;

        public TestDiffValueSubstituter()
        {
            _om = new SnapshotObjectMother();
            _om.Collection.SetTimeSource(new FakeTimeSource("2020-09-20 09:13:21", "2020-09-20 09:13:42", "2020-09-20 09:14:42"));
        }

        [Test]
        public void ValuesAreReplacedInDiff()
        {
            //Arrange
            var table1 = _om.Make<SnapshotObjectMother.Unpredictables>(Guid.Parse("14123F35-519B-4DB0-957C-B773BF5D3082"), Guid.Parse("BCC981FA-FE9A-4A28-87C0-469901D6683A"));

            void TakeSnapshot(string name)
            {
                var builder = _om.NewSnapshot(name);
                table1.ToSnapshotTable(builder);
            }

            table1[0].TimeStamp = DateTime.Parse("2020-07-18 08:45");
            table1[0].Int = 100;
            table1[1].TimeStamp = DateTime.Parse("2020-07-18 08:45");
            table1[1].Int = 100;

            TakeSnapshot("Before");

            table1[0].TimeStamp = DateTime.Parse("2020-09-20 09:14:42");
            table1[0].Int = 101;
            table1[0].NullableInt = 100;

            table1[1].TimeStamp = DateTime.Parse("2020-09-20 19:14:42");

            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after);
            var cols = UnpredictableColumnLocator.Locate(_om.Collection.TablesInDefinitionOrder.ToList())
                .Single(t => t.Table.TableName == nameof(SnapshotObjectMother.Unpredictables));
            var columnValues = UnpredictableColumnLocator.Locate(diffs.Select(td => td.TableDefinition).ToList())
                .SelectMany(u => UnpredictableValueScanner.Scan(u, diffs))
                .ToList();
            var substitutes = columnValues
                .Select(uv => new SubstituteValues( uv, SubstitutionMaker.Make(uv.Values, TrackerMaker.Make(uv, _om.Collection))))
                .ToList();

            //Act
            var result = DiffValueSubstituter.Substitute(diffs, substitutes);

            //Assert
            var output = new Output();
            var originalFields = diffs.Single(d => d.TableDefinition.TableName == nameof(SnapshotObjectMother.Unpredictables))
                .RowDifferences.SelectMany(rd => rd.Differences.Differences);
            var resultFields = result.Single(d => d.TableDefinition.TableName == nameof(SnapshotObjectMother.Unpredictables))
                .RowDifferences.SelectMany(rd => rd.Differences.Differences);
            var combined = originalFields.Zip(resultFields,
                (o, r) => new
                {
                    o.Name, OriginalBefore = o.Before, OriginalAfter = o.After, SubstitutedBefore = r.Before,
                    SubstitutedAfter = r.After
                }).OrderBy(o => o.Name);
            output.FormatTable(combined);
            output.Report.Verify();
        }

        [Test]
        public void ReferenceIdValuesAreReplacedInDiff()
        {
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
            table1[0].Variable = "edited";
            table1[1].Variable = "edited";
            TakeSnapshot("After");

            var before = _om.GetSnapshot("Before");
            var after = _om.GetSnapshot("After");

            var diffs = DifferenceRegulator.ExpandDifferences(_om.Collection, SnapshotDifferenceCalculator.GetDifferences(_om.Collection, before, after), before);
            var cols = UnpredictableColumnLocator.Locate(_om.Collection.TablesInDefinitionOrder.ToList())
                .Single(t => t.Table.TableName == nameof(SnapshotObjectMother.Unpredictables));
            var columnValues = UnpredictableColumnLocator.Locate(diffs.Select(td => td.TableDefinition).ToList())
                .SelectMany(u => UnpredictableValueScanner.Scan(u, diffs))
                .ToList();
            var substitutes = columnValues
                .Select(uv => new SubstituteValues(uv, SubstitutionMaker.Make(uv.Values, TrackerMaker.Make(uv, _om.Collection))))
                .ToList();

            //Act
            var result = DiffValueSubstituter.Substitute(diffs, substitutes);

            //Assert
            var output = new Output();
            var originalFields = diffs.Single(d => d.TableDefinition.TableName == nameof(SnapshotObjectMother.ReferencingTable))
                .RowDifferences.SelectMany(rd => rd.Differences.Differences);
            var resultFields = result.Single(d => d.TableDefinition.TableName == nameof(SnapshotObjectMother.ReferencingTable))
                .RowDifferences.SelectMany(rd => rd.Differences.Differences);
            var combined = originalFields.Zip(resultFields,
                (o, r) => new
                {
                    o.Name, OriginalBefore = o.Before, OriginalAfter = o.After, SubstitutedBefore = r.Before,
                    SubstitutedAfter = r.After
                }).OrderBy(o => o.Name);
            output.FormatTable(combined);
            output.Report.Verify();
        }
    }
}
