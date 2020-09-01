using System.Collections.Generic;
using System.Linq;
using TestConsole.OutputFormatting;
using TestConsoleLib;

namespace SnapshotTests.Snapshots
{
    internal static class TableDefinitionReporter
    {
        internal static void Report(TableDefinition tableDefinition, Output output)
        {
            Report(new []{ tableDefinition }, output);
        }

        internal static void Report(IEnumerable<TableDefinition> tableDefinitions, Output output)
        {
            var report = MakeReport(tableDefinitions, true);
            output.FormatTable(report);
        }

        private static Report<TableDefinition> MakeReport(IEnumerable<TableDefinition> tableDefinitions, bool addTitle)
        {
            var report = tableDefinitions.AsReport(rep =>
            {
                rep.RemoveBufferLimit()
                    .AddColumn(d => d.TableName)
                    .AddColumn(d => d.ExcludeFromComparison)
                    .AddColumn(d => d.IncludeInComparison)
                    .AddColumn(d => string.Join(", ", d.DefiningTypes.Select(t => t.Name)), cc => cc.Heading("Defined By Types"))
                    .AddChild(d => d.Columns, colRep => colRep.RemoveBufferLimit()
                        .AddColumn(c => c.Name)
                        .AddColumn(c => c.IsPrimaryKey)
                        .AddColumn(c => c.IsCompareKey)
                        .AddColumn(c => c.IsUnpredictable)
                        .AddColumn(c => c.IsPredictable)
                        .AddColumn(c => c.IsRequired)
                        .AddColumn(c => c.ReferencedTableName)
                        .AddColumn(c => c.ReferencedPropertyName));

                if (addTitle)
                    rep.Title("Table Definitions");
            });

            return report;
        }
    }
}