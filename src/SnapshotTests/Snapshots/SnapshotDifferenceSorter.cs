using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;

namespace SnapshotTests.Snapshots
{
    internal static class SnapshotDifferenceSorter
    {
        public static List<SnapshotTableDifferences> SortDifferences(SnapshotCollection collection, List<SnapshotTableDifferences> tableDiffs)
        {
            return tableDiffs.Select(t => t.TableDefinition.SortColumns.Any() 
                ? SortTable(t) 
                : t)
                .ToList();
        }

        private static SnapshotTableDifferences SortTable(SnapshotTableDifferences tableDifferences)
        {
            Func<IEnumerable<RowDifference>, IEnumerable<RowDifference>> sorter = null;
            foreach (var fieldSortSpec in SortFieldOrderer.Order(tableDifferences.TableDefinition.SortColumns))
            {
                var fieldName = fieldSortSpec.Field;
                Func<RowDifference, object> fetch = (r) => r.After.GetField(fieldName) ?? r.Before.GetField(fieldName);

                Func<IEnumerable<RowDifference>, IEnumerable<RowDifference>> nextSorter;
                if (fieldSortSpec.SortOrder == SortOrder.Ascending)
                    nextSorter = r => r.OrderBy(fetch, ValueComparer.Comparer);
                else
                    nextSorter = r => r.OrderByDescending(fetch, ValueComparer.Comparer);

                if (sorter == null)
                    sorter = nextSorter;
                else
                {
                    var currentSorter = sorter;
                    sorter = r => currentSorter(nextSorter(r));
                }
            }

            Debug.Assert(sorter != null);
            var rowDifferences = sorter(tableDifferences.RowDifferences).ToList();
            return new SnapshotTableDifferences(rowDifferences, tableDifferences.TableDefinition);
        }
    }
}