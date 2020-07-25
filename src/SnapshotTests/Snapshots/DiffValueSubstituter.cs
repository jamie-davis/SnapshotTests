using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class DiffValueSubstituter
    {
        public static List<SnapshotTableDifferences> Substitute(List<SnapshotTableDifferences> diffs, List<SubstituteValues> substituteValues)
        {
            return diffs.Select(d => SubstituteDiff(d, substituteValues)).ToList();
        }

        private static SnapshotTableDifferences SubstituteDiff(SnapshotTableDifferences tableDiff, List<SubstituteValues> substituteValues)
        {
            var amendedDiffs = tableDiff.RowDifferences.Select(rd => SubstituteRowDiffs(tableDiff.TableDefinition, rd, substituteValues)).ToList();
            return new SnapshotTableDifferences(amendedDiffs, tableDiff.TableDefinition);
        }

        private static RowDifference SubstituteRowDiffs(TableDefinition table, RowDifference row, List<SubstituteValues> substituteValues)
        {
            var compareResult = new RowDataCompareResult(row.Differences.Matched, SubstitutedDifferences(table, row.Differences, substituteValues), row.Before, row.After);
            
            return new RowDifference(row.Key, compareResult, row.DifferenceType);
        }

        private static List<FieldDifference> SubstitutedDifferences(TableDefinition table, RowDataCompareResult rowDifferences, List<SubstituteValues> substituteValues)
        {
            return rowDifferences.Differences.Select(fd => SubstituteField(table, fd, substituteValues)).ToList();
        }

        private static FieldDifference SubstituteField(TableDefinition table, FieldDifference fieldDifference, List<SubstituteValues> substituteValues)
        {
            var subs = GetColumnSubstitutions(table, fieldDifference, substituteValues)
                ??  GetReferencedColumnSubstitutions(table, fieldDifference, substituteValues);

            if (subs == null) return fieldDifference;

            return new FieldDifference(fieldDifference.Name, SubstituteValue(fieldDifference.Before, subs.Substitutes), SubstituteValue(fieldDifference.After, subs.Substitutes));
        }

        private static SubstituteValues GetColumnSubstitutions(TableDefinition table, FieldDifference fieldDifference, List<SubstituteValues> substituteValues)
        {
            return substituteValues.SingleOrDefault(sv => sv.ColumnValueSet.Table.TableName == table.TableName 
                                                          && sv.ColumnValueSet.Column.Name == fieldDifference.Name);
        }

        private static SubstituteValues GetReferencedColumnSubstitutions(TableDefinition table, FieldDifference fieldDifference, List<SubstituteValues> substituteValues)
        {
            var col = table.Columns.FirstOrDefault(c => c.Name == fieldDifference.Name 
                                                        && c.ReferencedTableName != null 
                                                        && c.ReferencedPropertyName != null);
            if (col == null) return null;
            return substituteValues.SingleOrDefault(sv => sv.ColumnValueSet.Table.TableName == col.ReferencedTableName
                                                          && sv.ColumnValueSet.Column.Name == col.ReferencedPropertyName);
        }

        private static object SubstituteValue(object valueInDiff, List<ValueSubstitution> substitutes)
        {
            if (valueInDiff == null)
                return null;

            return substitutes.FirstOrDefault(s => s.Value.Equals(valueInDiff))?.SubstituteValue ?? valueInDiff;
        }
    }
}