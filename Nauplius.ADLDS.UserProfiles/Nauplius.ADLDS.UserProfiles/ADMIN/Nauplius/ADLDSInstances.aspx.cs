using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace Nauplius.ADLDS.UserProfiles.Layouts.Nauplius.ADLDS.UserProfiles
{
    public partial class ADLDSInstances : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadGlobalSettingsOrSetDefaults();
            }
        }

        protected void WebAppSelector_OnChanged(object sender, EventArgs e)
        {
            SPWebApplication selectedWebApp = ddlWebApp.CurrentItem;
            FillItems(selectedWebApp);
        }

        protected void btnSave_OnSave(object sender, EventArgs e)
        {
            SPWebApplication selectedWebApp = ddlWebApp.CurrentItem;
            SaveOrUpdateList(selectedWebApp);
        }

        protected void btnUpdate_OnUpdate(object sender, EventArgs e)
        {
            UpdateGlobalSettings();
        }

        protected void SaveOrUpdateList(SPWebApplication selectedWebApp)
        {
            using (SPSite siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    try
                    {
                        SPList list = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - WebAppSettings");
                        if (list != null)
                        {
                            if (selectedWebApp != null)
                            {
                                string webAppUrl = selectedWebApp.GetResponseUri(SPUrlZone.Default).AbsoluteUri;

                                SPListItemCollection items = list.Items;

                                foreach (SPListItem item in items)
                                {
                                    if (item["WebApplicationUrl"].ToString() == webAppUrl)
                                    {
                                        SPListItem updateItem = list.Items[item.UniqueId];
                                        updateItem["WebApplicationUrl"] = webAppUrl;
                                        updateItem["ADLDSServer"] = tBSN.Text;
                                        updateItem["ADLDSPort"] = tBPrtNo.Text;
                                        updateItem["ADLDSDN"] = tBDNPath.Text;
                                        updateItem["ADLDSUseSSL"] = cBUseSSL.Checked;
                                        updateItem["ADLDSLoginAttrib"] = tBLoginAttrib.Text;
                                        updateItem.Update();
                                    }
                                }

                                SPListItem newItem = list.Items.Add();
                                newItem["WebApplicationUrl"] = webAppUrl;
                                newItem["ADLDSServer"] = tBSN.Text;
                                newItem["ADLDSPort"] = tBPrtNo.Text;
                                newItem["ADLDSDN"] = tBDNPath.Text;
                                newItem["ADLDSUseSSL"] = cBUseSSL.Checked;
                                newItem["ADLDSLoginAttrib"] = tBLoginAttrib.Text;
                                newItem.Update();
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        protected void FillItems(SPWebApplication selectedWebApp)
        {
            using (SPSite siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    try
                    {
                        SPList list = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - WebAppSettings");
                        if (list != null)
                        {
                            if (selectedWebApp != null)
                            {
                                string webAppUrl = selectedWebApp.GetResponseUri(SPUrlZone.Default).AbsoluteUri;

                                SPListItemCollection items = list.Items;

                                foreach (SPListItem item in items)
                                {
                                    if (item["WebApplicationUrl"].ToString() == webAppUrl)
                                    {
                                        tBSN.Text = item["ADLDSServer"].ToString();
                                        tBPrtNo.Text = item["ADLDSPort"].ToString();
                                        tBDNPath.Text = item["ADLDSDN"].ToString();
                                        cBUseSSL.Checked = (bool)item["ADLDSUseSSL"];
                                        tBLoginAttrib.Text = item["ADLDSLoginAttrib"].ToString();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        protected void UpdateGlobalSettings()
        {
            using (SPSite siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                { 
                    try
                    {
                        SPList list = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - GlobalSettings");
                        if (list != null)
                        {
                            if (list.ItemCount < 1)
                            {
                                SPListItem newItem = list.Items.Add();
                                newItem["GlobalDefault"] = "GlobalDefaultValues";
                                newItem["ClaimsIdentifier"] = tBCI.Text;
                                newItem["LDAPFilter"] = tBLF.Text;
                                newItem["DeleteProfiles"] = cBDelProf.Checked;
                                newItem["Department"] = tBDept.Text;
                                newItem["FirstName"] = tBFN.Text;
                                newItem["LastName"] = tBLN.Text;
                                newItem["Office"] = tBOff.Text;
                                newItem["PreferredName"] = tBPN.Text;
                                newItem["UserTitle"] = tBTitle.Text;
                                newItem["WebSite"] = tBWS.Text;
                                newItem["WorkEmail"] = tBWE.Text;
                                newItem["WorkPhone"] = tBWP.Text;
                                newItem.Update();
                            }
                            else if (list.ItemCount >= 1)
                            {
                                foreach(SPListItem item in list.Items)
                                {
                                    if (item["GlobalDefault"].ToString() == "GlobalDefaultValues")
                                    {
                                        SPListItem updateItem = item;
                                        updateItem["ClaimsIdentifier"] = tBCI.Text;
                                        updateItem["LDAPFilter"] = tBLF.Text;
                                        updateItem["DeleteProfiles"] = cBDelProf.Checked;
                                        updateItem["Department"] = tBDept.Text;
                                        updateItem["FirstName"] = tBFN.Text;
                                        updateItem["LastName"] = tBLN.Text;
                                        updateItem["Office"] = tBOff.Text;
                                        updateItem["PreferredName"] = tBPN.Text;
                                        updateItem["UserTitle"] = tBTitle.Text;
                                        updateItem["WebSite"] = tBWS.Text;
                                        updateItem["WorkEmail"] = tBWE.Text;
                                        updateItem["WorkPhone"] = tBWP.Text;
                                        updateItem.Update();
                                    }
                                    else
                                    {
                                        SPListItem newItem = list.Items.Add();
                                        newItem["GlobalDefault"] = "GlobalDefaultValues";
                                        newItem["ClaimsIdentifier"] = tBCI.Text;
                                        newItem["LDAPFilter"] = tBLF.Text;
                                        newItem["DeleteProfiles"] = cBDelProf.Checked;
                                        newItem["Department"] = tBDept.Text;
                                        newItem["FirstName"] = tBFN.Text;
                                        newItem["LastName"] = tBLN.Text;
                                        newItem["Office"] = tBOff.Text;
                                        newItem["PreferredName"] = tBPN.Text;
                                        newItem["UserTitle"] = tBTitle.Text;
                                        newItem["WebSite"] = tBWS.Text;
                                        newItem["WorkEmail"] = tBWE.Text;
                                        newItem["WorkPhone"] = tBWP.Text;
                                        newItem.Update();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        protected void LoadGlobalSettingsOrSetDefaults()
        {
            using (SPSite siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    site.AllowUnsafeUpdates = true;
                    try
                    {
                        SPList list = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - GlobalSettings");
                        if (list != null)
                        {
                            if (list.ItemCount == 0)
                            {
                                SPListItem newItem = list.Items.Add();
                                newItem["GlobalDefault"] = "GlobalDefaultValues";
                                newItem["ClaimsIdentifier"] = "i:0#.f";
                                newItem["LDAPFilter"] = "(&(objectClass=user))";
                                newItem["DeleteProfiles"] = 0;
                                newItem["Department"] = "department";
                                newItem["FirstName"] = "givenName";
                                newItem["LastName"] = "sn";
                                newItem["Office"] = "physicalDeliveryOfficeName";
                                newItem["PreferredName"] = "displayName";
                                newItem["UserTitle"] = "title";
                                newItem["WebSite"] = "wWWHomePage";
                                newItem["WorkEmail"] = "mail";
                                newItem["WorkPhone"] = "telephoneNumber";
                                newItem.Update();
                                LoadGlobalSettingsOrSetDefaults();
                            }
                            else if (list.ItemCount >= 1)
                            {
                                foreach (SPListItem item in list.Items)
                                {
                                    if (item["GlobalDefault"].ToString() == "GlobalDefaultValues")
                                    {
                                        tBCI.Text = item["ClaimsIdentifier"].ToString();
                                        tBLF.Text = item["LDAPFilter"].ToString();
                                        cBDelProf.Checked = (bool)item["DeleteProfiles"];
                                        tBDept.Text = item["Department"].ToString();
                                        tBFN.Text = item["FirstName"].ToString();
                                        tBLN.Text = item["LastName"].ToString();
                                        tBOff.Text = item["Office"].ToString();
                                        tBPN.Text = item["PreferredName"].ToString();
                                        tBTitle.Text = item["UserTitle"].ToString();
                                        tBWS.Text = item["WebSite"].ToString();
                                        tBWE.Text = item["WorkEmail"].ToString();
                                        tBWP.Text = item["WorkPhone"].ToString();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    { }
                }
            }
        }
    }
}