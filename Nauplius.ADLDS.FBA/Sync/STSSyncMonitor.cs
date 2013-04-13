using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Sync.ListWR;

namespace Sync
{
    [Guid("DF4A0729-0577-4EFB-8C7E-857BBBDA5CCC")]
    public class STSSyncMonitor : SPJobDefinition
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

            try
            {
                var lists = new Lists
                                {
                                    Url = adminWebApp.Sites[0].Url + "/_vti_bin/Lists.asmx",
                                    Credentials = CredentialCache.DefaultNetworkCredentials
                                };

                var listName = "Nauplius.ADLDS.FBA - StsFarm";
                var rowLimit = "25";

                var document = new XmlDocument();
                XmlElement query = document.CreateElement("Query");
                XmlElement viewFields = document.CreateElement("ViewFields");

                query.InnerXml =
                    "<Query><Where><And><BeginsWith><FieldRef Name='Title'></FieldRef><Value Type='Text'>MasterXmlFragment</Value></BeginsWith><IsNotNull><FieldRef Name='Title'></FieldRef></IsNotNull></And></Where></Query>";
                viewFields.InnerXml = "<FieldRef Name='XMLStsConfig' />";

                var listItem = lists.GetListItems(listName, null, query, viewFields, rowLimit, null, null);

                foreach (XmlNode node in listItem)
                {
                    if (node.Name == "rs:data")
                    {
                        for (int i = 0; i < node.ChildNodes.Count; i++)
                        {
                            if (node.ChildNodes[i].Name == "z:row")
                            {
                                MasterXmlFragment.LoadXml(node.ChildNodes[i].Attributes["ows_XMLStsConfig"].Value);

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

                                    XmlNode importNode =
                                        config.ImportNode(MasterXmlFragment.SelectSingleNode("system.web"), true);
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
            catch (Exception)
            {
                Logging.LogMessage(903, Logging.LogCategories.Health,
                                   TraceSeverity.Unexpected,
                                   "Error calling Lists SOAP service {0}.",
                                   new object[] { SPServer.Local.DisplayName });
            }
            Logging.LogMessage(900, Logging.LogCategories.TimerJob, TraceSeverity.Medium, "Leaving " + tJobName,
                               new object[] { null });
            IsDisabled = true;
            Update();
        }
    }
}