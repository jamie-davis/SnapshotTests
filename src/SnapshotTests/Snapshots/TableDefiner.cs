using System;

namespace SnapshotTests.Snapshots
{
    public sealed class TableDefiner
    {
        private readonly TableDefinition _tableDefinition;

        internal TableDefiner(TableDefinition tableDefinition)
        {
            _tableDefinition = tableDefinition;
        }

        public TableDefiner PrimaryKey(string fieldName)
        {
            _tableDefinition.SetPrimaryKey(fieldName);
            return this;
        }

        public TableDefiner CompareKey(string fieldName)
        {
            _tableDefinition.SetCompareKey(fieldName);
            return this;
        }

        public TableDefiner IsUnpredictable(string fieldName)
        {
            _tableDefinition.SetUnpredictable(fieldName);
            return this;
        }

        public TableDefiner IsPredictable(string fieldName)
        {
            _tableDefinition.SetPredictable(fieldName);
            return this;
        }

        public TableDefiner IncludeInComparison()
        {
            _tableDefinition.IncludeInComparison = true;
            _tableDefinition.ExcludeFromComparison = false;
            return this;
        }

        public TableDefiner ExcludeFromComparison()
        {
            _tableDefinition.ExcludeFromComparison = true;
            _tableDefinition.IncludeInComparison = false;
            return this;
        }

        /// <summary>
        /// Indicate that a field is mandatory in difference reports when a row from the table is present, even if it is not a difference. This can be
        /// a useful way to enhance readability in difference output.
        /// </summary>
        /// <param name="fieldName">The name of the field that should be present in difference output.</param>
        public TableDefiner IsRequired(string fieldName)
        {
            _tableDefinition.SetRequired(fieldName);
            return this;
        }

        /// <summary>
        /// Define that a field on this table contains a value that identifies a row on another table.
        /// </summary>
        /// <param name="fieldName">The field on this table doing the referencing</param>
        /// <param name="tableReferenced">The table being referenced</param>
        /// <param name="fieldReferenced">The field on the referenced table that contains the value that matches the reference</param>
        public TableDefiner IsReference(string fieldName, string tableReferenced, string fieldReferenced)
        {
            _tableDefinition.SetReference(fieldName, tableReferenced, fieldReferenced);
            return this;
        }

        /// <summary>
        /// Define that the table should be sorted by a field. Calling this method will result in differences for the table being sorted by this field in ascending order.
        /// <remarks>To sort by multiple fields, call this method multiple times.</remarks>
        /// </summary>
        /// <param name="fieldName">The sort field</param>
        public TableDefiner Sort(string fieldName)
        {
            _tableDefinition.SetSortColumn(fieldName, SortOrder.Ascending);
            return this;
        }

        /// <summary>
        /// Define that the table should be sorted by a field. Calling this method will result in differences for the table being sorted by this field in descending order.
        /// <remarks>To sort by multiple fields, call this method multiple times.</remarks>
        /// </summary>
        /// <param name="fieldName">The sort field</param>
        public TableDefiner SortDescending(string fieldName)
        {
            _tableDefinition.SetSortColumn(fieldName, SortOrder.Descending);
            return this;
        }

        /// <summary>
        /// Define that a field contains a UTC date. This is only useful on unpredictable date fields, but it allows the field to be tagged as a "mid-test" timestamp. The collection
        /// needs to know how to handle the date values for this feature to work. You can apply this setting to any field, but it will have no effect unless the field is an
        /// unpredictable date that has its value set to the current time.
        /// </summary>
        /// <param name="fieldName">The field name</param>
        public TableDefiner Utc(string fieldName)
        {
            _tableDefinition.SetDateType(fieldName, DateTimeKind.Utc);
            return this;
        }

        /// <summary>
        /// Define that a field contains a local date. This is only useful on unpredictable date fields, but it allows the field to be tagged as a "mid-test" timestamp. The collection
        /// needs to know how to handle the date values for this feature to work. You can apply this setting to any field, but it will have no effect unless the field is an
        /// unpredictable date that has its value set to the current time.
        /// <remarks>The assumption is made that "local" means the current machine's local date time. You cannot specify a particular time zone.</remarks>
        /// </summary>
        /// <param name="fieldName">The field name</param>
        public TableDefiner Local(string fieldName)
        {
            _tableDefinition.SetDateType(fieldName, DateTimeKind.Local);
            return this;
        }

        /// <summary>
        /// Indicate that a field should be excluded from comparisons. By default all fields are included. If an excluded field must be included, <see cref="Include"/> it
        /// to override this indicator.
        /// <remarks>If <see cref="Exclude"/> and <see cref="Include"/> are both specified, the field will be included.</remarks>
        /// </summary>
        /// <param name="fieldName">The field name</param>
        public TableDefiner Exclude(string fieldName)
        {
            _tableDefinition.ExcludeColumnFromComparison(fieldName);
            return this;
        }

        /// <summary>
        /// Indicate that a field should be included in comparisons. By default all fields are included, so this will have no effect unless <see cref="Exclude"/> is also used.
        /// <remarks>If <see cref="Exclude"/> and <see cref="Include"/> are both specified, the field will be included.</remarks>
        /// </summary>
        /// <param name="fieldName">The field name</param>
        public TableDefiner Include(string fieldName)
        {
            _tableDefinition.IncludeColumnInComparison(fieldName);
            return this;
        }
    }
}   