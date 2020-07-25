using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a field as having an unpredictable value so that the actual value can be replaced in difference output.
    /// This attribute should be used in a class flagged with the <see cref="SnapshotDefinitionAttribute"/>.
    /// <remarks>Examples of unpredictable values are GUIDs, timestamps, hashes or mechanically generated key values (e.g. "identity"
    /// key values outside the control of the test).</remarks>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UnpredictableAttribute : Attribute
    {
    }
}