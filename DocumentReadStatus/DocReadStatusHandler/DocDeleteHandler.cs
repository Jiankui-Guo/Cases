using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace DocumentReadStatus.EventReceiver
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class DocReadStatusHandler : SPItemEventReceiver
    {
        /// <summary>
        /// An item was updated.
        /// </summary>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);

            CleanUpDocReadStatus(properties.SiteId,
                properties.Web.ID,
                properties.BeforeUrl);
        }

        /// <summary>
        /// An item was deleted.
        /// </summary>
        public override void ItemDeleted(SPItemEventProperties properties)
        {
            base.ItemDeleted(properties);

            CleanUpDocReadStatus(properties.SiteId,
                properties.Web.ID,
                properties.BeforeUrl);
        }

        private void CleanUpDocReadStatus(Guid siteId, Guid webId, string itemUrl)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SPSite site = new SPSite(siteId))
                {
                    using (SPWeb web = site.OpenWeb(webId))
                    {
                        SPList readStatusList = web.Lists["DocReadStatus"];

                        if (readStatusList == null)
                        {
                            return;
                        }

                        SPQuery query = new SPQuery();
                        query.Query = string.Format(string.Concat(
                          "<Where><Eq>",
                             "<FieldRef Name='Title'/>",
                             "<Value Type='Text'>{0}</Value></Eq>",
                          "</Where>"), "/" + itemUrl);

                        query.ViewFields = "<FieldRef Name='Title' /><FieldRef Name='ViewPeople' />";
                        query.ViewFieldsOnly = true;

                        SPListItemCollection items = readStatusList.GetItems(query);

                        if (items.Count > 0)
                        {
                            site.AllowUnsafeUpdates = true;
                            web.AllowUnsafeUpdates = true;

                            foreach (SPListItem item in items)
                            {
                                readStatusList.Items.DeleteItemById(item.ID);
                            }

                            web.AllowUnsafeUpdates = false;
                            site.AllowUnsafeUpdates = false;
                        }

                    }
                }
            });
        }
    }
}