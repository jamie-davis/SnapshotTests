using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class SortFieldOrderer
    {
        public static IEnumerable<TableDefinition.SortField> Order(IEnumerable<TableDefinition.SortField> tableDefinitionSortFields)
        {
            //This has the required behaviour with nulls, so a simple linq order by is sufficient. However, in the unlikely event of a change
            //in linq's behaviour with regard to preserving the order of equal values, this may need to be more sophisticated.
            return tableDefinitionSortFields.OrderBy(sf => sf.SortIndex);
        }
    }
}