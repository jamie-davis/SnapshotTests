namespace SnapshotTests.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition("DoNotCompare")]
    [ExcludeFromComparison]
    public static class NotComparedTable
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }
    }
}