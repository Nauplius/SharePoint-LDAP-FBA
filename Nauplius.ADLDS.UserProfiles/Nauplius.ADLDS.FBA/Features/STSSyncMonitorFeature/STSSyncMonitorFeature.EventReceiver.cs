using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Health;

namespace Nauplius.ADLDS.FBA.Features.STSSyncMonitorFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("7e73cd0e-89e8-4963-bd43-952e884b6320")]
    public class STSSyncMonitorFeatureEventReceiver : SPFeatureReceiver
    {
        const string tJobName = "Nauplius ADLDS FBA STS Sync Monitor";

        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var adminWebApplication = properties.Feature.Parent as SPWebApplication;

            if (((SPWebApplication)properties.Feature.Parent).IsAdministrationWebApplication)
            {
                foreach (SPJobDefinition job in adminWebApplication.JobDefinitions)
                {
                    if (job.Name == tJobName)
                    {
                        job.Delete();
                    }
                }
                var newTimerJob = new STSSyncMonitor(tJobName, adminWebApplication);

                var jobSchedule = new SPOneTimeSchedule(DateTime.UtcNow);
                newTimerJob.Schedule = jobSchedule;
                newTimerJob.Update();
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var adminWebApplication = properties.Feature.Parent as SPWebApplication;

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

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            var adminWebApplication = properties.Feature.Parent as SPWebApplication;

            foreach (SPJobDefinition job in adminWebApplication.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.Delete();
                }
            }
        }

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
