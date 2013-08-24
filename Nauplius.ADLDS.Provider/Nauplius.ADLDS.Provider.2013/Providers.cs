using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Web.Security;
using System.Xml;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using SearchScope = System.DirectoryServices.SearchScope;
using System.Globalization;

namespace Nauplius.ADLDS.Provider
{
    public sealed class LdapMembership : MembershipProvider
    {
        public string MemProvider;
        public string AppName;
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            try
            {
                var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                                                         _ldapUseSsl,
                                                         _ldapUserContainer,
                                                         _ldapUserName, _ldapPassword,
                                                         _ldapSimpleBind);
                
                if (!string.IsNullOrEmpty(_ldapUserName) && !string.IsNullOrEmpty(_ldapPassword))
                {
                    directoryEntry.Username = _ldapUserName;
                    directoryEntry.Password = _ldapPassword;
                }

                var directorySearcher = new DirectorySearcher(directoryEntry)
                {
                    Filter =
                        String.Format("(&(ObjectClass={0})({1}=*{2}*))",
                                      _ldapUserObjectClass,
                                      _ldapUserNameAttribute,
                                      providerUserKey),
                    SearchScope = _ldapUserSearchScope
                };

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user;
                }
            }
            catch (ActiveDirectoryServerDownException exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "AD LDS Server is not responding " +
                                                      exception.StackTrace);

            }
            catch (Exception exception2)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "Unexpected exception in GetUser(ob) " +
                                                      exception2.StackTrace);
            }

            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            try
            {
                var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                                                         _ldapUseSsl,
                                                         _ldapUserContainer,
                                                         _ldapUserName, _ldapPassword,
                                                         _ldapSimpleBind);

                directoryEntry.Username = _ldapUserName;
                directoryEntry.Password = _ldapPassword;

                var directorySearcher = new DirectorySearcher(directoryEntry)
                {
                    Filter =
                        String.Format("(&(ObjectClass={0})({1}=*{2}*))",
                                      _ldapUserObjectClass,
                                      _ldapUserNameAttribute,
                                      username),
                    SearchScope = _ldapUserSearchScope
                };

                
                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user;
                }
            }
            catch (ActiveDirectoryServerDownException exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "AD LDS Server is not responding " + exception.Message +
                                                      exception.StackTrace);

            }
            catch (Exception exception2)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "Unexpected exception in GetUser(sb) " + exception2.Message + 
                                                      exception2.StackTrace);
            }

            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            try
            {
                var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                                                         _ldapUseSsl,
                                                         _ldapUserContainer,
                                                         _ldapUserName, _ldapPassword,
                                                         _ldapSimpleBind);

                directoryEntry.Username = _ldapUserName;
                directoryEntry.Password = _ldapPassword;

                var directorySearcher = new DirectorySearcher(directoryEntry)
                {
                    Filter =
                        String.Format(
                            "(&(ObjectClass={0})(mail={1}))",
                            _ldapUserObjectClass, email),
                    SearchScope =
                        _ldapUserSearchScope
                };

                var result = directorySearcher.FindOne();

                if (result != null)
                {
                    var user = GetUserFromSearchResult(result.GetDirectoryEntry());
                    return user.UserName;
                }
            }
            catch (ActiveDirectoryServerDownException exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "AD LDS Server is not responding " +
                                                      exception.StackTrace);

            }
            catch (Exception exception2)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.Unexpected,
                                                      "Unexpected exception in GetUserNameByEmail(s) " +
                                                      exception2.StackTrace);
            }

            return null;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                                                         _ldapUseSsl,
                                                         _ldapUserContainer,
                                                         _ldapUserName, _ldapPassword,
                                                         _ldapSimpleBind);

                if (!string.IsNullOrEmpty(_ldapUserName) && !string.IsNullOrEmpty(_ldapPassword))
                {
                    directoryEntry.Username = _ldapUserName;
                    directoryEntry.Password = _ldapPassword;
                }

                var directorySearcher = new DirectorySearcher(directoryEntry)
                {
                    Filter =
                        String.Format("(&(ObjectClass={0})({1}=*))",
                                      _ldapUserObjectClass,
                                      _ldapUserNameAttribute),
                    SearchScope = _ldapUserSearchScope,
                    PageSize = pageSize
                };

                var results = directorySearcher.FindAll();

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
            catch (ActiveDirectoryServerDownException exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "AD LDS Server is not responding " +
                                                      exception.StackTrace);

            }
            catch (Exception exception2)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.Unexpected,
                                                      "Unexpected exception in GetAllUsers(ii) " +
                                                      exception2.StackTrace);
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                                                         _ldapUseSsl,
                                                         _ldapUserContainer,
                                                         _ldapUserName, _ldapPassword,
                                                         _ldapSimpleBind);

                directoryEntry.Username = _ldapUserName;
                directoryEntry.Password = _ldapPassword;

                var directorySearcher = new DirectorySearcher(directoryEntry)
                {
                    Filter =
                        String.Format("(&(ObjectClass={0})({1}=*{2}*))",
                                      _ldapUserObjectClass,
                                      _ldapUserNameAttribute,
                                      usernameToMatch),
                    SearchScope = _ldapUserSearchScope,
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
            catch (ActiveDirectoryServerDownException exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "AD LDS Server is not responding " +
                                                      exception.StackTrace);

            }
            catch (Exception exception2)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.Unexpected,
                                                      "Unexpected exception in FindUsersByName(sii) - " +
                                                      exception2.StackTrace);
            }

            return users;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                                                         _ldapUseSsl,
                                                         _ldapUserContainer,
                                                         _ldapUserName, _ldapPassword,
                                                         _ldapSimpleBind);

                directoryEntry.Username = _ldapUserName;
                directoryEntry.Password = _ldapPassword;

                var directorySearcher = new DirectorySearcher(directoryEntry)
                {
                    Filter =
                        String.Format(
                            "(&(ObjectClass={0})(mail=*{1}*))",
                            _ldapUserObjectClass,
                            emailToMatch),
                    SearchScope =
                        _ldapUserSearchScope,
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
            catch (ActiveDirectoryServerDownException exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "AD LDS Server is not responding " +
                                                      exception.StackTrace);

            }
            catch (Exception exception2)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.Unexpected,
                                                      "Unexpected exception in FindUsersByEmail(sii) " +
                                                      exception2.StackTrace);
            }

            return users;
        }

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return isValid;
            }

            string _server;
            var _port = 389;
            var _useSSL = false;
            var _path = string.Empty;
            var _username = string.Empty;
            var _password = string.Empty;
            var _userNameAttribute = string.Empty;
            var _scope = new SearchScope();
            var _simpleBind = false;

            var directoryEntry = StsManager.ProviderNode(Name, true, out _server, out _port, out _useSSL, out _path, out _username,
                                                         out _password, out _userNameAttribute, out _scope, out _simpleBind);

            var credential = new NetworkCredential(username, password);
            var directoryIdentifier = new LdapDirectoryIdentifier(_server, Convert.ToInt32(_port));
            var connection = new LdapConnection(directoryIdentifier, credential, AuthType.Basic);

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
            catch (ActiveDirectoryServerDownException exception)
            {
                isValid = false;
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.High,
                                                      "AD LDS Server is not responding " +
                                                      exception.StackTrace);

            }
            catch (Exception exception2)
            {
                //No result code mapping available
                isValid = false;
                SPDiagnosticsService.Local.WriteTrace(100,
                                                      new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                                                                TraceSeverity.High, EventSeverity.Error,
                                                                                0, 100), TraceSeverity.Unexpected,
                                                      "Unexpected exception in ValidateUser(ss) " +
                                                      exception2.StackTrace);
            }

            return isValid;
        }

        private MembershipUser GetUserFromSearchResult(DirectoryEntry result)
        {
            object providerUserKey = result.Path;
            string userName = result.Properties[_ldapUserNameAttribute].Value.ToString();

            var user = new MembershipUser(Name, userName, providerUserKey,
                null, null, null, true, false, DateTime.UtcNow,
                DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow,
                DateTime.UtcNow);

            return user;
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _Name = name;
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            try
            {
                if (config["server"] != null)
                {
                    _ldapServer = config["server"];
                }
                if (config["port"] != null)
                {
                    _ldapPort = Convert.ToInt32(config["port"], CultureInfo.InvariantCulture);
                }
                if (config["useSSL"] != null)
                {
                    _ldapUseSsl = Convert.ToBoolean(config["useSSL"], CultureInfo.InvariantCulture);
                }
                if (config["userDNAttribute"] != null)
                {
                    _ldapUserDnAttribute = config["userDNAttribute"];
                }
                if (config["useDNAttribute"] != null)
                {
                    _ldapUseDnAttribute = Convert.ToBoolean(config["useDNAttribute"], CultureInfo.InvariantCulture);
                }
                if (config["userNameAttribute"] != null)
                {
                    _ldapUserNameAttribute = config["userNameAttribute"];
                }
                if (config["userContainer"] != null)
                {
                    _ldapUserContainer = config["userContainer"];
                }
                if (config["userObjectClass"] != null)
                {
                    _ldapUserObjectClass = config["userObjectClass"];
                }
                if (config["userFilter"] != null)
                {
                    _ldapUserFilter = config["userFilter"];
                }
                if (config["scope"] != null)
                {
                    _ldapUserSearchScope = (SearchScope)Enum.Parse(typeof(SearchScope), config["scope"]);
                }
                if (config["Username"] != null)
                {
                    _ldapUserName = config["Username"];
                }
                if (config["Password"] != null)
                {
                    _ldapPassword = config["Password"];
                }
                if (config["otherRequiredUserAttributes"] != null)
                {
                    string str = config["otherRequiredUserAttributes"];
                    _ldapOtherRequiredUserAttributes = (str == null) ? null : str.Split(new char[] { ',' });
                }
                _ldapOtherRequiredUserAttributes = new[] { _ldapUserDnAttribute, _ldapUserNameAttribute, "mail", "cn" };
            }
            catch (Exception exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                            new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                            TraceSeverity.Unexpected, EventSeverity.Error,
                                            0, 100), TraceSeverity.Unexpected, "Error during Membership Initialization - " + exception.Message, exception.StackTrace);
            }

        }

        #region Private Properties

        #region PrivateUsedProps
        private string _ldapServer = "localhost";
        private int _ldapPort = 0x185;
        private bool _ldapUseSsl;
        private string _ldapUserDnAttribute = "distinguishedName";
        private string _ldapUserNameAttribute = "userPrincipalName";
        private string _ldapUserContainer;
        private string _ldapUserObjectClass = "person";
        private SearchScope _ldapUserSearchScope = SearchScope.Subtree;
        private string _ldapUserName;
        private string _ldapPassword;
        private bool _ldapSimpleBind = false;
        private string _Name = string.Empty;
        private string[] _ldapOtherRequiredUserAttributes = new[] { "sn", "givenname", "cn" };
        #endregion

        #region PrivateUnusedProps

        private bool _ldapUseDnAttribute = true;
        private string _ldapUserFilter = "@(&(objectClass=*))";
        #endregion  

        #endregion

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

        public override string ApplicationName {
            get
            {
                if (string.IsNullOrEmpty(AppName))
                {
                    AppName = SPContext.Current.Web.Site.WebApplication.ToString();
                }
                return AppName;
            }
            set { this.AppName = SPContext.Current.Web.Site.WebApplication.ToString(); }
        }

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
            var _simpleBind = false;

            var directoryEntry = StsManager.ProviderNode(Name, false, out _server, out _port, out _useSSL,
                out _path, out _username, out _password, out _userNameAttribute, out _scope, out _simpleBind);

            var directorySearcher = new DirectorySearcher(directoryEntry)
            {
                Filter = String.Format("(&(ObjectClass=user)({0}={1}))",
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
            var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                _ldapUseSsl, _ldapGroupContainer,
                _ldapUserName, _ldapPassword, _ldapSimpleBind);

            var directorySearcher = new DirectorySearcher(directoryEntry)
                                        {
                                            Filter = String.Format("(&(ObjectClass=group)({0}={1}))",
                                                                   _ldapGroupNameAttribute, roleName),
                                            SearchScope = _ldapUserSearchScope
                                        };

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

            var directoryEntry = LdapManager.Connect(_ldapServer, _ldapPort,
                _ldapUseSsl, _ldapGroupContainer, _ldapUserName, _ldapPassword, _ldapSimpleBind);

            var directorySearcher = new DirectorySearcher(directoryEntry);
            directorySearcher.Filter = String.Format("(&(ObjectClass=group)({0}={1}))",
                                                     _ldapGroupNameAttribute, roleName);
            directorySearcher.SearchScope = _ldapUserSearchScope;

            var result = directorySearcher.FindOne();

            if (result != null)
            {
                foreach (DirectoryEntry user in result.Properties["memberof"])
                {
                    users.Add(user.Properties[_ldapUserName].Value.ToString());
                }
                return users.ToArray();
            }

            return null;
        }

        private MembershipUser GetUserFromSearchResult(DirectoryEntry result)
        {
            object providerUserKey = result.Path;
            string userName = result.Properties[_ldapUserNameAttribute].Value.ToString();

            var user = new MembershipUser(Name, userName, providerUserKey,
                null, null, null, true, false, DateTime.UtcNow,
                DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow,
                DateTime.UtcNow);

            return user;
        }

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            base.Initialize(name, config);

            _Name = name;
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            try
            {
                if (config["server"] != null)
                {
                    _ldapServer = config["server"];
                }
                if (config["port"] != null)
                {
                    _ldapPort = Convert.ToInt32(config["port"], CultureInfo.InvariantCulture);
                }
                if (config["useSSL"] != null)
                {
                    _ldapUseSsl = Convert.ToBoolean(config["useSSL"], CultureInfo.InvariantCulture);
                }
                if (config["groupNameAttribute"] != null)
                {
                    _ldapGroupNameAttribute = config["groupNameAttribute"];
                }
                if (config["groupContainer"] != null)
                {
                    _ldapGroupContainer = config["groupContainer"];
                }
                if (config["groupMemberAttribute"] != null)
                {
                    _ldapGroupMemberAttribute = config["groupMemberAttribute"];
                }
                if (config["userNameAttribute"] != null)
                {
                    _ldapUserNameAttribute = config["userNameAttribute"];
                }
                if (config["dnAttribute"] != null)
                {
                    _ldapdnAttribute = config["dnAttribute"];
                }
                if (config["useUserDNAttribute"] != null)
                {
                    _ldapUseUserDnAttribute = Convert.ToBoolean(config["useUserDNAttribute"], CultureInfo.InvariantCulture);
                }
                if (config["scope"] != null)
                {
                    _ldapUserSearchScope = (SearchScope)Enum.Parse(typeof(SearchScope), config["scope"]);
                }
                if (config["Username"] != null)
                {
                    _ldapUserName = config["Username"];
                }
                if (config["Password"] != null)
                {
                    _ldapPassword = config["Password"];
                }
                if (config["userFilter"] != null)
                {
                    _ldapUserFilter = config["userFilter"];
                }
                if (config["groupFilter"] != null)
                {
                    _ldapUserFilter = config["groupFilter"];
                }
            }
            catch (Exception exception)
            {
                SPDiagnosticsService.Local.WriteTrace(100,
                            new SPDiagnosticsCategory("NaupliusADLDSProvider",
                                            TraceSeverity.Unexpected, EventSeverity.Error,
                                            0, 100), TraceSeverity.Unexpected, "Error during Role Initialization - " + exception.Message, exception.StackTrace);
            }

        }

        #region Private Properties
        #region PrivateUsedProps
        private string _ldapServer = "localhost";
        private int _ldapPort = 0x185;
        private bool _ldapUseSsl;
        private SearchScope _ldapUserSearchScope = SearchScope.Subtree;
        private string _ldapUserName;
        private string _ldapPassword;
        private bool _ldapSimpleBind = false;
        private string _ldapGroupNameAttribute;
        private string _ldapGroupContainer;
        private string _ldapUserNameAttribute = "userPrincipalName";
        private string _Name = string.Empty;
        #endregion

        #region PrivateUnusedProps
        private string _ldapdnAttribute;
        private string _ldapGroupMemberAttribute;
        private bool _ldapUseUserDnAttribute = true;
        private string[] _ldapOtherRequiredUserAttributes = new[] { "sn", "givenname", "cn" };
        private string _ldapUserDnAttribute = "distinguishedName";
        private bool _ldapUseDnAttribute = true;
        private string _ldapUserContainer = string.Empty;
        private string _ldapUserObjectClass = "person";
        private string _ldapUserFilter = "@(&(objectClass=*))";
        #endregion
        #endregion

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

    class StsManager
    {
        public static DirectoryEntry ProviderNode(string providerName, bool IsProviderMembership, out string _server, out int _port, out bool _useSSL, out string _path,
            out string _username, out string _password, out string _userNameAttribute, out SearchScope _scope, out bool _simpleBind)
        {
            XmlNode provider = new XmlDocument();
            var ldapPath = string.Empty;

            var path = SPUtility.GetVersionedGenericSetupPath(@"WebServices\SecurityToken\web.config", 15);
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

            try
            {
                _simpleBind = (provider.Attributes["simpleBind"].Value != null) && Convert.ToBoolean(provider.Attributes["simpleBind"].Value);
            }
            catch (NullReferenceException)
            {
                _simpleBind = false;
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


            var directoryEntry = LdapManager.Connect(_server, _port, _useSSL, _path, _username, _password, _simpleBind);

            return directoryEntry;
        }
    }

    class LdapManager
    {
        public static DirectoryEntry Connect(string server, int port, bool useSSL,
            string dn, string username, string password, bool simpleBind)
        {
            var ldapPath = LdapPath(server, port, dn);

            var directoryEntry = new DirectoryEntry(ldapPath.ToUpper());

            if (username != string.Empty && password != string.Empty)
            {
                directoryEntry.AuthenticationType = LdapAuthentication(useSSL, simpleBind);
                directoryEntry.Username = username;
                directoryEntry.Password = password;
            }
            else
            {
                directoryEntry.AuthenticationType = LdapAuthentication(useSSL, simpleBind);
            }

            return directoryEntry;
        }

        public static AuthenticationTypes LdapAuthentication(bool UseSSL, bool simpleBind)
        {
            AuthenticationTypes types;

            if (!simpleBind)
            {
                types = AuthenticationTypes.Secure;
            }
            else
            {
                types = AuthenticationTypes.None;
            }


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
