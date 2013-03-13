using System;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

//ToDo: Replace & with &amp; in all strings.

namespace UI.ADMIN.Nauplius.ADLDS.FBA
{
    public partial class FBAWebApplicationSettings : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void WebAppSelector_OnChanged(object sender, EventArgs e)
        {
            SPWebApplication selectedWebApp = ddlWebApp.CurrentItem;
            FillItems(selectedWebApp);
        }

        protected void btnSave_OnSave(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SPWebApplication selectedWebApp = ddlWebApp.CurrentItem;
                SaveOrUpdateList(selectedWebApp);
                Response.Redirect("../../applications.aspx");
            }
            else if (!Page.IsValid)
            {
                tBPortNoCustValidation.Visible = true;
            }
        }

        protected void btnCancel(object sender, EventArgs e)
        {
            Response.Redirect("../../applications.aspx");
        }

        protected void portValidation(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = false;

            try
            {
                int val = Convert.ToInt32(e.Value);

                if (val == 389 || val == 636 || (val >= 1024 && val <= 65535))
                {
                    e.IsValid = true;
                }
            }
            catch (Exception)
            {
                e.IsValid = false;
            }
        }

        protected void SaveOrUpdateList(SPWebApplication selectedWebApp)
        {
            using (var siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (var site = siteCollection.OpenWeb())
                {
                    try
                    {
                        var list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");
                        if (list != null)
                        {
                            if (selectedWebApp != null)
                            {
                                string webAppUrl = selectedWebApp.GetResponseUri(SPUrlZone.Default).AbsoluteUri;
                                
                                //Amerstands can't be passed directly to XML
                                string grpUsrFilter = Regex.Replace(txtGrpUsrFilter.Text, "&(?!amp;)", "&amp;");
                                string grpFilter = Regex.Replace(txtGrpFilter.Text, "&(?!amp;)", "&amp;");

                                var items = list.Items;

                                foreach (SPListItem item in items)
                                {
                                    if (item["WebApplicationUrl"].ToString() == webAppUrl)
                                    {
                                        SPListItem updateItem = list.Items[item.UniqueId];
                                        
                                        //Web Application
                                        updateItem["WebApplicationUrl"] = webAppUrl;
                                        updateItem["WebApplicationZone"] = "Default";
                                        updateItem["WebApplicationMembershipProvider"] = txtMemProv.Text;
                                        updateItem["WebApplicationRoleProvider"] = txtRoleProv.Text;
                                        updateItem["CustomUrl"] = txtCustomUrl.Text;

                                        //AD LDS Server
                                        updateItem["ADLDSServer"] = tBSN.Text;
                                        updateItem["ADLDSPort"] = tBPrtNo.Text;
                                        updateItem["ADLDSUseSSL"] = cBUseSSL.Checked;

                                        //User
                                        updateItem["ADLDSLoginAttrib"] = tBLoginAttrib.Text;
                                        updateItem["ADLDSUserDNAttrib"] = txtUsrDNAttrib.Text;
                                        updateItem["ADLDSUserContainer"] = txtUsrContainer.Text;
                                        updateItem["ADLDSUserObjectClass"] = txtUsrObjClass.Text;
                                        updateItem["ADLDSUserFilter"] = txtUsrFilter.Text;
                                        updateItem["ADLDSUserScope"] = txtUsrScope.Text;
                                        updateItem["ADLDSUserOtherReqAttrib"] = txtUsrOtherAttribs.Text;

                                        //Group
                                        updateItem["ADLDSGroupContainer"] = txtGrpContainer.Text;
                                        updateItem["ADLDSGroupNameAttrib"] = txtGrpNameAttrib.Text;
                                        updateItem["ADLDSGroupNameAltSearchAttrib"] = txtGrpAltSearchAttrib.Text;
                                        updateItem["ADLDSGroupMemAttrib"] = txtGrpMemAttrib.Text;
                                        updateItem["ADLDSGroupDNAttrib"] = txtGrpDNAttrib.Text;
                                        updateItem["ADLDSGroupUserFilter"] = grpUsrFilter;
                                        updateItem["ADLDSGroupFilter"] = grpFilter;
                                        updateItem["ADLDSGroupScope"] = txtGrpScope.Text;

                                        updateItem.Update();
                                        return;
                                    }
                                }

                                SPListItem newItem = list.Items.Add();

                                newItem["WebApplicationUrl"] = webAppUrl;
                                newItem["WebApplicationZone"] = "Default";
                                newItem["WebApplicationMembershipProvider"] = txtMemProv.Text;
                                newItem["WebApplicationRoleProvider"] = txtRoleProv.Text;
                                newItem["CustomUrl"] = txtCustomUrl.Text;

                                //AD LDS Server
                                newItem["ADLDSServer"] = tBSN.Text;
                                newItem["ADLDSPort"] = tBPrtNo.Text;
                                newItem["ADLDSUseSSL"] = cBUseSSL.Checked;

                                //User
                                newItem["ADLDSLoginAttrib"] = tBLoginAttrib.Text;
                                newItem["ADLDSUserDNAttrib"] = txtUsrDNAttrib.Text;
                                newItem["ADLDSUserContainer"] = txtUsrContainer.Text;
                                newItem["ADLDSUserObjectClass"] = txtUsrObjClass.Text;
                                newItem["ADLDSUserFilter"] = txtUsrFilter.Text;
                                newItem["ADLDSUserScope"] = txtUsrScope.Text;
                                newItem["ADLDSUserOtherReqAttrib"] = txtUsrOtherAttribs.Text;

                                //Group
                                newItem["ADLDSGroupContainer"] = txtGrpContainer.Text;
                                newItem["ADLDSGroupNameAttrib"] = txtGrpNameAttrib.Text;
                                newItem["ADLDSGroupNameAltSearchAttrib"] = txtGrpAltSearchAttrib.Text;
                                newItem["ADLDSGroupMemAttrib"] = txtGrpMemAttrib.Text;
                                newItem["ADLDSGroupDNAttrib"] = txtGrpDNAttrib.Text;
                                newItem["ADLDSGroupUserFilter"] = grpUsrFilter;
                                newItem["ADLDSGroupFilter"] = grpFilter;
                                newItem["ADLDSGroupScope"] = txtGrpScope.Text;

                                newItem.Update();
                            }
                        }
                    }
                    catch (SPException)
                    { }
                }
            }
        }

        protected void FillItems(SPWebApplication selectedWebApp)
        {
            using (var siteCollection = new SPSite(SPContext.Current.Site.ID))
            {
                using (var site = siteCollection.OpenWeb())
                {
                    try
                    {
                        var list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");
                        if (list != null)
                        {
                            if (selectedWebApp != null)
                            {
                                string webAppUrl = selectedWebApp.GetResponseUri(SPUrlZone.Default).AbsoluteUri;

                                var items = list.Items;

                                foreach (SPListItem item in items)
                                {
                                    if (item["WebApplicationUrl"].ToString() == webAppUrl)
                                    {
                                        //txtWebAppZone.text = item["WebApplicationZone"].ToString();
                                        txtMemProv.Text = item["WebApplicationMembershipProvider"].ToString();
                                        txtRoleProv.Text = item["WebApplicationRoleProvider"].ToString();

                                        try
                                        {
                                            txtCustomUrl.Text = item["CustomUrl"].ToString();
                                        }
                                        catch (Exception)
                                        {
                                            //CustomUrl is null
                                        }

                                        //AD LDS Server
                                        tBSN.Text = item["ADLDSServer"].ToString();
                                        tBPrtNo.Text = item["ADLDSPort"].ToString();
                                        cBUseSSL.Checked = (bool)item["ADLDSUseSSL"];

                                        //User
                                        tBLoginAttrib.Text = item["ADLDSLoginAttrib"].ToString();
                                        txtUsrDNAttrib.Text = item["ADLDSUserDNAttrib"].ToString();
                                        txtUsrContainer.Text = item["ADLDSUserContainer"].ToString();
                                        txtUsrObjClass.Text = item["ADLDSUserObjectClass"].ToString();
                                        txtUsrFilter.Text = item["ADLDSUserFilter"].ToString();
                                        txtUsrScope.Text = item["ADLDSUserScope"].ToString();
                                        txtUsrOtherAttribs.Text = item["ADLDSUserOtherReqAttrib"].ToString();

                                        //Group
                                        txtGrpContainer.Text = item["ADLDSGroupContainer"].ToString();
                                        txtGrpNameAttrib.Text = item["ADLDSGroupNameAttrib"].ToString();
                                        txtGrpAltSearchAttrib.Text = item["ADLDSGroupNameAltSearchAttrib"].ToString();
                                        txtGrpMemAttrib.Text = item["ADLDSGroupMemAttrib"].ToString();
                                        txtGrpDNAttrib.Text = item["ADLDSGroupDNAttrib"].ToString();
                                        txtGrpUsrFilter.Text = item["ADLDSGroupUserFilter"].ToString();
                                        txtGrpFilter.Text = item["ADLDSGroupFilter"].ToString();
                                        txtGrpScope.Text = item["ADLDSGroupScope"].ToString();
                                    }
                                }
                            }
                        }
                    }
                    catch (SPException)
                    { }
                }
            }
        }
    }
}
