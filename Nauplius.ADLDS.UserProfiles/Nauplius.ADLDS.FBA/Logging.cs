using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;

namespace Nauplius.ADLDS.FBA
{
    public class Logging : SPDiagnosticsServiceBase
    {
        
        public static string NaupliusDiagnosticArea = "Nauplius";
   
        public Logging() 
            : base (DefaultName, SPFarm.Local)
        { }

        public static Logging Local
        {
            get
            {
                return SPFarm.Local.Services.GetValue<Logging>(DefaultName);
            }
        }
        protected override bool HasAdditionalUpdateAccess()
        {
            return true;
        }
        

        public static class LogCategories
        {
            //public static string Profiles = "Profiles";
            //public static string LDAP = "LDAP";
            public static string TimerJob = "Timer Job";
        }

        public static string DefaultName
        {
            get { return NaupliusDiagnosticArea; }
        }

        public static string AreaName
        {
            get
            {
                return NaupliusDiagnosticArea;
            }
        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(NaupliusDiagnosticArea, 0, 0, false, new List<SPDiagnosticsCategory>
                    {
                        //new SPDiagnosticsCategory(LogCategories.Profiles, null, TraceSeverity.Unexpected, EventSeverity.Error, 0, 0, false, true),
                        //new SPDiagnosticsCategory(LogCategories.LDAP, null, TraceSeverity.Unexpected, EventSeverity.Error, 0, 0, false, true),
                        new SPDiagnosticsCategory(LogCategories.TimerJob, null, TraceSeverity.Medium, EventSeverity.Information, 0, 0, false, true),
                    })
            };
            return areas;
        }

        public static void LogMessage(ushort id, string LogCategory, TraceSeverity traceSeverity, string message)
        {
            try
            {
                Logging log = Local;

                if (log != null)
                {
                    SPDiagnosticsCategory category = log.Areas[NaupliusDiagnosticArea].Categories[LogCategory];
                    log.WriteTrace(id, category, traceSeverity, message);
                }
            }
            catch (Exception)
            { }
        }
    }
}
