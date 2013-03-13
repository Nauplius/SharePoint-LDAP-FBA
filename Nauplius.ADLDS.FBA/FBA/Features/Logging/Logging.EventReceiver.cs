using System;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Nauplius.ADLDS.FBA.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("cbb4dd4d-cb8b-4a0e-934d-123abbb78eff")]
    public class ULSLog : SPFeatureReceiver
    {
        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
            RegisterLogging(properties, true);
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            RegisterLogging(properties, false);
        }

        private static void RegisterLogging(SPFeatureReceiverProperties properties, bool register)
        {
            SPFarm farm = properties.Definition.Farm;

            if (farm != null)
            {
                var log = Nauplius.ADLDS.FBA.Logging.Local;

                if (register)
                {
                    if (log == null)
                    {
                        log = new Nauplius.ADLDS.FBA.Logging();
                        log.Update();

                        if (log.Status != SPObjectStatus.Offline)
                        {
                            log.Status = SPObjectStatus.Offline;
                        }

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
                        try
                        {
                            log.Unprovision();
                        }
                        catch
                        {
                        }
                        finally
                        {
                            log.Delete();
                        }
                    }
                }
            }
        }
    }
}
