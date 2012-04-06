using System;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Security.Principal;

using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Runtime.InteropServices;

namespace Nauplius.ADLDS.UserProfiles
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

        public ADLDSImportJob(String name, SPWebApplication adminWebApplication, SPServer server, SPJobLockType lockType)
            : base(name, adminWebApplication, server, lockType) { }

        public ADLDSImportJob(String name, SPWebApplication adminWebApplication)
            : base(name, adminWebApplication, null, SPJobLockType.Job)
        {
            this.Title = tJobName;
        }


        public override void Execute(Guid targetInstanceId)
        {
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

                    SPList list2 = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - WebAppSettings");
                    if (list2 != null)
                    {
                        if (list2.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list2.Items)
                            {
                                WebApplication = SPWebApplication.Lookup(new Uri(item["WebApplicationUrl"].ToString()));
                                ServerName = item["ADLDSServer"].ToString();
                                PortNumber = (int)item["ADLDSPort"];
                                DistinguishedNameRoot = item["ADLDSDN"].ToString();
                                UseSSL = (bool)item["ADLDSUseSSL"];
                                LoginAttribute = item["ADLDSLoginAttrib"].ToString();

                                DirectoryEntry de = DirEntry(ServerName, PortNumber, DistinguishedNameRoot);
                                SearchResultCollection results = ResultCollection(de);

                                Create(results, LoginAttribute, WebApplication, ServerName, PortNumber);

                                if (Convert.ToBoolean(DeleteProfiles))
                                {
                                    Delete(results, LoginAttribute, WebApplication, ServerName, PortNumber);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static DirectoryEntry DirEntry(String serverName, int serverPort, String distinguishedName)
        {
            DirectoryEntry de = new DirectoryEntry();
            string path = "LDAP://" + serverName + ":" + serverPort + "/" + distinguishedName;

            if (UseSSL)
            {
                de.AuthenticationType = AuthenticationTypes.Secure | AuthenticationTypes.SecureSocketsLayer;
            }
            else
            {
                de.AuthenticationType = AuthenticationTypes.Secure;
            }

            if (Environment.UserInteractive)
            {
                Console.WriteLine("Binding to {0} with user {1}", path, WindowsIdentity.GetCurrent().Name);
            }

            try
            {
                de.Path = path;
                de.RefreshCache();
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("Bound to {0}", path);
                }
            }
            catch (Exception ex)
            {
                if (!Environment.UserInteractive)
                {
                    Environment.Exit(1);
                }
                else
                {
                    Console.WriteLine("Failed to bind to {0} with error: " + ex.Message, path);
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
            return de;
        }

        private SearchResultCollection ResultCollection(DirectoryEntry de)
        {
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.SearchRoot = de;
            ds.SearchScope = SearchScope.Subtree;
            ds.Filter = LDAPFilter;

            //Console.WriteLine("Searching for users...");

            SearchResultCollection results = ds.FindAll();

            if (results.Count > 0)
            {
                if (Environment.UserInteractive)
                {
                    Console.WriteLine("Found {0} users.", results.Count);
                }

                ds.Dispose();
                return results;
            }

            ds.Dispose();
            return null;
        }

        public static void Create(SearchResultCollection users, string loginAttribute, SPWebApplication webApplication, string serverName, int portNumber)
        {
            foreach (SearchResult user in users)
            {
                DirectoryEntry de2 = user.GetDirectoryEntry();
                SPSite site = null;
                try
                {
                    site = new SPSite(WebApplication.GetResponseUri(SPUrlZone.Default).AbsoluteUri);

                    //SPWebApplication wa = SPWebApplication.Lookup(new Uri(siteUrl));
                    SPIisSettings iisSettings = webApplication.GetIisSettingsWithFallback(SPUrlZone.Default);

                    foreach (SPAuthenticationProvider provider in iisSettings.ClaimsAuthenticationProviders)
                    {
                        if (provider.GetType() == typeof(SPFormsAuthenticationProvider))
                        {
                            SPFormsAuthenticationProvider formsProvider = provider as SPFormsAuthenticationProvider;

                           // string claimIdentifier = ConfigurationManager.AppSettings.Get("ClaimsIdentifier");
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
                                        }
                                        catch (Exception ex)
                                        { }
                                    }
                                    else if (uPM.UserExists(ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                        de2.Properties[loginAttribute].Value.ToString()))
                                    {
                                        UserProfile updateProfile = uPM.GetUserProfile(ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                            de2.Properties[loginAttribute].Value.ToString());

                                        updateProfile[PropertyConstants.Department].Value = (de2.Properties[DepartmentAttrib].Value == null) ? String.Empty :
                                            de2.Properties[DepartmentAttrib].Value.ToString();
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
                                        }
                                        catch (Exception ex)
                                        { }
                                    }
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                { }

                finally
                {
                    if (site != null)
                    {
                        site.Dispose();
                    }
                }
            }
        }

        public static void Delete(SearchResultCollection users, string loginAttribute, SPWebApplication webApplication, string serverName, int portNumber)
        {
            SPSite site = null;

            try
            {
                //site = new SPSite(siteUrl);

               // SPWebApplication wa = SPWebApplication.Lookup(new Uri(siteUrl));
                SPIisSettings iisSettings = webApplication.GetIisSettingsWithFallback(SPUrlZone.Default);

                foreach (SPAuthenticationProvider provider in iisSettings.ClaimsAuthenticationProviders)
                {
                    if (provider.GetType() == typeof(SPFormsAuthenticationProvider))
                    {
                        SPFormsAuthenticationProvider formsProvider = provider as SPFormsAuthenticationProvider;

                        //string claimIdentifier = ConfigurationManager.AppSettings.Get("ClaimsIdentifier");
                        SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                        UserProfileManager uPM = new UserProfileManager(serviceContext);

                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            string search = ClaimsIdentifier + "|" + formsProvider.MembershipProvider + "|";
                            ProfileBase[] uPAResults = uPM.Search(search);

                            foreach (ProfileBase profile in uPAResults)
                            {
                                UserProfile uP = (UserProfile)profile;
                                DirectoryEntry de = DirEntry(ServerName, PortNumber, DistinguishedNameAttrib);

                                DirectorySearcher ds = new DirectorySearcher(de);
                                ds.SearchRoot = de;
                                ds.SearchScope = SearchScope.Subtree;
                                ds.Filter = "(&(distinguishedName=" + uP[PropertyConstants.DistinguishedName].Value.ToString() + "))";

                                try
                                {
                                    SearchResult result = ds.FindOne();
                                    if (result == null)
                                    {
                                        uPM.RemoveProfile(profile);
                                    }
                                }
                                catch (Exception ex)
                                { }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            { }

            finally
            {
                if (site != null)
                {
                    site.Dispose();
                }
            }
        }
    }
}