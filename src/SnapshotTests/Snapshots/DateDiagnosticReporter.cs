using System;
using System.Collections.Generic;
using System.Linq;
using TestConsole.OutputFormatting;
using TestConsoleLib;

namespace SnapshotTests.Snapshots
{
    internal static class DateDiagnosticReporter
    {
        public static void Report(Output output, List<NamedTimeRange> ranges, Snapshot before, Snapshot after, SnapshotDifferences snapshotDifferences)
        {
            bool HasDate(RowDifference rowDifference, FieldDifference fieldDifference)
            {
                var isDate = fieldDifference.After?.GetType() == typeof(DateTime) ||
                             fieldDifference.Before?.GetType() == typeof(DateTime);
                if (isDate) return true;

                var beforeOriginal = rowDifference.Before?.GetField(fieldDifference.Name);
                var afterOriginal = rowDifference.After?.GetField(fieldDifference.Name);
                var isSubstitutedDate = beforeOriginal?.GetType() == typeof(DateTime) ||
                                        afterOriginal?.GetType() == typeof(DateTime);
                return isSubstitutedDate;
            }

            output.WrapLine("====> Date diagnostics begin");
            output.FormatTable(ranges.Select(r => new { Snapshot = r.Name, r.Start, r.End }), title:"Snapshot date ranges");
            var tablesThatHaveDateDifferences = snapshotDifferences
                .TableDifferences
                .Select(t => new
                {
                    t.TableDefinition, //For a table
                    Diffs = t.RowDifferences.Select(r => new
                        {
                            RowDifference = r, //Each set of differences for an individual row
                            DateDiffs = r.Differences.Differences
                                .Where(d => HasDate(r, d)).ToList() //and whichever of the differences includes a date
                        }) 
                        .Where(d => d.DateDiffs.Any()) //but only if there is at least one date difference
                        .ToList()
                })
                .Where(t => t.Diffs.Any()) //but only tables where there are dates in the row differences
                .ToList(); //so now we have a list of tables that have row differences (with dates), and their actual rows that contain the date differences

            var cols = tablesThatHaveDateDifferences.SelectMany(t => t.Diffs.SelectMany(d => d.DateDiffs.Select(dd => dd.Name))).Distinct();
            var keyCols = tablesThatHaveDateDifferences.SelectMany(t => t.Diffs.SelectMany(d => d.RowDifference.Key.GetFieldNames())).Distinct();

            var rep = tablesThatHaveDateDifferences
                .AsReport(r => r.Title("Dates From Difference Set")
                    .AddColumn(t => t.TableDefinition.TableName)
                    .AddChild(t => t.Diffs, crep =>
                    {
                        crep.RemoveBufferLimit();
                        foreach (var keyCol in keyCols)
                        {
                            crep.AddColumn(crep.Lambda(cr => GetKeyValue(keyCol, cr.RowDifference)),
                                cc => cc.Heading(keyCol));
                        }

                        foreach (var col in cols)
                        {
                            crep.AddColumn(crep.Lambda(cr => GetDateValue(col, cr.DateDiffs, cr.RowDifference, true)),
                                cc => cc.Heading($"{col} Before"));
                            crep.AddColumn(crep.Lambda(cr => GetDateValue(col, cr.DateDiffs, cr.RowDifference, false)),
                                cc => cc.Heading($"{col} After"));
                        }
                    }));

            output.WriteLine();
            output.FormatTable(rep);
            output.WrapLine("====> Date diagnostics end");
        }

        private static object GetKeyValue(string keyCol, RowDifference rowDifference)
        {
            var diff = rowDifference.Differences.Differences.FirstOrDefault(d => d.Name == keyCol);
            return diff?.After ?? diff?.Before;
        }

        private static object GetDateValue(string column, List<FieldDifference> dateDiffs, RowDifference rowDifference, bool before)
        {
            var colDiff = dateDiffs.FirstOrDefault(dd => dd.Name == column);
            var row = before ? rowDifference.Before : rowDifference.After;
            var value = before ? colDiff?.Before : colDiff?.After;

            if (row != null && !(value is DateTime))
            {
                var baseValue = row?.GetField(column);
                return baseValue is DateTime dateTime ? $"{value} ({baseValue})" : value;
            }

            return row == null ? null : value;
        }

    }
}