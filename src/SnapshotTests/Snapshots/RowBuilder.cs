namespace SnapshotTests.Snapshots
{
    public class RowBuilder
    {
        private readonly TableDefinition _table;
        private readonly Snapshot _snapshot;

        internal SnapshotRow Row { get; }

        internal RowBuilder(TableDefinition table, Snapshot snapshot)
        {
            _table = table;
            _snapshot = snapshot;
            Row = _snapshot.AddRow(table);
        }

        public object this[string field]
        {
            get => Row.GetField(field);
            set => Row.SetField(field, value);
        }
    }
}