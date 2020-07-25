using System.Collections.Generic;
using System.Linq;
using TestConsoleLib;

namespace SnapshotTests.Snapshots
{
    internal class Snapshot
    {
        private Dictionary<string, SnapshotTable> _tables = new Dictionary<string, SnapshotTable>();

        internal string Name { get; }

        internal Snapshot(string name)
        {
            Name = name;
        }

        internal SnapshotRow AddRow(TableDefinition table)
        {
            if (!_tables.TryGetValue(table.TableName, out var snapshotTable))
            {
                snapshotTable = new SnapshotTable(table);
                _tables[table.TableName] = snapshotTable;
            }

            return snapshotTable.AddRow();
        }

        internal void ReportContents(Output output, params string[] tables)
        {
            var tableList = tables.Any() 
                ? tables.ToList() 
                : _tables.Keys.OrderBy(k => k).ToList();
            var snapshotTables = tableList
                .Where(t => _tables.ContainsKey(t))
                .Select(t => _tables[t])
                .ToList();
            foreach (var table in snapshotTables)
            {
                table.ReportContents(output);
            }
        }

        public IEnumerable<SnapshotRow> Rows(string tableName)
        {
            if (_tables.TryGetValue(tableName, out var snapshotTable))
            {
                return snapshotTable.Rows;
            }

            return null;
        }


        public SnapshotRow GetRow(SnapshotRowKey key, string tableName)
        {
            if (!_tables.TryGetValue(tableName, out var snapshotTable))
                return null;

            return snapshotTable.Rows.SingleOrDefault(r => Equals(new SnapshotRowKey(r, snapshotTable.TableDefinition), key));
        }
    }
}