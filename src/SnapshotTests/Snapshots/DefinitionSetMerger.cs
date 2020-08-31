using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class DefinitionSetMerger
    {
        public static IEnumerable<TableDefinition> Merge(DefinitionSet loaded, List<TableDefinition> tablesInDefinitionOrder)
        {
            foreach (var tableDefinition in loaded.Tables)
            {
                var current = tablesInDefinitionOrder?
                    .SingleOrDefault(t => t.TableName == tableDefinition.TableName);
                if (current == null)
                    yield return tableDefinition;
                else
                    yield return TableDefinitionMerger.Merge(current, tableDefinition);
            }
        }
    }
}