using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Sync
{
    [Guid("A925C800-D446-402A-9882-1956C57D3D51")]
    class ADLDSImportJob : SPJobDefinition
    {
        const string tJobName = "Nauplius ADLDS User Profile Import";
        public static string AccountNameAttrib;
        public static string DepartmentAttrib;
        public static string DistinguishedNameAttrib = "distinguishedName";
        public static string DistinguishedNameRoot;
        public static string FirstNameAttrib;
        public static string LastNameAttrib;
        public static string OfficeAttrib;
        public static string PreferredNameAttrib;
        public static string UserTitleAttrib;
        public static string WebSiteAttrib;
        public static string WorkEmailAttrib;
        public static string WorkPhoneAttrib;
        public static string LDAPFilter;
        public static string ClaimsIdentifier;
        public static SPWebApplication WebApplication;
        public static string ServerName;
        public static int PortNumber;
        public static bool UseSSL;
        public static bool DeleteProfiles;
        public static string LoginAttribute;
        public static string AccountName { get; set; }
        public static string Department { get; set; }
        public static string DistinguishedName { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string Office { get; set; }
        public static string PreferredName { get; set; }
        public static string UserTitle { get; set; }
        public static string WebSite { get; set; }
        public static string WorkEmail { get; set; }
        public static string WorkPhone { get; set; }

        public ADLDSImportJob() : base() {}

        public ADLDSImportJob(String name, SPService service, SPServer server, SPJobLockType lockType)
            : base(name, service, server, lockType)
        {
        }

        public ADLDSImportJob(String name, SPService service)
            : base(name, service, null, SPJobLockType.None)
        {
            this.Title = tJobName;
        }


        public override void Execute(Guid targetInstanceId)
        {
            Logging.LogMessage(900, Logging.LogCategories.TimerJob, TraceSeverity.Medium, "Entering " + tJobName, new object[] { null });

            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;
            using (SPSite siteCollection = new SPSite(adminWebApp.Sites[0].Url))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList list = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - GlobalSettings");
                    if (list != null)
                    {
                        if (list.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list.Items)
                            {
                                if (item["GlobalDefault"].ToString() == "GlobalDefaultValues")
                                {
                                    ClaimsIdentifier = item["ClaimsIdentifier"].ToString();
                                    LDAPFilter = item["LDAPFilter"].ToString();
                                    DeleteProfiles = (bool)item["DeleteProfiles"];
                                    DepartmentAttrib = item["Department"].ToString();
                                    FirstNameAttrib = item["FirstName"].ToString();
                                    LastNameAttrib = item["LastName"].ToString();
                                    OfficeAttrib = item["Office"].ToString();
                                    PreferredNameAttrib = item["PreferredName"].ToString();
                                    UserTitleAttrib = item["UserTitle"].ToString();
                                    WebSiteAttrib = item["WebSite"].ToString();
                                    WorkEmailAttrib = item["WorkEmail"].ToString();
                                    WorkPhoneAttrib = item["WorkPhone"].ToString();
                                }
                            }
                        }
                    }

                    SPList list2 = site.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");
                    if (list2 != null)
                    {
                        if (list2.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list2.Items)
                            {
                                WebApplication = SPWebApplication.Lookup(new Uri(item["WebApplicationUrl"].ToString()));
                                ServerName = item["ADLDSServer"].ToString();
                                PortNumber = Convert.ToInt32(item["ADLDSPort"].ToString());
                                DistinguishedNameRoot = item["ADLDSUserContainer"].ToString();
                                UseSSL = (bool)item["ADLDSUseSSL"];
                                LoginAttribute = item["ADLDSLoginAttrib"].ToString();
                                var zone = GetZone(item);
                                DirectoryEntry de = DirEntry(ServerName, PortNumber, DistinguishedNameRoot);

                                if (de != null)
                                {
                                    SearchResultCollection results = ResultCollection(de);

                                    Create(results, LoginAttribute, WebApplication, ServerName, PortNumber, zone);

                                    if (DeleteProfiles)
                                    {
                                        Delete(results, LoginAttribute, WebApplication, ServerName, PortNumber, zone);
                                    }
                                }
                            }

                            Logging.LogMessage(901, Logging.LogCategories.TimerJob, TraceSeverity.Medium, "Exiting " + tJobName, new object[] { null });
                        }
                    }
                }
            }
        }

        private static DirectoryEntry DirEntry(String serverName, int serverPort, String distinguishedName)
        {
            DirectoryEntry de = new DirectoryEntry();
            string path = "LDAP://" + serverName + ":" + serverPort + "/" + distinguishedName;

            Logging.LogMessage(220, Logging.LogCategories.LDAP, TraceSeverity.Verbose, "Binding to " +
                path, new object[] { null });

            if (UseSSL)
            {
                de.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.SecureSocketsLayer;
            }
            else
            {
                de.AuthenticationType = AuthenticationTypes.Secure;
            }

            try
            {
                de.Path = path;
                de.RefreshCache();
                Logging.LogMessage(221, Logging.LogCategories.LDAP, TraceSeverity.Verbose, "Bound to " +
                    path, new object[] { null });
            }
            catch (Exception ex)
            {
                Logging.LogMessage(500, Logging.LogCategories.LDAP, TraceSeverity.Unexpected, ex.Message, new object[] { null });
                return null;
            }
            return de;
        }

        private SearchResultCollection ResultCollection(DirectoryEntry de)
        {
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.SearchRoot = de;
            ds.PageSize = 10000;
            ds.SearchScope = SearchScope.Subtree;
            ds.Filter = LDAPFilter;

            Logging.LogMessage(222, Logging.LogCategories.LDAP, TraceSeverity.Verbose, "Searching for users.", new object[] { null });

            SearchResultCollection results = ds.FindAll();

            if (results.Count > 0)
            {
                ds.Dispose();
                Logging.LogMessage(223, Logging.LogCategories.LDAP, TraceSeverity.Verbose, "Found " + results.Count + " users.", new object[] { null });
                return results;
            }

            ds.Dispose();
            Logging.LogMessage(224, Logging.LogCategories.LDAP, TraceSeverity.Verbose, "Found 0 users.", new object[] { null });

            return null;
        }

        public static void Create(SearchResultCollection users, string loginAttribute, SPWebApplication webApplication, string serverName, int portNumber, SPUrlZone zone)
        {
            foreach (SearchResult user in users)
            {
                DirectoryEntry de2 = user.GetDirectoryEntry();
                SPSite site = null;
                try
                {
                    site = new SPSite(WebApplication.GetResponseUri(zone).AbsoluteUri);

                    SPIisSettings iisSettings = webApplication.GetIisSettingsWithFallback(zone);

                    foreach (SPAuthenticationProvider provider in iisSettings.ClaimsAuthenticationProviders)
                    {
                        if (provider is SPFormsAuthenticationProvider)
                        {
                            SPFormsAuthenticationProvider formsProvider = provider as SPFormsAuthenticationProvider;
                            SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                            UserProfileManager uPM = new UserProfileManager(serviceContext);

                            SPSecurity.RunWithElevatedPrivileges(delegate()
                            {
                                if (de2.Properties[loginAttribute].Value != null)
                                {
                                    if (!uPM.UserExists(ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                    de2.Properties[loginAttribute].Value.ToString()))
                                    {
                                        Department = (de2.Properties[DepartmentAttrib].Value == null) ? String.Empty :
                                            de2.Properties[DepartmentAttrib].Value.ToString();
                                        DistinguishedName = de2.Properties[DistinguishedNameAttrib].Value.ToString();
                                        FirstName = (de2.Properties[FirstNameAttrib].Value == null) ? String.Empty :
                                            de2.Properties[FirstNameAttrib].Value.ToString();
                                        LastName = (de2.Properties[LastNameAttrib].Value == null) ? String.Empty :
                                            de2.Properties[LastNameAttrib].Value.ToString();
                                        Office = (de2.Properties[OfficeAttrib].Value == null) ? String.Empty :
                                            de2.Properties[OfficeAttrib].Value.ToString();
                                        PreferredName = (de2.Properties[PreferredNameAttrib].Value == null) ? String.Empty :
                                            de2.Properties[PreferredNameAttrib].Value.ToString();
                                        UserTitle = (de2.Properties[UserTitleAttrib].Value == null) ? String.Empty :
                                            de2.Properties[UserTitleAttrib].Value.ToString();
                                        WebSite = (de2.Properties[WebSiteAttrib].Value == null) ? String.Empty :
                                            de2.Properties[WebSiteAttrib].Value.ToString();
                                        WorkEmail = (de2.Properties[WorkEmailAttrib].Value == null) ? String.Empty :
                                            de2.Properties[WorkEmailAttrib].Value.ToString();
                                        WorkPhone = (de2.Properties[WorkPhoneAttrib].Value == null) ? String.Empty :
                                            de2.Properties[WorkPhoneAttrib].Value.ToString();

                                        UserProfile newProfile = uPM.CreateUserProfile(ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                            de2.Properties[loginAttribute].Value.ToString(), PreferredName);

                                        newProfile[PropertyConstants.Department].Add(Department);
                                        newProfile[PropertyConstants.DistinguishedName].Add(DistinguishedName);
                                        newProfile[PropertyConstants.FirstName].Add(FirstName);
                                        newProfile[PropertyConstants.LastName].Add(LastName);
                                        newProfile[PropertyConstants.Office].Add(Office);
                                        newProfile[PropertyConstants.Title].Add(UserTitle);
                                        newProfile[PropertyConstants.WebSite].Add(WebSite);
                                        newProfile[PropertyConstants.WorkEmail].Add(WorkEmail);
                                        newProfile[PropertyConstants.WorkPhone].Add(WorkPhone);

                                        try
                                        {
                                            newProfile.Commit();
                                            Logging.LogMessage(210, Logging.LogCategories.Profiles, TraceSeverity.Verbose, "Created profile " +
                                                DistinguishedName, new object[] { null });
                                        }
                                        catch (Exception ex)
                                        {
                                            Logging.LogMessage(510, Logging.LogCategories.Profiles, TraceSeverity.Unexpected, "Failed to create profile " +
                                                DistinguishedName + " " + ex.Message, new object[] { null });
                                        }
                                    }
                                    else if (uPM.UserExists(ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                        de2.Properties[loginAttribute].Value.ToString()))
                                    {
                                        UserProfile updateProfile = uPM.GetUserProfile(ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                            de2.Properties[loginAttribute].Value.ToString());

                                        updateProfile[PropertyConstants.Department].Value = (de2.Properties[DepartmentAttrib].Value == null) ? String.Empty :
                                            de2.Properties[DepartmentAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.DistinguishedName].Value = de2.Properties[DistinguishedNameAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.FirstName].Value = (de2.Properties[FirstNameAttrib].Value == null) ? String.Empty :
                                            de2.Properties[FirstNameAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.LastName].Value = (de2.Properties[LastNameAttrib].Value == null) ? String.Empty :
                                            de2.Properties[LastNameAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.Office].Value = (de2.Properties[OfficeAttrib].Value == null) ? String.Empty :
                                            de2.Properties[OfficeAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.PreferredName].Value = (de2.Properties[PreferredNameAttrib].Value == null) ? String.Empty :
                                            de2.Properties[PreferredNameAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.Title].Value = (de2.Properties[UserTitleAttrib].Value == null) ? String.Empty :
                                            de2.Properties[UserTitleAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.WebSite].Value = (de2.Properties[WebSiteAttrib].Value == null) ? String.Empty :
                                            de2.Properties[WebSiteAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.WorkEmail].Value = (de2.Properties[WorkEmailAttrib].Value == null) ? String.Empty :
                                            de2.Properties[WorkEmailAttrib].Value.ToString();
                                        updateProfile[PropertyConstants.WorkPhone].Value = (de2.Properties[WorkPhoneAttrib].Value == null) ? String.Empty :
                                            de2.Properties[WorkPhoneAttrib].Value.ToString();

                                        try
                                        {
                                            updateProfile.Commit();
                                            Logging.LogMessage(211, Logging.LogCategories.Profiles, TraceSeverity.Verbose, "Updated profile " +
                                               updateProfile[PropertyConstants.DistinguishedName].Value, new object[] { null });
                                        }
                                        catch (Exception ex)
                                        {
                                            Logging.LogMessage(511, Logging.LogCategories.Profiles, TraceSeverity.Unexpected, "Failed to update profile " +
                                                updateProfile[PropertyConstants.DistinguishedName].Value + " " + ex.Message, new object[] { null });
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.LogMessage(502, Logging.LogCategories.Profiles, TraceSeverity.Unexpected, ex.Message, new object[] { null });
                }

                finally
                {
                    if (site != null)
                    {
                        site.Dispose();
                    }
                }
            }
        }

        public static void Delete(SearchResultCollection users, string loginAttribute, SPWebApplication webApplication, string serverName, int portNumber, SPUrlZone zone)
        {
            SPSite site = null;

            try
            {
                site = new SPSite(WebApplication.GetResponseUri(zone).AbsoluteUri);

                SPIisSettings iisSettings = webApplication.GetIisSettingsWithFallback(zone);

                foreach (SPAuthenticationProvider provider in iisSettings.ClaimsAuthenticationProviders)
                {
                    if (provider is SPFormsAuthenticationProvider)
                    {
                        SPFormsAuthenticationProvider formsProvider = provider as SPFormsAuthenticationProvider;

                        SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                        UserProfileManager uPM = new UserProfileManager(serviceContext);

                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            string search = ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|";

                            List<UserProfile> uPAResults = uPM.Search(search).Cast<UserProfile>().ToList();
                            List<SearchResult> usersList = users.Cast<SearchResult>().ToList();

                            var query = usersList.Select(sr => sr.GetDirectoryEntry().Properties["distinguishedName"].Value.ToString());
                           
                            HashSet<string> paths = new HashSet<string>(query);

                            var profiles = uPAResults.Select(profile => new
                            {
                                ShouldKeep = paths.Contains(profile[PropertyConstants.DistinguishedName].Value.ToString()),
                                Profile = profile
                            });

                            foreach (var profile in profiles.Where(result => !result.ShouldKeep))
                            {
                                try
                                {
                                    uPM.RemoveProfile(profile.Profile);
                                    Logging.LogMessage(212, Logging.LogCategories.Profiles, TraceSeverity.Verbose, "Removed profile " +
                                        profile.Profile[PropertyConstants.DistinguishedName].Value, new object[] { null });
                                }
                                catch (Exception ex)
                                {
                                    Logging.LogMessage(502, Logging.LogCategories.Profiles,
                                        TraceSeverity.Unexpected,
                                        "Failed to delete profile " + profile.Profile[PropertyConstants.DistinguishedName].Value +
                                        " " + ex.Message, new object[] { null });
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(502, Logging.LogCategories.Profiles, TraceSeverity.Unexpected, ex.Message, new object[] { null });
            }

            finally
            {
                if (site != null)
                {
                    site.Dispose();
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
                case "Default": return SPUrlZone.Default;
                case "Intranet": return SPUrlZone.Intranet;
                case "Internet": return SPUrlZone.Internet;
                case "Extranet": return SPUrlZone.Extranet;
                case "Custom": return SPUrlZone.Custom;
                default: return SPUrlZone.Default;
            }
        }
    }
}