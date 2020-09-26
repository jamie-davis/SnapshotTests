using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace SnapshotTests.Tests.Snapshots.TestSupportUtils
{
    /// <summary>
    /// Temporarily override the culture in which this test runs
    /// </summary>
    public class OverrideCulture : IDisposable
    {
        private readonly CultureInfo _startCulture;

        public OverrideCulture(string culture = null, string shortDatePattern = null)
        {
            _startCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture ?? "en-GB", true)
            {
                DateTimeFormat = {ShortDatePattern = shortDatePattern ?? "dd/MM/yyyy"}
            };
        }

        #region IDisposable

        public void Dispose()
        {
            Thread.CurrentThread.CurrentCulture = _startCulture;
        }

        #endregion
    }

}
