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
                Nauplius.ADLDS.UserProfiles.ADLDSImportJob newTimerJob = new Nauplius.ADLDS.UserProfiles.ADLDSImportJob(tJobName, adminWebApplication);

                SPHourlySchedule jobSchedule = new SPHourlySchedule();
                jobSchedule.BeginMinute = 0;
                jobSchedule.EndMinute = 59;
                newTimerJob.Schedule = jobSchedule;
                newTimerJob.Update();

            }
        }

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

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            SPAdministrationWebApplication adminWebApp = SPAdministrationWebApplication.Local;
            using (SPSite siteCollection = new SPSite(adminWebApp.Sites[0].Url))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList list = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - GlobalSettings");
                    if (list != null)
                    {
                        try
                        {
                            list.Delete();
                        }
                        catch (Exception)
                        { }
                    }

                    SPList list2 = site.Lists.TryGetList("Nauplius.ADLDS.UserProfiles - WebAppSettings");
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
        }
    }
}
