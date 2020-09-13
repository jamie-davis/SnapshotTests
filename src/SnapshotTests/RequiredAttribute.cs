using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as being mandatory in difference output.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class RequiredAttribute : Attribute
    {
    }
}