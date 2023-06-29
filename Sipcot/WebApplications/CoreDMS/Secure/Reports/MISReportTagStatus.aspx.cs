using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data;
using System.IO;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Reports
{
    public partial class MISReportTagStatus : PageBase
    {
        protected int currentPageNumber;
        protected int PAGE_SIZE;

        protected void Page_Load(object sender, EventArgs e)
        {
            
            txtCreatedDateFrom.Attributes.Add("readonly", "readonly");
            txtCreatedDateTo.Attributes.Add("readonly", "readonly");
            btnsearchSub.Attributes.Add("onclick", "javascript:return validate()");
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";

            if (!Page.IsPostBack)
            {
                txtCreatedDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtCreatedDateFrom.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("d2") + "-" + "01";

                BatchUploadBL bal = new BatchUploadBL();

                DataSet ds = bal.ComboFillerBySP("USP_COMBOFILLER_PAGE_SIZE");
                ddlRows.DataTextField = "_Name";
                ddlRows.DataValueField = "_ID";
                ddlRows.DataSource = ds;
                ddlRows.DataBind();

                ddlRows.SelectedValue = "10";// System.Configuration.ConfigurationSettings.AppSettings["RowsPerPage"].ToString();

                GetDocumentType(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
            }
        }

        protected void btnsearchSub_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            currentPageNumber = 1;
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            ViewState["SortOrder"] = "ASC";
            pBindData(null, false);
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
                    cmbDepartment.Items.Add(new ListItem("<Select>", "0"));
                    foreach (Department dp in res.Departments)
                    {
                        cmbDepartment.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.Message.ToString());
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
                    cmbDocumentType.Items.Add(new ListItem("<Select>", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.Message.ToString());
            }
        }

        #region "Page Events"

        protected void GetPageIndex(object sender, CommandEventArgs e)
        {

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
            pBindData(null, false);
        }

        protected void ddlPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);  // Added later
            currentPageNumber = Int32.Parse(ddlPage.SelectedValue);
            pBindData(null, false);
        }

        protected void grdView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {

                e.Row.Cells[1].Visible = false;

            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Visible = false;


                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    //    e.Row.Cells[i].CssClass = "GridItem1";
                    if (e.Row.Cells[i].Text.Trim() != "&nbsp;")
                    {
                        e.Row.Cells[i].Attributes.Add("title", e.Row.Cells[i].Text);
                        if (e.Row.Cells[i].Text.Trim() == "0")
                        {
                            e.Row.BackColor = System.Drawing.Color.FromName("white");
                            e.Row.Cells[i].BackColor = System.Drawing.Color.LawnGreen;
                        }

                    }

                    //e.Row.Attributes.Add("onmouseover", "javascript:this.className = 'GridRowHover'");
                    //e.Row.Attributes.Add("onmouseout", "javascript:this.className = ''");
                    //e.Row.TabIndex = -1;
                    //e.Row.Attributes["onclick"] = string.Format("javascript:SelectRow(this, {0});", e.Row.RowIndex);
                    //e.Row.Attributes["onkeydown"] = "javascript:return SelectSibling(event);";
                    //e.Row.Attributes["onselectstart"] = "javascript:return false;";

                }

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

        protected void lnkbtnExport_Click(object sender, EventArgs e)
        {
            if (grdView.Rows.Count > 0)
            {
                pBindData(null, true);
                ///export to excel
                pExportGridToExcel(grdView, cmbDocumentType.SelectedItem.Text + "_" + Convert.ToString(DateTime.Now) + ".xls");
            }
        }

        #endregion

        #region Public Method

        public override void VerifyRenderingInServerForm(Control control)
        {
        }

        #endregion

        #region Private Method

        private int GetTotalPages(double totalRows)
        {
            int totalPages = (int)Math.Ceiling(totalRows / PAGE_SIZE);

            return totalPages;
        }


        private void pBindData(string aSortExp, bool aIsCompleteData)
        {

            DataSet ds = null;
            BatchUploadBL bl = new BatchUploadBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                // Clear grid data
                grdView.DataSource = null;
                grdView.DataBind();

                filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                filter.StartDate = txtCreatedDateFrom.Text.Trim();
                filter.EndDate = txtCreatedDateTo.Text.Trim();
                filter.startRowIndex = ((currentPageNumber - 1) * PAGE_SIZE) + 1;
                filter.endRowIndex = (aIsCompleteData == true ? -1 : (currentPageNumber * PAGE_SIZE));

                string action = "";
                ds = bl.ReportDocumentWise(filter, action, hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    grdView.DataSource = ds.Tables[0];
                    grdView.DataBind();

                    ///Fill Data Set
                    Logger.TraceErrorLog("Successfully executed the report DAL funtion");

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

            }
            catch (Exception ex)
            {
                lblMessage.Text = ex.Message;
                Logger.TraceErrorLog(ex.Message.ToString());
            }
        }
        private void pExportGridToExcel(GridView grdGridView, String fileName)
        {
            // grdView.Columns[1].Visible = false;  //Remove button column from export
            UserBase loginUser = (UserBase)Session["LoggedUser"];

            Response.Clear();
            Response.AddHeader("content-disposition",
            String.Format("attachment;filename={0}", fileName));
            Response.Charset = "";
            Response.ContentType = "application/vnd.xls";

            StringWriter stringWrite = new StringWriter();
            HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);
            ClearControls(grdView);
            grdGridView.RenderControl(htmlWrite);
            string headerTable = @"<Table cellspacing='0' cellpadding='0' rules='all' border='1'><tr><td><b>" + loginUser.LoginOrgName.ToString() + "</b></td><td></td><td></td><td></td></tr><tr><td><b>Project Type : </b></td><td>" + cmbDocumentType.SelectedItem.Text + "</td><td></td><td></td></tr><tr><td><b>Report Type : </b></td><td>Mis Report</td><td></td><td></td></tr><tr><td><b>From Date : </b></td><td>" + txtCreatedDateFrom.Text + "</td><td><b>To Date : </b></td><td>" + txtCreatedDateTo.Text + "</td></tr></Table>";
            ///string headerTable = @"<Table><tr><td>Mis Report</td></tr><tr><td>From Date:</td><td>" + txtCreatedDateFrom.Text + "</td><td>To Date:</td><td>" + txtCreatedDateTo.Text + "</td></tr></td></Table>";
            Response.Write(headerTable);
            Response.Write(stringWrite.ToString().Replace(HttpUtility.HtmlDecode("&nbsp;"), " "));
            Response.End();
        }

        private void ClearControls(Control control)
        {
            try
            {
                for (int i = control.Controls.Count - 1; i >= 0; i--)
                {
                    ClearControls(control.Controls[i]);
                }
                if (!(control is TableCell))
                {
                    if (control.GetType().GetProperty("SelectedItem") != null)
                    {
                        LiteralControl literal = new LiteralControl();
                        control.Parent.Controls.Add(literal);
                        try
                        {
                            literal.Text = (String)control.GetType().GetProperty("SelectedItem").GetValue(control, null);
                        }
                        catch
                        {
                        }
                        control.Parent.Controls.Remove(control);
                    }
                    else
                        if (control.GetType().GetProperty("Text") != null)
                        {
                            LiteralControl literal = new LiteralControl();
                            control.Parent.Controls.Add(literal);
                            literal.Text = (String)control.GetType().GetProperty("Text").GetValue(control, null);
                            control.Parent.Controls.Remove(control);
                        }
                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.Message.ToString());
            }
            return;
        }

        #endregion

        protected void btnclear_Click(object sender, EventArgs e)
        {
            divMsg.InnerHtml = "";
            grdView.Visible = false;
            cmbDepartment.SelectedIndex = 0;
            cmbDocumentType.SelectedIndex = 0;
            txtCreatedDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtCreatedDateFrom.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("d2") + "-" + "01";
        }

        protected void cmbDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {

            GetDepartments(hdnLoginOrgId.Value, hdnLoginToken.Value);
            DocumentPanel.Update();
        }

    }
}