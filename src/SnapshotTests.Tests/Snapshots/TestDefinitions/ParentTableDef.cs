using SnapshotTests.Snapshots;

namespace SnapshotTests.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition("ParentTable")]
    public static class ParentTableDef
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }
    }
}