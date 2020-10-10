namespace SnapshotTests.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition("TableWithExcludedProperty")]
    public static class TableWithExcludedPropDef
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }

        [ExcludeFromComparison]
        public static byte[] BigValue { get; set; }
    }
}