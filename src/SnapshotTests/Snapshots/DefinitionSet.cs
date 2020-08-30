using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    public class DefinitionSet
    {
        public DefinitionSet(IEnumerable<TableDefinition> tables)
        {
            Tables = tables.ToList();
        }

        public IReadOnlyList<TableDefinition> Tables { get; }
    }
}