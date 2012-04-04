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
        public static string AccountName;
        public static string Department;
        public static string DistinguishedName;
        public static string FirstName;
        public static string LastName;
        public static string Office;
        public static string PreferredName;
        public static string UserTitle;
        public static string WebSite;
        public static string WorkEmail;
        public static string WorkPhone;
        public static string LDAPFilter;
        public static string ClaimsIdentifier;
        public static string ServerName;
        public static int PortNumber;
        public static bool UseSSL;
        public static bool DeleteProfiles;
        public static string LoginAttribute;

        public ADLDSImportJob() : base() {}

        public ADLDSImportJob(String name, SPWebApplication webApp, SPServer server, SPJobLockType lockType)
            : base(name, webApp, server, lockType) { }

        public ADLDSImportJob(String name, SPWebApplication webApp)
            : base(name, webApp, null, SPJobLockType.Job)
        {
            this.Title = tJobName;
        }

        public override void Execute(Guid targetInstanceId)
        {
            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;
            using (SPSite siteCollection = new SPSite(adminWebApp.Id))
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
                                    Department = item["Department"].ToString();
                                    FirstName = item["FirstName"].ToString();
                                    LastName = item["LastName"].ToString();
                                    Office = item["Office"].ToString();
                                    PreferredName = item["PreferredName"].ToString();
                                    UserTitle = item["UserTitle"].ToString();
                                    WebSite = item["WebSite"].ToString();
                                    WorkEmail = item["WorkEmail"].ToString();
                                    WorkPhone = item["WorkPhone"].ToString();
                                }
                            }
                        }
                    }

                    SPList list2 = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - WebAppSettings");
                    if (list != null)
                    {
                        if (list.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list.Items)
                            {
                                ServerName = item["ADLDSServer"].ToString();
                                PortNumber = (int)item["ADLDSPort"];
                                DistinguishedName = item["ADLDSDN"].ToString();
                                UseSSL = (bool)item["ADLDSUseSSL"];
                                LoginAttribute = item["ADLDSLoginAttrib"].ToString();

                                DirectoryEntry de = DirEntry(ServerName, PortNumber, DistinguishedName);
                                SearchResultCollection results = ResultCollection(de);

                                Create(results, partition.logonAttribute, partition.webApplication, partition);

                                if (Convert.ToBoolean(ConfigurationManager.AppSettings["DeleteProfiles"]))
                                {
                                    Delete(results, partition.logonAttribute, partition.webApplication, partition);
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

        public static void Create(SearchResultCollection users, string loginAttribute, string siteUrl, Partition partition)
        {
            foreach (SearchResult user in users)
            {
                DirectoryEntry de2 = user.GetDirectoryEntry();
                SPSite site = null;
                try
                {
                    site = new SPSite(siteUrl);

                    SPWebApplication wa = SPWebApplication.Lookup(new Uri(siteUrl));
                    SPIisSettings iisSettings = wa.GetIisSettingsWithFallback(SPUrlZone.Default);

                    foreach (SPAuthenticationProvider provider in iisSettings.ClaimsAuthenticationProviders)
                    {
                        if (provider.GetType() == typeof(SPFormsAuthenticationProvider))
                        {
                            SPFormsAuthenticationProvider formsProvider = provider as SPFormsAuthenticationProvider;

                            string claimIdentifier = ConfigurationManager.AppSettings.Get("ClaimsIdentifier");
                            SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                            UserProfileManager uPM = new UserProfileManager(serviceContext);

                            SPSecurity.RunWithElevatedPrivileges(delegate()
                            {
                                if (de2.Properties[loginAttribute].Value != null)
                                {
                                    if (!uPM.UserExists(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                    de2.Properties[loginAttribute].Value.ToString()))
                                    {
                                        Department = (de2.Properties[ConfigurationManager.AppSettings["Department"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["Department"]].Value.ToString();
                                        DistinguishedName = de2.Properties["distinguishedName"].Value.ToString();
                                        FirstName = (de2.Properties[ConfigurationManager.AppSettings["FirstName"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["FirstName"]].Value.ToString();
                                        LastName = (de2.Properties[ConfigurationManager.AppSettings["LastName"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["LastName"]].Value.ToString();
                                        Office = (de2.Properties[ConfigurationManager.AppSettings["Office"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["Office"]].Value.ToString();
                                        PreferredName = (de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value.ToString();
                                        Title = (de2.Properties[ConfigurationManager.AppSettings["Title"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["UserTitle"]].Value.ToString();
                                        WebSite = (de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value.ToString();
                                        WorkEmail = (de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value.ToString();
                                        WorkPhone = (de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value.ToString();

                                        UserProfile newProfile = uPM.CreateUserProfile(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
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

                                            if (Environment.UserInteractive)
                                            {
                                            }
                                            else
                                            {
                                                Console.WriteLine("Created new profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString());
                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            if (!Environment.UserInteractive)
                                            {
                                            }
                                            else
                                            {
                                                Console.WriteLine("Failed to create new profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString() + Environment.NewLine + ex.Message);
                                            }
                                        }
                                    }
                                    else if (uPM.UserExists(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                        de2.Properties[loginAttribute].Value.ToString()))
                                    {
                                        UserProfile updateProfile = uPM.GetUserProfile(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                            de2.Properties[loginAttribute].Value.ToString());

                                        updateProfile["Department"].Value = (de2.Properties[ConfigurationManager.AppSettings["Department"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["Department"]].Value.ToString();
                                        updateProfile["FirstName"].Value = (de2.Properties[ConfigurationManager.AppSettings["FirstName"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["FirstName"]].Value.ToString();
                                        updateProfile["LastName"].Value = (de2.Properties[ConfigurationManager.AppSettings["LastName"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["LastName"]].Value.ToString();
                                        updateProfile["Office"].Value = (de2.Properties[ConfigurationManager.AppSettings["Office"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["Office"]].Value.ToString();
                                        updateProfile["PreferredName"].Value = (de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value.ToString();
                                        updateProfile["Title"].Value = (de2.Properties[ConfigurationManager.AppSettings["Title"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["Title"]].Value.ToString();
                                        updateProfile["WebSite"].Value = (de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value.ToString();
                                        updateProfile["WorkEmail"].Value = (de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value.ToString();
                                        updateProfile["WorkPhone"].Value = (de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value == null) ? String.Empty :
                                            de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value.ToString();

                                        try
                                        {
                                            updateProfile.Commit();

                                            if (!Environment.UserInteractive)
                                            {
                                            }
                                            else
                                            {
                                                Console.WriteLine("Updated profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString());
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            if (!Environment.UserInteractive)
                                            {
                                            }
                                            else
                                            {
                                                Console.WriteLine("Failed to update profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString() + Environment.NewLine + ex.Message);
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (!Environment.UserInteractive)
                    {
                    }
                    else
                    {
                        Console.WriteLine("Unable to create SPSite object for Url " + siteUrl + Environment.NewLine + ex.Message);
                    }
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

        public static void Delete(SearchResultCollection users, string loginAttribute, string siteUrl, Partition partition)
        {
            SPSite site = null;

            try
            {
                site = new SPSite(siteUrl);

                SPWebApplication wa = SPWebApplication.Lookup(new Uri(siteUrl));
                SPIisSettings iisSettings = wa.GetIisSettingsWithFallback(SPUrlZone.Default);

                foreach (SPAuthenticationProvider provider in iisSettings.ClaimsAuthenticationProviders)
                {
                    if (provider.GetType() == typeof(SPFormsAuthenticationProvider))
                    {
                        SPFormsAuthenticationProvider formsProvider = provider as SPFormsAuthenticationProvider;

                        string claimIdentifier = ConfigurationManager.AppSettings.Get("ClaimsIdentifier");
                        SPServiceContext serviceContext = SPServiceContext.GetContext(site);
                        UserProfileManager uPM = new UserProfileManager(serviceContext);

                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            string search = claimIdentifier + "|" + formsProvider.MembershipProvider + "|";
                            ProfileBase[] uPAResults = uPM.Search(search);

                            foreach (ProfileBase profile in uPAResults)
                            {
                                UserProfile uP = (UserProfile)profile;
                                DirectoryEntry de = DirEntry(partition);

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
                                        if (!Environment.UserInteractive)
                                        {
                                        }
                                        else if (Environment.UserInteractive)
                                        {
                                            Console.WriteLine("Removing Profile for deleted user " +
                                                uP[PropertyConstants.DistinguishedName].Value.ToString());
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (!Environment.UserInteractive)
                                    {
                                    }
                                    else if (Environment.UserInteractive)
                                    {
                                        Console.WriteLine("Error attempting to remove Profile for deleted user " +
                                            uP[PropertyConstants.DistinguishedName].Value.ToString() +
                                            Environment.NewLine + ex.Message);
                                    }
                                }
                            }
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if (!Environment.UserInteractive)
                {
                }
                else
                {
                    Console.WriteLine("Unable to create SPSite object for Url " + siteUrl + Environment.NewLine + ex.Message);
                }
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
}