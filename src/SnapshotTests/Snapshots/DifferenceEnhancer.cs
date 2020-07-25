using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// This class manipulates sets of differences to enhance clarity. For example, a set of differences may contain edits to references to other rows that
    /// are not currently part of the difference set. To aid understanding of what changed, and possibly revealing issues, the referenced rows should be
    /// forced into the difference set, even though they do not contain any edits.
    /// </summary>
    internal static class DifferenceEnhancer
    {
        /// <summary>
        /// Add mandatory rows to a difference set as references if they do not currently exist.
        /// </summary>
        /// <param name="collection">The collection that generated the differences.</param>
        /// <param name="tableDiffs">The difference set.</param>
        /// <param name="rows">The rows that are mandatory.</param>
        /// <param name="additionalRows">The rows that are mandatory.</param>
        /// <param name="differenceSide">One of the snapshots - it does not generally matter which because rows that were not extracted should be identical in both snapshots.</param>
        internal static List<SnapshotTableDifferences> RequireRows(SnapshotCollection collection, List<SnapshotTableDifferences> tableDiffs, AdditionalReferencedRows additionalRows, Snapshot differenceSide)
        {
            var result = new List<SnapshotTableDifferences>();
            foreach (var table in collection.TablesInDefinitionOrder)
            {
                var current = tableDiffs.SingleOrDefault(t => t.TableDefinition.TableName == table.TableName);
                var additional = additionalRows.Tables.SingleOrDefault(t => t.TableDefinition.TableName == table.TableName);
                if (additional == null && current != null)
                    result.Add(current);
                else if (additional != null)
                {
                    var snapRows = differenceSide.Rows(additional.TableDefinition.TableName).ToList();
                    var addRows = additional.Keys.Select(rowRequest => snapRows.SingleOrDefault(r => r.GetField(rowRequest.ColumnName)?.Equals(rowRequest.RequestedValue) ?? false))
                        .Where(r => r != null)
                        .ToList();

                    var requiredSnapRows = snapRows.Select((r, i) => new
                        {
                            Index = i, SnapRow = r,
                            Difference = current?.RowDifferences.SingleOrDefault(d => ReferenceEquals(d.After, r) || ReferenceEquals(d.Before, r)),
                            Additional = additional.Keys.FirstOrDefault(a => r.GetField(a.ColumnName)?.Equals(a.RequestedValue) ?? false)
                        })
                        .Where(req => req.Additional != null || req.Difference != null)
                        .ToList();

                    var allDiffs = requiredSnapRows
                        .Select(r => r.Difference ?? new RowDifference(new SnapshotRowKey(r.SnapRow, table), DifferenceType.Reference, r.SnapRow, r.SnapRow))
                        .ToList();

                    var newDiffs = new SnapshotTableDifferences(allDiffs, additional.TableDefinition);
                    result.Add(newDiffs);
                }
            }

            return result;
        }
    }
}