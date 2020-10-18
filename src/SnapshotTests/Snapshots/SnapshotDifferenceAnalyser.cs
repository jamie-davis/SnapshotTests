using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// This class extracts the differences between two snapshots
    /// </summary>
    internal static class SnapshotDifferenceAnalyser
    {
        internal static SnapshotDifferences ExtractDifferences(SnapshotCollection collection, Snapshot before, Snapshot after, ChangeReportOptions changeReportOptions = ChangeReportOptions.Default)
        {
            var tableDiffs = SnapshotDifferenceCalculator.GetDifferences(collection, before, after);
            tableDiffs = SnapshotDifferenceSorter.SortDifferences(collection, tableDiffs);
            tableDiffs = DifferenceRegulator.CleanDifferences(collection, tableDiffs, before, (changeReportOptions & ChangeReportOptions.NoSubs) == 0);
            return new SnapshotDifferences(tableDiffs);
        }

        public static bool Match(SnapshotCollection collection, Snapshot before, Snapshot after)
        {
            foreach (var tableDefinition in collection.TablesInDefinitionOrder.Where(t => !t.ExcludeFromComparison))
            {
                //find all keys
                var beforeRows = SnapshotKeyExtractor.GetKeys(before, tableDefinition) ?? new Dictionary<SnapshotRowKey, SnapshotRow>();
                var afterRows = SnapshotKeyExtractor.GetKeys(after, tableDefinition) ?? new Dictionary<SnapshotRowKey, SnapshotRow>();
                if (!beforeRows.Keys.SequenceEqual(afterRows.Keys))
                    return false; //snapshots have row difference

                foreach (var snapshotRowKey in beforeRows.Keys)
                {
                    var beforeRow = beforeRows[snapshotRowKey];
                    var afterRow = afterRows[snapshotRowKey];
                    var match = RowDataComparer.Compare(tableDefinition, snapshotRowKey, beforeRow, afterRow);
                    if (!match.Matched)
                        return false;
                }
            }

            return true;
        }
    }
}