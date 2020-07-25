using System.Linq;
using SnapshotTests.Utilities;
using TestConsole.OutputFormatting.ReportDefinitions;

namespace SnapshotTests.Snapshots
{
    internal static class SnapshotColumnFormatter
    {
        public static void Format(ColumnConfig cc, TableDefinition table, SnapshotColumnInfo column)
        {
            cc.Heading(column.Name);
            if (column.ObservedTypes.All(NumericTypeHelper.IsNumeric))
                cc.RightAlign();
        }
    }
}