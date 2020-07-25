using SnapshotTests.Exceptions;
using TestConsoleLib;

namespace SnapshotTests.Snapshots
{
    public class SnapshotBuilder
    {
        private readonly Snapshot _snapshot;
        private readonly SnapshotCollection _collection;

        internal SnapshotCollection Collection => _collection;

        internal SnapshotBuilder(Snapshot snapshot, SnapshotCollection collection)
        {
            _snapshot = snapshot;
            _collection = collection;
        }

        public RowBuilder AddNewRow(string tableName)
        {
            var table = _collection.GetTableDefinition(tableName);
            if (table == null)
                throw new UndefinedTableInSnapshotException(tableName);

            return new RowBuilder(table, _snapshot);
        }

        public void ReportContents(Output output, params string[] tables)
        {
            _snapshot.ReportContents(output, tables);
        }
    }
}