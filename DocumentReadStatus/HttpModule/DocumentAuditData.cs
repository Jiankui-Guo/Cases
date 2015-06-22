using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.SharePoint;
using System.Data.SqlClient;
using System.Data;

namespace DocumentReadStatus.HttpModule
{
    class DocumentReadStatusModule : IHttpModule
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += WriteDocStatus;
        }

        private void WriteDocStatus(object sender, EventArgs e)
        {
            try
            {

                string itemURL = HttpUtility.UrlDecode(HttpContext.Current.Request.RawUrl);
                Guid listId = Guid.Empty;
                Guid itemId = Guid.Empty;

                if (SPContext.Current.Web == null)
                    return;

                DataTable result = new DataTable();

                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (SqlConnection conn = new SqlConnection(SPContext.Current.Web.Site.ContentDatabase.DatabaseConnectionString))
                    {
                        string cmdString =
                            string.Format("SELECT Id,ListId,DirName,LeafName FROM AllDocs WITH(NOLOCK) WHERE LeafName='{0}'",
                            itemURL.Substring(itemURL.LastIndexOf('/') + 1));
                        using (SqlCommand comm = new SqlCommand(cmdString, conn))
                        {
                            conn.Open();
                            SqlDataAdapter sqlAdapter = new SqlDataAdapter(comm);
                            sqlAdapter.Fill(result);
                        }
                    }
                });

                if (result.Rows.Count == 0)
                {
                    result = null;
                    return;
                }

                if (result.Rows.Count == 1)
                {
                    listId = (Guid)result.Rows[0][1];
                    itemId = (Guid)result.Rows[0][0];
                }
                else
                {
                    foreach (DataRow row in result.Rows)
                    {
                        if (itemURL.Contains((string)row[2]))
                        {
                            listId = (Guid)row[1];
                            itemId = (Guid)row[0];
                            break;
                        }
                    }
                }

                //For application pages such as /_layouts/15/addanapp.aspx
                if (listId == Guid.Empty || itemId == Guid.Empty)
                    return;

                SPList list = SPContext.Current.Web.Lists.GetList(listId, true);
                SPListItem doc = null;

                if (list.BaseType != SPBaseType.DocumentLibrary)
                    return;

                try
                {
                    doc = list.GetItemByUniqueId(itemId);
                }
                catch
                {
                    //Ingore allitems.aspx and so on
                }

                if (doc != null)
                {
                    Guid siteId = SPContext.Current.Site.ID;
                    Guid webId = SPContext.Current.Web.ID;

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {

                        using (SPSite site = new SPSite(SPContext.Current.Site.ID))
                        {
                            using (SPWeb web = site.OpenWeb(SPContext.Current.Web.ID))
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
                                  "</Where>"), itemURL);

                                query.ViewFields = "<FieldRef Name='Title' /><FieldRef Name='ViewPeople' />";
                                query.ViewFieldsOnly = true;

                                SPListItemCollection items = readStatusList.GetItems(query);

                                site.AllowUnsafeUpdates = true;
                                web.AllowUnsafeUpdates = true;

                                string userId = SPContext.Current.Web.CurrentUser.ID.ToString();
                                if (items.Count == 0)
                                {
                                    SPListItem item = readStatusList.AddItem();
                                    item["Title"] = itemURL;
                                    item["ViewPeople"] = ";" + userId + ";";
                                    item.Update();
                                }
                                //ITEM COUNT SHOULD BE EQUEAL TO 1
                                else
                                {
                                    string peopleIds = items[0]["ViewPeople"].ToString() + ";";
                                    if (!peopleIds.Contains(";" + userId + ";"))
                                        items[0]["ViewPeople"] = peopleIds + userId + ";";
                                }

                                web.AllowUnsafeUpdates = false;
                                site.AllowUnsafeUpdates = false;
                            }
                        }

                    });

                }

                result = null;
            }
            catch(Exception ex)
            {
                Microsoft.SharePoint.Administration.SPDiagnosticsService.Local.WriteTrace(
                    0,
                    new Microsoft.SharePoint.Administration.SPDiagnosticsCategory("Document Read Status",
                        Microsoft.SharePoint.Administration.TraceSeverity.High,
                        Microsoft.SharePoint.Administration.EventSeverity.Information),
                        Microsoft.SharePoint.Administration.TraceSeverity.High,
                        ex.Message,
                        ex.StackTrace);
            }
        }

    }
}
