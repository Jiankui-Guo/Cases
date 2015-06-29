<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocStatusWebPart.ascx.cs" Inherits="DocumentReadStatus.DocStatusWebPart.DocStatusWebPart" %>
<SharePoint:ScriptLink ID="jQuery" Name="/_layouts/15/DocumentReadStatus/jquery-1.11.3.min.js" runat="server" Localizable="false" />
<SharePoint:ScriptBlock runat="server">
var clientContext;
var collListItem;
var web;
var userId;
 
//_spPageContextInfo
function onQuerySucceeded(sender, args)
{
    var items = [];
    var listItemEnumerator = collListItem.getEnumerator();
    while(listItemEnumerator.moveNext())
    {
        var oListItem = listItemEnumerator.get_current();
        var item = oListItem.get_item('Title');
        items.push(item);
        //alert(item);
    }
  
    var rows = $('tr[class*="ms-itmhover"] td[class*="ms-vb-title"]');
    //to be used to detect if it is a folder
    //var icons = $('tr[class*="ms-itmhover"] td[class*="ms-vb-icon"]');
    rows.each(function () {
       if(this.innerHTML.toLowerCase().indexOf("handlefolder") >=0) {
            return;
        }

        var bViewed = false;
        for (var i = 0; i < items.length; i++) {
            var title = items[i];
            if (this.innerHTML.toLowerCase().indexOf(title.toLowerCase()) >= 0) {
                bViewed = true;
                break;
            }
        }
        if (!bViewed) {
            $(this).closest("tr").css("font-weight", "bold");
        }
    }, null);
}
    
function onQueryFailed(sender, args)
{
    alert('Request failed' + args.get_message() + '\n' + arg.get_stackTrace());
}
 
function onQueryUserSucceeded(sender, args) {
    userId = web.get_currentUser().get_id();
    //alert(userId);
    var list = web.get_lists().getByTitle('DocReadStatus');
    var camlQuery = new SP.CamlQuery();
    var strCaml = "<View><ViewFields><FieldRef Name='Title'/></ViewFields>"
        + "<Query><Where><Contains><FieldRef Name='ViewPeople' />"
        + "<Value Type='Text'>" + ";" +userId + ";" + "</Value></Contains></Where></Query></View>";
 
    camlQuery.set_viewXml(strCaml);
    collListItem = list.getItems(camlQuery);
    clientContext.load(collListItem);
    clientContext.executeQueryAsync(onQuerySucceeded, onQueryFailed);
}
 
function onQueryUserFailed(sender, args) {
    alert('Request failed' + args.get_message() + '\n' + args.get_stackTrace());
}
 
function getWebUserData() {
    clientContext = SP.ClientContext.get_current();
    web = clientContext.get_web();
    user = web.get_currentUser();
    user.retrieve();
    clientContext.load(web);
    clientContext.executeQueryAsync(onQueryUserSucceeded, onQueryUserFailed);
}
 
$(document).ready(function () {
    //alert('start');
    ExecuteOrDelayUntilScriptLoaded(getWebUserData, "sp.js");
});
</SharePoint:ScriptBlock>
