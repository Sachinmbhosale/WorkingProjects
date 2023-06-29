/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * -----------------------------------------------------------------------------------------------------------------------------------
 * 21 Mar 2015      Yogeesha Naik       DMS04-3459	        Renamed page name (original aspx page name) 
 * 18 Apr 2015      Yogeesha Naik       DMS5-3933	        Menu filtering and redirecting page
 * 27 Apr 2015      Yogeesha Naik       DMS5-4052	        Menu - Re factor menu implementation
====================================================================================================================================== */

using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using System.Collections;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class DocumentMaster : MasterPageBase
    {
        #region Page Laod
        protected void Page_Load(object sender, EventArgs e)
        {


            imgLogo.Src = GetCompanyLogoPath();
            imgLogo.Alt = GetCompanyName();
            UserBase user = (UserBase)Session["LoggedUser"];
            lblUser.Text = "Hi! " + user.FirstName + " " + user.LastName;            

            DocumentViewBL objDocumentViewBL = new DocumentViewBL();
            DataSet dsData = new DataSet();

            try
            {

                if (Session["LoggedUser"] != null)
                {
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                    hdnLoginToken.Value = loginUser.LoginToken;


                    if (!IsPostBack)
                    {
                        LoadMenus(mnuMain); // DMS5-4052 M - Removed old methods and created new.
                        dsData = objDocumentViewBL.GetAllTreeViewData(hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value), "", 0, 0, 0, 0);
                        TreeviewBind(dsData, tvDocument);
                        gvDocument.Visible = false;

                    }

                }
            }
            catch (Exception)
            {


            }
            TodayDate.Text = DateTime.Now.ToString("yyyy-MM-dd hh:mm:tt");


        }
        #endregion


        protected void mnuMain_MenuItemClick(object sender, MenuEventArgs e)
        {
            Session["CurrentReportSubMenu"] = null;
        }

        protected void lnkChangepassword_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Default/ChangePassword.aspx", false);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Accounts/LogOut.aspx?msg=Logout", false);
        }

        public void ClearApplicationCache()
        {
            List<string> keys = new List<string>();


            // retrieve application Cache enumerator
            IDictionaryEnumerator enumerator = Cache.GetEnumerator();


            // copy all keys that currently exist in Cache
            while (enumerator.MoveNext())
            {
                keys.Add(enumerator.Key.ToString());
            }


            // delete every key from cache
            for (int i = 0; i < keys.Count; i++)
            {
                Cache.Remove(keys[i]);
            }
        }

        #region TreeView

        /// <summary>
        /// Create treeview 
        /// </summary>
        /// <param name="dsData"></param>
        /// <param name="tvDocument"></param>
        public void TreeviewBind(DataSet dsData, TreeView tvDocument)
        {
            try
            {
                tvDocument.Nodes.Clear();
                if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dsData.Tables[0].Rows)
                    {
                        TreeNode node = new TreeNode(row.ItemArray[1].ToString());
                        node.Value = row.ItemArray[0].ToString();
                        node.ToolTip = row.ItemArray[3].ToString();
                        tvDocument.Nodes.Add(node);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Function to search in treeview the node exist or not
        /// </summary>
        /// <param name="nodetext">node name</param>
        /// <param name="trv">treeview</param>
        /// <returns></returns>
        private TreeNode Searchnode(string nodetext, TreeView trv)
        {
            TreeNode node1 = null;
            foreach (TreeNode node in trv.Nodes)
            {
                if (node.Value == nodetext)
                {
                    node1 = node;
                }
            }
            return node1;
        }

        protected void tvDocument_SelectedNodeChanged(object sender, EventArgs e)
        {
            DocumentViewBL objDocumentViewBL = new DocumentViewBL();
            DataSet dsData = new DataSet();

            try
            {
                TreeNode CurrentNode = ((TreeView)sender).SelectedNode;
                if (CurrentNode != null)
                {
                    string CurrentNodeId = CurrentNode.Value;
                    Session["CurrentNodeId"] = CurrentNodeId;
                    string CurrentNodeParentId = CurrentNode.Parent != null ? CurrentNode.Parent.Value : "0";
                    Session["CurrentNodeParentId"] = CurrentNodeParentId;
                    string CurrentNodeTootip = CurrentNode.ToolTip;
                    Session["CurrentNodeTootip"] = CurrentNodeTootip;
                    int DocumentTypeId = 0;
                    int DepartmentId = 0;

                    if (CurrentNodeTootip == "Department")
                    {
                        DocumentTypeId = Convert.ToInt32(CurrentNode.Parent.Value);
                        ViewState["DocumentType"] = DocumentTypeId;
                        Session["DocumentTypeId"] = DocumentTypeId;
                        DepartmentId = Convert.ToInt32(CurrentNode.Value);
                        ViewState["Department"] = DepartmentId;
                        Session["DepartmentId"] = DepartmentId;
                    }
                    else if (CurrentNodeTootip == "Main Tag")
                    {
                        if (ViewState["DocumentType"] != null)
                        {
                            DocumentTypeId = Convert.ToInt32(ViewState["DocumentType"]);
                            ViewState["DocumentType"] = DocumentTypeId;
                        }
                        DepartmentId = Convert.ToInt32(CurrentNode.Parent.Value);
                        ViewState["Department"] = DepartmentId;
                    }
                    else if (CurrentNodeTootip == "Sub Tag")
                    {
                        if (ViewState["DocumentType"] != null)
                        {
                            DocumentTypeId = Convert.ToInt32(ViewState["DocumentType"]);
                            ViewState["DocumentType"] = DocumentTypeId;
                        }
                        if (ViewState["Department"] != null)
                        {
                            DepartmentId = Convert.ToInt32(CurrentNode.Parent.Value);
                            ViewState["Department"] = DepartmentId;
                        }
                    }
                    dsData = objDocumentViewBL.GetAllTreeViewData(hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value), CurrentNodeTootip, Convert.ToInt32(CurrentNodeId), Convert.ToInt32(CurrentNodeParentId), DocumentTypeId, DepartmentId);
                    if (dsData != null && dsData.Tables.Count > 0 && dsData.Tables[0].Rows.Count > 0)
                    {
                        // Clear all child nodes
                        CurrentNode.ChildNodes.Clear();

                        foreach (DataRow row in dsData.Tables[0].Rows)
                        {
                            TreeNode node = new TreeNode(row.ItemArray[1].ToString());
                            node.Value = row.ItemArray[0].ToString();
                            node.ToolTip = row.ItemArray[3].ToString();
                            CurrentNode.ChildNodes.Add(node);

                        }



                    }

                    Session["lastNode"] = tvDocument.SelectedNode;
                    CurrentNode.ExpandAll();
                }

                //if (contentCallEvent != null)
                //    contentCallEvent(this, EventArgs.Empty);

                //foreach (TreeNode t in tvDocument.Nodes)
                //{
                //    for (int iParent = 0; iParent < t.ChildNodes.Count; iParent++)
                //    {
                //        for (int iChild = 0; iChild < t.ChildNodes[iParent].ChildNodes.Count; iChild++)
                //        {
                //            if (t.ChildNodes[iParent].ChildNodes[iChild].ChildNodes.Count > 0)
                //            {
                //                Response.Redirect("DocumentView.aspx", false);
                //            }
                //        }
                //    }
                //}

                BindTreeviewGridview();
            }
            catch (Exception)
            {


            }



        }

        #endregion


        #region document grid

        DocumentViewBL objDocumentViewBL = new DocumentViewBL();
        DataSet dsData = new DataSet();
        string CurrentNodeTootip;
        int CurrentNodeId, CurrentNodeParentId, DocumentTypeId, DepartmentId;

        protected void gvDocument_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {

                //DMS04-3436BS
                //Hide unnecessary columns- 1:TotalRows, 3:MainTag, 4:SubTag, 6:ProcessId, 11: Deleted
                e.Row.Cells[1].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[6].Visible = false;
                e.Row.Cells[11].Visible = false;
                //DMS04-3436BE
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

            dsData = objDocumentViewBL.GetAllTreeViewData(hdnLoginToken.Value, Convert.ToInt32(hdnLoginOrgId.Value), CurrentNodeTootip != null ? CurrentNodeTootip : string.Empty,
                     Session["CurrentNodeId"] != null ? CurrentNodeId : 0, Session["CurrentNodeParentId"] != null ? CurrentNodeParentId : 0,
                       Session["DocumentTypeId"] != null ? DocumentTypeId : 0, Session["DepartmentId"] != null ? DepartmentId : 0);
            if (dsData.Tables.Count > 1 && dsData.Tables[1].Rows.Count > 0)
            {
                ViewState["GridSource"] = dsData.Tables[1];
                gvDocument.DataSource = dsData.Tables[1];
                gvDocument.DataBind();
                ContentPlaceHolder1.Visible = false;
                gvDocument.Visible = true;
            }
            else
            {
                ViewState["GridSource"] = null;
                gvDocument.DataSource = null;
                gvDocument.DataBind();
            }
        }
        #endregion
    }
}
