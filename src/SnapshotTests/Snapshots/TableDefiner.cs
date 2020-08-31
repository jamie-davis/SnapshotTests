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
    }
}   