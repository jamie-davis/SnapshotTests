using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as being mandatory in difference output.</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredAttribute : Attribute
    {
    }
}