using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as being a sort field that sorts in ascending order. Use <see cref="DescendingSortFieldAttribute"/> to specify a sort in descending order.
    /// Marking multiple fields as sort fields will result in sorting by all of those fields. When specifying multiple sort fields, you must set <see cref="SortSequencer"/> to indicate
    /// the order in which the fields should be applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class SortFieldAttribute : Attribute
    {
        /// <summary>
        /// Indicates the order in which this field should be applied as a sort when combined with other sort fields.
        /// </summary>
        public int? SortSequencer { get; }

        public SortFieldAttribute()
        {

        }
        public SortFieldAttribute(int sortSequencer)
        {
            SortSequencer = sortSequencer;
        }
    }
}