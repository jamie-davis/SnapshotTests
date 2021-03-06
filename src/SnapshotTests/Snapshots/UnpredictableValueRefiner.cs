﻿using System.Collections.Generic;
using System.Linq;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// This class orchestrates the process of extracting unpredictable values 
    /// </summary>
    internal static class UnpredictableValueRefiner
    {
        public static List<SnapshotTableDifferences> Refine(SnapshotCollection collection, List<SnapshotTableDifferences> tableDiffs, bool performSubstitution)
        {
            var unpredictableValues = UnpredictableColumnLocator.Locate(tableDiffs.Select(td => td.TableDefinition).ToList())
                    .SelectMany(u => UnpredictableValueScanner.Scan(u, tableDiffs))
                    .ToList();

            if (performSubstitution)
                return ValueSubstituter.Substitute(unpredictableValues, tableDiffs, collection);

            return tableDiffs;
        }
    }
}