﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnapshotTests.Exceptions;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// This class merges table definitions, so that definitions can have multiple sources. In all cases, the definition being merged in
    /// will override settings in the existing definition. The result will be a new definition containing the merged result - the original
    /// definitions will not be altered. 
    /// </summary>
    internal static class TableDefinitionMerger
    {
        /// <summary>
        /// Merge <see cref="changes"/> into <see cref="main"/>.
        /// </summary>
        /// <param name="main">The existing definition for the table</param>
        /// <param name="changes">The overrides for the table</param>
        /// <returns>A new table definition containing the merged table description.</returns>
        internal static TableDefinition Merge(TableDefinition main, TableDefinition changes)
        {
            if (main.TableName != changes.TableName)
                throw new MergeDefinitionsTargetDifferentTables(main.TableName, changes.TableName);

            var tableDefinition = new TableDefinition(main.TableName, main.DefiningTypes.Concat(changes.DefiningTypes));
            GetExclusion(main, changes, tableDefinition);

            GetPrimaryKey(main, changes, tableDefinition);
            GetCompareKey(main, changes, tableDefinition);
            MergeReferences(main, changes, tableDefinition);
            GetRequiredFields(main, changes, tableDefinition);
            GetUnpredictableFields(main, changes, tableDefinition);
            GetSortFields(main, changes, tableDefinition);
            GetExcludedFields(main, changes, tableDefinition);
            GetDateKinds(main, changes, tableDefinition);
            return tableDefinition;
        }

        private static void GetDateKinds(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            foreach (var dateField in main.LocalDates.Concat(main.UtcDates).Concat(changes.LocalDates).Concat(changes.UtcDates).Distinct())
            {
                if (changes.LocalDates.Contains(dateField))
                    tableDefinition.SetDateType(dateField, DateTimeKind.Local);
                else if (changes.UtcDates.Contains(dateField))
                    tableDefinition.SetDateType(dateField, DateTimeKind.Utc);
                else if (main.LocalDates.Contains(dateField))
                    tableDefinition.SetDateType(dateField, DateTimeKind.Local);
                else if (main.UtcDates.Contains(dateField))
                    tableDefinition.SetDateType(dateField, DateTimeKind.Utc);
            }
        }

        private static void GetExclusion(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            if (changes.IncludeInComparison)
                tableDefinition.IncludeInComparison = true;
            else if (changes.ExcludeFromComparison)
                tableDefinition.ExcludeFromComparison = true;
            else
            {
                tableDefinition.IncludeInComparison = main.IncludeInComparison;
                tableDefinition.ExcludeFromComparison = main.ExcludeFromComparison;
            }
        }

        private static void GetUnpredictableFields(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            foreach (var required in main.Unpredictable.Concat(changes.Unpredictable).Distinct().Except(changes.Predictable))
            {
                tableDefinition.SetUnpredictable(required);
            }
        }

        private static void GetRequiredFields(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            foreach (var required in main.RequiredColumns.Concat(changes.RequiredColumns))
            {
                tableDefinition.SetRequired(required);
            }
        }

        private static void MergeReferences(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            foreach (var reference in main.References.Concat(changes.References))
            {
                tableDefinition.SetReference(reference.OurField, reference.TargetTable, reference.TargetField);
            }
        }

        private static void GetPrimaryKey(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            var source = changes.PrimaryKeys.Any() ? changes : main;
            foreach (var key in source.PrimaryKeys) 
                tableDefinition.SetPrimaryKey(key);
        }

        private static void GetCompareKey(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            var source = changes.CompareKeys.Any() ? changes : main;
            foreach (var key in source.CompareKeys) 
                tableDefinition.SetCompareKey(key);
        }

        private static void GetSortFields(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            var source = changes.SortColumns.Any() ? changes : main;
            foreach (var sortField in source.SortColumns) 
                tableDefinition.SetSortColumn(sortField.Field, sortField.SortOrder, sortField.SortIndex);
        }

        private static void GetExcludedFields(TableDefinition main, TableDefinition changes, TableDefinition tableDefinition)
        {
            foreach (var fieldName in main.ExcludedColumns.Concat(changes.ExcludedColumns).Except(changes.IncludedColumns)) 
                tableDefinition.ExcludeColumnFromComparison(fieldName);
        }
    }
}
