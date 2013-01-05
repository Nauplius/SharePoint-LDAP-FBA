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

        private const string ProviderMemberType =
            @"Microsoft.Office.Server.Security.LdapMembershipProvider, Microsoft.Office.Server, 
            Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";

        private const string ProviderRoleType =
            @"Microsoft.Office.Server.Security.LdapRoleProvider, Microsoft.Office.Server, 
            Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c";

        public static void CreateWildcardNode(bool removeModification, SPFeatureReceiverProperties properties)
        {
            string featureId = properties.Feature.DefinitionId.ToString();
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if(webApp.UseClaimsAuthentication)
            {
                if (removeModification)
                {
                    RemoveAllModifications(properties);
                }

                string name, xpath, value;
                SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);

                xpath = "configuration/SharePoint/PeoplePickerWildcards";
                name = String.Format("add[@key='{0}']", provider["WebApplicationMembershipProvider"]);
                value = String.Format("<add key='{0}' value='*' />", provider["WebApplicationMembershipProvider"]);
                ModifyWebConfig(webApp, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

                name = String.Format("add[@key='{0}'", provider["WebApplicationRoleProvider"]);
                value = String.Format("<add key='{0}' value='*' />", provider["WebApplicationRoleProvider"]);
                ModifyWebConfig(webApp, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

                try
                {
                    webApp.Farm.Services.GetValue<SPWebService>().ApplyWebConfigModifications();
                }
                catch (Exception ex)
                {
                    RemoveAllModifications(properties);
                    throw ex;
                }
            }
        }

        public static void CreateProviderNode(bool removeModification, SPFeatureReceiverProperties properties)
        {
            string featureId = properties.Feature.DefinitionId.ToString();
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if (webApp.UseClaimsAuthentication)
            {
                if (removeModification)
                {
                    RemoveAllModifications(properties);
                }

                string name, xpath, value;
                SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);

                /* <add name="FabrikamMember" type="Microsoft.Office.Server.Security.LdapMembershipProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" 
                 * server="adlds01.nauplius.local" port="636" useSSL="true" enableSearchMethods="true"
                 * userDNAttribute="distinguishedName" userNameAttribute="mail" 
                 * userContainer="CN=SharePoint,DC=Fabrikam,DC=local" 
                 * userObjectClass="user" userFilter="(ObjectClass=*)" 
                 * scope="Subtree" otherRequiredUserAttributes="sn,givenname,cn" />
                */

                name = string.Format("add[@name='{0}'", provider["WebApplicationMembershipProvider"]);
                xpath = "configuration/system.web/membership/providers";
                value = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                        "useSSL='{4}' enableSearchMethods='{5}' userDNAttribute='{6}' userNameAttribute='{7}' " +
                                        "userContainer='{8}' userObjectClass='{9}' userFilter='{10}' scope='{11}' " +
                                        "otherRequiredUserAttributes='{12}' />", provider["WebApplicationMembershipProvider"],
                                        ProviderMemberType, provider["ADLDSServer"], provider["ADLDSPort"], provider["ADLDSUseSSL"],
                                        "true", provider["ADLDSUserDNAttrib"], provider["ADLDSUserLoginAttrib"], provider["ADLDSUserContainer"],
                                        provider["ADLDSUserObjectClass"], provider["ADLDSUserfilter"], provider["ADLDSUserScope"],
                                        provider["ADLDSUserOtherReqAttrib"]);
                ModifyWebConfig(webApp, name, xpath, value, SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode);

                /* add name="FabrikamRole" 
                 * type="Microsoft.Office.Server.Security.LdapRoleProvider, Microsoft.Office.Server, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" 
                 * server="adlds01.nauplius.local" port="636" useSSL="true" enableSearchMethods="true" 
                 * groupContainer="CN=SharePoint,DC=Fabrikam,DC=local" groupNameAttribute="cn" 
                 * groupNameAlternateSearchAttribute="cn" groupMemberAttribute="member" userNameAttribute="mail" 
                 * dnAttribute="distinguishedName" useUserDNAttribute="true" scope="Subtree" 
                 * userFilter="&amp;(objectClass=user)(objectCategory=person)" 
                 * groupFilter="&amp;(objectCategory=Group)(objectClass=group)" />
                */

                name = String.Format("add[@name='{0}'", provider["WebApplicationRoleProvider"]);
                xpath = "configuration/system.web/roleManager/providers";
                value = String.Format("<add name='{0}' type=''{1}'' server='{2}' port='{3}' " +
                                        "useSSL='{4}' enableSearchMethods='{5}' groupNameAttribute='{6}' " +
                                        "groupContainer='{7}' groupNameAlterateSearchAttribute='{8}' groupMemberAttribute='{9}' " +
                                        "userNameAttribute='{10}' dnAttribute='{11}' useUserDNAttribute='{12}' scope='{13}' " +
                                        "userFilter='{14}' groupFilter='{15}' />", provider["WebApplicationRoleProvider"],
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
                    RemoveAllModifications(properties);
                    throw ex;
                }
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

        public static void RemoveAllModifications(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = (SPWebApplication) properties.Feature.Parent;

            List<SPWebConfigModification> modifications = new List<SPWebConfigModification>();

            foreach (SPWebConfigModification modification in webApp.WebConfigModifications)
            {
                if (modification.Owner == ModificationOwner)
                    modifications.Add(modification);
            }

            foreach (SPWebConfigModification modification in modifications)
            {
                webApp.WebConfigModifications.Remove(modification);
            }

            webApp.Update();
        }

        public static SPListItem GetClaimProvider(SPWebApplication webApp, SPUrlZone zone)
        {
            IEnumerable<SPAuthenticationProvider> providers = webApp.GetIisSettingsWithFallback(zone).ClaimsAuthenticationProviders;
            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;

            using (SPSite siteCollection = new SPSite(adminWebApp.Sites[0].Url))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - Administration");
                    if (list != null)
                    {
                        if (list.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list.Items)
                            {
                                if (SPWebApplication.Lookup(new Uri(item["WebApplicationUrl"].ToString())).GetResponseUri((SPUrlZone)(item["WebApplicationZone"])).AbsoluteUri == webApp.GetResponseUri(zone).AbsoluteUri)
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

        public static void CreateStsProviderNode(bool removeModification, SPFeatureReceiverProperties properties)
        {
            string featureId = properties.Feature.DefinitionId.ToString();
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if (webApp.UseClaimsAuthentication)
            {
                if (removeModification)
                {
                    //remove sts modification  
                }

                string xpath, xpath2, value, value2;
                SPListItem provider = GetClaimProvider(webApp, SPUrlZone.Default);
            
                foreach (SPServer spServer in SPFarm.Local.Servers)
                {
                    string path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
                    var config = new XmlDocument();
                    config.Load(path);

                    if (config.SelectSingleNode(@"configuration/system.web") == null)
                    {
                        CreateStsXPath(config, path, "configuration/system.web");
                    }

                    if (config.SelectSingleNode(@"configuration/system.web/membership") == null)
                    {
                        CreateStsXPath(config, path, "configuration/system.web/membership");
                    }

                    if (config.SelectSingleNode(@"configuration/system.web/membership/providers") == null)
                    {
                        CreateStsXPath(config, path, "configuration/system.web/membership/providers");
                    }

                    if (config.SelectSingleNode(@"configuration/system.web/roleManager") == null)
                    {
                        CreateStsXPath(config, path, "configuration/system.web/roleManager");
                    }

                    if (config.SelectSingleNode(@"configuration/system.web/roleManager/providers") == null)
                    {
                        CreateStsXPath(config, path, "configuration/system.web/roleManager/providers");
                    }


                    xpath = "configuration/system.web/membership/providers";
                    value = String.Format("<add name='{0}' type='{1}' server='{2}' port='{3}' " +
                                            "useSSL='{4}' enableSearchMethods='{5}' userDNAttribute='{6}' userNameAttribute='{7}' " +
                                            "userContainer='{8}' userObjectClass='{9}' userFilter='{10}' scope='{11}' " +
                                            "otherRequiredUserAttributes='{12}' />", provider["WebApplicationMembershipProvider"],
                                            ProviderMemberType, provider["ADLDSServer"], provider["ADLDSPort"], provider["ADLDSUseSSL"],
                                            "true", provider["ADLDSUserDNAttrib"], provider["ADLDSUserLoginAttrib"], provider["ADLDSUserContainer"],
                                            provider["ADLDSUserObjectClass"], provider["ADLDSUserfilter"], provider["ADLDSUserScope"],
                                            provider["ADLDSUserOtherReqAttrib"]);

                    xpath2 = "configuration/system.web/roleManager/providers";
                    value2 = String.Format("<add name='{0}' type=''{1}'' server='{2}' port='{3}' " +
                                            "useSSL='{4}' enableSearchMethods='{5}' groupNameAttribute='{6}' " +
                                            "groupContainer='{7}' groupNameAlterateSearchAttribute='{8}' groupMemberAttribute='{9}' " +
                                            "userNameAttribute='{10}' dnAttribute='{11}' useUserDNAttribute='{12}' scope='{13}' " +
                                            "userFilter='{14}' groupFilter='{15}' />", provider["WebApplicationRoleProvider"],
                                            ProviderRoleType, provider["ADLDSServer"], provider["ADLDSPort"],
                                            provider["ADLDSUseSSL"], "true", provider["ADLDSGroupNameAttrib"],
                                            provider["ADLDSGroupContainer"],
                                            provider["ADLDSGroupNameAltSearchAttrib"], provider["ADLDSGroupMemAttrib"],
                                            provider["ADLDSLoginAttrib"], provider["ADLDSGroupDNAttrib"], "true",
                                            provider["ADLDSGroupScope"], provider["ADLDSGroupUserFilter"],
                                            provider["ADLDSGroupFilter"]);   
                }
            }
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
