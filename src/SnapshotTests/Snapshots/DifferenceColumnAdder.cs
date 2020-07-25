using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class DifferenceColumnAdder
    {
        public static List<SnapshotTableDifferences> RequireColumns(SnapshotCollection collection, List<SnapshotTableDifferences> diffs, TableDefinition table, IEnumerable<string> requiredColumns, Snapshot before)
        {
            var result = new List<SnapshotTableDifferences>();
            var columnList = requiredColumns.ToList();
            foreach (var differences in diffs)
            {
                if (differences.TableDefinition.TableName == table.TableName)
                {
                    var rowDiffs = differences.RowDifferences.Select(r => RequireColumns(r, columnList)).ToList();
                    var newDiffs = new SnapshotTableDifferences(rowDiffs, differences.TableDefinition);
                    result.Add(newDiffs);
                }
                else
                    result.Add(differences);
            }

            return result;
        }

        private static RowDifference RequireColumns(RowDifference diff, List<string> requiredColumns)
        {
            var missingCols = requiredColumns
                .Where(col => !diff.Differences?.Differences.Any(d => d.Name == col) ?? true)
                .ToList();
            if (!missingCols.Any())
                return diff;

            var differences = diff.Differences?.Differences?.ToList() ?? new List<FieldDifference>();

            FieldDifference MakeAddedColumnDifference(string col)
            {
                var value = (diff.Before ?? diff.After)?.GetField(col);
                return new FieldDifference(col, value, value);
            }

            differences.AddRange(missingCols.Select(MakeAddedColumnDifference));
            var diffResult = new RowDataCompareResult(diff.Differences?.Matched ?? true, differences, diff.Before, diff.After);

            return new RowDifference(diff.Key, diffResult, diff.DifferenceType);
        }
    }
}