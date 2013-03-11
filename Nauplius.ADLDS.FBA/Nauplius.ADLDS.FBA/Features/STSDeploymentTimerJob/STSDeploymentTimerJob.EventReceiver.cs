using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Nauplius.ADLDS.FBA.Features.STSDeploymentTimerJob
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("e90bf058-1d8a-4fda-9397-d5056bde2aa5")]
    public class STSDeploymentTimerJobEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        const string tJobName = "Nauplius ADLDS FBA STS Sync Monitor";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication adminWebApplication = properties.Feature.Parent as SPWebApplication;
            
            foreach (SPJobDefinition job in adminWebApplication.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.Delete();
                }
            }

            if (((SPWebApplication)properties.Feature.Parent).IsAdministrationWebApplication)
            {
                STSSyncMonitor newTimerJob = new STSSyncMonitor(tJobName, adminWebApplication);

                var jobSchedule = new SPOneTimeSchedule();
                newTimerJob.Schedule = jobSchedule;
                newTimerJob.Update();

            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication adminWebApplication = properties.Feature.Parent as SPWebApplication;

            foreach (SPJobDefinition job in adminWebApplication.JobDefinitions)
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
