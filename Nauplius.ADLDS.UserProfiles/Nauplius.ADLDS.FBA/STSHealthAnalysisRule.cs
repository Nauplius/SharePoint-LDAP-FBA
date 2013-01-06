using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Health;
using Microsoft.SharePoint.Utilities;


namespace Nauplius.ADLDS.FBA
{
    internal class STSHealthAnalysisRule : SPRepairableHealthAnalysisRule
    {
        private List<SPServer> _servers = new List<SPServer>(); 
        private const string _summary = @"Security Token Service has incorrect or missing entries used to support Active Directory Lightweight Directory Services/Active Directory Application Mode.";
        private const string _explanation = @"The Security Token Service configuration file must be consistent between all SharePoint Servers in the farm.";
        private const string _remedy = "";
        private static readonly XmlDocument MasterXmlFragment = new XmlDocument();
        private static XmlNode _masterXmlNode = null;

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
                                    MasterXmlFragment.LoadXml(item["XmlStsConfig"].ToString());
                                    _masterXmlNode = MasterXmlFragment.DocumentElement;

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

            foreach (SPServer server in SPFarm.Local.Servers)
            {
                uint num = 0;
                string path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
                var config = new XmlDocument();
                config.Load(path);

                XmlNode xmlNode = config.SelectSingleNode("system.web");

                if (xmlNode == null)
                {
                    return SPHealthCheckStatus.Failed;
                }

                if (MasterXmlFragment != xmlNode)
                {
                    Logging.LogMessage(901, Logging.LogCategories.Health, TraceSeverity.Unexpected, "SharePoint Server {0} does not match master Security Token Service configuration.", new object[] {server.Name});

                    if (!_servers.Contains(server))
                    {
                        _servers.Add(server);
                    }
                }
            }

            return _servers.Count >= 1 ? SPHealthCheckStatus.Failed : SPHealthCheckStatus.Passed;
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
            get { return SPHealthCategory.System; }
        }

        public override SPHealthRepairStatus Repair()
        {
            foreach (SPServer server in SPFarm.Local.Servers)
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
                                        MasterXmlFragment.LoadXml(item["XmlStsConfig"].ToString());
                                        _masterXmlNode = MasterXmlFragment.DocumentElement;

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
                                                                       new object[] { server.Name });
                                                    return SPHealthRepairStatus.Failed;
                                                }
                                            }

                                            XmlNode importNode = config.ImportNode(MasterXmlFragment, true);
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
                                                                   new object[] { server.Name });
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
            return SPHealthRepairStatus.Succeeded;
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
