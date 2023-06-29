using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class SearchTemplates : PageBase
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            CheckAuthentication();
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                hdnPageId.Value = "0";
                string pageRights = GetPageRights();
                hdnPageRights.Value = pageRights;
                ApplyPageRights(pageRights, this.Form.Controls);

                //LoadTemplateNames(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                //drpTemplates.Focus();
            }

        }
        //public void LoadTemplateNames(string loginOrgId, string loginToken)
        //{
        //    drpTemplates.Items.Clear();
        //    drpTemplates.Items.Add(new ListItem("<select>", "0"));
        //    TemplateBL bl = new TemplateBL();
        //    SearchFilter filter = new SearchFilter();
        //    Results res = bl.SearchTemplates(filter, "SearchTemplates", loginOrgId, loginToken);
        //    if (res.Templates != null)
        //    {

        //        foreach (Template tpl in res.Templates)
        //        {
        //            drpTemplates.Items.Add(new ListItem(tpl.TemplateName, tpl.TemplateId.ToString()));
        //        }
        //    }
        //}
    }
}
