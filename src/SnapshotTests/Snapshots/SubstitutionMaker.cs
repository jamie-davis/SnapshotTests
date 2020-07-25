using System.Collections.Generic;
using System.Linq;
using SnapshotTests.Snapshots.ValueTrackers;

namespace SnapshotTests.Snapshots
{
    internal static class SubstitutionMaker
    {
        public static List<ValueSubstitution> Make(List<object> unpredictables, List<ISubstitutableValueTracker> trackers)
        {
            return unpredictables.Select(v => Make((object) v, trackers)).ToList();
        }

        private static ValueSubstitution Make(object value, List<ISubstitutableValueTracker> trackers)
        {
            if (value == null) 
                return new ValueSubstitution(null, null);

            var tracker = trackers.FirstOrDefault(t => t.TrackedType == value.GetType())
                ?? trackers.FirstOrDefault(t => t.TrackedType == null);

            return tracker == null 
                ? new ValueSubstitution(value, null) 
                : new ValueSubstitution(value, tracker.GetSubstitute(value));
        }
    }
}