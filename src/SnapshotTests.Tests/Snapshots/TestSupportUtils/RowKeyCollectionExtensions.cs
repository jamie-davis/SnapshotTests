using System.Collections.Generic;
using System.Linq;
using SnapshotTests.Snapshots;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.ReportDefinitions;

namespace SnapshotTests.Tests.Snapshots.TestSupportUtils
{
    internal static class RowKeyCollectionExtensions
    {
        public static Report<SnapshotRowKey> MakeKeyReport(this Dictionary<SnapshotRowKey, SnapshotRow> rowIndex)
        {
            var keys = rowIndex.Keys.ToList();
            return keys.MakeKeyReport();
        }

        public static Report<SnapshotRowKey> MakeKeyReport(this List<SnapshotRowKey> keys)
        {
            var cols = keys.Max(k => k.AllKeys.Count());

            object GetColValue(SnapshotRowKey key, int i)
            {
                return key.AllKeys.Skip(i).FirstOrDefault();
            }

            void MakeReport(ReportParameters<SnapshotRowKey> rep)
            {
                for (var i = 0; i < cols; i++)
                {
                    var colIndex = i;
                    rep.AddColumn(rep.Lambda((r) => GetColValue(r, colIndex)), cc => cc.Heading($"Key {colIndex + 1}"));
                }
            }

            return keys.AsReport(MakeReport);
        }
    }
}