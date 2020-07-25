using System.Collections.Generic;

namespace SnapshotTests.Snapshots
{
    internal class SnapshotRow
    {
        private readonly SnapshotTable _snapshotTable;
        private Dictionary<string, object> _fields = new Dictionary<string, object>();

        public SnapshotRow(SnapshotTable snapshotTable)
        {
            _snapshotTable = snapshotTable;
        }

        public object GetField(string field)
        {
            _fields.TryGetValue(field, out var value);
            return value;
        }

        public void SetField(string field, object value)
        {
            _snapshotTable.ColumnAdded(field, value?.GetType());
            _fields[field] = value;
        }

        public IEnumerable<string> GetFieldNames() => _fields.Keys;
    }
}