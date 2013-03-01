using System;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Net.Sockets;
using System.Web.Security;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;


namespace Nauplius.ADLDS.Provider
{
    public sealed class LdapMembership : MembershipProvider
    {
        public string memProvider = null;

        private bool ValidateServer()
        {
            Socket socket = null;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
            IAsyncResult result = socket.BeginConnect(LdapMembershipManager.Server, LdapMembershipManager.Port, null, null);
            bool connected = result.AsyncWaitHandle.WaitOne(200, true);
            return connected;
        }


        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (ValidateServer())
            {
                var directoryEntry = Connect();

                var directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = String.Format("(&(&(&(ObjectClass={0})({1}=*{2}*))))", LdapMembershipManager.UserObjectClass,
                                                         LdapMembershipManager.UserNameAttribute,
                                                         providerUserKey);
                directorySearcher.SearchScope = LdapMembershipManager.Scope;
                directorySearcher.PageSize = 10000;

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user;
                }
            }

            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (ValidateServer())
            {
                var directoryEntry = Connect();

                var directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = String.Format("(&(&(&(ObjectClass={0})({1}=*{2}*))))", LdapMembershipManager.UserObjectClass,
                                                         LdapMembershipManager.UserNameAttribute,
                                                         username);
                directorySearcher.SearchScope = LdapMembershipManager.Scope;
                directorySearcher.PageSize = 10000;

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user;
                }
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            if (ValidateServer())
            {
                var directoryEntry = Connect();

                var directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = String.Format("(&(&(&(ObjectClass={0})({1}={2}))))", LdapMembershipManager.UserObjectClass,
                                                         LdapMembershipManager.UserNameAttribute,
                                                         email);

                directorySearcher.SearchScope = LdapMembershipManager.Scope;
                directorySearcher.PageSize = 10000;

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user.UserName;
                }
            }

            return null;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            if (ValidateServer())
            {
                var directoryEntry = Connect();

                var directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = String.Format("(&(&(&(ObjectClass={0})({1}=*))))", LdapMembershipManager.UserObjectClass,
                                                         LdapMembershipManager.UserNameAttribute);
                directorySearcher.SearchScope = LdapMembershipManager.Scope;
                directorySearcher.PageSize = pageSize;

                SearchResultCollection results = directorySearcher.FindAll();

                {
                    totalRecords = results.Count;

                    if (pageSize == Int32.MaxValue)
                    {
                        pageSize = totalRecords;
                    }

                    var i = pageIndex*pageSize;

                    for (var n = i; (n < (i + pageSize)) && (n < totalRecords); n++)
                    {
                        users.Add(GetUserFromSearchResult(results[n].GetDirectoryEntry()));
                    }
                }
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            if (ValidateServer())
            {
                var directoryEntry = Connect();

                var directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = String.Format("(&(&(&(ObjectClass={0})({1}=*{2}*))))", LdapMembershipManager.UserObjectClass,
                                                         LdapMembershipManager.UserNameAttribute, usernameToMatch);
                directorySearcher.SearchScope = LdapMembershipManager.Scope;
                directorySearcher.PageSize = pageSize;
                SearchResultCollection results = directorySearcher.FindAll();
                totalRecords = results.Count;

                {
                    totalRecords = results.Count;

                    if (pageSize == Int32.MaxValue)
                    {
                        pageSize = totalRecords;
                    }

                    var i = pageIndex * pageSize;

                    for (var n = i; (n < (i + pageSize)) && (n < totalRecords); n++)
                    {
                        users.Add(GetUserFromSearchResult(results[n].GetDirectoryEntry()));
                    }
                }
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            if (ValidateServer())
            {
                var directoryEntry = Connect();

                var directorySearcher = new DirectorySearcher(directoryEntry);
                directorySearcher.Filter = String.Format("(&(&(&(ObjectClass={0})({1}=*{2}*))))", LdapMembershipManager.UserObjectClass,
                                                         LdapMembershipManager.UserNameAttribute, emailToMatch);
                directorySearcher.SearchScope = LdapMembershipManager.Scope;
                directorySearcher.PageSize = pageSize;
                SearchResultCollection results = directorySearcher.FindAll();
                totalRecords = results.Count;

                {
                    totalRecords = results.Count;

                    if (pageSize == Int32.MaxValue)
                    {
                        pageSize = totalRecords;
                    }

                    var i = pageIndex * pageSize;

                    for (var n = i; (n < (i + pageSize)) && (n < totalRecords); n++)
                    {
                        users.Add(GetUserFromSearchResult(results[n].GetDirectoryEntry()));
                    }
                }
            }

            return users;
        }

        public override bool ValidateUser(string username, string password)
        {
            XmlNode membershipProvider = new XmlDocument();

            string path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            membershipProvider =
                xmlDocument.SelectSingleNode((String.Format("configuration/system.web/membership/providers/add[@name='{0}']", this.Name
                                                            )));

            bool isValid = false;



            using (var pc = new PrincipalContext(ContextType.ApplicationDirectory,
                                                 String.Format("{0}:{1}", membershipProvider.Attributes["server"].Value,
                                                               membershipProvider.Attributes["port"].Value), membershipProvider.Attributes["userContainer"].Value, ContextOptions.SimpleBind))
            {

                isValid = pc.ValidateCredentials(username, password, ContextOptions.SimpleBind);
            }

            if (isValid)
            {
                return isValid;
            }

            return isValid;
        }

        private MembershipUser GetUserFromSearchResult(DirectoryEntry result)
        {
            object providerUserKey = result.Path;
            string userName = result.Properties[LdapMembershipManager.UserNameAttribute].Value.ToString();

            var user = new MembershipUser(Name, userName, providerUserKey,
                null, null, null, true, false, DateTime.UtcNow,
                DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow,
                DateTime.UtcNow);

            return user;
        }

        private DirectoryEntry Connect()
        {
            string ldapPath = string.Empty;

            ldapPath = LdapMembershipManager.UseSSL ? "LDAPS://" : "LDAP://";
            ldapPath = ldapPath + String.Format("{0}:{1}/{2}", LdapMembershipManager.Server,
                                                LdapMembershipManager.Port, LdapMembershipManager.UserContainer);

            var directoryEntry = new DirectoryEntry(ldapPath.ToUpper());

            if (LdapMembershipManager.UserName != string.Empty && LdapMembershipManager.Password != string.Empty)
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.None;
                directoryEntry.Username = LdapMembershipManager.UserName;
                directoryEntry.Password = LdapMembershipManager.Password;
            }
            else
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.Secure;
            }

            return directoryEntry;
        }

#region NotImplemented
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer,
                                                  bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion,
                                                             string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }


        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { throw new NotImplementedException(); }
        }

        public override string ApplicationName { get; set; }

        public override int MaxInvalidPasswordAttempts
        {
            get { throw new NotImplementedException(); }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override bool RequiresUniqueEmail
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredPasswordLength
        {
            get { throw new NotImplementedException(); }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { throw new NotImplementedException(); }
        }
#endregion
    }

    public sealed class LdapRole : RoleProvider
    {
        public override string[] GetRolesForUser(string username)
        {
            string ldapPath = string.Empty;

            XmlNode roleProvider = new XmlDocument();

            string path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            roleProvider =
                xmlDocument.SelectSingleNode((String.Format("configuration/system.web/roleManager/providers/add[@name='{0}']", this.Name
                                                            )));

            ldapPath = Convert.ToBoolean(roleProvider.Attributes["useSSL"].Value) ? "LDAPS://" : "LDAP://";
            ldapPath = ldapPath +
                       String.Format("{0}:{1}/{2}", roleProvider.Attributes["server"].Value,
                                     roleProvider.Attributes["port"].Value,
                                     roleProvider.Attributes["groupContainer"].Value);

            var directoryEntry = new DirectoryEntry(ldapPath.ToUpper());
            var directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = String.Format("(&(&(&(ObjectClass=user)({0}=*{1}*))))",
                                                     roleProvider.Attributes["userNameAttribute"].Value,
                                                     username);

            directorySearcher.SearchScope = SearchScope.Subtree;
            directorySearcher.PageSize = 10000;

            var result = directorySearcher.FindOne();

            if (result != null)
            {
                var roles = new string[result.GetDirectoryEntry().Properties["memberof"].Count];
                
                for (var i = 0; i < result.GetDirectoryEntry().Properties["memberof"].Count; i++)
                {
                //ToDo: Correct
                    string groupName = result.Properties["memberof"][i].ToString();
                    roles[i] = groupName.Replace(',', '.');
                }

                return roles;
            }

            return null;
        }

        public override bool RoleExists(string roleName)
        {
            var directoryEntry = Connect();

            var directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = String.Format("(&(&(&(ObjectClass=group)({0}={1}))))",
                                                     LdapRoleManager.GroupNameAttribute, roleName);
            directorySearcher.SearchScope = LdapRoleManager.Scope;
            directorySearcher.PageSize = 10000;

            var result = directorySearcher.FindOne();

            if (result != null)
            {
                return true;
            }

            return false;
        }

        private MembershipUser GetUserFromSearchResult(DirectoryEntry result)
        {
            object providerUserKey = result.Path;
            string userName = result.Properties[LdapMembershipManager.UserNameAttribute].Value.ToString();

            var user = new MembershipUser(Name, userName, providerUserKey,
                null, null, null, true, false, DateTime.UtcNow,
                DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow,
                DateTime.UtcNow);

            return user;
        }

        private DirectoryEntry ConnectMembership()
        {
            string ldapPath = string.Empty;

            ldapPath = LdapMembershipManager.UseSSL ? "LDAPS://" : "LDAP://";
            ldapPath = ldapPath + String.Format("{0}:{1}/{2}", LdapMembershipManager.Server,
                                                LdapMembershipManager.Port, LdapMembershipManager.UserContainer);

            var directoryEntry = new DirectoryEntry(ldapPath.ToUpper());

            if (LdapMembershipManager.UserName != string.Empty && LdapMembershipManager.Password != string.Empty)
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.None;
                directoryEntry.Username = LdapMembershipManager.UserName;
                directoryEntry.Password = LdapMembershipManager.Password;
            }
            else
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.Secure;
            }

            return directoryEntry;
        }

        private DirectoryEntry Connect()
        {
            string ldapPath = string.Empty;

            ldapPath = LdapRoleManager.UseSSL ? "LDAPS://" : "LDAP://";
            ldapPath = ldapPath + String.Format("{0}:{1}/{2}", LdapRoleManager.Server,
                                                LdapRoleManager.Port, LdapRoleManager.GroupContainer);

            var directoryEntry = new DirectoryEntry(ldapPath.ToUpper());

            if (LdapRoleManager.UserName != string.Empty && LdapRoleManager.Password != string.Empty)
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.None;
                directoryEntry.Username = LdapRoleManager.UserName;
                directoryEntry.Password = LdapRoleManager.Password;
            }
            else
            {
                directoryEntry.AuthenticationType = AuthenticationTypes.Secure;
            }

            return directoryEntry;
        }

        #region NotImplemented

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
            /*
            var directoryEntry = Connect();

            var directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = String.Format("(&(&(&(ObjectClass={0})({1}={2}))))", LdapMembershipManager.UserObjectClass,
                                                     LdapRoleManager.GroupNameAttribute,
                                                     roleName);

            directorySearcher.SearchScope = LdapMembershipManager.Scope;
            directorySearcher.PageSize = 10000;

            var result = directorySearcher.FindOne();

            if (result != null)
            {
                if (result.Properties["members"].Contains(username))
                {
                    return true;
                }
            }

            return false;
             */
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }

        #endregion
    }

    class LdapMembershipManager
    {
        public static XmlNode MembershipProviderNode()
        {
            XmlNode membershipProvider = new XmlDocument();

            if (SPContext.Current == null)
            {           }
            else
            {
                var webApp = SPContext.Current.Web.Site.WebApplication;
                var zone = SPContext.Current.Site.Zone;
                var settings = webApp.IisSettings[zone];
                DirectoryInfo directoryInfo = settings.Path;
                var webConfig = directoryInfo.FullName + "\\web.config";
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(webConfig);

                membershipProvider =
                    xmlDocument.SelectSingleNode((String.Format("configuration/system.web/membership/providers/add[@name='{0}']",
                                                                settings.FormsClaimsAuthenticationProvider.MembershipProvider)));
            }
            return membershipProvider;
        }

        public static string Server
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _server = membershipProvider.Attributes["server"].Value;
                return _server;
            }
        }

        public static int Port
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _port = membershipProvider.Attributes["port"].Value;
                return Convert.ToInt32(_port);
            }
        }

        public static bool UseSSL
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _useSsl = membershipProvider.Attributes["useSSL"].Value;
                return Convert.ToBoolean(_useSsl);
            }
        }

        public static string UserDNAttribute
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _userDnAttribute = membershipProvider.Attributes["userDNAttribute"].Value;
                return _userDnAttribute;
            }
        }

        public static bool UseDNAttribute
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _useDnAttribute = membershipProvider.Attributes["useDNAttribute"].Value;
                return Convert.ToBoolean(_useDnAttribute);
            }
        }


        public static string UserNameAttribute
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _userNameAttribute = membershipProvider.Attributes["userNameAttribute"].Value;
                return _userNameAttribute;
            }
        }

        public static string UserContainer
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _userContainer = membershipProvider.Attributes["userContainer"].Value;
                return _userContainer;
            }
        }

        public static string UserObjectClass
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _userObjectClass = membershipProvider.Attributes["userObjectClass"].Value;
                return _userObjectClass;
            }
        }

        public static string UserFilter
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _userFilter = membershipProvider.Attributes["userFilter"].Value;
                return _userFilter;
            }
        }

        public static SearchScope Scope
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _scope = membershipProvider.Attributes["scope"].Value;

                  if (_scope == "Base")
                  {
                      return SearchScope.Base;
                  }
                  if (_scope == "OneLevel")
                    {
                        return SearchScope.OneLevel;
                    }
                if (_scope == "Subtree")
                {
                    return SearchScope.Subtree;
                }
                return SearchScope.Subtree;
            }
        }

        public static string OtherRequiredUserAttributes
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _otherRequiredUserAttributes =
                    membershipProvider.Attributes["otherRequiredUserAttributes"].Value;
                return _otherRequiredUserAttributes;
            }
        }

        public static string UserName
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _userName = string.Empty;
                try
                {
                    _userName = membershipProvider.Attributes["Username"].Value;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                }  
                return _userName;
            }
        }

        public static string Password
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _password = string.Empty;
                try
                {
                    _password = membershipProvider.Attributes["password"].Value;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                }    
                return _password;
            }
        }
    }

    class LdapRoleManager
    {
        public static XmlNode RoleProviderNode()
        {
            XmlNode roleProvider = new XmlDocument();

            if (SPContext.Current == null)
            { }
            else
            {
                var webApp = SPContext.Current.Web.Site.WebApplication;
                var zone = SPContext.Current.Site.Zone;
                var settings = webApp.IisSettings[zone];
                DirectoryInfo directoryInfo = settings.Path;
                var webConfig = directoryInfo.FullName + "\\web.config";
                var xmlDocument = new XmlDocument();
                xmlDocument.Load(webConfig);

                roleProvider =
                    xmlDocument.SelectSingleNode((String.Format("configuration/system.web/roleManager/providers/add[@name='{0}']",
                                                                settings.FormsClaimsAuthenticationProvider.RoleProvider)));
            }
            return roleProvider;
        }

                public static string Server
        {
            get
            {
                var roleProvider = RoleProviderNode();
                var _server = roleProvider.Attributes["server"].Value;
                return _server;
            }
        }

        public static int Port
        {
            get
            {
                var roleProvider = RoleProviderNode();
                var _port = roleProvider.Attributes["port"].Value;
                return Convert.ToInt32(_port);
            }
        }

        public static bool UseSSL
        {
            get 
            {
                var roleProvider = RoleProviderNode();
                var _useSsl = roleProvider.Attributes["useSSL"].Value;
                return Convert.ToBoolean(_useSsl);
            }
        }

        public static string GroupNameAttribute
        {
            get { var roleProvider = RoleProviderNode();
                var _groupNameAttribute = roleProvider.Attributes["groupNameAttribute"].Value;
                return _groupNameAttribute;
            }
        }

        public static string GroupContainer
        {
            get { var roleProvider = RoleProviderNode();
                var _groupContainer = roleProvider.Attributes["groupContainer"].Value;
                return _groupContainer;
            }
        }

        public static string GroupMemberAttribute
        {
            get { var roleProvider = RoleProviderNode();
                var _groupMemberAttribute = roleProvider.Attributes["groupMemberAttribute"].Value;
                return _groupMemberAttribute;
            }
        }

        public static string UserNameAttribute
        {
            get { var roleProvider = RoleProviderNode();
                var _userNameAttribute = roleProvider.Attributes["userNameAttribute"].Value;
                return _userNameAttribute;
            }
        }

        public static string DnAttribute
        {
            get { var roleProvider = RoleProviderNode();
                var _dnAttribute = roleProvider.Attributes["dnAttribute"].Value;
                return _dnAttribute;
            }
        }

        public static bool UseUserDnAttribute
        {
            get { var roleProvider = RoleProviderNode();
                var _useUserDnAttribute = roleProvider.Attributes["useUserDNAttribute"].Value;
                return Convert.ToBoolean(_useUserDnAttribute);
            }
        }

        public static SearchScope Scope
        {
            get 
            {
                var roleProvider = RoleProviderNode();
                var _scope = roleProvider.Attributes["scope"].Value;

                  if (_scope == "Base")
                  {
                      return SearchScope.Base;
                  }
                  if (_scope == "OneLevel")
                    {
                        return SearchScope.OneLevel;
                    }
                if (_scope == "Subtree")
                {
                    return SearchScope.Subtree;
                }
                return SearchScope.Subtree;
            }
        }

        public static string UserFilter
        {
            get { var roleProvider = RoleProviderNode();
                var _userFilter = roleProvider.Attributes["userFilter"].Value;
                return _userFilter;
            }
        }

        public static string GroupFilter
        {
            get { var roleProvider = RoleProviderNode();
                var _groupFilter = roleProvider.Attributes["groupFilter"].Value;
                return _groupFilter;
            }
        }

        public static string UserName
        {
            get
            {
                var roleProvider = RoleProviderNode();
                var _userName = string.Empty;
                try
                {
                    _userName = roleProvider.Attributes["Username"].Value;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                }
                return _userName;
            }
        }

        public static string Password
        {
            get
            {
                var roleProvider = RoleProviderNode();
                var _password = string.Empty;
                try
                {
                    _password = roleProvider.Attributes["password"].Value;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                }
                return _password;
            }
        }
    }
}
  