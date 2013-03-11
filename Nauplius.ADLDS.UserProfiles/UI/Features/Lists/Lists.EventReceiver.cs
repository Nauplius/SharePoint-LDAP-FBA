using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace UI.Features.ListsFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("621a255b-a7ca-4196-b4c4-141e998f3e20")]
    public class ListsFeatureEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var web = properties.Feature.Parent as SPWeb;

            using (web = web.Site.OpenWeb())
            {
                try
                {
                    web.AllowUnsafeUpdates = true;
                    Guid listId1 = web.Lists.Add("Nauplius.ADLDS.FBA - StsFarm",
                        "AD LDS FBA Security Token Service Validation Fragment",
                        "Lists/Nauplius.ADLDS.FBA-StsFarm",
                        "f6fcbb14-b9ac-46bb-8dfd-2167ad6aae80", 50900, "101");
                    web.Update();
                    Guid listId2 = web.Lists.Add("Nauplius.ADLDS.FBA - WebApplicationSettings",
                        "AD LDS User Profile Import Web Application Settings",
                        "Lists/Nauplius.ADLDS.FBA-WebApplicationSettings",
                        "f6fcbb14-b9ac-46bb-8dfd-2167ad6aae80", 50901, "101");
                    Guid listId3 = web.Lists.Add("Nauplius.ADLDS.UserProfiles - GlobalSettings",
                        "AD LDS User Profile Import Global Settings",
                        "Lists/Nauplius.ADLDS.UserProfiles-GlobalSettings",
                        "f6fcbb14-b9ac-46bb-8dfd-2167ad6aae80", 50910, "101");
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
            var web = properties.Feature.Parent as SPWeb;

            using (web = web.Site.OpenWeb())
            {
                SPList list = web.Lists.TryGetList("Nauplius.ADLDS.FBA - StsFarm");
                if (list != null)
                {
                    try
                    {
                        list.Delete();
                    }
                    catch (Exception)
                    { }
                }

                SPList list2 = web.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");
                if (list2 != null)
                {
                    try
                    {
                        list2.Delete();
                    }
                    catch (Exception)
                    { }
                }

                SPList list3 = web.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - GlobalSettings");
                if (list != null)
                {
                    try
                    {
                        list3.Delete();
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

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            var web = properties.Feature.Parent as SPWeb;

            using (web = web.Site.OpenWeb())
            {
                SPList list = web.Lists.TryGetList("Nauplius.ADLDS.FBA - StsFarm");
                if (list != null)
                {
                    try
                    {
                        list.Delete();
                    }
                    catch (Exception)
                    { }
                }

                SPList list2 = web.Lists.TryGetList("Nauplius.ADLDS.FBA - WebApplicationSettings");
                if (list2 != null)
                {
                    try
                    {
                        list2.Delete();
                    }
                    catch (Exception)
                    { }
                }

                SPList list3 = web.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - GlobalSettings");
                if (list != null)
                {
                    try
                    {
                        list3.Delete();
                    }
                    catch (Exception)
                    { }
                }
            }
        }

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
