using System;

namespace SnapshotTests
{
    /// <summary>
    /// Use this attribute to flag a date/time field as containing local data.
    /// DateTime can specify whether it contains a UTC or local date/time, and this attribute will only make a difference when neither is specified.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class LocalAttribute : Attribute
    {
    }
}