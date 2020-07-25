using System.Collections.Generic;
using System.Linq;
using SnapshotTests.Snapshots;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsoleLib;

namespace SnapshotTests.Tests.Snapshots.TestSupportUtils
{
    internal static class DifferenceExtensions
    {
        internal static void Report(this List<SnapshotTableDifferences> result, Output output)
        {
            var first = true;
            foreach (var tableDiffs in result)
            {
                var report = tableDiffs
                    .RowDifferences
                    .AsReport(rep => AttachDiffs(rep, tableDiffs));

                if (!first)
                    output.WriteLine();
                else
                    first = false;

                output.FormatTable(report);
            }

        }

        private static void AttachDiffs(ReportParameters<RowDifference> rep, SnapshotTableDifferences tableDiffs)
        {
            var allCols = tableDiffs.RowDifferences.Where(r => r.Differences != null).SelectMany(r => r.Differences.Differences.Select(d => d.Name)).Distinct();
            var key = tableDiffs.RowDifferences.FirstOrDefault()?.Key;

            rep.RemoveBufferLimit();
            rep.Title(tableDiffs.TableDefinition.TableName);

            rep.AddColumn(rd => rd.DifferenceType.ToString(), cc => cc.Heading("Difference"));
            
            if (key != null)
            {
                var keyIndex = 0;
                foreach (var keyField in key.GetFieldNames())
                    rep.AddColumn(rd => rd.Key.AllKeys.Skip(keyIndex).First(), cc => cc.Heading(keyField));
            }

            foreach (var col in allCols)
            {
                rep.AddColumn(rep.Lambda(rd => rd.Differences?.Differences.FirstOrDefault(fd => fd.Name == col)?.Before), cc => cc.Heading($"{col} before"));
                rep.AddColumn(rep.Lambda(rd => rd.Differences?.Differences.FirstOrDefault(fd => fd.Name == col)?.After), cc => cc.Heading($"{col} after"));
            }

        }
    }
}
