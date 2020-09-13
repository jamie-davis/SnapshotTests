using System;
using System.Collections.Generic;
using System.Text;

namespace SnapshotTests.Tests.Snapshots.TestDefinitions
{
    [SnapshotDefinition("SortedTable")]
    public static class SortedDataTable
    {
        [Unpredictable] 
        [Key]
        public static int Id { get; set; }

        [SortField]
        public static string Name { get; set; }

        [SortField(1)]
        public static DateTime Created { get; set; }
    }
}
