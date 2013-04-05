using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml;
using FBA;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace UI.Features.FBA
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("0ef534dc-eda0-47d9-b59d-d79515c0c5ea")]
    public class FBAFeatureEventReceiver : SPFeatureReceiver
    {
        private const string ProviderMemberType =
            @"Microsoft.Office.Server.Security.LdapMembershipProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";

        private const string ProviderRoleType =
            @"Microsoft.Office.Server.Security.LdapRoleProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";
        private static readonly XmlDocument MasterXmlFragment = new XmlDocument();
        const string tJobName = "Nauplius ADLDS FBA STS Sync Monitor";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var webApp = properties.Feature.Parent as SPWebApplication;

            //Build MasterXmlFragment if SPListItem is blank or missing attributes
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
                                        try
                                        {
                                            MasterXmlFragment.LoadXml((string)item["XMLStsConfig"]);
                                        }
                                        catch (XmlException)
                                        {
                                            MasterXmlFragment.AppendChild(
                                                MasterXmlFragment.CreateNode(XmlNodeType.Element, "system.web", ""));
                                        }

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/membership") == null)
                                        {
                                            CreateStsXPath(MasterXmlFragment, item, "system.web/membership");
                                        }

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/membership/providers") ==
                                            null)
                                        {
                                            CreateStsXPath(MasterXmlFragment, item, "system.web/membership/providers");
                                        }

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/roleManager") == null)
                                        {
                                            CreateStsXPath(MasterXmlFragment, item, "system.web/roleManager");
                                        }

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/roleManager/providers") ==
                                            null)
                                        {
                                            CreateStsXPath(MasterXmlFragment, item, "system.web/roleManager/providers");
                                        }

                                        if (
                                            MasterXmlFragment.SelectSingleNode(
                                                @"system.web/roleManager[@enabled='true']") == null)
                                        {
                                            var roleManagerNode =
                                                (XmlElement)
                                                MasterXmlFragment.SelectSingleNode(@"system.web/roleManager");
                                            roleManagerNode.SetAttribute("enabled", "true");
                                            item["XMLStsConfig"] = MasterXmlFragment.OuterXml;

                                            try
                                            {
                                                item.Update();
                                            }
                                            catch (SPException ex)
                                            {
                                                Logging.LogMessage(950, Logging.LogCategories.STSXML,
                                                                   TraceSeverity.Unexpected,
                                                                   String.Format(
                                                                       "Unable to update the StsFarm List in Central Administration. {0}",
                                                                       ex.StackTrace),
                                                                   new object[] { null });
                                                throw new SPException(
                                                    @"Unable to update the StsFarm List in Central Administration.  Check to see if the item was removed.");
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SPListItem item = list.Items.Add();
                                item["StsConfig"] = "MasterXmlFragment";
                                item["XMLStsConfig"] =
                                    @"<system.web><membership><providers /></membership><roleManager enabled='true'><providers /></roleManager></system.web>";

                                try
                                {
                                    item.Update();
                                }
                                catch (SPException ex)
                                {
                                    Logging.LogMessage(950, Logging.LogCategories.STSXML, TraceSeverity.Unexpected,
                                                       String.Format(
                                                           "Unable to update the StsFarm List in Central Administration. {0}",
                                                           ex.StackTrace),
                                                       new object[] { null });
                                    throw new SPException(
                                        @"Unable to update the StsFarm List in Central Administration.  Check to see if the item was removed.");
                                }
                            }
                        }
                    }
                    catch (SPException ex)
                    {
                        Logging.LogMessage(950, Logging.LogCategories.STSXML, TraceSeverity.Unexpected,
                                            String.Format("Unable to update the StsFarm List in Central Administration. {0}",
                                            ex.StackTrace),
                                            new object[] { null });
                        throw new SPException(@"Unable to update the StsFarm List in Central Administration.  Validate the list exists.");
                    }

                    try
                    {
                        SPList list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");

                        if (list != null)
                        {
                            SPListItemCollection items = list.Items;

                            foreach (SPListItem item in items)
                            {
                                var zone = GetZone(item);
                                if (item["WebApplicationUrl"].ToString() ==
                                    webApp.GetResponseUri(zone).ToString())
                                {
                                    //Set the Membership and Role providers for the Web Application
                                    var ap = new SPFormsAuthenticationProvider(
                                        item["WebApplicationMembershipProvider"].ToString(), item["WebApplicationRoleProvider"].ToString());

                                    //Set the custom URL
                                    try
                                    {
                                        var customUrl = item["CustomUrl"].ToString();
                                        webApp.IisSettings[zone].ClaimsAuthenticationRedirectionUrl =
                                            new Uri(customUrl, UriKind.RelativeOrAbsolute);
                                    }
                                    catch (NullReferenceException)
                                    {
                                        //CustomUrl is null
                                    }

                                    try
                                    {
                                        webApp.IisSettings[zone].AddClaimsAuthenticationProvider(ap);
                                        webApp.Update();
                                        webApp.ProvisionGlobally();
                                    }
                                    catch (ArgumentException)
                                    {
                                        foreach (
                                            var provider in
                                                webApp.IisSettings[zone].ClaimsAuthenticationProviders)
                                        {
                                            if (provider.ClaimProviderName == "Forms")
                                            {
                                                webApp.IisSettings[zone].DeleteClaimsAuthenticationProvider(provider);
                                                webApp.Update();
                                                break;
                                            }
                                        }
                                        webApp.IisSettings[zone].AddClaimsAuthenticationProvider(ap);
                                        webApp.Update();
                                        webApp.ProvisionGlobally();
                                    }

                                    try
                                    {
                                        WebModifications.CreateWildcardNode(false, webApp, zone);
                                        WebModifications.CreateProviderNode(false, webApp, zone);
                                        WebModifications.CreateStsProviderNode(false, properties, zone);
                                        WebModifications.CreateAdminWildcardNode(false, webApp, zone);
                                        WebModifications.CreateAdminProviderNode(false, webApp, zone);

                                        var local = SPFarm.Local;

                                        var services = from s in local.Services
                                                       where s.Name == "SPTimerV4"
                                                       select s;

                                        var service = services.First();

                                        foreach (SPJobDefinition job in service.JobDefinitions)
                                        {
                                            if (job.Name == tJobName)
                                            {
                                                if (job.IsDisabled)
                                                    job.IsDisabled = false;
                                                job.Update();
                                                job.RunNow();
                                            }
                                        }
                                    }
                                    catch (SPException ex)
                                    {
                                        Logging.LogMessage(952, Logging.LogCategories.STSXML, TraceSeverity.Unexpected,
                                                            String.Format("An unknown error has occurred. {0}",
                                                            ex.StackTrace),
                                                            new object[] { null });
                                        throw new SPException(@"An unknown error has occurred. Please review the ULS file.");
                                    }
                                }
                            }
                        }
                    }
                    catch (SPException ex)
                    {
                        Logging.LogMessage(951, Logging.LogCategories.STSXML, TraceSeverity.Unexpected,
                                            String.Format("Unable to update the WebApplicationSettings List in Central Administration. {0}",
                                            ex.StackTrace),
                                            new object[] { null });
                        throw new SPException(@"Unable to update the WebApplicationSettings List in Central Administration.  Validate the list exists.");
                    }
                }
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var webApp = properties.Feature.Parent as SPWebApplication;
            RemoveFbaSettings(webApp, properties);
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            var webApp = properties.Feature.Parent as SPWebApplication;
            RemoveFbaSettings(webApp, properties);
        }

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}

        public static void CreateStsXPath(XmlDocument fragment, SPListItem item, string xpath)
        {
            if (fragment.SelectSingleNode(xpath) == null)
            {
                XmlNode parentNode = null;
                try
                {
                    parentNode = fragment.SelectSingleNode(xpath.Remove(xpath.LastIndexOf("/", System.StringComparison.Ordinal)));
                }
                catch (XmlException)
                {
                }

                XmlNode childNode = MasterXmlFragment.CreateNode(XmlNodeType.Element, xpath.Substring(xpath.LastIndexOf("/", System.StringComparison.Ordinal) + 1), "");
                if (parentNode != null) parentNode.AppendChild(childNode);
                try
                {
                    item["XMLStsConfig"] = fragment.OuterXml;
                    item.Update();
                }
                catch (SPException)
                {
                }
            }
        }

        protected SPUrlZone GetZone(SPListItem item)
        {
            var zone = (item["WebApplicationZone"] == null)
                           ? String.Empty
                           : item["WebApplicationZone"].ToString();

            switch (zone)
            {
                case "Default" : return SPUrlZone.Default;
                case "Intranet" : return SPUrlZone.Intranet;
                case "Internet" : return SPUrlZone.Internet;
                case "Extranet" : return SPUrlZone.Extranet;
                case "Custom" : return SPUrlZone.Custom;
                default: return SPUrlZone.Default;
            }
        }

        protected void RemoveFbaSettings(SPWebApplication webApp, SPFeatureReceiverProperties properties)
        {
            var local = SPFarm.Local;

            var services = from s in local.Services
                           where s.Name == "SPTimerV4"
                           select s;

            var service = services.First();

            foreach (var job in service.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    if (job.IsDisabled)
                        job.IsDisabled = false;
                    job.Update();
                    job.RunNow();
                }
            }

            using (SPSite siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    try
                    {
                        SPList list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");

                        if (list != null)
                        {
                            SPListItemCollection items = list.Items;

                            foreach (SPListItem item in items)
                            {
                                var zone = GetZone(item);

                                if (item["WebApplicationUrl"].ToString() == webApp.GetResponseUri(zone).AbsoluteUri)
                                {
                                    //Remove the Forms Authentication provider for the Web Application
                                    try
                                    {
                                        foreach (var provider in
                                            webApp.IisSettings[zone].ClaimsAuthenticationProviders)
                                        {
                                            if (provider.ClaimProviderName == "Forms")
                                            {
                                                webApp.IisSettings[zone].DeleteClaimsAuthenticationProvider(provider);
                                                webApp.Update();
                                                webApp.ProvisionGlobally();
                                                break;
                                            }
                                        }
                                    }
                                    catch (ArgumentNullException)
                                    {
                                        //Forms provider already removed
                                    }
                                    catch (ArgumentException)
                                    {
                                        //Claims provider is null
                                    }
                                    finally
                                    {
                                        webApp.IisSettings[zone].ClaimsAuthenticationRedirectionUrl = null;
                                        webApp.Update();
                                        webApp.ProvisionGlobally();
                                    }

                                    WebModifications.CreateWildcardNode(true, webApp, zone);
                                    WebModifications.CreateProviderNode(true, webApp, zone);
                                    WebModifications.CreateStsProviderNode(true, properties, zone);
                                    WebModifications.CreateAdminWildcardNode(true, webApp, zone);
                                    WebModifications.CreateAdminProviderNode(true, webApp, zone);
                                }
                            }
                        }
                    }
                    catch (SPException ex)
                    {
                        Logging.LogMessage(951, Logging.LogCategories.STSXML, TraceSeverity.Unexpected,
                                            String.Format("Unable to update the WebApplicationSettings List in Central Administration. {0}",
                                            ex.StackTrace),
                                            new object[] { null });
                        throw new SPException(@"Unable to update the WebApplicationSettings List in Central Administration.  Validate the list exists.");
                    }
                }
            }            
        }
    }
}
