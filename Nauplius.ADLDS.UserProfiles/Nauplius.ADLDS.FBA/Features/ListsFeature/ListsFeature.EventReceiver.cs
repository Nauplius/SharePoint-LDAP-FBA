using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace Nauplius.ADLDS.FBA.Features.AdministrationFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("dd2ca666-f9bc-4271-a6f7-778c99b43f31")]
    public class AdministrationFeatureEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;

            using (web = web.Site.OpenWeb())
            {
                try
                {
                    web.AllowUnsafeUpdates = true;
                    Guid listId1 = web.Lists.Add("Nauplius.ADLDS.FBA - Administration",
                        "AD LDS Administration",
                        "Lists/Nauplius.ADLDS.FBA-Administration",
                        "3e5d29da-1e38-42ce-872d-b9e87a09eb5c", 20001, "101");
                    web.Update();
                    Guid listId2 = web.Lists.Add("Nauplius.ADLDS.FBA - User Approval",
                        "AD LDS User Approval",
                        "Lists/Nauplius.ADLDS.FBA-UserApproval",
                        "3e5d29da-1e38-42ce-872d-b9e87a09eb5c", 20002, "101");
                    web.AllowUnsafeUpdates = false;
                }
                catch (Exception)
                { }
            }
        }

        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWeb web = properties.Feature.Parent as SPWeb;

            using (web = web.Site.OpenWeb())
            {
                SPList list = web.Lists.TryGetList("Nauplius.ADLDS.FBA - Administration");
                if (list != null)
                {
                    try
                    {
                        list.Delete();
                    }
                    catch (Exception)
                    { }
                }

                SPList list2 = web.Lists.TryGetList("Nauplius.ADLDS.FBA - User Approval");
                if (list2 != null)
                {
                    try
                    {
                        list2.Delete();
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        //public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        //{
        //}

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
