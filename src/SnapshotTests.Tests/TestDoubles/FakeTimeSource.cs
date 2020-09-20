using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SnapshotTests.Tests.TestDoubles
{
    internal class FakeTimeSource : ITimeSource
    {
        public DateTime[] Times { get; set; }
        public int Index { get; set; }
        public DateTime? Fallback { get; set; }

        public FakeTimeSource(params string[] times) : this(times.Select(t => DateTime.Parse(t)).ToArray())
        {
        }

        public FakeTimeSource(params DateTime[] times)
        {
            Times = times;
            Index = 0;
        }

        #region Implementation of ITimeSource

        public DateTime GetUtcTime()
        {
            if (Index >= Times.Length)
                return Fallback ?? (Times[Times.Length - 1] + TimeSpan.FromSeconds(Index++ - Times.Length));
            return Times[Index++];
        }

        #endregion
    }

}
