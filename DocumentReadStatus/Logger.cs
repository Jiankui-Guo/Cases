using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Administration;

namespace DocumentReadStatus
{
    public static class Logger
    {
        public static void WriteLog(TraceSeverity ts, string message, params object[] data)
        {
            SPDiagnosticsService.Local.WriteTrace(
                   0,
                   new SPDiagnosticsCategory("Document Read Status",
                       TraceSeverity.High,
                       EventSeverity.Information),
                       ts,
                       message,
                       data);
        }

        public static void WriteVerboseLog(string message, params object[] data)
        {
            WriteLog(TraceSeverity.High, message, data);
        }
    }
}
