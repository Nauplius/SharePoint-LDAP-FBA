using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

using Nauplius.ADLDS.FBA;

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
            @"Microsoft.Office.Server.Security.LdapMembershipProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";

        private const string ProviderRoleType =
            @"Microsoft.Office.Server.Security.LdapRoleProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";
        private static readonly XmlDocument MasterXmlFragment = new XmlDocument();

        const string tJobName = "Nauplius ADLDS FBA STS Sync Monitor";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var webApp = properties.Feature.Parent as SPWebApplication;
            var adminWebApp = SPAdministrationWebApplication.Local;

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
                                            MasterXmlFragment.LoadXml((string) item["XMLStsConfig"]);
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

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/membership/providers") == null)
                                        {
                                            CreateStsXPath(MasterXmlFragment, item, "system.web/membership/providers");
                                        }

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/roleManager") == null)
                                        {
                                            CreateStsXPath(MasterXmlFragment, item, "system.web/roleManager");
                                        }

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/roleManager/providers") == null)
                                        {
                                            CreateStsXPath(MasterXmlFragment, item, "system.web/roleManager/providers");
                                        }

                                        if (MasterXmlFragment.SelectSingleNode(@"system.web/roleManager[@enabled='true']") == null)
                                        {
                                            var roleManagerNode = (XmlElement)MasterXmlFragment.SelectSingleNode(@"system.web/roleManager");
                                            roleManagerNode.SetAttribute("enabled", "true");

                                            try
                                            {
                                                item["XMLStsConfig"] = MasterXmlFragment.OuterXml;
                                                item.Update();
                                            }
                                            catch (Exception)
                                            {}
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
                                item.Update();
                            }
                        }
                    }
                    catch (Exception)
                    {}

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
                                    //Set the Membership and Role providers for the Web Application
                                    var ap = new SPFormsAuthenticationProvider(
                                        item["WebApplicationMembershipProvider"].ToString(), item["WebApplicationRoleProvider"].ToString());

                                    try
                                    {
                                        webApp.IisSettings[SPUrlZone.Default].AddClaimsAuthenticationProvider(ap);
                                    }
                                    catch (ArgumentException)
                                    {
                                        foreach (
                                            SPAuthenticationProvider provider in
                                                webApp.IisSettings[SPUrlZone.Default].ClaimsAuthenticationProviders)
                                        {
                                            if (provider.ClaimProviderName == "Forms")
                                            {
                                                webApp.IisSettings[SPUrlZone.Default].DeleteClaimsAuthenticationProvider(provider);
                                                break;
                                            }

                                            webApp.IisSettings[SPUrlZone.Default].AddClaimsAuthenticationProvider(ap);
                                            //webApp.IisSettings[SPUrlZone.Default].ClaimsAuthenticationRedirectionUrl = new Uri("/_layouts/Nauplius.ADLDS.FBA/login.aspx", UriKind.RelativeOrAbsolute);
                                        }
                                    }

                                    try
                                    {
                                        WebModifications.CreateWildcardNode(false, webApp);
                                        WebModifications.CreateProviderNode(false, webApp);
                                        bool successful = WebModifications.CreateStsProviderNode(false, properties);
                                        if (successful)
                                        {
                                            foreach (SPJobDefinition job in adminWebApp.JobDefinitions)
                                            {
                                                if (job.Name == tJobName)
                                                {
                                                    job.IsDisabled = false;
                                                    job.Execute(Guid.Empty);
                                                    job.IsDisabled = true;
                                                }
                                            }
                                        }

                                        WebModifications.CreateAdminWildcardNode(false, webApp);
                                        WebModifications.CreateAdminProviderNode(false, webApp);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var webApp = properties.Feature.Parent as SPWebApplication;
            var adminWebApp = SPAdministrationWebApplication.Local;

            WebModifications.CreateWildcardNode(true, webApp);
            WebModifications.CreateProviderNode(true, webApp);
            WebModifications.CreateStsProviderNode(true, properties);

            foreach (SPJobDefinition job in adminWebApp.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.IsDisabled = false;
                    job.Execute(Guid.Empty);
                    job.IsDisabled = true;
                }                               
            }

            WebModifications.CreateAdminWildcardNode(true, webApp);
            WebModifications.CreateAdminProviderNode(true, webApp);

            //Remove the Forms Authentication provider for the Web Application
            try
            {
                foreach (SPAuthenticationProvider provider in
                    webApp.IisSettings[SPUrlZone.Default].ClaimsAuthenticationProviders)
                {
                    if (provider.ClaimProviderName == "Forms")
                    {
                        webApp.IisSettings[SPUrlZone.Default].DeleteClaimsAuthenticationProvider(provider);
                        //webApp.IisSettings[SPUrlZone.Default].ClaimsAuthenticationRedirectionUrl = null;
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
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            var webApp = properties.Feature.Parent as SPWebApplication;
            var adminWebApp = new SPAdministrationWebApplication();

            WebModifications.CreateWildcardNode(true, webApp);
            WebModifications.CreateProviderNode(true, webApp);
            WebModifications.CreateStsProviderNode(true, properties);

            foreach (SPJobDefinition job in adminWebApp.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.IsDisabled = false;
                    job.Execute(Guid.Empty);
                    job.IsDisabled = true;
                }
            }

            WebModifications.CreateAdminWildcardNode(true, webApp);
            WebModifications.CreateAdminProviderNode(true, webApp);

            //Remove the Forms Authentication provider for the Web Application
            try
            {
                foreach (SPAuthenticationProvider provider in
                    webApp.IisSettings[SPUrlZone.Default].ClaimsAuthenticationProviders)
                {
                    if (provider.ClaimProviderName == "Forms")
                    {
                        webApp.IisSettings[SPUrlZone.Default].DeleteClaimsAuthenticationProvider(provider);
                        //webApp.IisSettings[SPUrlZone.Default].ClaimsAuthenticationRedirectionUrl = null;
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
                catch (Exception)
                {
                }
            }
        }
    }
}
