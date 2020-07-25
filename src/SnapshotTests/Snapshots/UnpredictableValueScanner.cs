using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// Scan a set of differences for instances of unpredictable values and return the values in sets
    /// </summary>
    internal static class UnpredictableValueScanner
    {
        public static List<ColumnValueSet> Scan(UnpredictableColumnSource table, List<SnapshotTableDifferences> tableDiffs)
        {
            return table.Columns.Select(c => ColumnValues(table, tableDiffs, c)).Distinct().ToList();
        }

        private static ColumnValueSet ColumnValues(UnpredictableColumnSource table, List<SnapshotTableDifferences> tableDiffs, UnpredictableColumnSource.UnpredictableColumn column)
        {
            var tableForValues = tableDiffs.SingleOrDefault(d => d.TableDefinition.TableName == table.Table.TableName);
            List<object> values = null;
            if (tableForValues != null) values = ColumnValues(column.Column.Name, tableForValues).ToList();

            var references = ColumnReferences(column, tableDiffs).Distinct().ToList();
            
            return new ColumnValueSet(table.Table, column.Column, values ?? new List<object>(), references ?? new List<object>());
        }

        private static IEnumerable<object> ColumnReferences(UnpredictableColumnSource.UnpredictableColumn column, List<SnapshotTableDifferences> tableDiffs)
        {
            foreach (var refCol in column.ReferencingColumns)
            {
                var tableForValues = tableDiffs.SingleOrDefault(d => d.TableDefinition.TableName == refCol.SourceTable.TableName);
                if (tableForValues != null)
                {
                    foreach(var value in ColumnValues(column.Column.Name, tableForValues))
                    {
                        yield return value;
                    }
                }
            }
        }

        private static IEnumerable<object> ColumnValues(string column, SnapshotTableDifferences tableForValues)
        {
            var items = tableForValues.RowDifferences.SelectMany(rd => rd.Differences.Differences.Where(d => d.Name == column));
            foreach (var item in items)
            {
                var columnValue = item.Before ?? item.After;

                yield return columnValue;
                if (item.Before != null && item.After != null && !item.Before.Equals(item.After))
                    yield return item.After;
            }
        }
    }
}