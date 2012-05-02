using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

namespace Nauplius.ADLDS.UserProfiles.Features.LoggingFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("f213eb5a-b819-46a3-a7f6-c3e1d20f3b47")]
    public class LoggingFeatureEventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            RegisterLogging(properties, true);
            /*
            SPFarm farm = properties.Definition.Farm;

            if (farm != null)
            {
                Logging log = Logging.Local;

                try
                {
                    if (log != null)
                    {
                        log = new Logging();
                        log.Update();

                        if (log.Status != SPObjectStatus.Unprovisioning)
                        {
                            log.Unprovision();
                            log.Delete();
                            RegisterLogging(properties, true);
                        }
                    }
                }
                catch (Exception)
                { }
            }
             */
        }
        
        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
           RegisterLogging(properties, false);
        }

        static void RegisterLogging(SPFeatureReceiverProperties properties, bool register)
        {
            SPFarm farm = properties.Definition.Farm;

            if (farm != null)
            {
                Logging log = Logging.Local;

                if (register)
                {
                    if (log == null)
                    {
                        log = new Logging();
                        // log.Update();

                        if (log.Status != SPObjectStatus.Online)
                        {
                            log.Provision();
                        }
                    }
                }
                else if (!register)
                {
                    if (log != null)
                    {
                        if (log.Status != SPObjectStatus.Unprovisioning)
                        {
                            log.Unprovision();
                            log.Delete();
                        }
                    }
                }
            }
        }
    }
}
