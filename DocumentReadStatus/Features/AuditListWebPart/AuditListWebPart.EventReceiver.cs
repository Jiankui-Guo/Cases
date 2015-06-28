using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace DocumentReadStatus.Features.AuditListWebPart
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("611faeee-bd86-4720-b33a-eabcad75077d")]
    public class AuditListWebPartEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;

            foreach (SPWeb web in site.AllWebs)
            {
                Guid readStatusListId = web.Lists.Add("DocReadStatus", "", SPListTemplateType.GenericList);
                SPList readStatusList = web.Lists[readStatusListId];
                readStatusList.Fields.Add("ViewPeople", SPFieldType.Text, false);
                readStatusList.Update();

                SPView allItemsView = readStatusList.Views["All Items"];
                if (!allItemsView.ViewFields.Exists("ViewPeople"))
                    allItemsView.ViewFields.Add("ViewPeople");

                readStatusList.Hidden = true;
                readStatusList.OnQuickLaunch = false;
                readStatusList.Update();

                Logger.WriteLog(Microsoft.SharePoint.Administration.TraceSeverity.Verbose,
                    "Executing AuditListWebPartEventReceiver FeatureActivated",
                    string.Format("Creating list for web:{0}", web.Url));
            }
        }


        // Uncomment the method below to handle the event raised before a feature is deactivated.

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPSite site = properties.Feature.Parent as SPSite;

            foreach (SPWeb web in site.AllWebs)
            {
                SPList list = web.Lists.TryGetList("DocReadStatus");

                if (list != null)
                {
                    web.Lists.Delete(list.ID);

                    Logger.WriteLog(Microsoft.SharePoint.Administration.TraceSeverity.Verbose,
                        "Executing AuditListWebPartEventReceiver FeatureDeactivating",
                        string.Format("Removing list for web:{0}", web.Url));
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
