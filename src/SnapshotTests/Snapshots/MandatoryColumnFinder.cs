using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class MandatoryColumnFinder
    {
        public class TableColumnRequest
        {
            private List<string> _columns;

            public TableColumnRequest(TableDefinition tableDefinition, List<string> columns)
            {
                TableDefinition = tableDefinition;
                _columns = columns;
            }

            public TableDefinition TableDefinition { get; }
            public IEnumerable<string> Columns => _columns.ToList();
        }


        public static List<TableColumnRequest> Find(SnapshotCollection collection, List<SnapshotTableDifferences> diffs)
        {
            return collection.TablesInDefinitionOrder
                .Select(t => new TableColumnRequest(t, GetColumns(t, diffs).Distinct().ToList()))
                .ToList();
        }

        private static IEnumerable<string> GetColumns(TableDefinition tableDefinition, List<SnapshotTableDifferences> diffs)
        {
            if (tableDefinition.CompareKeys?.Any() ?? false)
            {
                foreach (var compareKeyColumnName in tableDefinition.CompareKeys)
                {
                    yield return compareKeyColumnName;
                }
            }
            else if (tableDefinition.PrimaryKeys?.Any() ?? false)
            {
                foreach (var primaryKeyColumnName in tableDefinition.PrimaryKeys)
                {
                    yield return primaryKeyColumnName;
                }
            }

            if (tableDefinition.RequiredColumns?.Any() ?? false)
            {
                foreach (var requiredKeyColumnName in tableDefinition.RequiredColumns)
                {
                    yield return requiredKeyColumnName;
                }
            }

            foreach (var tableDiffs in diffs.Where(d => d.TableDefinition.TableName != tableDefinition.TableName))
            {
                foreach (var reference in tableDiffs.TableDefinition.References)
                {
                    if (reference.TargetTable == tableDefinition.TableName && tableDiffs.RowDifferences.Any(d => ContainsReference(d, reference.OurField)))
                    {
                        yield return reference.TargetField;
                    }
                }
            }
        }

        private static bool ContainsReference(RowDifference rowDifferences, string referencedField)
        {
            return rowDifferences.Differences.Differences.Any(rd => rd.Name == referencedField);
        }
    }
}