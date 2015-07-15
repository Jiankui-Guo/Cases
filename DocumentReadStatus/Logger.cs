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

    public class LoggingService : SPDiagnosticsServiceBase
    {
        public static string MyDiagnosticAreaName = "DocumentStatus";
        private static LoggingService _Current;
        public static LoggingService Current
        {
            get
            {
                if (_Current == null)
                { _Current = new LoggingService(); }
                return _Current;
            }
        }

        private LoggingService()
            : base("DocumentStatus Logging Service", SPFarm.Local)
        { }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>  {            
                new SPDiagnosticsArea(MyDiagnosticAreaName, new List<SPDiagnosticsCategory>
                {new SPDiagnosticsCategory("DocumentStatusHandler", TraceSeverity.Unexpected, EventSeverity.Error) })};
            return areas;
        }

        public static void LogError(string categoryName, string errorMessage, params object[] data)
        {
            SPDiagnosticsCategory category = LoggingService.Current.Areas[MyDiagnosticAreaName].Categories[categoryName];
            LoggingService.Current.WriteTrace(0, category, TraceSeverity.Unexpected, errorMessage, data);
        }

        public static void LogError(string errorMessage, params object[] data)
        {
            LogError("DocumentStatusHandler", errorMessage, data);
        }

        public static void LogInfo(string message, params object[] data)
        {
            SPDiagnosticsCategory category = LoggingService.Current.Areas[MyDiagnosticAreaName].Categories["DocumentStatusHandler"];
            LoggingService.Current.WriteTrace(0, category, TraceSeverity.Medium, message, data);
        }
    }

}
