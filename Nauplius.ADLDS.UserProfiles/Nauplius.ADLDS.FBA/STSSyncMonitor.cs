using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.Web.Administration;
using Microsoft.SharePoint.Administration.Claims;
using System.Xml;

namespace Nauplius.ADLDS.FBA
{
    [Guid("DF4A0729-0577-4EFB-8C7E-857BBBDA5CCC")]
    internal class STSSyncMonitor : SPJobDefinition
    {
        private const string tJobName = "Nauplius ADLDS FBA STS Sync Monitor";

        public STSSyncMonitor() : base()
        {
        }

        public STSSyncMonitor(String name, SPWebApplication adminWebApplication, SPServer server, SPJobLockType lockType)
            : base(name, adminWebApplication, server, lockType)
        {
        }

        public STSSyncMonitor(String name, SPWebApplication adminWebApplication)
            : base(name, adminWebApplication, null, SPJobLockType.Job)
        {
            this.Title = tJobName;
        }

        public override void Execute(Guid targetInstanceId)
        {
            Logging.LogMessage(900, Logging.LogCategories.TimerJob, TraceSeverity.Medium, "Entering " + tJobName);
            List<XmlDocument> stsConfigurations = new List<XmlDocument>();

            foreach (SPServer spServer in SPFarm.Local.Servers)
            {
                string path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
                XmlDocument config = new XmlDocument();
                config.Load(path);
                stsConfigurations.Add(config);
            }
        }
    }
}
