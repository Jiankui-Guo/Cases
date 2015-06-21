using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.SharePoint;

namespace DocumentReadStatus.HttpModule
{
    class DocumentAuditDataModule:IHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += WriteAuditData;
        }

        private void WriteAuditData(object sender, EventArgs e)
        {
            if (SPContext.Current.List.BaseType != SPBaseType.DocumentLibrary)
                return;

            SPItem doc = SPContext.Current.ListItem;
            if (doc != null)
            {
                string xmlData = "<AuditInfo><User>{0}</User><Item>{0}</Item></AuditInfo>";
                xmlData = string.Format(xmlData, SPContext.Current.Web.CurrentUser.ID,
                    SPContext.Current.ListItem.ID);
                SPContext.Current.Site.Audit.WriteAuditEvent(SPAuditEventType.View, "DocumentStatus", xmlData);
            }
        }
    }
}
