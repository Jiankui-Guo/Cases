using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Reflection;

namespace DocumentReadStatus.Features.Feature1
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("63b6842a-e3a7-4dbf-974f-828477b2f0e6")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if (webApp != null)
            {
                SPWebConfigModification modification = new SPWebConfigModification(
                    "add[@name='DocumentAuditDataModule']", "configuration/system.webServer/modules");
                modification.Sequence = 0;
                modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
                modification.Value = string.Format(@"<add name=""DocumentAuditDataModule"" type=""DocumentReadStatus.HttpModule.DocumentAuditDataModule, {0}"" />",
                    Assembly.GetExecutingAssembly().FullName);

                webApp.WebConfigModifications.Add(modification);
                webApp.Update();

                webApp.WebService.ApplyWebConfigModifications();
            }

        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;

            if (webApp != null)
            {
                SPWebConfigModification modification = new SPWebConfigModification(
                    "add[@name='DocumentAuditDataModule']", "configuration/system.webServer/modules");
                modification.Sequence = 0;
                modification.Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode;
                modification.Value = string.Format(@"<add name=""DocumentAuditDataModule"" type=""DocumentReadStatus.HttpModule.DocumentAuditDataModule, {0}"" />",
                    Assembly.GetExecutingAssembly().FullName);

                webApp.WebConfigModifications.Remove(modification);
                webApp.Update();

                webApp.WebService.ApplyWebConfigModifications();
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
