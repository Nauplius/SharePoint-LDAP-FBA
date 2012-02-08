using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Nauplius.SharePoint.ADLDS.UserProfiles
{
    class Logging
    {
        private static string sourceName = "Nauplius.SharePoint.ADLDS.UserProfiles";

        public enum LogLevel
        {
            Error = 0,
            Informational = 1
        }

        public static void CreateSource()
        {
            if (!EventLog.SourceExists(sourceName))
            {
                try
                {
                    EventLog.CreateEventSource(sourceName, "Application");
                }
                catch { }
            }
        }

        public static void WriteEventLog(int eventId, string message, EventLogEntryType type, LogLevel eventLevel)
        {
            if (EventLog.SourceExists(sourceName))
            {
                LogLevel configLevel = (LogLevel)Enum.Parse(typeof(LogLevel), ConfigurationManager.AppSettings["Logging"]);

                if (configLevel == LogLevel.Informational)
                {
                    EventLog.WriteEntry(sourceName, message, type, eventId);
                }
                else if (configLevel == LogLevel.Error)
                {
                    if (eventLevel == LogLevel.Error)
                    {
                        EventLog.WriteEntry(sourceName, message, type, eventId);
                    }
                }
            }
        }
    }
}
