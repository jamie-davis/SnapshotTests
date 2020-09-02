using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    internal static class DefinitionSetMerger
    {
        public static IEnumerable<TableDefinition> Merge(DefinitionSet loaded, List<TableDefinition> tablesInDefinitionOrder)
        {
            var loadedTables = loaded.Tables.ToList();
            if (tablesInDefinitionOrder != null)
            {
                foreach (var tableDefinition in tablesInDefinitionOrder)
                {
                    var current = loadedTables.SingleOrDefault(t => t.TableName == tableDefinition.TableName);
                    if (current == null)
                        yield return tableDefinition;
                    else
                    {
                        loadedTables.Remove(current);
                        yield return TableDefinitionMerger.Merge(tableDefinition, current);
                    }
                }
            }

            foreach (var tableDefinition in loadedTables)
            {
                yield return tableDefinition;
            }
        }
    }
}