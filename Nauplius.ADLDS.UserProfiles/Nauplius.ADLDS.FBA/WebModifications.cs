using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;
using Microsoft.SharePoint.IdentityModel;
using Microsoft.SharePoint.Utilities;

namespace Nauplius.ADLDS.FBA
{
    class WebModifications
    {
        private const string ModificationOwner = "Nauplius.ADLDS.FBA";
        private static readonly XmlDocument MasterXmlFragment = new XmlDocument();

        private const string ProviderMemberType =
            @"Microsoft.Office.Server.Security.LdapMembershipProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";

        private const string ProviderRoleType =
            @"Microsoft.Office.Server.Security.LdapRoleProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";

        public static void CreateWildcardNode(bool removeModification, SPWebApplication webApp)
        {
            if (webApp.UseClaimsAuthentication)
            {
                if (removeModification)
                {
                    RemoveAllModifications(webApp);

                    try
                    {
                        webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                    }
                    catch(Exception)
                    {}

                    return;
                }

                string name, xpath, value;
                SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);

                xpath = "configuration/SharePoint/PeoplePickerWildcards";
                name = String.Format("add[@key='{0}']", provider["WebApplicationMembershipProvider"]);
                value = String.Format("<add key='{0}' value='*' />", provider["WebApplicationMembershipProvider"]);
                ModifyWebConfig(webApp, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

                name = String.Format("add[@key='{0}']", provider["WebApplicationRoleProvider"]);
                value = String.Format("<add key='{0}' value='*' />", provider["WebApplicationRoleProvider"]);
                ModifyWebConfig(webApp, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

                try
                {
                    webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                }
                catch (Exception ex)
                {
                    RemoveAllModifications(webApp);
                    throw ex;
                }
            }
        }

        public static void CreateProviderNode(bool removeModification, SPWebApplication webApp)
        {
            if (webApp.UseClaimsAuthentication)
            {
                if (removeModification)
                {
                    RemoveAllModifications(webApp);

                    try
                    {
                        webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                    }
                    catch (Exception)
                    { }

                    return;
                }

                string name, xpath, value;
                SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);

                name = string.Format("add[@name='{0}']", provider["WebApplicationMembershipProvider"]);
                xpath = "configuration/system.web/membership/providers";
                value = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                        "useSSL='{4}' enableSearchMethods='{5}' userDNAttribute='{6}' userNameAttribute='{7}' " +
                                        "userContainer='{8}' userObjectClass='{9}' userFilter='{10}' scope='{11}' " +
                                        "otherRequiredUserAttributes='{12}' />", provider["WebApplicationMembershipProvider"],
                                        ProviderMemberType, provider["ADLDSServer"], provider["ADLDSPort"], provider["ADLDSUseSSL"],
                                        "true", provider["ADLDSUserDNAttrib"], provider["ADLDSLoginAttrib"], provider["ADLDSUserContainer"],
                                        provider["ADLDSUserObjectClass"], provider["ADLDSUserFilter"], provider["ADLDSUserScope"],
                                        provider["ADLDSUserOtherReqAttrib"]);
                ModifyWebConfig(webApp, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

                name = String.Format("add[@name='{0}']", provider["WebApplicationRoleProvider"]);
                xpath = "configuration/system.web/roleManager/providers";
                value = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                        "useSSL='{4}' enableSearchMethods='{5}' groupNameAttribute='{6}' " +
                                        "groupContainer='{7}' groupNameAlterateSearchAttribute='{8}' groupMemberAttribute='{9}' " +
                                        "userNameAttribute='{10}' dnAttribute='{11}' useUserDNAttribute='{12}' scope='{13}' " +
                                        "userFilter=\"{14}\" groupFilter=\"{15}\" />", provider["WebApplicationRoleProvider"],
                                        ProviderRoleType, provider["ADLDSServer"], provider["ADLDSPort"],
                                        provider["ADLDSUseSSL"], "true", provider["ADLDSGroupNameAttrib"],
                                        provider["ADLDSGroupContainer"],
                                        provider["ADLDSGroupNameAltSearchAttrib"], provider["ADLDSGroupMemAttrib"],
                                        provider["ADLDSLoginAttrib"], provider["ADLDSGroupDNAttrib"], "true",
                                        provider["ADLDSGroupScope"], provider["ADLDSGroupUserFilter"],
                                        provider["ADLDSGroupFilter"]);
                ModifyWebConfig(webApp, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

                try
                {
                    webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                }
                catch (Exception ex)
                {
                    RemoveAllModifications(webApp);
                    throw ex;
                }
            }
        }

        public static void CreateAdminWildcardNode(bool removeModification, SPWebApplication webApp)
        {
            var adminWebApplication = SPAdministrationWebApplication.Local;

            if (removeModification)
            {
                RemoveAllModifications(webApp);

                try
                {
                    SPWebService.AdministrationService.WebApplications[adminWebApplication.Id].WebService.ApplyWebConfigModifications();
                }
                catch (Exception)
                { }

                return;
            }

            string name, xpath, value;
            SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);

            xpath = "configuration/SharePoint/PeoplePickerWildcards";
            name = String.Format("add[@key='{0}']", provider["WebApplicationMembershipProvider"]);
            value = String.Format("<add key='{0}' value='*' />", provider["WebApplicationMembershipProvider"]);
            ModifyAdminWebConfig(adminWebApplication, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

            name = String.Format("add[@key='{0}']", provider["WebApplicationRoleProvider"]);
            value = String.Format("<add key='{0}' value='*' />", provider["WebApplicationRoleProvider"]);
            ModifyAdminWebConfig(adminWebApplication, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

            try
            {
                SPWebService.AdministrationService.WebApplications[adminWebApplication.Id].WebService.ApplyWebConfigModifications();
            }
            catch (Exception ex)
            {
                RemoveAllModifications(webApp);
                throw ex;
            }
        }

        public static void CreateAdminProviderNode(bool removeModification, SPWebApplication webApp)
        {
            var adminWebApplication = SPAdministrationWebApplication.Local;

            if (removeModification)
            {
                RemoveAllModifications(webApp);

                try
                {
                    SPWebService.AdministrationService.WebApplications[adminWebApplication.Id].WebService.ApplyWebConfigModifications();
                }
                catch (Exception)
                { }

                return;
            }

            string name, xpath, value;
            SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);

            name = string.Format("add[@name='{0}']", provider["WebApplicationMembershipProvider"]);
            xpath = "configuration/system.web/membership/providers";
            value = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                    "useSSL='{4}' enableSearchMethods='{5}' userDNAttribute='{6}' userNameAttribute='{7}' " +
                                    "userContainer='{8}' userObjectClass='{9}' userFilter='{10}' scope='{11}' " +
                                    "otherRequiredUserAttributes='{12}' />", provider["WebApplicationMembershipProvider"],
                                    ProviderMemberType, provider["ADLDSServer"], provider["ADLDSPort"], provider["ADLDSUseSSL"],
                                    "true", provider["ADLDSUserDNAttrib"], provider["ADLDSLoginAttrib"], provider["ADLDSUserContainer"],
                                    provider["ADLDSUserObjectClass"], provider["ADLDSUserFilter"], provider["ADLDSUserScope"],
                                    provider["ADLDSUserOtherReqAttrib"]);
            ModifyAdminWebConfig(adminWebApplication, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

            name = String.Format("add[@name='{0}']", provider["WebApplicationRoleProvider"]);
            xpath = "configuration/system.web/roleManager/providers";
            value = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                    "useSSL='{4}' enableSearchMethods='{5}' groupNameAttribute='{6}' " +
                                    "groupContainer='{7}' groupNameAlterateSearchAttribute='{8}' groupMemberAttribute='{9}' " +
                                    "userNameAttribute='{10}' dnAttribute='{11}' useUserDNAttribute='{12}' scope='{13}' " +
                                    "userFilter=\"{14}\" groupFilter=\"{15}\" />", provider["WebApplicationRoleProvider"],
                                    ProviderRoleType, provider["ADLDSServer"], provider["ADLDSPort"],
                                    provider["ADLDSUseSSL"], "true", provider["ADLDSGroupNameAttrib"],
                                    provider["ADLDSGroupContainer"],
                                    provider["ADLDSGroupNameAltSearchAttrib"], provider["ADLDSGroupMemAttrib"],
                                    provider["ADLDSLoginAttrib"], provider["ADLDSGroupDNAttrib"], "true",
                                    provider["ADLDSGroupScope"], provider["ADLDSGroupUserFilter"],
                                    provider["ADLDSGroupFilter"]);
            ModifyAdminWebConfig(adminWebApplication, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

            try
            {
                SPWebService.AdministrationService.WebApplications[adminWebApplication.Id].WebService.ApplyWebConfigModifications();
            }
            catch (Exception ex)
            {
                RemoveAllModifications(webApp);
                throw ex;
            }
        }

        private static void ModifyWebConfig(SPWebApplication webApp, string modificationName, string modificationPath,
            string modificationValue, SPWebConfigModification.SPWebConfigModificationType modificationType)
        {
            SPWebConfigModification modification = new SPWebConfigModification(modificationName, modificationPath);
            modification.Value = modificationValue;
            modification.Sequence = 0;
            modification.Type = modificationType;
            modification.Owner = ModificationOwner;

            try
            {
                webApp.WebConfigModifications.Add(modification);
                webApp.Update();
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog();
                eventLog.Source = ModificationOwner;
                eventLog.WriteEntry(ex.Message);
                throw ex;
            }
        }

        private static void ModifyAdminWebConfig(SPAdministrationWebApplication adminWebApp, string modificationName, string modificationPath,
    string modificationValue, SPWebConfigModification.SPWebConfigModificationType modificationType)
        {
            SPWebConfigModification modification = new SPWebConfigModification(modificationName, modificationPath);
            modification.Value = modificationValue;
            modification.Sequence = 0;
            modification.Type = modificationType;
            modification.Owner = ModificationOwner;

            try
            {
                adminWebApp.WebConfigModifications.Add(modification);
                adminWebApp.Update();
            }
            catch (Exception ex)
            {
                EventLog eventLog = new EventLog();
                eventLog.Source = ModificationOwner;
                eventLog.WriteEntry(ex.Message);
                throw ex;
            }
        }

        public static void RemoveAllModifications(SPWebApplication webApp, string name)
        {
            List<SPWebConfigModification> modifications = new List<SPWebConfigModification>();

            foreach (SPWebConfigModification modification in webApp.WebConfigModifications)
            {
                if (modification.Owner == ModificationOwner)
                    modifications.Add(modification);
            }

            foreach (SPWebConfigModification modification in modifications)
            {
                if(modification.Name == name)
                {
                    webApp.WebConfigModifications.Remove(modification);
                }
            }

            webApp.Update();
        }

        public static SPListItem GetClaimProvider(SPWebApplication webApp, SPUrlZone zone)
        {
            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;

            using (SPSite siteCollection = new SPSite(adminWebApp.Sites[0].Url))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");
                    if (list != null)
                    {
                        if (list.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list.Items)
                            {
                                if (item["WebApplicationUrl"].ToString() == webApp.GetResponseUri(zone).AbsoluteUri)
                                {
                                    return item;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static bool CreateStsProviderNode(bool removeModification, SPFeatureReceiverProperties properties)
        {
            string featureId = properties.Feature.DefinitionId.ToString();
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if (webApp.UseClaimsAuthentication)
            {
                if (removeModification)
                {
                    //remove sts modification
                    return true;
                }

                SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);

                string value = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                        "useSSL='{4}' enableSearchMethods='{5}' userDNAttribute='{6}' userNameAttribute='{7}' " +
                                        "userContainer='{8}' userObjectClass='{9}' userFilter='{10}' scope='{11}' " +
                                        "otherRequiredUserAttributes='{12}' />", provider["WebApplicationMembershipProvider"],
                                        ProviderMemberType, provider["ADLDSServer"], provider["ADLDSPort"], provider["ADLDSUseSSL"],
                                        "true", provider["ADLDSUserDNAttrib"], provider["ADLDSLoginAttrib"], provider["ADLDSUserContainer"],
                                        provider["ADLDSUserObjectClass"], provider["ADLDSUserFilter"], provider["ADLDSUserScope"],
                                        provider["ADLDSUserOtherReqAttrib"]);

                string value2 = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                        "useSSL='{4}' enableSearchMethods='{5}' groupNameAttribute='{6}' " +
                                        "groupContainer='{7}' groupNameAlterateSearchAttribute='{8}' groupMemberAttribute='{9}' " +
                                        "userNameAttribute='{10}' dnAttribute='{11}' useUserDNAttribute='{12}' scope='{13}' " +
                                        "userFilter=\"{14}\" groupFilter=\"{15}\" />", provider["WebApplicationRoleProvider"],
                                        ProviderRoleType, provider["ADLDSServer"], provider["ADLDSPort"],
                                        provider["ADLDSUseSSL"], "true", provider["ADLDSGroupNameAttrib"],
                                        provider["ADLDSGroupContainer"],
                                        provider["ADLDSGroupNameAltSearchAttrib"], provider["ADLDSGroupMemAttrib"],
                                        provider["ADLDSLoginAttrib"], provider["ADLDSGroupDNAttrib"], "true",
                                        provider["ADLDSGroupScope"], provider["ADLDSGroupUserFilter"],
                                        provider["ADLDSGroupFilter"]);

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

                                            XmlDocumentFragment xmlFrag1 = MasterXmlFragment.CreateDocumentFragment();
                                            xmlFrag1.InnerXml = value2;

                                            try
                                            {
                                                string nvalue =
                                                    xmlFrag1.FirstChild.Attributes.GetNamedItem("name").Value;
                                                XmlNode node =
                                                    MasterXmlFragment.DocumentElement.SelectSingleNode(
                                                        "roleManager/providers/add[@name='" + nvalue + "']");
                                                node.ParentNode.RemoveChild(node);
                                            }
                                            catch (Exception)
                                            {}

                                            MasterXmlFragment.DocumentElement.SelectSingleNode("roleManager/providers")
                                                             .AppendChild(xmlFrag1);

                                            XmlDocumentFragment xmlFrag2 = MasterXmlFragment.CreateDocumentFragment();
                                            xmlFrag2.InnerXml = value;

                                            try
                                            {
                                                string nvalue =
                                                    xmlFrag2.FirstChild.Attributes.GetNamedItem("name").Value;
                                                XmlNode node =
                                                    MasterXmlFragment.DocumentElement.SelectSingleNode(
                                                        "membership/providers/add[@name='" + nvalue + "']");
                                                node.ParentNode.RemoveChild(node);
                                            }
                                            catch (Exception)
                                            { }

                                            MasterXmlFragment.DocumentElement.SelectSingleNode("membership/providers")
                                                             .AppendChild(xmlFrag2);

                                            item["XMLStsConfig"] = MasterXmlFragment.OuterXml;
                                            item.Update();
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                        catch
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static void CreateStsXPath(XmlDocument config, string path, string xpath)
        {
            if (config.SelectSingleNode(xpath) == null)
            {
                XmlNode parentNode = config.SelectSingleNode(xpath.Remove(xpath.LastIndexOf("/", System.StringComparison.Ordinal)));
                XmlNode childNode = config.CreateNode(XmlNodeType.Element, xpath.Substring(xpath.LastIndexOf("/", System.StringComparison.Ordinal) + 1), "");
                if (parentNode != null) parentNode.AppendChild(childNode);
                try
                {
                    config.Save(path);
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
