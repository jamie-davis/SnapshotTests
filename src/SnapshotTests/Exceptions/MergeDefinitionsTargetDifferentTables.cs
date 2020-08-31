using System;
using System.Collections.Generic;
using System.Text;

namespace SnapshotTests.Exceptions
{
    public class MergeDefinitionsTargetDifferentTables : Exception
    {
        public string Main { get; }
        public string Merge { get; }

        public MergeDefinitionsTargetDifferentTables(string main, string merge) : base("Unable to merge table definitions because they target different tables")
        {
            Main = main;
            Merge = merge;
        }
    }
}
