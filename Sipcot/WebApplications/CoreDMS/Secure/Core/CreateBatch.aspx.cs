/* ============================================================================  
Author     : Pratheesh A
Create date: 02 Dec 2013
Description: Enhancement 2 - Sprint 2 - Create Batch
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
=============================================================================== */
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class CreateBatch : PageBase
    {
       
        protected int currentPageNumber;
        protected int PAGE_SIZE;
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";
            if (!IsPostBack)
            {

                GetDocumentType(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);


                string pageRights = GetPageRights();
                hdnPageRights.Value = pageRights;
                ApplyPageRights(pageRights, this.Form.Controls);

                // Managing data in viewstate
                Createtemptable();

                BatchUploadBL bal = new BatchUploadBL();
                DataSet ds = bal.ComboFillerBySP("USP_COMBOFILLER_PAGE_SIZE");
                ddlRows.DataTextField = "_Name";
                ddlRows.DataValueField = "_ID";
                ddlRows.DataSource = ds;
                ddlRows.DataBind();

                ddlRows.SelectedValue = "10";

                PageDiv.Visible = false;

                btnProcess.Attributes.Add("Style", "visibility:hidden");
                btnDelete.Attributes.Add("Style", "visibility:hidden");
                //To make visible two checkbox
                chkSelectPage.Attributes.Add("Style", "visibility:hidden");
                chkSelectAll.Attributes.Add("Style", "visibility:hidden");
                Statuspanel.Attributes.Add("Style", "visibility:hidden");
            }
        }

        public void Createtemptable()
        {
            DataTable DT = new DataTable();
            DT.Columns.Add("UploadDocID", typeof(int));
            ViewState["TempDT"] = DT;
        }



        public void GetDepartments(string loginOrgId, string loginToken)
        {

            DepartmentBL bl = new DepartmentBL();
            SearchFilter filter = new SearchFilter();
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            try
            {
                string action = "DepartmentsForUpload";
                Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.Departments != null)
                {
                    cmbDepartment.Items.Clear();
                    cmbDepartment.Items.Add(new ListItem("--Select--", "0"));
                    foreach (Department dp in res.Departments)
                    {
                        cmbDepartment.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetDocumentType(string loginOrgId, string loginToken)
        {

            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "DocumentTypeForUpload";
                Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    cmbDocumentType.Items.Clear();
                    cmbDocumentType.Items.Add(new ListItem("--Select--", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetBatch(string loginOrgId, string loginToken)
        {
            Results result = new Results();
            BatchUploadBL bal = new BatchUploadBL();
            SearchFilter filter = new SearchFilter();
            filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            result = bal.ManageUploadBatches(filter, "GetBatches", loginOrgId, loginToken);

            if (result.ResultDS != null && result.ResultDS.Tables.Count > 0)
            {
                cmbBatchName.DataTextField = "TextField";
                cmbBatchName.DataValueField = "ValueField";
                cmbBatchName.DataSource = result.ResultDS;
                cmbBatchName.DataBind();
                btnProcess.Attributes.Add("Style", "visibility:visible");
                btnDelete.Attributes.Add("Style", "visibility:hidden");
            }
        }

        #region "Page Events"

        public void SetAction()
        {
            if (cmbBatchName.SelectedValue != "0")
            {
                hdnBatchatcion.Value = "GetDataForBatchEdit";
            }
            else
            {
                hdnBatchatcion.Value = "GetDataForBatchCreation";
            }
        }

        protected void GetPageIndex(object sender, CommandEventArgs e)
        {

            SetAction();
            switch (e.CommandName)
            {
                case "First":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = 1;
                    break;

                case "Previous":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(ddlPage.SelectedValue) - 1;
                    break;

                case "Next":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(ddlPage.SelectedValue) + 1;
                    break;

                case "Last":
                    PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
                    currentPageNumber = Int32.Parse(lblTotalPages.Text);
                    break;
            }

            pBindData(null, false);
        }

        protected void ddlRows_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentPageNumber = 1;
            grdView.PageSize = Int32.Parse(ddlRows.SelectedValue);
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            SetAction();
            pBindData(null, false);
        }

        protected void ddlPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
            currentPageNumber = Int32.Parse(ddlPage.SelectedValue);
            SetAction();
            pBindData(null, false);
        }

        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                CheckBox chkbox = (CheckBox)e.Row.FindControl("chkSelected");

                for (int i = 1; i < e.Row.Cells.Count; i++)
                {
                    string encoded = e.Row.Cells[i].Text;
                    e.Row.Cells[i].Text = Context.Server.HtmlDecode(encoded);

                    e.Row.Cells[1].Visible = false; // Total Rows
                    e.Row.Cells[3].Visible = false; // Document Id /Process Id
                    e.Row.Cells[4].Visible = false; // Checkbox checked status
                    if (chkDisplaySelected.Checked)
                    {
                        grdView.HeaderRow.Cells[0].Visible = false;
                        e.Row.Cells[0].Visible = false;
                    }
                    DataTable DT = (DataTable)ViewState["TempDT"];
                    if (e.Row.Cells[i].Text == "CHECKED")
                    {
                        DT.Rows.Add(Convert.ToInt32(e.Row.Cells[3].Text));
                        DT.AcceptChanges();
                        chkbox.Checked = true;
                        ViewState["TempDT"] = DT;
                    }
                    else
                    {
                        if (DT.Rows.Count > 0 && DT != null)
                        {
                            foreach (DataRow row in DT.Rows)
                            {
                                if (e.Row.Cells[3].Text == row["UploadDocID"].ToString())
                                {
                                    chkbox.Checked = true;
                                }
                            }
                        }
                    }
                }


                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    e.Row.Cells[i].CssClass = "GridItem1";
                    if (e.Row.Cells[i].Text.Trim() != "&nbsp;")
                    {
                        e.Row.Cells[i].Attributes.Add("title", e.Row.Cells[i].Text);
                    }
                }
                e.Row.Attributes.Add("onmouseover", "javascript:this.className = 'GridRowHover'");
                e.Row.Attributes.Add("onmouseout", "javascript:this.className = ''");
                e.Row.TabIndex = -1;
                e.Row.Attributes["onclick"] = string.Format("javascript:SelectRow(this, {0});", e.Row.RowIndex);
                e.Row.Attributes["onkeydown"] = "javascript:return SelectSibling(event);";
                e.Row.Attributes["onselectstart"] = "javascript:return false;";
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Visible = false; // Total Rows
                e.Row.Cells[3].Visible = false; // Document Id /Process Id
                e.Row.Cells[4].Visible = false; // Checkbox checked status
            }


        }

        protected void grdView_Sorting(object sender, GridViewSortEventArgs e)
        {
            if (string.Compare(Convert.ToString(ViewState["SortOrder"]), " ASC", true) == 0)
            {
                ViewState["SortOrder"] = " DESC";
            }
            else
            {
                ViewState["SortOrder"] = " ASC";
            }
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
            currentPageNumber = Int32.Parse(ddlPage.SelectedValue);  // Added later
            pBindData("[" + e.SortExpression + "]" + ViewState["SortOrder"], false);
        }

        #endregion

        #region Private Method

        private int GetTotalPages(double totalRows)
        {
            int totalPages = (int)Math.Ceiling(totalRows / PAGE_SIZE);

            return totalPages;
        }

        private DataTable Getgriddata(DataTable iDt, string action)
        {

            DataTable dt = new DataTable();

            if (chkDisplaySelected.Checked)
            {
                DataView cdv = new DataView(iDt);
                cdv.RowFilter = ("[Checked] = 'CHECKED'");
                iDt = cdv.ToTable();

            }

            if (action != "All")
            {
                DataView dv = new DataView(iDt);
                dv.RowFilter = ("[Upload Status] = '" + action + "'");

                dt = dv.ToTable();
            }
            else
            {
                dt = iDt;
            }


            return dt;
        }

        private void pBindData(string aSortExp, bool aIsCompleteData)
        {
            Results result = new Results();
            BatchUploadBL bal = new BatchUploadBL();
            SearchFilter filter = new SearchFilter();
            DataSet ds = null;
            DataTable dt = new DataTable();
            try
            {
                Logger.Trace("pBindData started", Session["LoggedUserId"].ToString());
                // Clear grid data
                grdView.DataSource = null;
                grdView.DataBind();

                filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                if (cmbBatchName.SelectedValue != "0")
                {
                    filter.BatchId = cmbBatchName.SelectedValue != null ? Convert.ToInt32(cmbBatchName.SelectedValue) : 0;
                    filter.BatchName = cmbBatchName.SelectedItem.Text;
                }
                else
                {
                    filter.BatchId = 0;
                    filter.BatchName = "";
                }
                filter.startRowIndex = ((currentPageNumber - 1) * PAGE_SIZE) + 1;
                filter.endRowIndex = (aIsCompleteData == true ? -1 : (currentPageNumber * PAGE_SIZE));
              //  filter.Batchaction = hdnBatchatcion.Value;

                result = bal.ManageUploadBatches(filter, hdnBatchatcion.Value, hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                ds = result.ResultDS;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    if (ddlstatus.SelectedItem.ToString() == "All")
                    {
                        dt = Getgriddata(ds.Tables[0], "All");
                        grdView.DataSource = dt;
                        grdView.DataBind();
                    }
                    else if (ddlstatus.SelectedItem.ToString() == "Uploaded")
                    {
                        dt = Getgriddata(ds.Tables[0], "Uploaded");
                        grdView.DataSource = dt;
                        grdView.DataBind();
                    }
                    else if (ddlstatus.SelectedItem.ToString() == "Pending")
                    {
                        dt = Getgriddata(ds.Tables[0], "Pending");
                        grdView.DataSource = dt;
                        grdView.DataBind();
                    }
                    else if (ddlstatus.SelectedItem.ToString() == "NotUploaded")
                    {
                        dt = Getgriddata(ds.Tables[0], "Not Uploaded");
                        grdView.DataSource = dt;
                        grdView.DataBind();
                    }


                    //grdView.DataSource = ds.Tables[0];
                    //grdView.DataBind();
                    PageDiv.Visible = true;
                    ///get the total rows 
                    double totalRows = 0;

                    totalRows = filter.PagingTotalRows;

                    lblTotalPages.Text = GetTotalPages(totalRows).ToString();

                    ddlPage.Items.Clear();
                    for (int i = 1; i < Convert.ToInt32(lblTotalPages.Text) + 1; i++)
                    {
                        ddlPage.Items.Add(new ListItem(i.ToString()));
                    }

                    ddlPage.SelectedValue = currentPageNumber.ToString();

                    if (currentPageNumber == 1)
                    {
                        lnkbtnPre.Enabled = false;
                        lnkbtnPre.CssClass = "GridPagePreviousInactive";
                        lnkbtnFirst.Enabled = false;
                        lnkbtnFirst.CssClass = "GridPageFirstInactive";

                        if (Int32.Parse(lblTotalPages.Text) > 1)
                        {
                            lnkbtnNext.Enabled = true;
                            lnkbtnNext.CssClass = "GridPageNextActive";
                            lnkbtnLast.Enabled = true;
                            lnkbtnLast.CssClass = "GridPageLastActive";
                        }
                        else
                        {
                            lnkbtnNext.Enabled = false;
                            lnkbtnNext.CssClass = "GridPageNextInactive";
                            lnkbtnLast.Enabled = false;
                            lnkbtnLast.CssClass = "GridPageLastInactive";
                        }
                    }
                    else
                    {
                        chkSelectPage.Checked = false; //NewCode
                        lnkbtnPre.Enabled = true;
                        lnkbtnPre.CssClass = "GridPagePreviousActive";
                        lnkbtnFirst.Enabled = true;
                        lnkbtnFirst.CssClass = "GridPageFirstActive";

                        if (currentPageNumber == Int32.Parse(lblTotalPages.Text))
                        {
                            lnkbtnNext.Enabled = false;
                            lnkbtnNext.CssClass = "GridPageNextInactive";
                            lnkbtnLast.Enabled = false;
                            lnkbtnLast.CssClass = "GridPageLastInactive";
                        }
                        else
                        {
                            lnkbtnNext.Enabled = true;
                            lnkbtnNext.CssClass = "GridPageNextActive";
                            lnkbtnLast.Enabled = true;
                            lnkbtnLast.CssClass = "GridPageLastActive";
                        }
                    }
                }
                else
                {
                    PageDiv.Visible = false;
                }
                Logger.Trace("pBindData finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        #endregion

        public void SelectPageWise()
        {
            foreach (GridViewRow row in grdView.Rows)
            {
                int UploadDocID = Convert.ToInt32(row.Cells[3].Text);
                DataTable DT = (DataTable)ViewState["TempDT"];
                CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkSelected");
                if (chkSelectPage.Checked == true)
                {
                    ChkBoxRows.Checked = true;
                    DT.Rows.Add(UploadDocID);
                }
                else
                {
                    ChkBoxRows.Checked = false;
                    for (int i = DT.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = DT.Rows[i];
                        if (dr["UploadDocID"].ToString() == UploadDocID.ToString())
                        {
                            dr.Delete();
                        }
                    }
                }
                DT.AcceptChanges();
                ViewState["TempDT"] = DT;
            }
        }

        protected void cmbDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeleteDataFromDT();
            GetDepartments(hdnLoginOrgId.Value, hdnLoginToken.Value);
            divMsg.InnerHtml = "";
            Statuspanel.Attributes.Add("Style", "visibility:hidden");
            chkDisplaySelected.Checked = false;
            if (cmbDocumentType.SelectedValue != "0" && cmbDepartment.SelectedValue != "0")
            {
                GetBatch(hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                lblMessage.Text = "";
                currentPageNumber = 1;
                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                ViewState["SortOrder"] = "ASC";
                hdnBatchatcion.Value = "GetDataForBatchCreation";
                pBindData(null, false);
            }
            else
            {
                cmbBatchName.Items.Clear();
                cmbBatchName.Items.Add(new ListItem("--Select--", "0"));
                btnProcess.Attributes.Add("Style", "visibility:hidden");
                btnDelete.Attributes.Add("Style", "visibility:hidden");
                grdView.DataSource = null;
                grdView.DataBind();
                PageDiv.Visible = false;

            }
        }

        protected void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            //To make visible two checkbox
            chkSelectPage.Attributes.Add("Style", "visibility:visible");
            chkSelectAll.Attributes.Add("Style", "visibility:visible");
            DeleteDataFromDT();
            Statuspanel.Attributes.Add("Style", "visibility:hidden");
            chkDisplaySelected.Checked = false;
            divMsg.InnerHtml = "";
            if (cmbDocumentType.SelectedValue != "0" && cmbDepartment.SelectedValue != "0")
            {
                GetBatch(hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                lblMessage.Text = "";
                currentPageNumber = 1;
                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                ViewState["SortOrder"] = "ASC";
                hdnBatchatcion.Value = "GetDataForBatchCreation";
                pBindData(null, false);
            }
            else
            {
                cmbBatchName.Items.Clear();
                cmbBatchName.Items.Add(new ListItem("--Select--", "0"));
                btnProcess.Attributes.Add("Style", "visibility:hidden");
                btnDelete.Attributes.Add("Style", "visibility:hidden");
                grdView.DataSource = null;
                grdView.DataBind();
                PageDiv.Visible = false;
            }
        }

        protected void cmbBatchName_SelectedIndexChanged(object sender, EventArgs e)
        {

            DeleteDataFromDT();
            if (cmbBatchName.SelectedValue != "0")
            {
                txtBatchName.Text = cmbBatchName.SelectedItem.Text;

                Statuspanel.Attributes.Add("Style", "visibility:visible");
                divMsg.InnerHtml = "";
                btnProcess.Attributes.Add("Style", "visibility:visible");
                btnDelete.Attributes.Add("Style", "visibility:visible");
                btnProcess.Text = "Save";
                hdnBatchatcion.Value = "GetDataForBatchEdit";
                Createtemptable();
                lblMessage.Text = "";
                currentPageNumber = 1;
                ddlstatus.SelectedIndex = ddlstatus.Items.IndexOf(ddlstatus.Items.FindByValue("All"));

                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                ViewState["SortOrder"] = "ASC";
                pBindData(null, false);
            }
            else
            {
                txtBatchName.Text = string.Empty;

                btnProcess.Attributes.Add("Style", "visibility:visible");
                btnDelete.Attributes.Add("Style", "visibility:hidden");
                btnProcess.Text = "Create";
                hdnBatchatcion.Value = "GetDataForBatchCreation";
                Createtemptable();
                lblMessage.Text = "";
                currentPageNumber = 1;
                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                ViewState["SortOrder"] = "ASC";
                chkDisplaySelected.Checked = false;

                ddlstatus.SelectedIndex = ddlstatus.Items.IndexOf(ddlstatus.Items.FindByValue("All"));
                pBindData(null, false);
                Statuspanel.Attributes.Add("Style", "visibility:hidden");


            }

        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnProcess_Click started", "TraceStatus");

                BatchUploadBL bal = new BatchUploadBL();
                SearchFilter filter = new SearchFilter();
                Results result = new Results();

                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                DataSet DS = new DataSet();
                DS.Tables.Add((DataTable)ViewState["TempDT"]);
                if (DS.Tables[0].Rows.Count != 0)
                {
                    filter.BatchData = DS.GetXml().Replace("<Table1>", "<Data>").Replace("</Table1>", "</Data>").Replace("<NewDataSet>", "<BatchData>").Replace("</NewDataSet>", "</BatchData>");

                    if (cmbBatchName.SelectedValue == "0")
                    {
                        filter.BatchName = txtBatchName.Text.Trim();
                        result = bal.ManageUploadBatches(filter, "SaveBatch", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                        divMsg.InnerHtml = result.Message;
                        GetBatch(hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                        if (result.ErrorState == 0)
                        {
                            ReloadBatchData("GetDataForBatchCreation");
                        }

                    }
                    else
                    {
                        filter.BatchId = cmbBatchName.SelectedValue != null ? Convert.ToInt32(cmbBatchName.SelectedValue) : 0;
                        filter.BatchName = txtBatchName.Text.Trim();

                        result = bal.ManageUploadBatches(filter, "SaveBatch", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                        divMsg.InnerHtml = result.Message;
                        GetBatch(hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                        btnProcess.Text = "Create";

                        ReloadBatchData("GetDataForBatchEdit");
                    }
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "Error", "alert('Please select atleast one record.');", true);
                }
                Logger.Trace("btnProcess_Click complted", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void chkSelected_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkBx = sender as CheckBox;
            GridViewRow row = chkBx.NamingContainer as GridViewRow;
            int UploadDocID = Convert.ToInt32(row.Cells[3].Text);
            DataTable DT = (DataTable)ViewState["TempDT"];
            if (chkBx.Checked == true)
            {
                DT.Rows.Add(UploadDocID);
            }
            else
            {
                for (int i = DT.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = DT.Rows[i];
                    if (dr["UploadDocID"].ToString() == UploadDocID.ToString())
                    {
                        dr.Delete();
                    }
                }
            }
            DT.AcceptChanges();
            ViewState["TempDT"] = DT;
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            BatchUploadBL bal = new BatchUploadBL();
            SearchFilter filter = new SearchFilter();
            Results result = new Results();
            Logger.Trace("btnDelete_Click started", Session["LoggedUserId"].ToString());
            filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
            filter.BatchId = cmbBatchName.SelectedValue != null ? Convert.ToInt32(cmbBatchName.SelectedValue) : 0;
            filter.BatchName = cmbBatchName.SelectedItem.Text;
            result = bal.ManageUploadBatches(filter, "DeleteBatch", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
            divMsg.InnerHtml = result.Message;
            GetBatch(hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
            btnProcess.Text = "Create";
            ReloadBatchData();
            Logger.Trace("btnDelete_Click finished", Session["LoggedUserId"].ToString());
        }

        protected void ManageChekbox()
        {
            if (chkSelectPage.Checked == true)
            {
                chkSelectAll.Checked = false;
                chkSelectAll.Enabled = false;
            }
            else if (chkSelectAll.Checked == true)
            {
                chkSelectPage.Checked = false;
                chkSelectPage.Enabled = false;
                chkSelectAll.Enabled = true;
            }

            else
            {
                chkSelectPage.Checked = false;
                chkSelectAll.Checked = false;
                chkSelectPage.Enabled = true;
                chkSelectAll.Enabled = true;

            }

        }

        protected void DeleteDataFromDT()
        {
            DataTable DT = (DataTable)ViewState["TempDT"];
            for (int i = DT.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = DT.Rows[i];
                dr.Delete();
            }
            foreach (GridViewRow row in grdView.Rows)
            {
                CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkSelected");
                ChkBoxRows.Checked = false;
            }
            chkSelectPage.Checked = false;
            chkSelectAll.Checked = false;
            chkSelectPage.Enabled = true;
            chkSelectAll.Enabled = true;

        }

        protected void chkSelectPage_CheckedChanged(object sender, EventArgs e)
        {
            ManageChekbox();
            SelectPageWise();


        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            ManageChekbox();
            Selectall();

        }

        protected void ManageCheckBoxInGridView()
        {
            int UploadDocID = 0, GUploadDocID = 0;

            DataTable DT = (DataTable)ViewState["TempDT"];
            for (int i = DT.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dr = DT.Rows[i];
                UploadDocID = Convert.ToInt32(dr["UploadDocID"]);

                foreach (GridViewRow row in grdView.Rows)
                {

                    GUploadDocID = Convert.ToInt32(row.Cells[3].Text);
                    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkSelected");
                    if (UploadDocID == GUploadDocID)
                    {
                        ChkBoxRows.Checked = true;
                   }

                }
            }

        }
        protected void Selectall()
        {
            Results result = new Results();
            BatchUploadBL bal = new BatchUploadBL();
            SearchFilter filter = new SearchFilter();
            DataSet ds = null;
            string UploadId = string.Empty;
            DataTable DT = (DataTable)ViewState["TempDT"];
            if (chkSelectAll.Checked == true)
            {
                filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                if (cmbBatchName.SelectedValue != "0")
                {
                    filter.BatchId = Convert.ToInt32(cmbBatchName.SelectedValue);
                }
                else
                {
                    filter.Subaction = "Create New";
                }
                result = bal.ManageUploadBatches(filter, "GetDocumentIdsForSelectAll", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                ds = result.ResultDS;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        UploadId = row["UPLOAD_iProcessID"].ToString();
                        DT.Rows.Add(UploadId);
                    }
                }
            }
            else
            {
                DeleteDataFromDT();
            }
            DT.AcceptChanges();
            ViewState["TempDT"] = DT;
            ManageCheckBoxInGridView();
        }

        protected void ddlstatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            MangeDisplayCheckBox();
            DeleteDataFromDT();
            if (ddlstatus.SelectedValue != "0")
            {
                Statuspanel.Attributes.Add("Style", "visibility:visible");
                divMsg.InnerHtml = "";
                btnProcess.Text = "Save";
                hdnBatchatcion.Value = "GetDataForBatchEdit";
                Createtemptable();
                lblMessage.Text = "";
                currentPageNumber = 1;
                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                ViewState["SortOrder"] = "ASC";
                pBindData(null, false);
            }
        }

        protected void MangeDisplayCheckBox()
        {
            if (chkDisplaySelected.Checked)
            {
                btnProcess.Attributes.Add("Style", "visibility:hidden");
                btnDelete.Attributes.Add("Style", "visibility:hidden");
                chkSelectAll.Checked = false;
                chkSelectPage.Checked = false;
                chkSelectAll.Enabled = false;
                chkSelectPage.Enabled = false;
            }
            else
            {
                btnProcess.Attributes.Add("Style", "visibility:visible");
                btnDelete.Attributes.Add("Style", "visibility:visible");
                chkSelectAll.Checked = false;
                chkSelectPage.Checked = false;
                chkSelectAll.Enabled = true;
                chkSelectPage.Enabled = true;
            }
        }

        protected void chkDisplaySelected_CheckedChanged(object sender, EventArgs e)
        {
               MangeDisplayCheckBox();
                Statuspanel.Attributes.Add("Style", "visibility:visible");
                //  chkDisplaySelected.Checked = false;
                divMsg.InnerHtml = "";
                //lblBatchName.Attributes.Add("Style", "visibility:hidden");
                //txtBatchName.Attributes.Add("Style", "visibility:hidden");
                //btnProcess.Attributes.Add("Style", "visibility:visible");
                //btnDelete.Attributes.Add("Style", "visibility:visible");
                btnProcess.Text = "Save";
                hdnBatchatcion.Value = "GetDataForBatchEdit";
                Createtemptable();
                lblMessage.Text = "";
                currentPageNumber = 1;
                PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
                ViewState["SortOrder"] = "ASC";
                pBindData(null, false);
            
        }

        private void ReloadBatchData(string Action = "GetDataForBatchCreation")
        {
            if (Action.Length > 0)
                hdnBatchatcion.Value = Action;

            Createtemptable();
            lblMessage.Text = string.Empty;
            txtBatchName.Text = string.Empty;
            currentPageNumber = 1;
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            ViewState["SortOrder"] = "ASC";
            pBindData(null, false);
        }

    }
}