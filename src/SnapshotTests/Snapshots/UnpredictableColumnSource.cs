using System.Collections.Generic;

namespace SnapshotTests.Snapshots
{
    internal class UnpredictableColumnSource
    {
        internal class UnpredictableColumn
        {
            public SnapshotColumnInfo Column { get; }
            public List<ReferencingColumns> ReferencingColumns { get; }

            public UnpredictableColumn(SnapshotColumnInfo column, List<ReferencingColumns> referencingColumns)
            {
                Column = column;
                ReferencingColumns = referencingColumns;
            }
        }

        internal class ReferencingColumns
        {
            public TableDefinition SourceTable { get; }
            public List<SnapshotColumnInfo> References { get; }

            public ReferencingColumns(TableDefinition sourceTable, List<SnapshotColumnInfo> references)
            {
                SourceTable = sourceTable;
                References = references;
            }
        }

        public TableDefinition Table { get; }
        public List<UnpredictableColumn> Columns { get; }

        public UnpredictableColumnSource(TableDefinition table, List<UnpredictableColumn> columns)
        {
            Table = table;
            Columns = columns;
        }
    }
}