using System;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.WebControls;

namespace Nauplius.ADLDS.UserProfiles.Layouts.Nauplius.ADLDS.UserProfiles
{
    public partial class ADLDSWebApplicationSettings : LayoutsPageBase
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
            catch (Exception ex)
            {
                e.IsValid = false;
            }
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
                                        return;
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
    }
}
