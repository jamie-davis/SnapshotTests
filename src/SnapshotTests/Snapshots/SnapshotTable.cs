using System;
using System.Collections.Generic;
using System.Linq;
using TestConsole.OutputFormatting;
using TestConsole.OutputFormatting.ReportDefinitions;
using TestConsoleLib;

namespace SnapshotTests.Snapshots
{
    internal class SnapshotTable
    {
        internal TableDefinition TableDefinition { get; }
        private readonly List<SnapshotRow> _rows = new List<SnapshotRow>();

        public SnapshotTable(TableDefinition tableDefinition)
        {
            TableDefinition = tableDefinition;
        }

        public IEnumerable<SnapshotRow> Rows => _rows.ToList();

        public SnapshotRow AddRow()
        {
            var row = new SnapshotRow(this);
            _rows.Add(row);
            return row;
        }

        public void ReportContents(Output output)
        {
            var report = _rows.AsReport(FormatRows);
            output.FormatTable(report);
        }

        private void FormatRows(ReportParameters<SnapshotRow> tableParams)
        {
            tableParams
                .RemoveBufferLimit()
                .Title(TableDefinition.TableName);

            foreach (var column in TableDefinition.Columns)
            {
                tableParams.AddColumn(r => r.GetField(column.Name), cc => SnapshotColumnFormatter.Format(cc, TableDefinition, column));
            }
        }

        internal void ColumnAdded(string field, Type observedType)
        {
            TableDefinition.ColumnAdded(field, observedType);
        }
    }
}