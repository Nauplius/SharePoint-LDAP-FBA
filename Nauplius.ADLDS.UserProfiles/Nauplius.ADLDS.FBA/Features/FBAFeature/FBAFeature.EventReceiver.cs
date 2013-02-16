using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

using Nauplius.ADLDS.FBA

namespace Nauplius.ADLDS.FBA.Features.FBAFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("9a3ccd26-4c96-44fa-964e-3c6f9832f688")]
    public class FBAFeatureEventReceiver : SPFeatureReceiver
    {
        private const string ProviderMemberType =
            @"Microsoft.Office.Server.Security.LdapMembershipProvider, Microsoft.Office.Server, 
            Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";

        private const string ProviderRoleType =
            @"Microsoft.Office.Server.Security.LdapRoleProvider, Microsoft.Office.Server, 
            Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";
        private static readonly XmlDocument MasterXmlFragment = new XmlDocument();

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            using (SPSite siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    try
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
                                        MasterXmlFragment.LoadXml((string) item["XMLStsConfig"]);

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web") == null)
                                        {
                                            //ToDo: Finish...
                                            MasterXmlFragment.CreateElement("system.web");
                                            MasterXmlFragment.CreateNode("system.web/providers")
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        
                        throw;
                    }
                    try
                    {
                        SPList list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");

                        if (list != null)
                        {
                            SPListItemCollection items = list.Items;

                            foreach (SPListItem item in items)
                            {
                                if (item["WebApplicationUrl"].ToString() ==
                                    webApp.GetResponseUri(SPUrlZone.Default).ToString())
                                {
                                    var iisSettings = new SPIisSettings();
                                    //ToDo: get auth providers, remove 'forms' (?), add in new provider
                                   // var ap1 = new SPFormsAuthenticationProvider(item["WebApplicationMembershipProvider"].ToString(), item["WebApplicationRoleProvider"].ToString());

                                  //  iisSettings.AddClaimsAuthenticationProvider(ap1);
                                   // webApp.IisSettings.Add(SPUrlZone.Default, iisSettings);

                                    //ToDo: Build Xml Fragment from list, if <system.web> is absent in MasterXmlFragment, build that as well.


                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            try
            {
                WebModifications.CreateWildcardNode(false, properties);
                WebModifications.CreateProviderNode(false, properties);
            }
            catch (Exception)
            {
                
                throw;
            }


            //WebModifications.CreateStsProviderNode(false, properties);
            //execute STSSyncMonitor Timer Job
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
            WebModifications.CreateWildcardNode(true, properties);
            WebModifications.CreateProviderNode(true, properties);
            //execute STSSyncMonitor Timer Job, removing STS modification
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
            WebModifications.CreateWildcardNode(true, properties);
            WebModifications.CreateProviderNode(true, properties);
            //execute STSSyncMonitor Timer Job, removing all STS modifications
        }

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
