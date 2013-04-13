using System;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Health;
using Microsoft.SharePoint.Utilities;

namespace Sync
{
    internal class STSHealthAnalysisRule : SPRepairableHealthAnalysisRule
    {
        private const string _summary = @"Security Token Service has incorrect or missing entries used to support Active Directory Lightweight Directory Services/Active Directory Application Mode.";
        private const string _explanation = @"The Security Token Service configuration file must be consistent between all SharePoint Servers in the farm.";
        private const string _remedy = "";
        private static readonly XmlDocument MasterXmlFragment = new XmlDocument();

        public override SPHealthCheckStatus Check()
        {
            if (!SPFarm.Joined)
            {
                throw new InvalidOperationException();
            }

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
                                    MasterXmlFragment.LoadXml((string)item["XMLStsConfig"]);

                                    if (MasterXmlFragment == null)
                                    {
                                        Logging.LogMessage(902, Logging.LogCategories.Health, TraceSeverity.Verbose, "AD LDS/ADAM Forms Based Authentication not configured.", new object[] {null});
                                        return SPHealthCheckStatus.Passed;
                                    }
                                }
                            }
                        }
                    }
                    if (list == null)
                    {
                        Logging.LogMessage(902, Logging.LogCategories.Health, TraceSeverity.Verbose, "AD LDS/ADAM Forms Based Authentication not configured.", new object[] {null});
                        return SPHealthCheckStatus.Passed;
                    }
                }
            }

            uint num = 0;
            string path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
            var config = new XmlDocument();
            config.Load(path);

            XmlNode xmlNode = config.SelectSingleNode("configuration/system.web");

            if (xmlNode != null && MasterXmlFragment.OuterXml != xmlNode.OuterXml)
            {
                Logging.LogMessage(901, Logging.LogCategories.Health, TraceSeverity.Unexpected, "SharePoint Server {0} does not match master Security Token Service configuration.", new object[] {SPServer.Local.DisplayName});
                return SPHealthCheckStatus.Failed;
            }

            return SPHealthCheckStatus.Passed;
        }

        public override string Summary
        {
            get { return _summary; }
        }

        public override string Explanation
        {
            get { return _explanation; }
        }

        public override string Remedy
        {
            get { return _remedy; }
        }

        public override SPHealthCheckErrorLevel ErrorLevel
        {
            get { return SPHealthCheckErrorLevel.Error; }
        }

        public override SPHealthCategory Category
        {
            get { return SPHealthCategory.Configuration; }
        }

        public override SPHealthRepairStatus Repair()
        {
            Logging.LogMessage(903, Logging.LogCategories.Health, TraceSeverity.Verbose,
                                "Starting Security Token Service configuration repair.", new object[] {null});

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

                                        XmlNode systemwebChild = config.SelectSingleNode("configuration/system.web");
                                            
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
                                                Logging.LogMessage(902, Logging.LogCategories.Health, TraceSeverity.Verbose,
                                                                    "Failed to save removal of child node to Security Token Service web.config on {0}.",
                                                                    new object[] { SPServer.Local.DisplayName });
                                                return SPHealthRepairStatus.Failed;
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
                                            Logging.LogMessage(902, Logging.LogCategories.Health, TraceSeverity.Verbose,
                                                                "Failed to save updates to Security Token Service web.config on {0}.",
                                                                new object[] { SPServer.Local.DisplayName });
                                            return SPHealthRepairStatus.Failed;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return SPHealthRepairStatus.Succeeded;
            }
        }

        public override SPHealthAnalysisRuleAutomaticExecutionParameters AutomaticExecutionParameters
        {
            get
            {
                var retval = 
                    new SPHealthAnalysisRuleAutomaticExecutionParameters
                        {
                            Schedule = SPHealthCheckSchedule.Daily,
                            Scope = SPHealthCheckScope.All,
                            ServiceType = typeof (SPTimerService)
                        };
                return retval;
            }
        }
    }
}
