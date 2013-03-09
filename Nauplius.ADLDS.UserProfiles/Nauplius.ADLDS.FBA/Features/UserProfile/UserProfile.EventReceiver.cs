using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Nauplius.ADLDS.UserProfiles;

namespace Nauplius.ADLDS.FBA.Features.UserProfile
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>
    //ToDo: Convert to Timer Service job
    [Guid("b29a2dc1-fe93-4812-831d-173234ce54cb")]
    public class Timer : SPFeatureReceiver
    {
        const string tJobName = "Nauplius ADLDS User Profile Import";

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var local = SPFarm.Local;

            var services = from s in local.Services
                           where s.Name == "SPTimerV4"
                           select s;

            var service = services.First();
            
            foreach (var job in service.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.Delete();
                }
            }

            var newTimerJob = new ADLDSImportJob(tJobName, service);

            var jobSchedule = new SPHourlySchedule {BeginMinute = 0, EndMinute = 59};
            newTimerJob.Schedule = jobSchedule;
            newTimerJob.Update();
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            var local = SPFarm.Local;

            var services = from s in local.Services
                           where s.Name == "SPTimerV4"
                           select s;

            var service = services.First();

            foreach (var job in service.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.Delete();
                }
            }
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            var local = SPFarm.Local;

            var services = from s in local.Services
                           where s.Name == "SPTimerV4"
                           select s;

            var service = services.First();

            foreach (var job in service.JobDefinitions)
            {
                if (job.Name == tJobName)
                {
                    job.Delete();
                }
            }
        }
    }
}
