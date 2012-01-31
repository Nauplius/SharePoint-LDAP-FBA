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

using Microsoft.Office.Server.UserProfiles;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint;
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

        public static void Create(SearchResultCollection users, string siteUrl)
        {

            foreach(SearchResult user in users)
            {
                DirectoryEntry de2 = user.GetDirectoryEntry();
                SPSite site = new SPSite(siteUrl);
                site.AllowUnsafeUpdates = true;
                SPServiceContext serviceContext = SPServiceContext.GetContext(site);

                UserProfileManager uPM = new UserProfileManager(serviceContext);
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    if (!uPM.UserExists("i:0#.f|fabrikammember|" + de2.Properties["mail"].Value.ToString()))
                    {
                        AccountName = de2.Properties["mail"].Value.ToString();
                        PreferredName = de2.Properties["displayName"].Value.ToString();
                        WorkPhone = de2.Properties["telephoneNumber"].Value.ToString();
                        WorkEmail = de2.Properties["mail"].Value.ToString();
                        //Department = de2.Properties["department"].Value.ToString();
                        //Title = de2.Properties["title"].Value.ToString();
                        DistinguishedName = de2.Path;
                        //Office = de2.Properties["physicalDeliveryOfficeName"].Value.ToString();

                        UserProfile newProfile = uPM.CreateUserProfile("i:0#.f|fabrikammember|" + de2.Properties["mail"].Value.ToString(), PreferredName);
                        newProfile[PropertyConstants.WorkPhone].Add(WorkPhone);
                        newProfile[PropertyConstants.WorkEmail].Add(WorkEmail);
                        //newProfile[PropertyConstants.Department].Add(Department);
                        //newProfile[PropertyConstants.Title].Add(Title);
                        newProfile[PropertyConstants.DistinguishedName].Add(DistinguishedName);
                        //newProfile[PropertyConstants.Office].Add(Office);
                        newProfile.Commit();
                    }
                    else if (uPM.UserExists("i:0#.f|fabrikammember|" + de2.Properties["mail"].Value.ToString()))
                    {
                        UserProfile updateProfile = uPM.GetUserProfile("i:0#.f|fabrikammember|" + de2.Properties["mail"].Value.ToString());
                        updateProfile.DisplayName = de2.Properties["displayName"].Value.ToString();
                        updateProfile[PropertyConstants.WorkPhone].Add(WorkPhone);
                        updateProfile.Commit();
                    }
                });
            }
        }

        static void Main()
        {

            NameValueCollection keys;
            keys = ConfigurationManager.AppSettings;

            foreach (string k in keys.AllKeys)
            {
                string path = k;
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
                    Create(results, keys.Get(k));
                }
            }


            //Define the AD LDS connection
            /*
            string path = "LDAP://adlds01:389/CN=SharePoint,DC=fabrikam,DC=local";
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
                Console.WriteLine("Failed to bind to {0}", path);
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
                Create(results);
            }
             */
        }
    }
}
