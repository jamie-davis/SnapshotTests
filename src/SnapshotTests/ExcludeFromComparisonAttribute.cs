using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a table definition as not to be compared. This can be useful if snapshots are taken that include data
    /// that sometimes should not be tracked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExcludeFromComparisonAttribute : Attribute
    {
    }
}