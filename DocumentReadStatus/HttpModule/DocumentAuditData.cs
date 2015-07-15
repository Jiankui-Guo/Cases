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
            //TODO: use callstorage.svc to open document
            //TODO: skip processing when the url is asmx or svc and all others which are not documents
            try
            {
                LoggingService.LogInfo("Enter  WriteDocStatus function::PreRequestHandlerExecute");
                LoggingService.LogInfo("Raw Url:{0}", HttpContext.Current.Request.RawUrl);
                //Logger.WriteVerboseLog("Executing WriteDocStatus|RawUrl:{0}", HttpContext.Current.Request.RawUrl);

                string itemURL = HttpUtility.UrlDecode(HttpContext.Current.Request.RawUrl);

                if (itemURL.Contains("/_vti_bin/owssvr.dll"))
                {
                    LoggingService.LogInfo("Ignore /_vti_bin/owssvr.dll.");
                    return;
                }

                //OWA: http://jg-pc-wfe01/_layouts/15/WopiFrame.aspx?sourcedoc=/Shared%20Documents/OWA.docx&action=default
                if (itemURL.Contains("WopiFrame.aspx"))
                {
                    itemURL = HttpUtility.UrlDecode(HttpContext.Current.Request.QueryString["sourcedoc"]);
                }

                LoggingService.LogInfo("Decoded Item Url:{0}", itemURL);

                Guid listId = Guid.Empty;
                Guid itemId = Guid.Empty;

                if (SPContext.Current.Web == null)
                {
                    LoggingService.LogInfo("Return:SPContext.Current.Web == Null");
                    return;
                }
                
                DataTable result = new DataTable();

                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (SqlConnection conn = new SqlConnection(SPContext.Current.Web.Site.ContentDatabase.DatabaseConnectionString))
                    {
                        string cmdString =
                            string.Format("SELECT Id,ListId,DirName,LeafName FROM AllDocs WITH(NOLOCK) WHERE LeafName=N'{0}'",
                            itemURL.Substring(itemURL.LastIndexOf('/') + 1));
                        using (SqlCommand comm = new SqlCommand(cmdString, conn))
                        {
                            conn.Open();
                            SqlDataAdapter sqlAdapter = new SqlDataAdapter(comm);
                            sqlAdapter.Fill(result);
                            LoggingService.LogInfo("Executed SQL:{0}", cmdString);
                        }
                    }
                });

                LoggingService.LogInfo("Sql Result Count:{0}", result.Rows.Count);

                if (result.Rows.Count == 0)
                {
                    result = null;
                    LoggingService.LogInfo("Return: Sql Result Count: 0");
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

                LoggingService.LogInfo("Found Item's List Id:{0}\tItem Id:{1}", listId, itemId);

                //For application pages such as /_layouts/15/addanapp.aspx
                if (listId == Guid.Empty || itemId == Guid.Empty)
                {
                    LoggingService.LogInfo("Return:ListId or ItemId is empty.");
                    return;
                }

                SPList list = SPContext.Current.Web.Lists.GetList(listId, true);
                SPListItem doc = null;

                LoggingService.LogInfo("Get List:{0}, BaseType:{1}", list.Title, list.BaseType);

                if (list.BaseType != SPBaseType.DocumentLibrary)
                {
                    LoggingService.LogInfo("Return:List is NOT Document Library.");
                    return;
                }

                try
                {
                    doc = list.GetItemByUniqueId(itemId);
                }
                catch(Exception ex)
                {
                    //Ingore allitems.aspx and so on
                    LoggingService.LogInfo("Return: Doc is null:{0}", ex.ToString());
                }

                LoggingService.LogInfo("Get Document:{0}", doc == null ? "null" : doc.Url);

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
                                    LoggingService.LogInfo("Error:Get List DocReadStatus");
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

                                LoggingService.LogInfo("Get status from status list: Count:{0}", items.Count);

                                site.AllowUnsafeUpdates = true;
                                web.AllowUnsafeUpdates = true;

                                string userId = SPContext.Current.Web.CurrentUser.ID.ToString();
                                if (items.Count == 0)
                                {
                                    LoggingService.LogInfo("Inserting status into list: Title:{0}\tViewPeople:{1}", itemURL, userId);

                                    SPListItem item = readStatusList.AddItem();
                                    item["Title"] = itemURL;
                                    item["ViewPeople"] = ";" + userId + ";";
                                    item.Update();
                                }
                                //ITEM COUNT SHOULD BE EQUEAL TO 1
                                else
                                {
                                    LoggingService.LogInfo("Updating status into list: Title:{0}\tViewPeople:{1}", itemURL, userId);
                                    //http://blogit.create.pt/miguelisidoro/2008/06/07/sharepoint-2007-value-does-not-fall-within-the-expected-range-when-updating-an-splistitem-in-a-search/
                                    SPListItem item = items[0].ParentList.GetItemById(items[0].ID);
                                    string peopleIds = item["ViewPeople"].ToString() + ";";
                                    if (!peopleIds.Contains(";" + userId + ";"))
                                        item["ViewPeople"] = peopleIds + userId + ";";
                                    item.Update();
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
                LoggingService.LogError(ex.Message, ex.StackTrace);
            }
        }
    }
}
