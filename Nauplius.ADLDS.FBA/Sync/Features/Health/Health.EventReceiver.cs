using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration.Health;

namespace Sync.Features.Health
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("f6687410-adc0-490c-acaa-331cac697b1a")]
    public class STSHealthAnalyzer : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //Register Health Rule
            Assembly a = Assembly.Load(this.GetType().Assembly.FullName);
            IDictionary<Type, Exception> exceptions = SPHealthAnalyzer.RegisterRules(a);

            if (exceptions != null)
            {
                string logEntry = a.FullName;
                if (exceptions.Count == 0)
                {
                    logEntry += " All rules were registered.";
                }
                else
                {
                    foreach (KeyValuePair<Type, Exception> pair in exceptions)
                    {
                        logEntry += string.Format(" Registration failed for type {0}. {1}",
                                                  pair.Key, pair.Value.Message);
                    }
                }
                System.Diagnostics.Trace.WriteLine(logEntry);
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //Unregister Health Rule
            Assembly a = Assembly.Load(this.GetType().Assembly.FullName);
            IDictionary<Type, Exception> exceptions = SPHealthAnalyzer.UnregisterRules(a);

            if (exceptions != null)
            {
                string logEntry = a.FullName;
                if (exceptions.Count == 0)
                {
                    logEntry += " All rules were unregistered.";
                }
                else
                {
                    foreach (KeyValuePair<Type, Exception> pair in exceptions)
                    {
                        logEntry += string.Format(" Unregistration failed for type {0}. {1}",
                                                  pair.Key, pair.Value.Message);
                    }
                }
                System.Diagnostics.Trace.WriteLine(logEntry);
            }
        }


        // Uncomment the method below to handle the event raised after a feature has been installed.

        //public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        //{
        //}


        // Uncomment the method below to handle the event raised before a feature is uninstalled.

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
            //Unregister Health Rule
            Assembly a = Assembly.Load(this.GetType().Assembly.FullName);
            IDictionary<Type, Exception> exceptions = SPHealthAnalyzer.UnregisterRules(a);

            if (exceptions != null)
            {
                string logEntry = a.FullName;
                if (exceptions.Count == 0)
                {
                    logEntry += " All rules were unregistered.";
                }
                else
                {
                    foreach (KeyValuePair<Type, Exception> pair in exceptions)
                    {
                        logEntry += string.Format(" Unregistration failed for type {0}. {1}",
                                                  pair.Key, pair.Value.Message);
                    }
                }
                System.Diagnostics.Trace.WriteLine(logEntry);
            }
        }

        // Uncomment the method below to handle the event raised when a feature is upgrading.

        //public override void FeatureUpgrading(SPFeatureReceiverProperties properties, string upgradeActionName, System.Collections.Generic.IDictionary<string, string> parameters)
        //{
        //}
    }
}
