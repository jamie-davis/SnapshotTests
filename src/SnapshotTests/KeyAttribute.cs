using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as being a primary key field in a snapshot table definition.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
    }
}