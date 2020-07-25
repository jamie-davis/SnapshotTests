namespace SnapshotTests.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition(null)]
    public static class UnnamedTable
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }
    }
}