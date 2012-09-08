using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;

using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;
using Microsoft.SharePoint.IdentityModel;

namespace Nauplius.ADLDS.FBA
{
    class WebModifications
    {
        const string owner = "Nauplius.ADLDS.FBA";

        private SPWebConfigModification EnsureWildcardNode()
        {
            SPWebConfigModification mod = new SPWebConfigModification();

            mod.Owner = owner;
            mod.Sequence = 0;
            mod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

            mod.Path = "configuration/SharePoint";
            mod.Value = "<PeoplePickerWildcards>";

            return mod;
        }

        public static SPWebConfigModification CreateWildcardNode(bool removeModification, SPFeatureReceiverProperties properties)
        {
            string featureId = properties.Feature.DefinitionId.ToString();
            
            SPWebConfigModification mod = new SPWebConfigModification();
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if(webApp.UseClaimsAuthentication)
            {
                mod.Owner = owner;
                mod.Sequence = 0;
                mod.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;

                mod.Path = "configuration/SharePoint/PeoplePickerWildcards";
                mod.Name = "add[@key={0}";
                mod.Value = String.Format("<add key='{0}' value='*' />", GetClaimProvider(webApp, SPUrlZone.Default, true));

                mod.Sequence = 1;
                mod.Value = String.Format("<add key='{0}' value='*' />", GetClaimProvider(webApp, SPUrlZone.Default, false));
            }

            return null;
        }

        public static SPClaimProvider GetClaimProvider(SPWebApplication webApp, SPUrlZone zone, bool membershipProvider)
        {
            IEnumerable<SPAuthenticationProvider> providers = webApp.GetIisSettingsWithFallback(zone).ClaimsAuthenticationProviders;
            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;

            using (SPSite siteCollection = new SPSite(adminWebApp.Sites[0].Url))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList list = site.Lists.TryGetList("Nauplius.ADLDS.FBA - Administration");
                    if (list != null)
                    {
                        if (list.ItemCount >= 1)
                        {
                            foreach (SPListItem item in list.Items)
                            {
                                if (SPWebApplication.Lookup(new Uri(item["WebApplicationUrl"].ToString())).GetResponseUri((SPUrlZone)(item["WebApplicationZone"])).AbsoluteUri == webApp.GetResponseUri(zone).AbsoluteUri)
                                {
                                    if (membershipProvider)
                                    {
                                        SPClaimProvider provider = item["WebApplicationMembershipProvider"] as SPClaimProvider;
                                        return provider;
                                    }
                                    else
                                    {
                                        SPClaimProvider provider = item["WebApplicationRoleProvider"] as SPClaimProvider;
                                        return provider;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
        }
    }
}
