using System.Globalization;
using SnapshotTests.Snapshots;

namespace SnapshotTests.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition("RelatedTable")]
    public static class RelatedTableDef
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }
            
        [Required]
        public static string Required { get; set; }

        [References("ParentTable", nameof(TestSnapshotDefinitionLoader.ParentTableDef.Id))] //implied unpredictable because the referenced property is unpredictable
        public static int ParentId1 { get; set; }
    }
}
