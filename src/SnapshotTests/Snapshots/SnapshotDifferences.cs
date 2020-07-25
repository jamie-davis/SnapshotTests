using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal class SnapshotDifferences
    {
        public ReadOnlyCollection<SnapshotTableDifferences> TableDifferences { get; }

        public SnapshotDifferences(IList<SnapshotTableDifferences> tableDiffs)
        {
            TableDifferences = new ReadOnlyCollection<SnapshotTableDifferences>(tableDiffs);
        }
    }
}