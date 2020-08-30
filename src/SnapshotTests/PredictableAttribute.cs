using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as having a predictable value so that the actual value need not be replaced in difference output.
    /// This attribute should be used in a class flagged with the <see cref="SnapshotDefinitionAttribute"/>.
    /// <remarks>The purpose of this attribute is to allow the user to override automatic configuration. For example, a utility may derive
    /// table configurations from a data source, and may automatically declare a field as unpredictable. However, in the test cases, the field's
    /// value is actually deterministic and need not be replaced, and the literal value is preferred in comparisons.</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PredictableAttribute : Attribute
    {
    }
}