using System;

namespace SnapshotTests.Exceptions
{
    public class SnapshotNotFoundException : Exception
    {
        public string SnapshotName { get; }

        internal SnapshotNotFoundException(string snapshotName) : base("Requested snapshot is not in the collection.")
        {
            SnapshotName = snapshotName;
        }
    }
}