using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class SnapshotKeyExtractor
    {
        public static Dictionary<SnapshotRowKey, SnapshotRow> GetKeys(Snapshot snapshot, TableDefinition tableDefinition)
        {
            return snapshot
                .Rows(tableDefinition.TableName)
                ?.ToDictionary(r => new SnapshotRowKey(r, tableDefinition), r => r, new SnapshotRowKeyComparer());
        }
    }

    internal class SnapshotRowKeyComparer : IEqualityComparer<SnapshotRowKey>
    {
        #region Implementation of IEqualityComparer<in SnapshotRowKey>

        public bool Equals(SnapshotRowKey x, SnapshotRowKey y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(SnapshotRowKey obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}