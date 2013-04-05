using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

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
            var zone = GetZone(ddlZonePicker.SelectedValue);
            FillItems(selectedWebApp, zone);
        }

        protected void ZoneSelector_OnLoad(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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
                                var zone = GetZone(ddlZonePicker.SelectedValue);
                                string webAppUrl = selectedWebApp.GetResponseUri(zone).AbsoluteUri;
                                
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
                                        updateItem["WebApplicationZone"] = ddlZonePicker.SelectedValue;
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
                                newItem["WebApplicationZone"] = ddlZonePicker.SelectedValue;
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

        protected void FillItems(SPWebApplication selectedWebApp, SPUrlZone zone)
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
                                string webAppUrl = selectedWebApp.GetResponseUri(zone).AbsoluteUri;

                                var items = list.Items;

                                foreach (SPListItem item in items)
                                {
                                    if (item["WebApplicationUrl"].ToString() == webAppUrl)
                                    {
                                        try
                                        {
                                            ddlZonePicker.SelectedValue = item["WebApplicationZone"].ToString();
                                        }
                                        catch (Exception)
                                        {
                                            //In case someone manually modified the SPUrlZone in the list
                                            ddlZonePicker.SelectedValue = "Default";
                                        }

                                        txtMemProv.Text = (item["WebApplicationMembershipProvider"] == null)
                                                              ? String.Empty
                                                              : item["WebApplicationMembershipProvider"].ToString();
                                        txtRoleProv.Text = (item["WebApplicationRoleProvider"] == null)
                                                               ? String.Empty
                                                               : item["WebApplicationRoleProvider"].ToString();

                                        txtCustomUrl.Text = (item["CustomUrl"] == null)
                                                                ? String.Empty
                                                                : item["CustomUrl"].ToString();

                                        //AD LDS Server
                                        tBSN.Text = (item["ADLDSServer"] == null)
                                                        ? String.Empty
                                                        : item["ADLDSServer"].ToString();
                                        tBPrtNo.Text = (item["ADLDSPort"] == null)
                                                           ? String.Empty
                                                           : item["ADLDSPort"].ToString();
                                        cBUseSSL.Checked = (bool) item["ADLDSUseSSL"];

                                        //User
                                        tBLoginAttrib.Text = (item["ADLDSLoginAttrib"] == null)
                                                                 ? String.Empty
                                                                 : item["ADLDSLoginAttrib"].ToString();
                                        txtUsrDNAttrib.Text = (item["ADLDSUserDNAttrib"] == null)
                                                                  ? String.Empty
                                                                  : item["ADLDSUserDNAttrib"].ToString();
                                        txtUsrContainer.Text = (item["ADLDSUserContainer"] == null)
                                                                   ? String.Empty
                                                                   : item["ADLDSUserContainer"].ToString();
                                        txtUsrObjClass.Text = (item["ADLDSUserObjectClass"] == null)
                                                                  ? String.Empty
                                                                  : item["ADLDSUserObjectClass"].ToString();
                                        txtUsrFilter.Text = (item["ADLDSUserFilter"] == null)
                                                                ? String.Empty
                                                                : item["ADLDSUserFilter"].ToString();
                                        txtUsrScope.Text = (item["ADLDSUserScope"] == null)
                                                               ? String.Empty
                                                               : item["ADLDSUserScope"].ToString();
                                        txtUsrOtherAttribs.Text = (item["ADLDSUserOtherReqAttrib"] == null)
                                                                      ? String.Empty
                                                                      : item["ADLDSUserOtherReqAttrib"].ToString();

                                        //Group
                                        txtGrpContainer.Text = (item["ADLDSGroupContainer"] == null)
                                                                   ? String.Empty
                                                                   : item["ADLDSGroupContainer"].ToString();
                                        txtGrpNameAttrib.Text = (item["ADLDSGroupNameAttrib"] == null)
                                                                    ? String.Empty
                                                                    : item["ADLDSGroupNameAttrib"].ToString();
                                        txtGrpAltSearchAttrib.Text = (item["ADLDSGroupNameAltSearchAttrib"] == null)
                                                                         ? String.Empty
                                                                         : item["ADLDSGroupNameAltSearchAttrib"]
                                                                               .ToString();
                                        txtGrpMemAttrib.Text = (item["ADLDSGroupMemAttrib"] == null)
                                                                   ? String.Empty
                                                                   : item["ADLDSGroupMemAttrib"].ToString();
                                        txtGrpDNAttrib.Text = (item["ADLDSGroupDNAttrib"] == null)
                                                                  ? String.Empty
                                                                  : item["ADLDSGroupDNAttrib"].ToString();
                                        txtGrpUsrFilter.Text = (item["ADLDSGroupUserFilter"] == null)
                                                                   ? String.Empty
                                                                   : item["ADLDSGroupUserFilter"].ToString();
                                        txtGrpFilter.Text = (item["ADLDSGroupFilter"] == null)
                                                                ? String.Empty
                                                                : item["ADLDSGroupFilter"].ToString();
                                        txtGrpScope.Text = (item["ADLDSGroupScope"] == null)
                                                               ? String.Empty
                                                               : item["ADLDSGroupScope"].ToString();
                                    }
                                    else
                                    {
                                        txtMemProv.Text = string.Empty;
                                        txtRoleProv.Text = string.Empty;
                                        txtCustomUrl.Text = string.Empty;
                                        tBSN.Text = string.Empty;
                                        tBPrtNo.Text = string.Empty;
                                        cBUseSSL.Checked = false;
                                        tBLoginAttrib.Text = string.Empty;
                                        txtUsrDNAttrib.Text = string.Empty;
                                        txtUsrContainer.Text = string.Empty;
                                        txtUsrObjClass.Text = string.Empty;
                                        txtUsrFilter.Text = string.Empty;
                                        txtUsrScope.Text = string.Empty;
                                        txtUsrOtherAttribs.Text = string.Empty;
                                        txtGrpContainer.Text = string.Empty;
                                        txtGrpNameAttrib.Text = string.Empty;
                                        txtGrpAltSearchAttrib.Text = string.Empty;
                                        txtGrpMemAttrib.Text = string.Empty;
                                        txtGrpDNAttrib.Text = string.Empty;
                                        txtGrpUsrFilter.Text = string.Empty;
                                        txtGrpScope.Text = string.Empty;
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
