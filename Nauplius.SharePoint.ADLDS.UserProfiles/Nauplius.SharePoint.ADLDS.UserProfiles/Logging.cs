using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Nauplius.SharePoint.ADLDS.UserProfiles
{
    class Logging
    {
        private static string sourceName = "Nauplius.SharePoint.ADLDS.UserProfiles";

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

        public static void WriteEventLog(int eventId, string message, EventLogEntryType type)
        {
            if (EventLog.SourceExists(sourceName))
            {
                EventLog.WriteEntry(sourceName, message, type, eventId);
            }
        }
    }
}
