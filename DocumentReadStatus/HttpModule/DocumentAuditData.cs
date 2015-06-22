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
    class DocumentAuditDataModule:IHttpModule
    {
        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += WriteAuditData;
        }

        private void WriteAuditData(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            string itemURL = HttpUtility.UrlDecode(HttpContext.Current.Request.RawUrl);
            Guid listId = Guid.Empty;
            Guid itemId = Guid.Empty;

            if (web == null)
                return;

            DataTable result = new DataTable();

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (SqlConnection conn = new SqlConnection(web.Site.ContentDatabase.DatabaseConnectionString))
                {
                    string cmdString =
                        string.Format("SELECT Id,ListId,DirName,LeafName FROM AllDocs WITH(NOLOCK) WHERE LeafName='{0}'",
                        itemURL.Substring(itemURL.LastIndexOf('/') + 1));
                    using (SqlCommand comm = new SqlCommand(cmdString,conn))
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

            SPList list = web.Lists.GetList(listId,true);
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
                string xmlData = "<AuditInfo><User>{0}</User><Item>{1}</Item></AuditInfo>";
                xmlData = string.Format(xmlData, SPContext.Current.Web.CurrentUser.ID, itemURL);

                doc.Audit.WriteAuditEvent(SPAuditEventType.View, "DocumentStatus", xmlData);
            }

            result = null;
        }
    }
}
