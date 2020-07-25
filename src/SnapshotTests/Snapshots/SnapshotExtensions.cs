using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SnapshotTests.Snapshots
{
    /// <summary>
    /// Extensions to assist in constructing snapshots. 
    /// </summary>
    public static class SnapshotExtensions
    {
        /// <summary>
        /// Add a collection as a table in a snapshot. If the table with the given table name does not exist, it will be added as a table to the snapshot collection.
        /// <para/>
        /// Snapshot tables must can be defined before they can be populated, and the extension method shortcuts this process by automatically creating the table definition.
        /// <para/>
        /// You may also specify key fields for the table, but these are optional. If key fields are never defined for a table then comparison will not be able to detect updates.
        /// </summary>
        /// <typeparam name="T">The type contained in the collection</typeparam>
        /// <param name="sourceData">The collection that is to be placed in a snapshot table</param>
        /// <param name="builder">The snapshot construction instance.</param>
        /// <param name="tableName">The name to give the table in the snapshot. If this is not specified then the item type name will be used.</param>
        /// <param name="keyFields">The names of the properties that provide the key for the collection.</param>
        public static SnapshotBuilder ToSnapshotTable<T>(this IEnumerable<T> sourceData, SnapshotBuilder builder, string tableName = null, params string[] keyFields)  where T : class
        {
            if (tableName == null) tableName = typeof(T).Name;

            var collection = builder.Collection;
            if (collection.GetTableDefinition(tableName) == null)
            {
                var definer = collection.DefineTable(tableName);
                if (keyFields != null)
                {
                    foreach (var keyField in keyFields) 
                        definer.CompareKey(keyField);
                }
            }

            var properties = typeof(T)
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                .ToList();

            foreach (var row in sourceData)
            {
                var rowBuilder = builder.AddNewRow(tableName);
                foreach (var property in properties) 
                    rowBuilder[property.Name] = property.GetValue(row);
            }

            return builder;
        }
    }
}
