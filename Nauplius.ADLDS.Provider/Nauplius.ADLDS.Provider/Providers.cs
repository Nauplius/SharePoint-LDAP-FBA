using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Web.Security;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using SearchScope = System.DirectoryServices.SearchScope;


namespace Nauplius.ADLDS.Provider
{
    public sealed class LdapMembership : MembershipProvider
    {
        public string memProvider = null;

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (LdapManager.ValidateServer(LdapMembershipManager.Server, LdapMembershipManager.Port))
            {
                var directoryEntry = LdapManager.Connect(LdapMembershipManager.Server, LdapMembershipManager.Port,
                                                         LdapMembershipManager.UseSSL,
                                                         LdapMembershipManager.UserContainer,
                                                         LdapMembershipManager.UserName, LdapMembershipManager.Password);

                var directorySearcher = new DirectorySearcher(directoryEntry)
                                            {
                                                Filter =
                                                    String.Format("(&(&(&(ObjectClass={0})({1}=*{2}*))))",
                                                                  LdapMembershipManager.UserObjectClass,
                                                                  LdapMembershipManager.UserNameAttribute,
                                                                  providerUserKey),
                                                SearchScope = LdapMembershipManager.Scope
                                            };

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user;
                }
            }
            else
            {
                throw new ActiveDirectoryServerDownException();
            }

            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (LdapManager.ValidateServer(LdapMembershipManager.Server, LdapMembershipManager.Port))
            {
                var directoryEntry = LdapManager.Connect(LdapMembershipManager.Server, LdapMembershipManager.Port,
                                                         LdapMembershipManager.UseSSL,
                                                         LdapMembershipManager.UserContainer,
                                                         LdapMembershipManager.UserName, LdapMembershipManager.Password);

                var directorySearcher = new DirectorySearcher(directoryEntry)
                                            {
                                                Filter =
                                                    String.Format("(&(&(&(ObjectClass={0})({1}=*{2}*))))",
                                                                  LdapMembershipManager.UserObjectClass,
                                                                  LdapMembershipManager.UserNameAttribute,
                                                                  username),
                                                SearchScope = LdapMembershipManager.Scope
                                            };

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user;
                }
            }
            else
            {
                throw new ActiveDirectoryServerDownException();
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            if (LdapManager.ValidateServer(LdapMembershipManager.Server, LdapMembershipManager.Port))
            {
                var directoryEntry = LdapManager.Connect(LdapMembershipManager.Server, LdapMembershipManager.Port,
                                                         LdapMembershipManager.UseSSL,
                                                         LdapMembershipManager.UserContainer,
                                                         LdapMembershipManager.UserName, LdapMembershipManager.Password);

                var directorySearcher = new DirectorySearcher(directoryEntry)
                                            {
                                                Filter =
                                                    String.Format(
                                                        "(&(&(&(ObjectClass={0})(mail={1}))))",
                                                        LdapMembershipManager
                                                            .UserObjectClass, email),
                                                SearchScope =
                                                    LdapMembershipManager.Scope
                                            };

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user.UserName;
                }
            }
            else
            {
                throw new ActiveDirectoryServerDownException();
            }

            return null;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;
            if (LdapManager.ValidateServer(LdapMembershipManager.Server, LdapMembershipManager.Port))
            {
                var directoryEntry = LdapManager.Connect(LdapMembershipManager.Server, LdapMembershipManager.Port,
                                                         LdapMembershipManager.UseSSL,
                                                         LdapMembershipManager.UserContainer,
                                                         LdapMembershipManager.UserName, LdapMembershipManager.Password);

                var directorySearcher = new DirectorySearcher(directoryEntry)
                                            {
                                                Filter =
                                                    String.Format("(&(&(&(ObjectClass={0})({1}=*))))",
                                                                  LdapMembershipManager.UserObjectClass,
                                                                  LdapMembershipManager.UserNameAttribute),
                                                SearchScope = LdapMembershipManager.Scope,
                                                PageSize = pageSize
                                            };

                var results = directorySearcher.FindAll();

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
            else
            {
                throw new ActiveDirectoryServerDownException();
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            if (LdapManager.ValidateServer(LdapMembershipManager.Server, LdapMembershipManager.Port))
            {
                var directoryEntry = LdapManager.Connect(LdapMembershipManager.Server, LdapMembershipManager.Port,
                                                         LdapMembershipManager.UseSSL,
                                                         LdapMembershipManager.UserContainer,
                                                         LdapMembershipManager.UserName, LdapMembershipManager.Password);

                var directorySearcher = new DirectorySearcher(directoryEntry)
                                            {
                                                Filter =
                                                    String.Format("(&(&(&(ObjectClass={0})({1}=*{2}*))))",
                                                                  LdapMembershipManager.UserObjectClass,
                                                                  LdapMembershipManager.UserNameAttribute,
                                                                  usernameToMatch),
                                                SearchScope = LdapMembershipManager.Scope,
                                                PageSize = pageSize
                                            };
                var results = directorySearcher.FindAll();
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
            else
            {
                throw new ActiveDirectoryServerDownException();
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            if (LdapManager.ValidateServer(LdapMembershipManager.Server, LdapMembershipManager.Port))
            {
                var directoryEntry = LdapManager.Connect(LdapMembershipManager.Server, LdapMembershipManager.Port,
                                                         LdapMembershipManager.UseSSL,
                                                         LdapMembershipManager.UserContainer,
                                                         LdapMembershipManager.UserName, LdapMembershipManager.Password);

                var directorySearcher = new DirectorySearcher(directoryEntry)
                                            {
                                                Filter =
                                                    String.Format(
                                                        "(&(&(&(ObjectClass={0})(mail=*{1}*))))",
                                                        LdapMembershipManager
                                                            .UserObjectClass,
                                                        emailToMatch),
                                                SearchScope =
                                                    LdapMembershipManager.Scope,
                                                PageSize = pageSize
                                            };
                var results = directorySearcher.FindAll();
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
            else
            {
                throw new ActiveDirectoryServerDownException();
            }

            return users;
        }

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            string _server;
            var _port = 389;
            var _useSSL = false;
            var _path = string.Empty;
            var _username = string.Empty;
            var _password = string.Empty;
            var _userNameAttribute = string.Empty;
            var _scope = new SearchScope();

            var directoryEntry = StsManager.ProviderNode(Name, true, out _server, out _port, out _useSSL, out _path, out _username,
                                                         out _password, out _userNameAttribute, out _scope);

            var connection = new LdapConnection(String.Format("{0}:{1}", _server, _port));

            var credential = new NetworkCredential(username, password);
            connection.AuthType = AuthType.Basic;

            if (_useSSL)
            {
                connection.SessionOptions.SecureSocketLayer = true;
            }
            else
            {
                connection.SessionOptions.Signing = true;
                connection.SessionOptions.Sealing = true;                
            }

            try
            {
                connection.Bind(credential);
                isValid = true;
            }
            catch (Exception)
            {
                //No result code mapping available
                isValid = false;
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
            var userRoles = new List<string>();
            var _server = string.Empty;
            var _port = 389;
            var _useSSL = false;
            var _path = string.Empty;
            var _username = string.Empty;
            var _password = string.Empty;
            var _userNameAttribute = string.Empty;
            var _scope = new SearchScope();

            var directoryEntry = StsManager.ProviderNode(Name, false, out _server, out _port, out _useSSL, 
                out _path, out _username, out _password, out _userNameAttribute, out _scope);

            var directorySearcher = new DirectorySearcher(directoryEntry)
            {
                Filter = String.Format("(&(&(&(ObjectClass=user)({0}={1}))))",
                                        _userNameAttribute,
                                        username),
                SearchScope = _scope
            };

            var results = directorySearcher.FindAll();

            foreach (SearchResult result in results)
            {
                var roleName = result.GetDirectoryEntry();
                userRoles.Add(roleName.Properties["distinguishedName"].Value.ToString());
            }

            return userRoles.ToArray();
        }

        public override bool RoleExists(string roleName)
        {         
            var directoryEntry = LdapManager.Connect(LdapRoleManager.Server, LdapRoleManager.Port, 
                LdapRoleManager.UseSSL, LdapRoleManager.GroupContainer,
                LdapRoleManager.UserName, LdapRoleManager.Password);

            var directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = String.Format("(&(&(&(ObjectClass=group)({0}={1}))))",
                                                     LdapRoleManager.GroupNameAttribute, roleName);
            directorySearcher.SearchScope = LdapRoleManager.Scope;

            var result = directorySearcher.FindAll();

            if (result.Count > 0)
            {
                return true;
            }
            
            return false;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            var users = new List<string>();

            var directoryEntry = LdapManager.Connect(LdapRoleManager.Server, LdapRoleManager.Port,
                LdapRoleManager.UseSSL, LdapRoleManager.GroupContainer,
                LdapRoleManager.UserName, LdapRoleManager.Password);

            var directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = String.Format("(&(&(&(ObjectClass=group)({0}={1}))))",
                                                     LdapRoleManager.GroupNameAttribute, roleName);
            directorySearcher.SearchScope = LdapRoleManager.Scope;

            var result = directorySearcher.FindOne();

            if (result != null)
            {
                foreach (DirectoryEntry user in result.Properties["memberof"])
                {
                    users.Add(user.Properties[LdapMembershipManager.UserName].Value.ToString());
                }
                return users.ToArray();
            }

            return null;
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

        #region NotImplemented

        public override bool IsUserInRole(string username, string roleName)
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

        public override string[] GetAllRoles()
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

            if (SPContext.Current != null)
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

                return membershipProvider;
            }

            return null;
        }

        public static string Server
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _server = "localhost";

                try
                {
                    _server = (membershipProvider.Attributes["server"].Value == null) ? "localhost" :
                        membershipProvider.Attributes["server"].Value;
                    return _server;
                }
                catch (NullReferenceException)
                {
                    return _server;
                }
            }
        }

        public static int Port
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _port = "389";

                try
                {
                    _port = (membershipProvider.Attributes["port"].Value == null) ? "389" :
                        membershipProvider.Attributes["port"].Value;
                    return Convert.ToInt32(_port);
                }
                catch (NullReferenceException)
                {
                    return Convert.ToInt32(_port);
                }

            }
        }

        public static bool UseSSL
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _useSsl = false;

                try
                {
                    _useSsl = (membershipProvider.Attributes["useSSL"].Value == null) ? false : 
                        membershipProvider.Attributes["useSSL"].Value == null;
                    return Convert.ToBoolean(_useSsl);
                }
                catch (NullReferenceException)
                {
                    return Convert.ToBoolean(_useSsl);
                }
            }
        }

        public static string UserDNAttribute
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _userDnAttribute = "distinguishedName";

                try
                {
                    _userDnAttribute = (membershipProvider.Attributes["userDNAttribute"].Value == null) ? "distinguishedName" : 
                        membershipProvider.Attributes["userDNAttribute"].Value;
                    return _userDnAttribute;
                }
                catch (NullReferenceException)
                {
                    return _userDnAttribute;
                }
            }
        }

        public static bool UseDNAttribute
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _useDnAttribute = "true";

                try
                {
                    _useDnAttribute = (membershipProvider.Attributes["useDNAttribute"].Value == null) ? "true" : 
                        membershipProvider.Attributes["useDNAttribute"].Value;
                    return Convert.ToBoolean(_useDnAttribute);
                }
                catch (NullReferenceException)
                {
                    return Convert.ToBoolean(_useDnAttribute);
                }
            }
        }


        public static string UserNameAttribute
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _userNameAttribute = "userPrincipalName";

                try
                {
                    _userNameAttribute = (membershipProvider.Attributes["userNameAttribute"].Value == null) ? "userPrincipalName" :
                        membershipProvider.Attributes["userNameAttribute"].Value;
                    return _userNameAttribute;
                }
                catch (NullReferenceException)
                {
                    return _userNameAttribute;
                }
            }
        }

        public static string UserContainer
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();
                var _userContainer = string.Empty;

                try
                {
                    _userContainer = (membershipProvider.Attributes["userContainer"].Value == null) ? string.Empty :
                        membershipProvider.Attributes["userContainer"].Value;
                    return _userContainer;
                }
                catch (NullReferenceException)
                {
                    return _userContainer;
                }
            }
        }

        public static string UserObjectClass
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _userObjectClass = "person";

                try
                {
                    _userObjectClass = (membershipProvider.Attributes["userObjectClass"].Value == null) ? "person" :
                        membershipProvider.Attributes["userObjectClass"].Value;
                    return _userObjectClass;
                }
                catch (NullReferenceException)
                {
                    return _userObjectClass;
                }
            }
        }

        public static string UserFilter
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _userFilter = @"(ObjectClass=*)";

                try
                {
                    _userFilter = (membershipProvider.Attributes["userFilter"].Value == null) ? @"(ObjectClass=*)" :
                        membershipProvider.Attributes["userFilter"].Value;
                    return _userFilter;
                }
                catch (NullReferenceException)
                {
                    return _userFilter;
                }
            }
        }

        public static SearchScope Scope
        {
            get 
            {
                var membershipProvider = MembershipProviderNode();

                try
                {
                    var _scope = (membershipProvider.Attributes["scope"].Value.ToUpper() == null) ? "SUBTREE" :
                        membershipProvider.Attributes["scope"].Value.ToUpper();
                    if (_scope == "BASE")
                    {
                        return SearchScope.Base;
                    }
                    if (_scope == "ONELEVEL")
                    {
                        return SearchScope.OneLevel;
                    }
                    if (_scope == "SUBTREE")
                    {
                        return SearchScope.Subtree;
                    }
                    return SearchScope.Subtree;
                }
                catch (NullReferenceException)
                {
                    return SearchScope.Subtree;
                }
            }
        }

        public static string OtherRequiredUserAttributes
        {
            get
            {
                var membershipProvider = MembershipProviderNode();
                var _otherRequiredUserAttributes = "sn,givenname,cn";

                try
                {
                    _otherRequiredUserAttributes =
                        (membershipProvider.Attributes["otherRequiredUserAttributes"].Value == null)
                            ? "sn,givenname,cn"
                            : membershipProvider.Attributes["otherRequiredUserAttributes"].Value;
                    return _otherRequiredUserAttributes;
                }
                catch (NullReferenceException)
                {
                    return _otherRequiredUserAttributes;
                }
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
                    return _userName;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                    return _userName;
                }  
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
                    _password = membershipProvider.Attributes["Password"].Value;
                    return _password;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                    return _password;
                }
            }
        }
    }

    class LdapRoleManager
    {
        public static XmlNode RoleProviderNode()
        {
            XmlNode roleProvider = new XmlDocument();

            if (SPContext.Current != null)
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

                return roleProvider;
            }

            return null;
        }

        public static string Server
        {
            get
            {
                var roleProvider = RoleProviderNode();
                var _server = "localhost";

                try
                {
                    _server = (roleProvider.Attributes["server"].Value == null) ? "localhost" : 
                        roleProvider.Attributes["server"].Value;
                    return _server;
                }
                catch (NullReferenceException)
                {
                    return _server;
                }
            }
        }

        public static int Port
        {
            get
            {
                var roleProvider = RoleProviderNode();
                var _port = "389";

                try
                {
                    _port = (roleProvider.Attributes["port"].Value == null)
                                ? "389"
                                : roleProvider.Attributes["port"].Value;
                    return Convert.ToInt32(_port);
                }
                catch (NullReferenceException)
                {
                    return Convert.ToInt32(_port);
                }
            }
        }

        public static bool UseSSL
        {
            get 
            {
                var roleProvider = RoleProviderNode();
                var _useSsl = "false";

                try
                {
                    _useSsl = (roleProvider.Attributes["useSSL"].Value == null) ? "false" :
                        roleProvider.Attributes["useSSL"].Value;
                    return Convert.ToBoolean(_useSsl);
                }
                catch (NullReferenceException)
                {
                    return Convert.ToBoolean(_useSsl);
                }
            }
        }

        public static string GroupNameAttribute
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _groupNameAttribute = "cn";

                try
                {
                    _groupNameAttribute = (roleProvider.Attributes["groupNameAttribute"].Value == null) ? "cn" : 
                    roleProvider.Attributes["groupNameAttribute"].Value;
                    return _groupNameAttribute;
                }
                catch (NullReferenceException)
                {
                    return _groupNameAttribute;
                }
            }
        }

        public static string GroupContainer
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _groupContainer = string.Empty;

                try
                {
                    _groupContainer = (roleProvider.Attributes["groupContainer"].Value == null) ? string.Empty :
                    roleProvider.Attributes["groupContainer"].Value;
                    return _groupContainer;
                }
                catch (NullReferenceException)
                {
                    return _groupContainer;
                }
            }
        }

        public static string GroupMemberAttribute
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _groupMemberAttribute = "member";

                try
                {
                    _groupMemberAttribute = (roleProvider.Attributes["groupMemberAttribute"].Value == null) ? "member" :
                    roleProvider.Attributes["groupMemberAttribute"].Value;
                    return _groupMemberAttribute;
                }
                catch (NullReferenceException)
                {
                    return _groupMemberAttribute;
                }
            }
        }

        public static string UserNameAttribute
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _userNameAttribute = "userPrincipalName";

                try
                {
                    _userNameAttribute = (roleProvider.Attributes["userNameAttribute"].Value == null) ? "userPrincipalName" :
                        roleProvider.Attributes["userNameAttribute"].Value;
                    return _userNameAttribute;
                }
                catch (NullReferenceException)
                {
                    return _userNameAttribute;
                }
            }
        }

        public static string DnAttribute
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _dnAttribute = "distinguishedName";

                try
                {
                    _dnAttribute = (roleProvider.Attributes["dnAttribute"].Value == null) ? "distinguishedName" :
                        roleProvider.Attributes["dnAttribute"].Value;
                    return _dnAttribute;
                }
                catch (NullReferenceException)
                {
                    return _dnAttribute;
                }
            }
        }

        public static bool UseUserDnAttribute
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _useUserDnAttribute = "true";

                try
                {
                    _useUserDnAttribute = (roleProvider.Attributes["useUserDNAttribute"].Value == null) ? "true" : 
                        roleProvider.Attributes["useUserDNAttribute"].Value;
                    return Convert.ToBoolean(_useUserDnAttribute);
                }
                catch (NullReferenceException)
                {
                    return Convert.ToBoolean(_useUserDnAttribute);
                }
            }
        }

        public static SearchScope Scope
        {
            get 
            {
                var roleProvider = RoleProviderNode();

                try
                {
                    var _scope = (roleProvider.Attributes["scope"].Value.ToUpper() == null) ? "SUBTREE" :
                    roleProvider.Attributes["scope"].Value.ToUpper();

                if (_scope == "BASE")
                {
                    return SearchScope.Base;
                }
                if (_scope == "ONELEVEL")
                {
                    return SearchScope.OneLevel;
                }
                if (_scope == "SUBTREE")
                {
                    return SearchScope.Subtree;
                }
                return SearchScope.Subtree;
                }
                catch (NullReferenceException)
                {
                    return SearchScope.Subtree;
                }
            }
        }

        public static string UserFilter
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _userFilter = @"&(ObjectClass=User)(ObjectCategory=person)";

                try
                {
                    _userFilter = (roleProvider.Attributes["userFilter"].Value == null) ? @"&(objectClass=user)(objectCategory=person)" :
                        roleProvider.Attributes["userFilter"].Value;
                    return _userFilter;
                }
                catch (NullReferenceException)
                {
                    return _userFilter;
                }
            }
        }

        public static string GroupFilter
        {
            get 
            { 
                var roleProvider = RoleProviderNode();
                var _groupFilter = @"&(ObjectClass=group)(ObjectCategory=group)";

                try
                {
                    _groupFilter = (roleProvider.Attributes["groupFilter"].Value == null) ? @"&(objectClass=group)(objectCategory=group)" :
                        roleProvider.Attributes["groupFilter"].Value;
                    return _groupFilter;
                }
                catch (NullReferenceException)
                {
                    return _groupFilter;
                }
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
                    return _userName;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                    return _userName;
                }
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
                    _password = roleProvider.Attributes["Password"].Value;
                    return _password;
                }
                catch (NullReferenceException)
                {
                    //Attribute does not exist
                    return _password;
                }
            }
        }
    }

    class StsManager
    {
        public static DirectoryEntry ProviderNode(string providerName, bool IsProviderMembership, out string _server, out int _port, out bool _useSSL, out string _path,
            out string _username, out string _password, out string _userNameAttribute, out SearchScope _scope)
        {
            XmlNode provider = new XmlDocument();
            var ldapPath = string.Empty;

            var path = SPUtility.GetGenericSetupPath(@"WebServices\SecurityToken\web.config");
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(path);

            if (IsProviderMembership)
            {
                provider =
                    xmlDocument.SelectSingleNode(
                        (String.Format("configuration/system.web/membership/providers/add[@name='{0}']", providerName)));
            }
            else
            {
                provider =
                    xmlDocument.SelectSingleNode(
                        (String.Format("configuration/system.web/roleManager/providers/add[@name='{0}']", providerName)));                
            }

            try
            {
                _server = (provider.Attributes["server"].Value == null)
                                  ? "localhost"
                                  : provider.Attributes["server"].Value;
            }
            catch (NullReferenceException)
            {
                _server = "localhost";
            }

            try
            {
                _port = (provider.Attributes["port"].Value == null) ? 389 : Convert.ToInt32(provider.Attributes["port"].Value);
            }
            catch (NullReferenceException)
            {
                _port = 389;
            }

            try
            {
                _useSSL = (provider.Attributes["useSSL"].Value != null) && Convert.ToBoolean(provider.Attributes["useSSL"].Value);
            }
            catch (NullReferenceException)
            {
                _useSSL = false;
            }


            _path = string.Empty;

            try
            {
                _username = provider.Attributes["Username"].Value ?? "";
            }
            catch (NullReferenceException)
            {
                //Attribute not present
                _username = string.Empty;
            }

            try
            {
                _password = provider.Attributes["Password"].Value ?? "";
            }
            catch (NullReferenceException)
            {
                //Attribute not present
                _password = string.Empty;
            }

            if (IsProviderMembership)
            {
                try
                {
                    _path = provider.Attributes["userContainer"].Value ?? string.Empty;
                }
                catch (NullReferenceException)
                {
                    _path = string.Empty;
                }

            }
            else
            {
                try
                {
                    _path = provider.Attributes["groupContainer"].Value ?? string.Empty;
                }
                catch (NullReferenceException)
                {
                    _path = string.Empty;
                }

            }

            try
            {
                _userNameAttribute = provider.Attributes["userNameAttribute"].Value ?? "userPrincipalName";
            }
            catch (NullReferenceException)
            {
                _userNameAttribute = "userPrincipalName";
            }

            try
            {
                var scope = (provider.Attributes["scope"].Value.ToUpper() == null) ? "SUBTREE" :
                    provider.Attributes["scope"].Value;

                switch (scope)
                {
                    case "BASE":
                        _scope = SearchScope.Base;
                        break;
                    case "ONELEVEL":
                        _scope = SearchScope.OneLevel;
                        break;
                    case "SUBTREE":
                        _scope = SearchScope.Subtree;
                        break;
                    default:
                        _scope = SearchScope.Subtree;
                        break;
                }
            }
            catch (NullReferenceException)
            {
                _scope = SearchScope.Subtree;
            }


            var directoryEntry = LdapManager.Connect(_server, _port, _useSSL, _path, _username, _password);

            return directoryEntry;
        }
    }

    class LdapManager
    {
        public static DirectoryEntry Connect(string server, int port, bool useSSL, 
            string dn, string username, string password)
        {
            var ldapPath = LdapPath(server, port, dn);

            var directoryEntry = new DirectoryEntry(ldapPath.ToUpper());

            if (username != string.Empty && password != string.Empty)
            {
                directoryEntry.AuthenticationType = LdapAuthentication(useSSL);
                directoryEntry.Username = username;
                directoryEntry.Password = password;
            }
            else
            {
                directoryEntry.AuthenticationType = LdapAuthentication(useSSL);
            }

            return directoryEntry;
        }

        public static bool ValidateServer(string server, int port)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
            var result = socket.BeginConnect(server, port, null, null);
            var connected = result.AsyncWaitHandle.WaitOne(200, true);
            return connected;
        }

        public static AuthenticationTypes LdapAuthentication(bool UseSSL)
        {
            var types = AuthenticationTypes.ServerBind | AuthenticationTypes.FastBind |
                                        AuthenticationTypes.ReadonlyServer;

            if (UseSSL)
            {
                types |= AuthenticationTypes.Encryption;
            }

            return types;
        }

        public static string LdapPath(string server, int port, string DN)
        {
            var ldapPath = String.Empty;
            if ((server != null) && (server.Trim().Length > 0))
            {
                ldapPath = (server.Trim() + ":" + port + "/" + DN);
            }

            return "LDAP://" + ldapPath;
        }
    }
}
  