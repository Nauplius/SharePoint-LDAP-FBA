using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace Nauplius.ADLDS.UserProfiles.Features.TimerJobFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("48312723-bcb8-45ab-8701-8800016a158a")]
    public class TimerJobFeatureEventReceiver : SPFeatureReceiver
    {
        const string tJobName = "Nauplius ADLDS User Profile Import";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;

            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.Delete();
                }
            }

            Nauplius.ADLDS.UserProfiles.ADLDSImportJob newTimerJob = new Nauplius.ADLDS.UserProfiles.ADLDSImportJob(tJobName, site.WebApplication);

            SPHourlySchedule jobSchedule = new SPHourlySchedule();
            jobSchedule.BeginMinute = 0;
            jobSchedule.EndMinute = 59;
            newTimerJob.IsDisabled = true;
            newTimerJob.Update();

        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = (SPSite)properties.Feature.Parent;

            foreach (SPJobDefinition job in site.WebApplication.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.Delete();
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
