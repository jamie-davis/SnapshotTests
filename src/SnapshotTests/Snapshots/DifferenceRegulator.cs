using System.Collections.Generic;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// This class cleans up a set of differences to "normalise" unpredictable values so that results are repeatable even when the values involved are random,
    /// time based or unpredictable in some other way.
    /// <para/>
    /// The idea is to accept that unpredictable values cannot be placed directly into difference results, because the result of a comparison must be
    /// deterministic. Therefore unpredictable values must be hidden and replaced with a consistent representation. There are other pitfalls that must also
    /// be navigated - for example, sorting by random values will change the order of results, which will also result in non-deterministic results (e.g. if
    /// we sort GUIDS used as keys, the first new value will potentially belong to a different row every test run). Therefore, we must process the keys in
    /// the order they were presented to the snapshot when substituting values. This requires us to be able to see all of the snapshots in order to determine
    /// which snapshot originated the key, as well as seeing the differences themselves.
    /// as the differences. 
    /// <para/>
    /// It should also be apparent that it is not possible to use the original value to generate the substitution. Apart from truly random data, we do not
    /// want to exclude the use of the library in acceptance tests, in which the keys seen may originate in an external data store, and may be incrementing
    /// from an unknown start point. This means that keys with a simple data type, such as Integer, need the same approach as GUIDs or hashes.
    /// <para/>
    /// In order to show relationships within the difference output, we also need to be aware when a field value in one table references a key value from
    /// another. This visibility of association puts a burden on the user code table definition that needs to be resolved.
    /// </summary>
    internal static class DifferenceRegulator
    {
        public static List<SnapshotTableDifferences> CleanDifferences(SnapshotCollection collection, List<SnapshotTableDifferences> tableDiffs, Snapshot before, bool performSubstitution)
        {
            tableDiffs = ExpandDifferences(collection, tableDiffs, before);

            return UnpredictableValueRefiner.Refine(collection, tableDiffs, performSubstitution);
        }

        internal static List<SnapshotTableDifferences> ExpandDifferences(SnapshotCollection collection, List<SnapshotTableDifferences> tableDiffs, Snapshot before)
        {
            var rows = ReferencedRowLocator.GetMissingRows(collection, tableDiffs);
            tableDiffs = DifferenceEnhancer.RequireRows(collection, tableDiffs, rows, before);
            var mandatoryFields = MandatoryColumnFinder.Find(collection, tableDiffs);
            foreach (var tableColumnRequest in mandatoryFields)
            {
                tableDiffs = DifferenceColumnAdder.RequireColumns(collection, tableDiffs, tableColumnRequest.TableDefinition,
                    tableColumnRequest.Columns, before);
            }

            return tableDiffs;
        }
    }
}