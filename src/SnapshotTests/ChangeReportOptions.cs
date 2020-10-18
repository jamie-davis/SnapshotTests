using System;

namespace SnapshotTests
{
    /// <summary>
    /// Flags that control the formatting of a change report.
    /// </summary>
    [Flags]
    public enum ChangeReportOptions
    {
        Default = 0,
        NoSubs = 1,
        ReportDateDiagnostics = 2
    }
}