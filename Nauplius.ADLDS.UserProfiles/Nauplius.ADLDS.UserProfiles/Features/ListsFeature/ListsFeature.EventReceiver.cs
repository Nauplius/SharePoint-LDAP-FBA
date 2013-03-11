using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace NaupliusADLDSUPAListsFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("ea3a8db2-df1a-4ffa-97e4-ce1f763d4b50")]
    public class Lists : SPFeatureReceiver
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
                        Guid listId1 = web.Lists.Add("Nauplius.ADLDS.UserProfiles - GlobalSettings",
                            "AD LDS User Profile Import Global Settings",
                            "Lists/Nauplius.ADLDS.UserProfiles-GlobalSettings",
                            "d81d5f6c-88ad-4f1b-bb14-05d929137637", 10001, "101");
                        web.Update();
                        Guid listId2 = web.Lists.Add("Nauplius.ADLDS.UserProfiles - WebAppSettings",
                            "AD LDS User Profile Import Web Application Settings",
                            "Lists/Nauplius.ADLDS.UserProfiles-WebAppSettings",
                            "d81d5f6c-88ad-4f1b-bb14-05d929137637", 10002, "101");
                        web.Update();
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
                    SPList list = web.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - GlobalSettings");
                    if (list != null)
                    {
                        try
                        {
                            list.Delete();
                        }
                        catch (Exception)
                        { }
                    }

                    SPList list2 = web.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - WebAppSettings");
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