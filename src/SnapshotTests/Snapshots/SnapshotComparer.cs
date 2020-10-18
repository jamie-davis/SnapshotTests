using System.Collections.Generic;
using System.Linq;
using SnapshotTests.Snapshots.ValueTrackers;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsoleLib;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// This class coordinates the comparison of snapshots. It does not do the comparison itself, but it manages the process.
    /// </summary>
    internal static class SnapshotComparer
    {
        internal static void ReportDifferences(SnapshotCollection collection, Snapshot before, Snapshot after, Output output, ChangeReportOptions changeReportOptions = ChangeReportOptions.Default)
        {
            var differences = SnapshotDifferenceAnalyser.ExtractDifferences(collection, before, after, changeReportOptions);
            var first = true;
            foreach (var tableDifference in differences.TableDifferences)
            {
                var report = tableDifference.RowDifferences.AsReport(rep => ReportDifferences(rep, tableDifference, output, changeReportOptions));
                if (!first)
                    output.WriteLine();

                output.FormatTable(report);
                first = false;
            }

            if ((changeReportOptions & ChangeReportOptions.ReportDateDiagnostics) != 0)
            {
                output.WriteLine();
                DateDiagnosticReporter.Report(output, TimeRangeExtractor.Extract(collection), before, after, differences);
            }
        }

        private static void ReportDifferences(ReportParameters<RowDifference> rep, SnapshotTableDifferences tableDifferences, Output output, ChangeReportOptions changeReportOptions)
        {
            var differenceCols
                = tableDifferences.RowDifferences
                .Where(r => r.Differences?.Differences != null )
                .SelectMany(r => r.Differences.Differences.Select(d => d.Name)).Distinct();

            var allCols = tableDifferences.TableDefinition.Columns.Where(c => differenceCols.Contains(c.Name)).Select(c => c.Name);

            rep.Title(tableDifferences.TableDefinition.TableName);
            rep.RemoveBufferLimit();

            rep.AddColumn(rd => rd.DifferenceType.ToString(), cc => cc.Heading("Difference"));
            
            foreach (var col in allCols)
            {
                rep.AddColumn(rep.Lambda(rd => DifferenceDisplay(rd, col)), cc => cc.Heading(col));
            }
        }

        private static string DifferenceDisplay(RowDifference rd, string col)
        {
            var before = rd.Differences?.Differences.FirstOrDefault(fd => fd.Name == col)?.Before;
            var after = rd.Differences?.Differences.FirstOrDefault(fd => fd.Name == col)?.After;
            if (before == null && after == null)
                return string.Empty;

            var beforeString = before?.ToString();
            var afterString = after?.ToString();
            if (before == null || after == null || before.Equals(after))
                return beforeString ?? afterString;
            return $"{beforeString} ==> {afterString}";
        }
    }
}