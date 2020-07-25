using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// From a provided set of tables, this class finds all unpredictable columns and columns that reference them.
    /// </summary>
    internal static class UnpredictableColumnLocator
    {
        public static List<UnpredictableColumnSource> Locate(List<TableDefinition> tables)
        {
            var unpredictableColumnsThatAreNotReferences = tables.SelectMany(t => t.Columns.Where(c => c.IsUnpredictable && c.ReferencedTableName == null)
                .Select(c => new { Table = t, Column = c}))
                .GroupBy(t => t.Table)
                .Select(g => new { Table = g.Key, Columns = g.Select(i => i.Column).ToList()})
                .ToList();

            var referencesToUnpredictableColumns = tables
                .SelectMany(t => t.Columns.Where(c => unpredictableColumnsThatAreNotReferences.Any(u =>
                    u.Table.TableName == c.ReferencedTableName &&
                    u.Columns.Any(uc => uc.Name == c.ReferencedPropertyName)))
                .Select(c => new { Table = unpredictableColumnsThatAreNotReferences.Single(u => u.Table.TableName == c.ReferencedTableName).Table, Column = c, ColumnSource = t }))
                .ToList();

            var matched = unpredictableColumnsThatAreNotReferences
                .Select(t =>
                {
                    var columns = t.Columns.Select(c =>
                    {
                        var referencingColumns = referencesToUnpredictableColumns
                            .Where(r => ReferenceEquals(r.Table, t.Table) && r.Column.ReferencedPropertyName == c.Name)
                            .GroupBy(r => r.ColumnSource)
                            .Select(g => new UnpredictableColumnSource.ReferencingColumns(g.Key, g.Select(rc => rc.Column).ToList()))
                            .ToList();
                        return new UnpredictableColumnSource.UnpredictableColumn(c, referencingColumns);
                    }).ToList();
                    return new UnpredictableColumnSource(t.Table, columns);
                })
                .ToList();
            return matched;
        }
    }
}