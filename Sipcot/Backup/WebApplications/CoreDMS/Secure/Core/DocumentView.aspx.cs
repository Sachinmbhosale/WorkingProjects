using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class DocumentView : PageBase
    {
        DocumentViewBL objDocumentViewBL = new DocumentViewBL();
        DataSet dsData = new DataSet();
        string CurrentNodeTootip;
        int CurrentNodeId, CurrentNodeParentId, DocumentTypeId, DepartmentId;
        protected void Page_PreInit(object sender, EventArgs e)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;

            if (Session["CurrentNodeTootip"] != null)
            {
                CurrentNodeTootip = (string)(Session["CurrentNodeTootip"]);
            }

            if ((Session["CurrentNodeId"]) != null)
            {
                CurrentNodeId = Convert.ToInt32(Session["CurrentNodeId"]);
            }

            if ((Session["CurrentNodeParentId"]) != null)
            {
                CurrentNodeParentId = Convert.ToInt32(Session["CurrentNodeParentId"]);
            }

            if ((Session["DocumentTypeId"]) != null)
            {
                DocumentTypeId = Convert.ToInt32(Session["DocumentTypeId"]);
            }

            if ((Session["DepartmentId"]) != null)
            {
                DepartmentId = Convert.ToInt32(Session["DepartmentId"]);
            }
            BindTreeviewGridview();


        }
        protected void gvDocument_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType != DataControlRowType.DataRow) return;
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    string encoded = e.Row.Cells[i].Text;
                    e.Row.Cells[i].Text = Context.Server.HtmlDecode(encoded);
                }

            }
            catch (Exception)
            {


            }
        }

        protected void gvDocument_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //CheckAuthentication();
            gvDocument.PageIndex = e.NewPageIndex;
            if (ViewState["GridSource"] != null)
            {
                gvDocument.DataSource = ViewState["GridSource"];
                gvDocument.DataBind();
            }
        }

        protected void BindTreeviewGridview()
        {
            dsData = objDocumentViewBL.GetAllTreeViewData(hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value), CurrentNodeTootip != null ? CurrentNodeTootip : string.Empty,
                     Session["CurrentNodeId"] != null ? CurrentNodeId : 0, Session["CurrentNodeParentId"] != null ? CurrentNodeParentId : 0,
                       Session["DocumentTypeId"] != null ? DocumentTypeId : 0, Session["DepartmentId"] != null ? DepartmentId : 0);
            if (dsData.Tables.Count > 1 && dsData.Tables[1].Rows.Count > 0)
            {
                ViewState["GridSource"] = dsData.Tables[1];
                gvDocument.DataSource = dsData.Tables[1];
                gvDocument.DataBind();
            }
            else
            {
                ViewState["GridSource"] = null;
                gvDocument.DataSource = null;
                gvDocument.DataBind();
            }
        }
    }
}