using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// This class locates all rows in a set of snapshot differences where a difference references rows from another table which are not present in
    /// the difference set. The purpose of this is to enable those referenced rows to be added to the set for clarity. This should allow the test result
    /// to prove whether or not the reference was set to a valid row.
    /// </summary>
    internal static class ReferencedRowLocator
    {
        /// <summary>
        /// Scan the differences in a set of table <see cref="SnapshotTableDifferences"/> for columns flagged as referencing rows in the snapshot. If the
        /// referenced row is present, but does not contain any differences, it will be returned as an additional reference. This method reads the differences,
        /// but will not make any changes.
        /// </summary>
        /// <param name="collection">The snapshot collection. Needed for table definitions.</param>
        /// <param name="tableDiffs">The difference set that will be analysed</param>
        /// <returns>An <see cref="AdditionalReferencedRows"/> instance containing the referenced row details. Only rows not currently in the difference set
        /// will be included.</returns>
        public static AdditionalReferencedRows GetMissingRows(SnapshotCollection collection, IReadOnlyCollection<SnapshotTableDifferences> tableDiffs)
        {
            var allKeysByReferencedTable = GeyKeysByReferencedTable(collection, tableDiffs)
                .Where(rk => rk.KeyValue != null)
                .GroupBy(rk => new {rk.ReferencedTableDefinition.TableName, rk.ColumnName})
                .ToList();

            var requiredTableNotPresent = allKeysByReferencedTable
                .Where(g => !tableDiffs.Any(td => td.TableDefinition.TableName == g.Key.TableName))
                .ToList();

            var missingKeys = allKeysByReferencedTable.Join(tableDiffs, kg => kg.Key.TableName, td => td.TableDefinition.TableName,
                (kg, td) => new
                {
                    TableDefinition = kg.Key.TableName, 
                    KeyField = kg.Key.ColumnName,
                    MissingRows = kg.Where(k => !td.RowDifferences.Any(row => ValueComparer.Compare(row?.After?.GetField(k.ColumnName), k.KeyValue) == 0))
                        .Select(k => k.KeyValue).ToList()
                })
                .Concat(requiredTableNotPresent.Select(r => new
                {
                    TableDefinition = r.Key.TableName,
                    KeyField = r.Key.ColumnName,
                    MissingRows = allKeysByReferencedTable.Where(g => g.Key.TableName == r.Key.TableName)
                        .SelectMany(g => g.Select(k => k.KeyValue))
                        .ToList()
                }))
                .GroupBy(req => req.TableDefinition)
                .Select(g => new AdditionalReferencedRows.RequiredTableRows(collection.GetTableDefinition(g.Key), g.SelectMany(r=> r.MissingRows.Select(mr => new AdditionalReferencedRows.RowRequest(r.KeyField, mr))).ToList()))
                .ToList();

            return new AdditionalReferencedRows(missingKeys);
        }

        private static IEnumerable<(TableDefinition ReferencedTableDefinition, string ColumnName, object KeyValue)> GeyKeysByReferencedTable(SnapshotCollection collection, IReadOnlyCollection<SnapshotTableDifferences> tableDiffs)
        {
            foreach (var tableDiff in tableDiffs)
            {
                var referenceColumns = tableDiff.TableDefinition
                    .Columns
                    .Where(n => n.ReferencedTableName != null && n.ReferencedPropertyName != null)
                    .ToList();
                foreach (var referenceColumn in referenceColumns)
                {
                    var diffs = tableDiff
                        .RowDifferences
                        .SelectMany(d => d.Differences.Differences)
                        .Where(d => d.Name == referenceColumn.Name);
                    foreach (var fieldDifference in diffs)
                    {
                        var referencedTableDefinition = collection.GetTableDefinition(referenceColumn.ReferencedTableName);

                        yield return (referencedTableDefinition, referenceColumn.ReferencedPropertyName, fieldDifference.Before);
                        yield return (referencedTableDefinition, referenceColumn.ReferencedPropertyName, fieldDifference.After);
                    }
                }
            }

        }
    }
}