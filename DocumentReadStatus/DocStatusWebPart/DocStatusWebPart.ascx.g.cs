﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DocumentReadStatus.DocStatusWebPart {
    using System.Web.UI.WebControls.Expressions;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Text;
    using System.Web.UI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.SessionState;
    using System.Configuration;
    using Microsoft.SharePoint;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.ComponentModel.DataAnnotations;
    using System.Web.UI.WebControls;
    using System.Web.Security;
    using System;
    using Microsoft.SharePoint.Utilities;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint.WebControls;
    
    
    public partial class DocStatusWebPart {
        
        protected global::Microsoft.SharePoint.WebControls.ScriptLink jQuery;
        
        public static implicit operator global::System.Web.UI.TemplateControl(DocStatusWebPart target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::Microsoft.SharePoint.WebControls.ScriptLink @__BuildControljQuery() {
            global::Microsoft.SharePoint.WebControls.ScriptLink @__ctrl;
            @__ctrl = new global::Microsoft.SharePoint.WebControls.ScriptLink();
            this.jQuery = @__ctrl;
            @__ctrl.ID = "jQuery";
            @__ctrl.Name = "/_layouts/15/DocumentReadStatus/jquery-1.11.3.min.js";
            @__ctrl.Localizable = false;
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::Microsoft.SharePoint.WebControls.ScriptBlock @__BuildControl__control2() {
            global::Microsoft.SharePoint.WebControls.ScriptBlock @__ctrl;
            @__ctrl = new global::Microsoft.SharePoint.WebControls.ScriptBlock();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\nvar clientContext;\r\nvar collListItem;\r\nvar web;\r\nvar userId;\r\n \r\n//_spPageConte" +
                        "xtInfo\r\nfunction onQuerySucceeded(sender, args)\r\n{\r\n    var items = [];\r\n    var" +
                        " listItemEnumerator = collListItem.getEnumerator();\r\n    while(listItemEnumerato" +
                        "r.moveNext())\r\n    {\r\n        var oListItem = listItemEnumerator.get_current();\r" +
                        "\n        var item = oListItem.get_item(\'Title\');\r\n        items.push(item);\r\n   " +
                        "     //alert(item);\r\n    }\r\n  \r\n    var rows = $(\'tr[class*=\"ms-itmhover\"] td[cl" +
                        "ass*=\"ms-vb-title\"]\');\r\n    rows.each(function () {\r\n        var bViewed = false" +
                        ";\r\n        for (var i = 0; i < items.length; i++) {\r\n            var title = ite" +
                        "ms[i];\r\n            if (this.innerHTML.toLowerCase().indexOf(title.toLowerCase()" +
                        ") >= 0) {\r\n                bViewed = true;\r\n                break;\r\n            " +
                        "}\r\n        }\r\n        if (!bViewed) {\r\n            $(this).closest(\"tr\").css(\"fo" +
                        "nt-weight\", \"bold\");\r\n        }\r\n    }, null);\r\n}\r\n    \r\nfunction onQueryFailed(" +
                        "sender, args)\r\n{\r\n    alert(\'Request failed\' + args.get_message() + \'\\n\' + arg.g" +
                        "et_stackTrace());\r\n}\r\n \r\nfunction onQueryUserSucceeded(sender, args) {\r\n    user" +
                        "Id = web.get_currentUser().get_id();\r\n    //alert(userId);\r\n    var list = web.g" +
                        "et_lists().getByTitle(\'DocReadStatus\');\r\n    var camlQuery = new SP.CamlQuery();" +
                        "\r\n    var strCaml = \"<View><ViewFields><FieldRef Name=\'Title\'/></ViewFields>\"\r\n " +
                        "       + \"<Query><Where><Contains><FieldRef Name=\'ViewPeople\' />\"\r\n        + \"<V" +
                        "alue Type=\'Text\'>\" + \";\" +userId + \";\" + \"</Value></Contains></Where></Query></V" +
                        "iew>\";\r\n \r\n    camlQuery.set_viewXml(strCaml);\r\n    collListItem = list.getItems" +
                        "(camlQuery);\r\n    clientContext.load(collListItem);\r\n    clientContext.executeQu" +
                        "eryAsync(onQuerySucceeded, onQueryFailed);\r\n}\r\n \r\nfunction onQueryUserFailed(sen" +
                        "der, args) {\r\n    alert(\'Request failed\' + args.get_message() + \'\\n\' + arg.get_s" +
                        "tackTrace());\r\n}\r\n \r\nfunction getWebUserData() {\r\n    clientContext = SP.ClientC" +
                        "ontext.get_current();\r\n    web = clientContext.get_web();\r\n    user = web.get_cu" +
                        "rrentUser();\r\n    user.retrieve();\r\n    clientContext.load(web);\r\n    clientCont" +
                        "ext.executeQueryAsync(onQueryUserSucceeded, onQueryUserFailed);\r\n}\r\n \r\n$(documen" +
                        "t).ready(function () {\r\n    //alert(\'start\');\r\n    ExecuteOrDelayUntilScriptLoad" +
                        "ed(getWebUserData, \"sp.js\");\r\n});\r\n"));
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControlTree(global::DocumentReadStatus.DocStatusWebPart.DocStatusWebPart @__ctrl) {
            global::Microsoft.SharePoint.WebControls.ScriptLink @__ctrl1;
            @__ctrl1 = this.@__BuildControljQuery();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(@__ctrl1);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
            global::Microsoft.SharePoint.WebControls.ScriptBlock @__ctrl2;
            @__ctrl2 = this.@__BuildControl__control2();
            @__parser.AddParsedSubObject(@__ctrl2);
            @__parser.AddParsedSubObject(new System.Web.UI.LiteralControl("\r\n"));
        }
        
        private void InitializeControl() {
            this.@__BuildControlTree(this);
            this.Load += new global::System.EventHandler(this.Page_Load);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual object Eval(string expression) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual string Eval(string expression, string format) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression, format);
        }
    }
}