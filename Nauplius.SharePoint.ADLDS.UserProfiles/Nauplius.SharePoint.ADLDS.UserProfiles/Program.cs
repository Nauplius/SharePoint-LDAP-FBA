using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Text.RegularExpressions;
using System.Threading;

using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.Server.Administration;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Portal.WebControls;

namespace Nauplius.SharePoint.ADLDS.UserProfiles
{
    class Program
    {
        public static string AccountName { get; set; }
        public static string PreferredName { get; set; }
        public static string WorkPhone { get; set; }
        public static string Department { get; set; }
        public static string Title { get; set; }
        public static string DistinguishedName { get; set; }
        public static string WorkEmail { get; set; }
        public static string Office { get; set; }
        public static string WebSite { get; set; }

        System.Threading.Timer timer;

        public static void Create(SearchResultCollection users, string loginAttribute, string siteUrl)
        {

            foreach(SearchResult user in users)
            {
                DirectoryEntry de2 = user.GetDirectoryEntry();
                SPSite site = new SPSite(siteUrl);
                SPWebApplication wa = SPWebApplication.Lookup(new Uri(siteUrl));

                SPIisSettings iisSettings = wa.GetIisSettingsWithFallback(SPUrlZone.Default);

                foreach (SPAuthenticationProvider provider in iisSettings.ClaimsAuthenticationProviders)
                {
                    if (provider.GetType() == typeof(SPFormsAuthenticationProvider))
                    {
                        SPFormsAuthenticationProvider formsProvider = provider as SPFormsAuthenticationProvider;

                        string claimIdentifier = ConfigurationManager.AppSettings.Get("ClaimsIdentifier");
                        //site.AllowUnsafeUpdates = true;
                        SPServiceContext serviceContext = SPServiceContext.GetContext(site);

                        UserProfileManager uPM = new UserProfileManager(serviceContext);

                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            if (!uPM.UserExists(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" + 
                                de2.Properties[loginAttribute].Value.ToString()))
                            {
                                PreferredName = (de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value.ToString();
                                WorkEmail = (de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value.ToString();
                                WorkPhone = (de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value.ToString();
                                Department = (de2.Properties[ConfigurationManager.AppSettings["Department"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["Department"]].Value.ToString();
                                Title = (de2.Properties[ConfigurationManager.AppSettings["Title"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["Title"]].Value.ToString();
                                Office = (de2.Properties[ConfigurationManager.AppSettings["Office"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["Office"]].Value.ToString();
                                WebSite = (de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value.ToString();

                                DistinguishedName = de2.Properties["distinguishedName"].Value.ToString();

                                UserProfile newProfile = uPM.CreateUserProfile(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                    de2.Properties[loginAttribute].Value.ToString(), PreferredName);

                                newProfile[PropertyConstants.WorkEmail].Add(WorkEmail);
                                newProfile[PropertyConstants.WorkPhone].Add(WorkPhone);
                                newProfile[PropertyConstants.Department].Add(Department);
                                newProfile[PropertyConstants.Title].Add(Title);
                                newProfile[PropertyConstants.DistinguishedName].Add(DistinguishedName);
                                newProfile[PropertyConstants.Office].Add(Office);
                                newProfile[PropertyConstants.WebSite].Add(WebSite);

                                try
                                {
                                    newProfile.Commit();
                                    Console.WriteLine("Created new profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                    de2.Properties[loginAttribute].Value.ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Failed to create new profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                    de2.Properties[loginAttribute].Value.ToString() + Environment.NewLine + ex.Message);
                                }
                            }
                            else if (uPM.UserExists(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                de2.Properties[loginAttribute].Value.ToString()))
                            {
                                UserProfile updateProfile = uPM.GetUserProfile(claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                    de2.Properties[loginAttribute].Value.ToString());

                                updateProfile["PreferredName"].Value = (de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["PreferredName"]].Value.ToString();
                                updateProfile["WorkEmail"].Value = (de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["WorkEmail"]].Value.ToString();
                                updateProfile["WorkPhone"].Value = (de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["WorkPhone"]].Value.ToString();
                                updateProfile["Department"].Value = (de2.Properties[ConfigurationManager.AppSettings["Department"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["Department"]].Value.ToString();
                                updateProfile["Title"].Value = (de2.Properties[ConfigurationManager.AppSettings["Title"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["Title"]].Value.ToString();
                                updateProfile["Office"].Value = (de2.Properties[ConfigurationManager.AppSettings["Office"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["Office"]].Value.ToString();
                                updateProfile["WebSite"].Value = (de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value == null) ? String.Empty :
                                    de2.Properties[ConfigurationManager.AppSettings["WebSite"]].Value.ToString();

                                try
                                {
                                    updateProfile.Commit();
                                    Console.WriteLine("Updated profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                        de2.Properties[loginAttribute].Value.ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Failed to update profile for " + claimIdentifier + "|" + formsProvider.MembershipProvider + "|" +
                                        de2.Properties[loginAttribute].Value.ToString() + Environment.NewLine + ex.Message);
                                }
                            }
                        });
                    }
 
                }
            }
        }

        public static void Delete()
        {
            //ToDo: find user in AD LDS based on DN, delete profile if DN not found
        }

        public void Timer()
        {
            int timerInterval = 300000;
            try
            {
                timerInterval = (int)TimeSpan.FromMinutes(Convert.ToInt32(ConfigurationManager.AppSettings["ImportTimer"])).TotalMilliseconds;
            }
            catch { }

            timer = new System.Threading.Timer(new TimerCallback(Main), null, 60000, timerInterval);
        }

        private void Main(object obj)
        {		
            PartitionsSection config = (PartitionsSection)ConfigurationManager.GetSection("partitionsSection");
            foreach (Partition partition in config.Partitions)
            {
                string path;
                if (partition.useSSL)
                {
                    path = "LDAPS://" + partition.server + ":" + partition.port + "/" + partition.dn;
                }
                else
                {
                    path = "LDAP://" + partition.server + ":" + partition.port + "/" + partition.dn;
                }

                Console.WriteLine("Binding to {0} with user {1}", path, WindowsIdentity.GetCurrent().Name);

                DirectoryEntry de = new DirectoryEntry();
                de.AuthenticationType = AuthenticationTypes.Secure;

                try
                {
                    de.Path = path;
                    de.RefreshCache();
                    Console.WriteLine("Bound to {0}", path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to bind to {0} with error: " + ex.Message, path);
                }

                DirectorySearcher ds = new DirectorySearcher(de);
                ds.SearchRoot = de;
                ds.SearchScope = SearchScope.Subtree;
                ds.Filter = "(&(objectClass=user))";

                Console.WriteLine("Searching for users...");

                SearchResultCollection results = ds.FindAll();

                if (results.Count > 0)
                {
                    Console.WriteLine("Found {0} users.", results.Count);
                    Create(results,partition.logonAttribute, partition.webApplication);
                }
            }
        }
    }
}