using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace Sync.Features.Logging
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("487b63c0-b85f-4189-bdb1-ff488a219eb4")]
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
                var log = Sync.Logging.Local;

                if (register)
                {
                    if (log == null)
                    {
                        log = new Sync.Logging();
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
