using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Administration.Claims;
using System.Xml;

namespace Nauplius.ADLDS.FBA
{
    [Guid("DF4A0729-0577-4EFB-8C7E-857BBBDA5CCC")]
    internal class STSSyncMonitor : SPJobDefinition
    {
        private const string tJobName = "Nauplius ADLDS FBA STS Sync Monitor";
        private static readonly XmlDocument MasterXmlFragment = new XmlDocument();

        public STSSyncMonitor()
            : base()
        {
        }

        public STSSyncMonitor(String name, SPService service, SPServer server, SPJobLockType lockType)
            : base(name, service, server, lockType)
        {
        }

        public STSSyncMonitor(String name, SPService service)
            : base(name, service, null, SPJobLockType.None)
        {
            this.Title = tJobName;
        }

        public override void Execute(Guid targetInstanceId)
        {
            Logging.LogMessage(900, Logging.LogCategories.TimerJob, TraceSeverity.Medium, "Entering " + tJobName,
                               new object[] {null});

            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;
            using (SPSite siteCollection = new SPSite(adminWebApp.Sites[0].Url))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - StsFarm");
                    if (list != null)
                    {
                        if (list.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list.Items)
                            {
                                if (item["StsConfig"].ToString() == "MasterXmlFragment")
                                {
                                    MasterXmlFragment.LoadXml(item["XMLStsConfig"].ToString());

                                    if (MasterXmlFragment == null)
                                    {
                                        Logging.LogMessage(902, Logging.LogCategories.Health, TraceSeverity.Verbose,
                                                           "AD LDS/ADAM Forms Based Authentication not configured.",
                                                           new object[] {null});
                                    }
                                    else if (MasterXmlFragment != null)
                                    {
                                        string path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
                                        var config = new XmlDocument();
                                        config.Load(path);

                                        XmlNode systemwebChild =
                                            config.SelectSingleNode("configuration/system.web");

                                        if (systemwebChild != null)
                                        {
                                            if (systemwebChild.ParentNode != null)
                                                systemwebChild.ParentNode.RemoveChild(systemwebChild);
                                            try
                                            {
                                                config.Save(path);
                                            }
                                            catch (Exception)
                                            {
                                                Logging.LogMessage(902, Logging.LogCategories.Health,
                                                                    TraceSeverity.Verbose,
                                                                    "Failed to save removal of child node to Security Token Service web.config on {0}.",
                                                                    new object[] {SPServer.Local.DisplayName});
                                            }
                                        }

                                        XmlNode importNode = config.ImportNode(MasterXmlFragment.SelectSingleNode("system.web"), true);
                                        if (config.DocumentElement != null)
                                            config.DocumentElement.AppendChild(importNode);

                                        try
                                        {
                                            config.Save(path);
                                        }
                                        catch (Exception)
                                        {
                                            Logging.LogMessage(902, Logging.LogCategories.Health,
                                                               TraceSeverity.Verbose,
                                                               "Failed to save updates to Security Token Service web.config on {0}.",
                                                               new object[] {SPServer.Local.DisplayName});
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Logging.LogMessage(900, Logging.LogCategories.TimerJob, TraceSeverity.Medium, "Leaving " + tJobName,
                   new object[] { null });
            IsDisabled = true;
            Update();
        }
    }
}