using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class SnapshotDifferenceCalculator
    {
        public static List<SnapshotTableDifferences> GetDifferences(SnapshotCollection collection, Snapshot before, Snapshot after)
        {
            return collection
                .TablesInDefinitionOrder
                .Select(t => GetTableDifferences(t, before, after))
                .Where(t => t != null)
                .ToList();
        }

        private static SnapshotTableDifferences GetTableDifferences(TableDefinition tableDefinition, Snapshot before, Snapshot after)
        {
            var beforeKeys = SnapshotKeyExtractor.GetKeys(before, tableDefinition)?.Keys ?? new Dictionary<SnapshotRowKey, SnapshotRow>().Keys;
            var afterKeys = SnapshotKeyExtractor.GetKeys(after, tableDefinition)?.Keys ?? new Dictionary<SnapshotRowKey, SnapshotRow>().Keys;

            var deletedKeys = beforeKeys.Except(afterKeys).ToList();
            var insertedKeys = afterKeys.Except(beforeKeys).ToList();

            var differences = beforeKeys.Intersect(afterKeys)
                .Select(k => new {Key = k, Before = before.GetRow(k, tableDefinition.TableName), After = after.GetRow(k, tableDefinition.TableName)})
                .Select(rr => new {rr.Key, Differences = RowDataComparer.Compare(rr.Key, rr.Before, rr.After)})
                .Where(m => !m.Differences.Matched)
                .ToList();

            if (!deletedKeys.Any() && !insertedKeys.Any() && !differences.Any())
            {
                return null;
            }

            var rowDiffs = deletedKeys.Select(k => new RowDifference(k, DifferenceType.Deleted, before.GetRow(k, tableDefinition.TableName), null))
                .Concat(insertedKeys.Select(k => new RowDifference(k, DifferenceType.Inserted, null, after.GetRow(k, tableDefinition.TableName))))
                .Concat(differences.Select(d => new RowDifference(d.Key, d.Differences, DifferenceType.Updated)))
                .OrderBy(d => d.Key)
                .ToList();

            return new SnapshotTableDifferences(rowDiffs, tableDefinition);
        }
    }
}