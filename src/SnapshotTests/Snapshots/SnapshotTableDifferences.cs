using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal class SnapshotTableDifferences
    {
        public ReadOnlyCollection<RowDifference> RowDifferences { get; set; }
        public TableDefinition TableDefinition { get; }

        public SnapshotTableDifferences(IList<RowDifference> rowDiffs, TableDefinition tableDefinition)
        {
            TableDefinition = tableDefinition;
            RowDifferences = new ReadOnlyCollection<RowDifference>(rowDiffs);
        }
    }
}