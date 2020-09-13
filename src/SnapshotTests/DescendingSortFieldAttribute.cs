using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as being a sort field that sorts in descending order. Use <see cref="SortFieldAttribute"/> to specify a sort in ascending order.
    /// Marking multiple fields as sort fields will result in sorting by all of those fields. When specifying multiple sort fields, you must set <see cref="SortSequencer"/> to indicate
    /// the order in which the fields should be applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class DescendingSortFieldAttribute : Attribute
    {
        /// <summary>
        /// Indicates the order in which this field should be applied as a sort when combined with other sort fields.
        /// </summary>
        public int? SortSequencer { get; }

        public DescendingSortFieldAttribute()
        {

        }
        public DescendingSortFieldAttribute(int sortSequencer)
        {
            SortSequencer = sortSequencer;
        }
    }
}