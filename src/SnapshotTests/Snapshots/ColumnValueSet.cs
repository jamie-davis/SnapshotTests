using System.Collections.Generic;

namespace SnapshotTests.Snapshots
{
    internal class ColumnValueSet
    {
        public TableDefinition Table { get; }
        public SnapshotColumnInfo Column { get; }
        public List<object> Values { get; }
        public List<object> References { get; }

        public ColumnValueSet(TableDefinition table, SnapshotColumnInfo column, List<object> values, List<object> references)
        {
            Table = table;
            Column = column;
            Values = values;
            References = references;
        }
    }
}