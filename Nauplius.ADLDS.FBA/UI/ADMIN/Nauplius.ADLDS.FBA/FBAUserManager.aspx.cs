using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.Security;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;
using System.DirectoryServices;
using System.Web.Configuration;
using System.DirectoryServices.AccountManagement;
using Sync;

namespace UI.ADMIN.Nauplius.ADLDS.FBA
{
    public partial class FBAUserManager : LayoutsPageBase
    {
        public static string AccountNameAttrib;
        public static string DepartmentAttrib;
        public static string FirstNameAttrib;
        public static string LastNameAttrib;
        public static string OfficeAttrib;
        public static string PreferredNameAttrib;
        public static string UserTitleAttrib;
        public static string WebSiteAttrib;
        public static string WorkEmailAttrib;
        public static string WorkPhoneAttrib;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void WebAppSelector_OnChanged(object sender, EventArgs e)
        {
            string providerName;
            string server, dn, loginAttrib;
            bool useSsl;
            int port = 0x185;
            ProviderSettings providerSettings;
            SPWebApplication selectedWebApp = ddlWebApp.CurrentItem;
            var zone = GetZone(ddlZonePicker.SelectedValue);

            foreach (SPFormsAuthenticationProvider membershipProvider in selectedWebApp.GetIisSettingsWithFallback(zone).ClaimsAuthenticationProviders.OfType<SPFormsAuthenticationProvider>())
            {
                providerName = membershipProvider.DisplayName;
                providerSettings = GetMembershipProvider(selectedWebApp, zone, providerName);

                if (providerSettings == null) break;
                server = providerSettings.Parameters["server"];
                port = Convert.ToInt32(providerSettings.Parameters["port"]);
                loginAttrib = providerSettings.Parameters["userNameAttribute"];
                dn = providerSettings.Parameters["userContainer"];
                useSsl = Convert.ToBoolean(providerSettings.Parameters["useSSL"]);

                var de = DirEntry(server, port, dn, useSsl);


                if (de != null)
                {
                    var results = ResultCollection(de);
                }               
            }
        }

        private void AttributeMapping()
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
                }
            }
        }


        public static
            ProviderSettings GetMembershipProvider(SPWebApplication webApplication, SPUrlZone zone, string providerName)
        {
            ConfigurationSection section;
            var manager = WebConfigurationManager.OpenWebConfiguration("/", webApplication.Name);
            var membershipSection = (MembershipSection)manager.GetSection("configuration/system.web/membership");
            var providerSettings = membershipSection.Providers[providerName];
            return providerSettings;
        }

        protected void ZoneSelector_OnLoad(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            var urlZones = new Dictionary<SPUrlZone, string>
                {
                    {SPUrlZone.Default, "Default"},
                    {SPUrlZone.Intranet, "Intranet"},
                    {SPUrlZone.Internet, "Internet"},
                    {SPUrlZone.Extranet, "Extranet"},
                    {SPUrlZone.Custom, "Custom"}
                };

            ddlZonePicker.DataSource = urlZones;
            ddlZonePicker.DataTextField = "Value";
            ddlZonePicker.DataValueField = "Key";
            ddlZonePicker.DataBind();
        }

        protected SPUrlZone GetZone(string zone)
        {
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

        private static DirectoryEntry DirEntry(string serverName, int serverPort, string distinguishedName, bool useSSL)
        {
            DirectoryEntry de = new DirectoryEntry();
            string path = "LDAP://" + serverName + ":" + serverPort + "/" + distinguishedName;

            Logging.LogMessage(220, Logging.LogCategories.LDAP, TraceSeverity.Verbose, "Binding to " +
                path, new object[] { null });

            if (useSSL)
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
    }
}
