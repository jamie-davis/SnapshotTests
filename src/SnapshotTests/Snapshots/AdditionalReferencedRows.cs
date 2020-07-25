using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// Rows that are not present in a difference set but are to be added.
    /// </summary>
    internal class AdditionalReferencedRows
    {
        internal class RowRequest
        {
            public RowRequest(string columnName, object requestedValue)
            {
                ColumnName = columnName;
                RequestedValue = requestedValue;
            }

            internal string ColumnName { get; }
            internal object RequestedValue { get; }
        }

        internal class RequiredTableRows
        {
            internal RequiredTableRows(TableDefinition tableDefinition, List<RowRequest> keys)
            {
                TableDefinition = tableDefinition;
                Keys = new ReadOnlyCollection<RowRequest>(keys);
            }

            internal TableDefinition TableDefinition { get; }
            internal ReadOnlyCollection<RowRequest> Keys { get; }
        }

        internal AdditionalReferencedRows(List<RequiredTableRows> tables)
        {
            Tables = new ReadOnlyCollection<RequiredTableRows>(tables);
        }

        internal ReadOnlyCollection<RequiredTableRows> Tables { get; }
    }
}