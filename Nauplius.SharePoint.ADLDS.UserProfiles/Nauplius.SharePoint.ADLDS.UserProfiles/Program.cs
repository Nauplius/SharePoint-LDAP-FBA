using System;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Security.Principal;
using System.Threading;

using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Nauplius.SharePoint.ADLDS.UserProfiles
{
    class Program
    {
        public static string AccountName { get; set; }
        public static string Department { get; set; }
        public static string DistinguishedName { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string Office { get; set; }
        public static string PreferredName { get; set; }
        public static string Title { get; set; }
        public static string WebSite { get; set; }
        public static string WorkEmail { get; set; }
        public static string WorkPhone { get; set; }

        System.Threading.Timer timer;

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
                                            de2.Properties[ConfigurationManager.AppSettings["Title"]].Value.ToString();
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
                                        newProfile[PropertyConstants.Title].Add(Title);
                                        newProfile[PropertyConstants.WebSite].Add(WebSite);
                                        newProfile[PropertyConstants.WorkEmail].Add(WorkEmail);
                                        newProfile[PropertyConstants.WorkPhone].Add(WorkPhone);
                                        
                                        try
                                        {
                                            newProfile.Commit();

                                            if (Environment.UserInteractive)
                                            {
                                                Logging.WriteEventLog(200, "Created new profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString(), EventLogEntryType.Information, Logging.LogLevel.Informational);
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
                                                Logging.WriteEventLog(400, "Failed to create new profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString() + Environment.NewLine + ex.Message, EventLogEntryType.Error,
                                                    Logging.LogLevel.Error);
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
                                                Logging.WriteEventLog(201, "Updated profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString(), EventLogEntryType.Information, Logging.LogLevel.Informational);
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
                                                Logging.WriteEventLog(401, "Failed to update profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                                    de2.Properties[loginAttribute].Value.ToString() + Environment.NewLine + ex.Message, EventLogEntryType.Error,
                                                    Logging.LogLevel.Informational);
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
                        Logging.WriteEventLog(405, "Unable to create SPSite object for Url " + siteUrl + Environment.NewLine +
                            ex.Message, EventLogEntryType.Error, Logging.LogLevel.Error);
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
                                                Logging.WriteEventLog(202, "Removed Profile for deleted user " + 
                                                    uP[PropertyConstants.DistinguishedName].Value.ToString(),
                                                    EventLogEntryType.Information, Logging.LogLevel.Informational);
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
                                            Logging.WriteEventLog(402, "Error attempting to remove Profile for deleted user " + 
                                                uP[PropertyConstants.DistinguishedName].Value.ToString() + Environment.NewLine +
                                                ex.Message,
                                                EventLogEntryType.Error, Logging.LogLevel.Error);
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
                        Logging.WriteEventLog(405, "Unable to create SPSite object for Url " + siteUrl + Environment.NewLine +
                            ex.Message, EventLogEntryType.Error, Logging.LogLevel.Error);
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

        public void Timer()
        {
            Logging.CreateSource();

            int timerInterval = 300000;
            try
            {
                timerInterval = (int)TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["ImportTimer"])).TotalMilliseconds;
            }
            catch { }

            timer = new System.Threading.Timer(new TimerCallback(Main), null, 0, timerInterval);
        }

        private void Main(object obj)
        {		
            PartitionsSection config = (PartitionsSection)ConfigurationManager.GetSection("partitionsSection");
            foreach (Partition partition in config.Partitions)
            {
                DirectoryEntry de = DirEntry(partition);
                SearchResultCollection results = ResultCollection(de);

                Create(results, partition.logonAttribute, partition.webApplication, partition);

                if (Convert.ToBoolean(ConfigurationManager.AppSettings["DeleteProfiles"]))
                {
                    Delete(results, partition.logonAttribute, partition.webApplication, partition);
                }
            }
        }

        private SearchResultCollection ResultCollection(DirectoryEntry de)
        {
            DirectorySearcher ds = new DirectorySearcher(de);
            ds.SearchRoot = de;
            ds.SearchScope = SearchScope.Subtree;
            ds.Filter = ConfigurationManager.AppSettings["LDAPObjectFilter"];

            Console.WriteLine("Searching for users...");

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

        private static DirectoryEntry DirEntry(Partition partition)
        {
            DirectoryEntry de = new DirectoryEntry();
            string path = "LDAP://" + partition.server + ":" + partition.port + "/" + partition.dn;

            if (partition.useSSL)
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
                    Logging.WriteEventLog(404, "Failed to bind to " + path + " with error " + ex.Message, EventLogEntryType.Error,
                        Logging.LogLevel.Error);
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
    }
}