using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class RowDataComparer
    {
        public static RowDataCompareResult Compare(SnapshotRowKey snapshotRowKey, SnapshotRow beforeRow, SnapshotRow afterRow)
        {
            var result = true;

            var nonKeyFieldNames = beforeRow.GetFieldNames()
                .Concat(afterRow.GetFieldNames())
                .Distinct()
                .Except(snapshotRowKey.GetFieldNames())
                .ToList();

            var diffs = new List<FieldDifference>();
            foreach (var fieldName in nonKeyFieldNames)
            {
                var before = beforeRow.GetField(fieldName);
                var after = afterRow.GetField(fieldName);
                var compareResult = ValueComparer.Compare(before, after);
                if (compareResult != 0)
                {
                    diffs.Add(new FieldDifference(fieldName, before, after));
                    result = false;
                }
            }

            return new RowDataCompareResult(result, diffs, beforeRow, afterRow);
        }
    }
}