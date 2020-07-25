using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SnapshotTests.Snapshots;
using SnapshotTests.Snapshots.ValueTrackers;
using TestConsoleLib;
using TestConsoleLib.Testing;

namespace SnapshotTests.Tests.Snapshots
{
    [TestFixture]
    public class TestSubstitutionMaker
    {

        #region Types for test

        public class TestTracker<T> : ISubstitutableValueTracker
        {
            private readonly string _col;
            private List<T> _values = new List<T>();

            public TestTracker(string col)
            {
                _col = col;
            }

            #region Implementation of ISubstitutableValueTracker

            public Type TrackedType => typeof(T);

            public string GetSubstitute(object value)
            {
                if (value is T typedValue)
                {
                    var index = _values.IndexOf(typedValue);
                    if (index < 0)
                    {
                        _values.Add(typedValue);
                        index = _values.Count - 1;
                    }

                    return $"{typeof(T).Name}-{_col}-{index + 1}";
                }

                return null;
            }

            #endregion
        }

        #endregion

        [Test]
        public void SubstitutionsAreAllocatedByAppropriateTracker()
        {
            //Arrange
            var trackers = new List<ISubstitutableValueTracker>
            {
                new DefaultValueTracker("TestCol(DEFAULT TRACKER)"), 
                new TestTracker<double>("TestCol"),
                new TestTracker<int>("TestCol")
            };

            var values = new List<object>
            {
                1.0,
                2.0,
                1,
                2,
                "usedefaultforthis",
                null
            };

            //Act
            var result = SubstitutionMaker.Make(values, trackers);

            //Assert
            var output = new Output();
            output.FormatTable(result.Select(v => new { Value = v.Value ?? "<null>", SubstituteValue = v.SubstituteValue ?? "<null>"}));
            output.Report.Verify();
        }
       
    }
}
