using System;

namespace SnapshotTests.Snapshots.ValueTrackers
{
    /// <summary>
    /// The definition of a value tracker. Trackers should know the name of the column they are tracking in order to build a substitute value.
    /// Trackers do not need to be thread safe.
    /// </summary>
    internal interface ISubstitutableValueTracker
    {
        /// <summary>
        /// The type tracked by this tracker. One of the trackers will be a "default" tracker and will return null,
        /// but specialised trackers will return a type that corresponds to the data type they are able to track.
        /// </summary>
        Type TrackedType { get; }

        /// <summary>
        /// Get a substitute for a value. The tracker should keep a list of unique values in the order it was
        /// handed to them, in order to generate a substitute. The same substitute should be returned for a value
        /// already substituted.
        /// <remarks>Trackers that only process a specific data type should return null if they are passed the wrong data type, or if they are passed a null value.</remarks>
        /// </summary>
        /// <param name="value">The value to be substituted</param>
        /// <returns>A string value to be used as a substitute for the value.</returns>
        string GetSubstitute(object value);
    }
}