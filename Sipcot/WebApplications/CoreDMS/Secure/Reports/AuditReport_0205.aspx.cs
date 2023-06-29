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
    public partial class AuditReport : PageBase
    {
        protected int currentPageNumber;
        protected int PAGE_SIZE;
        protected string action;
        protected void Page_Load(object sender, EventArgs e)
        {

            txtCreatedDateFrom.Attributes.Add("readonly", "readonly");
            txtCreatedDateTo.Attributes.Add("readonly", "readonly");
            btnsearchSub.Attributes.Add("onclick", "javascript:return validation()");
            chkUser.Attributes.Add("onclick", "javascript:return searchfilter('USER')");
            chkDoc.Attributes.Add("onclick", "javascript:return searchfilter('DOC')");
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";
          
            
            if (!IsPostBack)
            {
                DateTime startingDate1 = DateTime.Today;
                txtCreatedDateFrom.Text = Convert.ToString("2023-04-01");
                txtCreatedDateFrom.Text = Convert.ToString(startingDate1.ToString("yyyy-MM-dd"));
                LoadOrgNames();

                enable();
                LoadUsers();
                GetDocumentType(hdnLoginOrgId.Value, hdnLoginToken.Value);
                txtCreatedDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtCreatedDateFrom.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("d2") + "-" + "01";

                BatchUploadBL bal = new BatchUploadBL();

                DataSet ds = bal.ComboFillerBySP("USP_COMBOFILLER_PAGE_SIZE");
                ddlRows.DataTextField = "_Name";
                ddlRows.DataValueField = "_ID";
                ddlRows.DataSource = ds;
                ddlRows.DataBind();

                ddlRows.SelectedValue = "10";
            }

        }
        public void enable()
        {

            if (chkDoc.Checked == true)
            {
                ddlusername.SelectedIndex = 0;
                ddlusername.Enabled = false;
                cmbDocumentType.Enabled = true;

            }
            else if (chkUser.Checked == true)
            {
                ddlusername.Enabled = true;
                cmbDocumentType.SelectedIndex = 0;
                cmbDocumentType.Enabled = false;
                
            }
            else
            {
                ddlusername.SelectedIndex = 0;
                cmbDocumentType.SelectedIndex = 0;
                ddlusername.Enabled = false;

                cmbDocumentType.Enabled = false;
            }
        }
        public void LoadUsers()
        {
            {
                ddlusername.Items.Clear();
                ddlusername.Items.Add(new ListItem("<Select>", "0"));

                UserBL bl = new UserBL();
                SearchFilter filter = new SearchFilter();
                filter.CurrOrgId = Convert.ToInt16(drpOrg.SelectedValue);    
                try
                {

                    Results res = bl.SearchUsers(filter, "SearchUsersForAudit",hdnLoginOrgId.Value, hdnLoginToken.Value);
                    if (res.ActionStatus == "SUCCESS" && res.Users != null)
                    {
                        foreach (User user in res.Users)
                        {
                            ddlusername.Items.Add(new ListItem(user.UserName, user.UserId.ToString()));
                        }

                    }


                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }

        }
        public void LoadOrgNames()
        {
            //txtCreatedDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
            //txtCreatedDateFrom.Text = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString("d2") + "-" + "01";

            drpOrg.Items.Clear();
            drpOrg.Items.Add(new ListItem("<Select>", "0"));
            SecurityBL bl = new SecurityBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Results res = bl.SearchOrgs(filter, "SearchOrgs", hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value);
                if (res.ActionStatus == "SUCCESS" && res.Orgs != null)
                {
                    foreach (Org org in res.Orgs)
                    {
                        drpOrg.Items.Add(new ListItem(org.OrgName, org.OrgId.ToString()));
                    }

                }

                drpOrg.SelectedIndex = drpOrg.Items.IndexOf(drpOrg.Items.FindByValue(hdnLoginOrgId.Value));
                drpOrg.Enabled = false;
            }
            catch
            {

            }

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
            string headerTable = @"<Table cellspacing='0' cellpadding='0' rules='all' border='1'><tr><td><b>" + loginUser.LoginOrgName.ToString() + "</b></td><td></td><td></td><td></td></tr><tr><td><b>Organization : </b></td><td>" + drpOrg.SelectedItem.Text + "</td><td></td><td></td></tr><tr><td><b>Report Type : </b></td><td>Audit Report</td><td></td><td></td></tr><tr><td><b>From Date : </b></td><td>" + txtCreatedDateFrom.Text + "</td><td><b>To Date : </b></td><td>" + txtCreatedDateTo.Text + "</td></tr></Table>";
            ///string headerTable = @"<Table><tr><td>Mis Report</td></tr><tr><td>From Date:</td><td>" + txtCreatedDateFrom.Text + "</td><td>To Date:</td><td>" + txtCreatedDateTo.Text + "</td></tr></td></Table>";
            Response.Write(headerTable);
            Response.Write(stringWrite.ToString().Replace(HttpUtility.HtmlDecode("&nbsp;"), " "));
            Response.End();
        }
        public override void VerifyRenderingInServerForm(Control control)
        {
        }
        public void GetDocumentType(string loginOrgId, string loginToken)
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
          
            try
            {
                filter.CurrOrgId = Convert.ToInt32(drpOrg.SelectedValue);
                string action = "DocumentTypeForAudit";
                Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);
                cmbDocumentType.Items.Clear();
                cmbDocumentType.Items.Add(new ListItem("<Select>", "0"));

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    
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
        protected void drpOrg_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsers();
            enable();
            GetDocumentType(hdnLoginOrgId.Value, hdnLoginToken.Value);
        }
        private int GetTotalPages(double totalRows)
        {
            int totalPages = (int)Math.Ceiling(totalRows / PAGE_SIZE);

            return totalPages;
        }
        private void pBindData(string aSortExp, bool aIsCompleteData)
        {
            //SqlCommand objCmd = null;
            DataSet ds = null;
            //SqlDataAdapter objAdp = null;
            BatchUploadBL bl = new BatchUploadBL();
            SearchFilter filter = new SearchFilter();

            try
            {
                grdView.Visible = true;
                // Clear grid data
                grdView.DataSource = null;
                grdView.DataBind();

                if (chkUser.Checked == true)
                {
                    filter.CurrOrgId = Convert.ToInt32(drpOrg.SelectedValue);
                   
                    if (ddlusername.SelectedIndex > 0)
                    {
                        action = "GetAuditDetailsForUserWithUserId";
                        filter.CurrUserId = Convert.ToInt32(ddlusername.SelectedValue);

                    }
                    else
                    {
                        action = "GetAuditDetailsForUser";

                    }

                }

                else if (chkDoc.Checked == true)
                {
                    action = "GetAuditDetailsForProjectType";
                    filter.CurrOrgId = Convert.ToInt32(drpOrg.SelectedValue);
                    filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                    

                }
                else if (chkDoc.Checked == false && chkUser.Checked == false)
                {
                    filter.Documentname = txtsearch.Text;
                    action = "GetAuditDetailsbyDocname";
                    filter.CurrOrgId = Convert.ToInt32(drpOrg.SelectedValue);

                }
                filter.Documentname = txtsearch.Text;
                filter.StartDate = txtCreatedDateFrom.Text.Trim();
                filter.EndDate = txtCreatedDateTo.Text.Trim();
                filter.startRowIndex = ((currentPageNumber - 1) * PAGE_SIZE) + 1;
                filter.endRowIndex = (aIsCompleteData == true ? -1 : (currentPageNumber * PAGE_SIZE));

                ds = bl.AuditReport(filter, action, hdnLoginOrgId.Value.ToString(), hdnLoginToken.Value.ToString());
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


        protected void btnsearchSub_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            currentPageNumber = 1;
            PAGE_SIZE = Int32.Parse(ddlRows.SelectedValue);
            ViewState["SortOrder"] = "ASC";
            pBindData(null, false);
            enable();
        }

        protected void ddlusername_SelectedIndexChanged(object sender, EventArgs e)
        {
            enable();
        }

        protected void cmbDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            enable();
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

                e.Row.Cells[1].Visible = true;

            }
            else if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].Visible = true;
            }

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
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

        }

        protected void lnkbtnExport_Click(object sender, EventArgs e)
        {
            if (grdView.Rows.Count > 0)
            {
                pBindData(null, true);
                ///export to excel
                pExportGridToExcel(grdView, drpOrg.SelectedItem.Text +"_AuditReport" + "_" + Convert.ToString(DateTime.Now) + ".xls");
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

        protected void btnclear_Click(object sender, EventArgs e)
        {


            DateTime startingDate1 = DateTime.Today;


            txtCreatedDateFrom.Text = Convert.ToString("2023-04-01");
            txtCreatedDateFrom.Text = Convert.ToString(startingDate1.ToString("yyyy-MM-dd"));
            txtsearch.Text = "";
            //drpOrg.SelectedIndex = 0;
            cmbDocumentType.SelectedIndex = 0;
            ddlusername.SelectedIndex = 0;
            chkDoc.Checked = false;
            chkUser.Checked = false;
            RdoPeriod.ClearSelection();
            grdView.DataSource = null;
            grdView.DataBind();
            grdView.Visible = false;
                
            enable();
                

        }

        protected void RdoPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {


            if(RdoPeriod.SelectedValue== "Last 1 week")
            { 
            //-------- Week----------------------
           // DayOfWeek weekStart1 = DayOfWeek.Monday; // or Sunday, or whenever
            DateTime startingDate1 = DateTime.Today;

            //while (startingDate1.DayOfWeek != weekStart1)
            //    startingDate1 = startingDate1.AddDays(-1);
            DateTime previousWeekStart1 = startingDate1.AddDays(-7);       
            txtCreatedDateFrom.Text = Convert.ToString(previousWeekStart1.ToString("yyyy-MM-dd"));
            }
            if (RdoPeriod.SelectedValue == "Last 1 month")
            {
               // DayOfWeek weekStart2 = DayOfWeek.Monday; // or Sunday, or whenever
                DateTime startingDate2 = DateTime.Today;

                //while (startingDate2.DayOfWeek != weekStart2)
                //    startingDate2 = startingDate2.AddDays(-1);
                DateTime previousWeekEnd2 = startingDate2.AddMonths(-1);
                txtCreatedDateFrom.Text = Convert.ToString(previousWeekEnd2.ToString("yyyy-MM-dd"));

            }
            if (RdoPeriod.SelectedValue == "Last 6 months")
            {
               // DayOfWeek weekStart = DayOfWeek.Monday; // or Sunday, or whenever
                DateTime startingDate = DateTime.Today;

                //while (startingDate.DayOfWeek != weekStart)
                //    startingDate = startingDate.AddDays(-1);
                DateTime Lastsixmonths = startingDate.AddMonths(-6);
                txtCreatedDateFrom.Text = Convert.ToString(Lastsixmonths.ToString("yyyy-MM-dd"));
            }

        }


    }
}