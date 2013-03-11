using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Security;

using Nauplius.ADLDS.UserProfiles;

namespace NaupliusADLDSUPALoggingFeature
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("f213eb5a-b819-46a3-a7f6-c3e1d20f3b47")]
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
                        { }
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
