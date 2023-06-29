using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Data;
using System.Configuration;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using OfficeConverter;
using PdfPrinter;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Ionic.Zip;
using System.Net;
using AjaxControlToolkit;


namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class CheckerDashboard : PageBase
    {
        public int ControlCount = 0;

        #region page events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                cmbDocumentType.Attributes.Add("onChange", "javascript:SetHiddenVal('Static');");
                cmbDepartment.Attributes.Add("onChange", "javascript:SetHiddenVal('Static');");

                lblMsg.Text = string.Empty;
                if (!IsPostBack)
                {
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                    hdnLoginToken.Value = loginUser.LoginToken;
                    QueryBL objQueryBL = new QueryBL();
                    Session["ComparisonOperators"] = objQueryBL.ManageQueryClause("GetComparisonOperators", 0, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);
                    BindDocumentType(hdnLoginOrgId.Value, hdnLoginToken.Value);
                    if (Convert.ToInt32(Request.QueryString["PId"]) != 0)
                    {
                        Logger.Trace("PageLoad of Documentdownloadsearch binding selected index changed event", Session["LoggedUserId"].ToString());
                        cmbDocumentType_SelectedIndexChanged(sender, e);
                        cmbDepartment_SelectedIndexChanged(sender, e);
                    }
                    if (Request.QueryString["Search"] != null)
                    {
                        string strData = Request.QueryString["Search"];
                        string[] strspliteddata = strData.Split('|');
                        cmbDocumentType.SelectedValue = strspliteddata[1];
                        cmbDocumentType_SelectedIndexChanged(sender, e);
                        cmbDepartment.SelectedValue = strspliteddata[2];

                        cmbDepartment_SelectedIndexChanged(sender, e);

                    }
                    if (Session["dtQueryClause"] != null)
                    {
                        Session.Remove("dtQueryClause");
                    }

                    Results.Visible = false;
                    btnDeleteQuery.Enabled = false;
                    btnSaveQuery.Enabled = false;
                }
                //rebind the clauses with existing values
                RecreateQueryClause();
                divMsg.InnerHtml = "";
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void cmbDocumentType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                BindDepartments(hdnLoginOrgId.Value, hdnLoginToken.Value);

                //Fill the saved queries
                if (Convert.ToInt32(cmbDocumentType.SelectedValue) != 0)
                {
                    BindQueries();
                    plhClauses.Controls.Clear();
                    //Clear values on placeholder
                    ViewState["ControlExist"] = "0";
                    Clear();
                }
                //this.NumberOfControls = 0;

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            try
            {
                Session["hdnProjectTypeId"] = cmbDocumentType.SelectedItem.ToString() != null ? cmbDocumentType.SelectedItem.ToString() : "0";
                Session["hdnDepartmentId"] = cmbDepartment.SelectedValue != null ? cmbDepartment.SelectedValue : "0";

                ds = GetTemplatefieldDetails();
                Session["TemplateFields"] = ds;
                plhClauses.Controls.Clear();
                Clear();

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void ddlQueryCondition_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ddlCurrentRowId = ((DropDownList)sender).ID.ToString();
            string ddlSelectedValue = ((DropDownList)sender).SelectedValue;
            string ddlSelectedItem = ((DropDownList)sender).SelectedItem.Text;

            string CurrentRow = ddlCurrentRowId.Substring(ddlCurrentRowId.LastIndexOf("_") + 1);
            DropDownList ddlFields = (DropDownList)sender;
            DropDownList ddlFieldValue = (DropDownList)plhClauses.FindControl("ddlFieldValue_Row_" + CurrentRow);
            TextBox txtFieldValue = (TextBox)plhClauses.FindControl("txtFieldValue_Row_" + CurrentRow);
            QueryBL objQueryBL = new QueryBL();
            try
            {
                if (Session["TemplateFields"] != null)
                {
                    DataSet dsFields = (DataSet)Session["TemplateFields"];
                    if (dsFields != null && dsFields.Tables.Count > 0 && dsFields.Tables[0].Rows.Count > 0)
                    {


                        DataRow[] result = dsFields.Tables[0].Select("TemplateFields_vName='" + ddlSelectedItem + "'");
                        if (result[0][4].ToString().ToUpper() == "DROPDOWNLIST")
                        {

                            txtFieldValue.Visible = false;
                            ddlFieldValue.SelectedValue = null;
                            ddlFieldValue.Visible = true;
                            int TemplateID = Convert.ToInt32(result[0]["TemplateFields_iId"]);
                            DataSet dsDrop = objQueryBL.ManageQueryClause("BindFieldValues", TemplateID,Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);
                            if (dsDrop != null && dsDrop.Tables.Count > 0 && dsDrop.Tables[0].Rows.Count > 0)
                            {
                                ddlFieldValue.DataSource = dsDrop.Tables[0];
                                ddlFieldValue.DataTextField = "TemplateFieldData_vValue";
                                ddlFieldValue.DataValueField = "TemplateFieldData_iId";
                                ddlFieldValue.DataBind();
                            }
                        }
                        else if (result[0][4].ToString().ToUpper() == "TEXTBOX")
                        {
                            if (result[0][3].ToString().ToUpper() == "DATETIME")
                            {
                                CalendarExtender calext = new CalendarExtender();
                                calext.ID = "cal_" + txtFieldValue.ID;
                                calext.TargetControlID = txtFieldValue.ID;
                                calext.Format = "dd/MM/yyyy";
                                plhClauses.Controls.Add(calext);
                                txtFieldValue.ToolTip = "dd/MM/yyyy";
                            }
                            else
                            {
                                //to remove control by type
                                foreach (Control item in plhClauses.Controls.OfType<CalendarExtender>())
                                {
                                    if (item.GetType() == typeof(CalendarExtender) && item.ID == "cal_" + txtFieldValue.ID)
                                    {
                                        plhClauses.Controls.Remove(item);
                                        txtFieldValue.Text = string.Empty;
                                    }
                                }
                            }

                            txtFieldValue.Visible = true;
                            txtFieldValue.Visible = true;
                            ddlFieldValue.SelectedValue = null;
                            ddlFieldValue.Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnPerformQueryClick(object sender, EventArgs e)
        {
            DocumentBL bl = new DocumentBL();
            QueryBL objQueryBL = new QueryBL();
            SearchFilter filter = new SearchFilter();
            Results results = new Results();
            DataSet dswhereclause = new DataSet();
            DataTable dtQueryclause = new DataTable();

            try
            {
                Logger.Trace("btnPerformQueryClick started", Session["LoggedUserId"].ToString());

                filter.WhereClause = QueryWhereClause();
                Logger.Trace("From Search click,QueryWhereClause() is executed properly ", Session["LoggedUserId"].ToString());
                filter.Active = 1;
                filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType.SelectedValue);
                filter.DepartmentID = Convert.ToInt32(cmbDepartment.SelectedValue);
                filter.DocPageNo = Convert.ToInt32(hdnPageNo.Value);
                filter.RowsPerPage = Convert.ToInt32(hdnRowsPerPage.Value);
                filter.SearchOption = "ANYWHERE";
                hdnSearchString.Value = filter.SearchOption + "|" + filter.DocumentTypeID + "|" + filter.DepartmentID + "|" + filter.Active + "|" + filter.DocPageNo + "|" + filter.RowsPerPage;
                string SearchAction = (cbxFullText.Checked == true) ? "SearchDocumentsUsingFullText" : "SearchDocuments";
                Logger.Trace("Before searching the details with whereclause : " + filter.WhereClause, Session["LoggedUserId"].ToString());
                results = bl.SearchDocumentsForMakerChecker(filter, SearchAction, hdnLoginOrgId.Value, hdnLoginToken.Value, "Checker");

                //binding the div with tml table
                if (results.DocumentDownloads.Count > 0)
                {
                    // Set paging controls
                    paging.Visible = true;
                    divRecordCountText.InnerHtml = results.RecordCountText;
                    divPagingText.InnerHtml = results.PagingText;

                    hdnTotalRowCount.Value = results.DocumentDownloads[0].TotalRowcount.ToString();
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Success", "createPagingIndexes();", true);
                    Logger.Trace("btnPerformQueryClick search result record count " + results.DocumentDownloads.Count, Session["LoggedUserId"].ToString());
                    // Bind html table to div
                    divSearchResults.Visible = true;
                    paging.Visible = true;
                    divSearchResults.InnerHtml = results.DocumentDownloads[0].HtmlTable;
                    ViewState["GridValue"] = results.DocumentDownloads[0].HtmlTable;

                    if (Convert.ToInt32(hdnTotalRowCount.Value) > 0)
                    {
                        paging.Attributes.Add("style", "Visibility:visible");
                        if (cbxFullText.Checked && plhClauses.Controls.Count > 0)
                        {

                            Session["FullTextClause"] = filter.WhereClause;
                        }
                        else
                        {
                            Session["FullTextClause"] = null;
                        }
                    }
                    else
                    {
                        paging.Attributes.Add("style", "Visibility:hidden");
                        divSearchResults.InnerHtml = "";
                        paging.Visible = false;
                        divMsg.InnerHtml = "No Records exist please try again with different values...!";
                    }
                }
                Logger.Trace("btnPerformQueryClick finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());

            }
        }

        protected void btnSearchAgain_Click(object sender, EventArgs e)
        {
            DocumentBL bl = new DocumentBL();
            SearchFilter filter = new SearchFilter();
            Results results = new Results();

            try
            {

                string[] strvalue = Request.QueryString["Search"].Split('|');

                filter.Active = 1;
                filter.DocumentTypeID = Convert.ToInt32(strvalue[1]);
                filter.DepartmentID = Convert.ToInt32(strvalue[2]);
                filter.DocPageNo = Convert.ToInt32(hdnPageNo.Value);
                filter.RowsPerPage = Convert.ToInt32(hdnRowsPerPage.Value);
                filter.WhereClause = QueryWhereClause();
                filter.SearchOption = "ANYWHERE";
                hdnSearchString.Value = filter.SearchOption + "|" + filter.DocumentTypeID + "|" + filter.DepartmentID + "|" + filter.Active + "|" + filter.DocPageNo + "|" + filter.RowsPerPage;
                string SearchAction = (cbxFullText.Checked == true) ? "SearchDocumentsUsingFullText" : "SearchDocuments";
                Logger.Trace("Before searching the details with whereclause : " + filter.WhereClause, Session["LoggedUserId"].ToString());
                results = bl.SearchDocumentsForMakerChecker(filter, SearchAction, hdnLoginOrgId.Value, hdnLoginToken.Value, "Checker");

                //binding the div with tml table
                if (results.DocumentDownloads.Count > 0)
                {
                    // Set paging controls
                    paging.Visible = true;
                    divRecordCountText.InnerHtml = results.RecordCountText;
                    divPagingText.InnerHtml = results.PagingText;

                    hdnTotalRowCount.Value = results.DocumentDownloads[0].TotalRowcount.ToString();
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Success", "createPagingIndexes();", true);

                    // Bind html table to div
                    divSearchResults.Visible = true;
                    paging.Visible = true;
                    divSearchResults.InnerHtml = results.DocumentDownloads[0].HtmlTable;
                    ViewState["GridValue"] = results.DocumentDownloads[0].HtmlTable;

                    if (Convert.ToInt32(hdnTotalRowCount.Value) > 0)
                    {
                        paging.Attributes.Add("style", "Visibility:visible");
                    }
                    else
                    {
                        paging.Attributes.Add("style", "Visibility:hidden");
                    }
                }
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void dropQueries_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (dropQueries.SelectedIndex != 0)
                {
                    EditQuery(sender, e);
                    btnDeleteQuery.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnAddClauseClick(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnAddClauseClick started", Session["LoggedUserId"].ToString());
                string ObjectType = "TextBox";
                DataTable dtQueryClause = new DataTable();
                DataRow dr;
                if (Session["dtQueryClause"] != null)
                {
                    dtQueryClause = (DataTable)Session["dtQueryClause"];

                    dr = dtQueryClause.NewRow();
                    dr["BoolOperator"] = "";
                    dr["FieldName"] = "";
                    dr["ComparisonOperator"] = "";
                    dr["FieldValue"] = "";
                    dr["ObjectType"] = "TextBox";

                    dtQueryClause.Rows.Add(dr);
                    Session["dtQueryClause"] = dtQueryClause;
                    ControlCount++;
                    CreateQueryClause(ObjectType, dr, ControlCount);
                    btnSaveQuery.Enabled = true;
                }
                else
                {

                    dtQueryClause.Columns.Add("BoolOperator");
                    dtQueryClause.Columns.Add("FieldName");
                    dtQueryClause.Columns.Add("ComparisonOperator");
                    dtQueryClause.Columns.Add("FieldValue");
                    dtQueryClause.Columns.Add("ObjectType");

                    dr = dtQueryClause.NewRow();
                    dr["BoolOperator"] = "";
                    dr["FieldName"] = "";
                    dr["ComparisonOperator"] = "";
                    dr["FieldValue"] = "";
                    dr["ObjectType"] = "TextBox";

                    dtQueryClause.Rows.Add(dr);
                    Session["dtQueryClause"] = dtQueryClause;
                    CreateQueryClause(ObjectType, dr, 1);
                    btnSaveQuery.Enabled = true;
                }
                divSearchResults.InnerHtml = "";
                paging.Visible = false;
                Logger.Trace("btnAddClauseClick finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnRemoveClauseClick(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnRemoveClauseClick started", Session["LoggedUserId"].ToString());
                if (Session["dtQueryClause"] != null)
                {
                    DataTable dtQueryClause = new DataTable();
                    dtQueryClause = (DataTable)Session["dtQueryClause"];


                    for (int cnt = 1; cnt <= dtQueryClause.Rows.Count; cnt++)
                    {
                        if (cnt == ControlCount)
                        {
                            Table panelrow = (Table)plhClauses.FindControl("Panel_Row_" + ControlCount.ToString());

                            plhClauses.Controls.Remove(panelrow);
                        }
                    }
                    dtQueryClause.Rows.RemoveAt(dtQueryClause.Rows.Count - 1);

                    if (dtQueryClause.Rows.Count > 0)
                    {
                        Session["dtQueryClause"] = dtQueryClause;
                    }
                    else
                    {
                        Session.Remove("dtQueryClause");
                        btnSaveQuery.Enabled = false;
                    }
                    divSearchResults.InnerHtml = "";
                    paging.Visible = false;
                }
                Logger.Trace("btnRemoveClauseClick finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnSaveQueryClick(object sender, EventArgs e)
        {
            QueryBL objQueryBL = new QueryBL();
            int returnVal = 0;
            try
            {
                Logger.Trace("btnSaveQueryClick started", Session["LoggedUserId"].ToString());
                if (Page.IsValid)
                {
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    string queryName = txtQueryName.Text.Trim();
                    string userName = loginUser.UserName;

                    if (queryName == String.Empty)
                    {
                        lblMsg.Text = "Please enter the query name to be saved...";
                        lblMsg.ForeColor = System.Drawing.Color.Red;
                        return;
                    }

                    List<QueryClause> queryClauses = GetQueryClauses();
                    if (queryClauses.Count == 0)
                        return;

                    bool success = false;

                    if (Convert.ToInt32(dropQueries.SelectedValue) == 0)
                    {
                        string SearchType = (cbxFullText.Checked == true) ? "Full Text Search" : "Advance Search";

                        success = objQueryBL.SaveQuery(Convert.ToInt32(cmbDocumentType.SelectedValue), queryName, chkGlobalQuery.Checked, SearchType, queryClauses,Convert.ToInt32(hdnLoginToken.Value),hdnLoginOrgId.Value);
                        Logger.Trace("Save button click, query executed" + success, Session["LoggedUserId"].ToString());
                        if (success)
                        {

                            BindQueries();
                            lblMsg.Text = "Query saved successfully...";
                            lblMsg.ForeColor = System.Drawing.Color.Green;
                            divSearchResults.InnerText = "";
                            paging.Visible = false;
                            plhClauses.Controls.Clear();
                            txtQueryName.Text = string.Empty;
                            cbxFullText.Checked = false;
                        }
                        else
                        {
                            lblMsg.Text = "Please check proper data is being entered...";
                            lblMsg.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                    else
                    {
                        returnVal = objQueryBL.UpdateQuery(Convert.ToInt32(dropQueries.SelectedValue), Convert.ToInt32(cmbDocumentType.SelectedValue), queryName, chkGlobalQuery.Checked, queryClauses, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);
                        Logger.Trace("Save button click,update query properly executed" + returnVal, Session["LoggedUserId"].ToString());
                        if (returnVal > 0)
                        {
                            BindQueries();
                            lblMsg.Text = "Query Updated successfully...";
                            lblMsg.ForeColor = System.Drawing.Color.Green;
                            divSearchResults.InnerText = "";
                            paging.Visible = false;
                            plhClauses.Controls.Clear();
                            txtQueryName.Text = string.Empty;
                            cbxFullText.Checked = false;
                            if (Session["dtQueryClause"] != null)
                            {
                                Session.Remove("dtQueryClause");
                            }
                        }
                        else
                        {
                            lblMsg.Text = "Please check proper data is being entered...";
                            lblMsg.ForeColor = System.Drawing.Color.Red;
                        }
                    }
                }
                Logger.Trace("btnSaveQueryClick finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());

            }
        }

        protected void AddQuery(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("AddQuery started", Session["LoggedUserId"].ToString());

                plhClauses.Controls.Clear();
                Clear();
                btnAddClauseClick(sender, e);
                divSearchResults.InnerHtml = "";
                paging.Visible = false;
                Logger.Trace("AddQuery finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        #endregion

        #region methods

        private void RecreateQueryClause()
        {
            string ObjectType = string.Empty;
            DataTable dtQueryClause = new DataTable();
            if (Session["dtQueryClause"] != null)
            {
                dtQueryClause = (DataTable)Session["dtQueryClause"];
                ControlCount = 0;
                foreach (DataRow drTemp in dtQueryClause.Rows)
                {
                    ControlCount++;
                    ObjectType = drTemp["ObjectType"].ToString();
                    CreateQueryClause(ObjectType, drTemp, ControlCount);

                }
            }
        }

        private void CreateQueryClause(string ObjectType, DataRow dr, int controlId)
        {
            Table tbl = new Table();
            TableRow tr = new TableRow();
            TableCell cell = new TableCell();
            tbl.Width = Unit.Pixel(560);
            tbl.ID = "Panel_Row_" + controlId.ToString();
            DataSet dsComparisonOperators = new DataSet();

            #region boolean operator
            DropDownList ddlBoolOperator = new DropDownList();
            ddlBoolOperator.CssClass = "";
            dsComparisonOperators = (DataSet)Session["ComparisonOperators"];
            ddlBoolOperator.ID = "ddlBoolOperator_Row_" + controlId.ToString();
            if (dsComparisonOperators != null && dsComparisonOperators.Tables.Count > 0 && dsComparisonOperators.Tables[0].Rows.Count > 0)
            {
                ddlBoolOperator.DataSource = cbxFullText.Checked ? dsComparisonOperators.Tables[2] : dsComparisonOperators.Tables[0];
                ddlBoolOperator.DataTextField = "BoolOperator_vName";
                ddlBoolOperator.DataValueField = "BoolOperator_iId";
                ddlBoolOperator.DataBind();
                ddlBoolOperator.Width = Unit.Pixel(100);

                if (dr["BoolOperator"].ToString() != "")
                {
                    try
                    {
                        ddlBoolOperator.SelectedValue = dr["BoolOperator"].ToString().Trim();
                    }
                    catch { }
                }
            }
            #endregion

            #region Comparison Operators
            DropDownList ddlQueryCondition = new DropDownList();
            ddlQueryCondition.CssClass = "";
            ddlQueryCondition.ID = "ddlQueryCondition_Row_" + controlId.ToString();
            if (dsComparisonOperators != null && dsComparisonOperators.Tables.Count > 0 && dsComparisonOperators.Tables[1].Rows.Count > 0)
            {
                ddlQueryCondition.DataSource = cbxFullText.Checked ? dsComparisonOperators.Tables[3] : dsComparisonOperators.Tables[1];
                ddlQueryCondition.DataTextField = "QueryCondition_vName";
                ddlQueryCondition.DataValueField = "QueryCondition_iId";
                ddlQueryCondition.DataBind();
                ddlQueryCondition.Width = Unit.Pixel(120);

                if (dr["ComparisonOperator"].ToString() != "")
                {
                    try
                    {
                        ddlQueryCondition.SelectedValue = dr["ComparisonOperator"].ToString().Trim();
                    }
                    catch { }
                }
            }
            #endregion

            #region field names
            DropDownList ddlFieldNames = new DropDownList();
            ddlFieldNames.CssClass = "";
            ddlFieldNames.ID = "ddlFieldNames_Row_" + controlId.ToString();

            if (cbxFullText.Checked)
            {
                ddlQueryCondition.Enabled = false;

                ddlFieldNames.Items.Insert(0, new ListItem("FULLTEXT"));
                ddlFieldNames.Enabled = false;
            }
            else
            {
                ddlFieldNames.Enabled = true;
                DataSet dsFields = (DataSet)Session["TemplateFields"];
                if (dsFields != null && dsFields.Tables.Count > 0 && dsFields.Tables[0].Rows.Count > 0)
                {
                    ddlFieldNames.DataSource = dsFields.Tables[0];
                    ddlFieldNames.DataTextField = "TemplateFields_vName";
                    ddlFieldNames.DataValueField = "TemplateFields_vDBFld";
                    ddlFieldNames.Width = Unit.Pixel(150);
                    ddlFieldNames.DataBind();
                    ddlFieldNames.AutoPostBack = true;
                    ddlFieldNames.SelectedIndexChanged += new EventHandler(ddlQueryCondition_SelectedIndexChanged);

                    if (dr["FieldName"].ToString() != "")
                    {
                        try
                        {
                            ddlFieldNames.SelectedValue = dr["FieldName"].ToString().Trim();
                        }
                        catch { }
                    }
                }
            }
            #endregion

            #region input value control
            DropDownList ddlFieldValue = new DropDownList();
            ddlFieldValue.CssClass = "";
            ddlFieldValue.ID = "ddlFieldValue_Row_" + controlId.ToString();
            ddlFieldValue.Width = Unit.Pixel(120);
            TextBox txtFieldValue = new TextBox();
            txtFieldValue.CssClass = "";
            txtFieldValue.ID = "txtFieldValue_Row_" + controlId.ToString();
            txtFieldValue.Width = Unit.Pixel(120);

            if (cbxFullText.Checked)
            {
                ddlFieldValue.Visible = false;
                txtFieldValue.Visible = true;
            }
            else
            {
                if (ObjectType == "TextBox")
                {
                    ddlFieldValue.Visible = false;
                    txtFieldValue.Visible = true;
                }
                else
                {
                    txtFieldValue.Visible = false;
                    ddlFieldValue.Visible = true;
                }
            }

            cell.Controls.Add(ddlBoolOperator);
            cell.Controls.Add(ddlFieldNames);
            cell.Controls.Add(ddlQueryCondition);
            cell.Controls.Add(ddlFieldValue);
            cell.Controls.Add(txtFieldValue);

            tr.Controls.Add(cell);
            tbl.Controls.Add(tr);
            plhClauses.Controls.Add(tbl);

            ddlQueryCondition_SelectedIndexChanged(ddlFieldNames, new EventArgs());

            if (dr["FieldValue"].ToString() != "")
            {
                try
                {
                    txtFieldValue.Text = dr["FieldValue"].ToString().Trim();
                }
                catch { }
            }

            if (dr["FieldValue"].ToString() != "")
            {
                try
                {
                    ddlFieldValue.SelectedIndex = ddlFieldValue.Items.IndexOf(ddlFieldValue.Items.FindByText(dr["FieldValue"].ToString().Trim()));
                }
                catch { }
            }
            #endregion
        }

        public void Clear()
        {
            try
            {
                if (Session["dtQueryClause"] != null)
                {
                    Session.Remove("dtQueryClause");
                }

                dropQueries.SelectedIndex = 0;
                txtQueryName.Text = string.Empty;
                chkGlobalQuery.Checked = false;
                divSearchResults.InnerText = "";
                paging.Visible = false;
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());

            }
        }

        protected void DeleteQuery(object sender, EventArgs e)
        {
            QueryBL objQueryBL = new QueryBL();
            bool result = false;
            try
            {
                if (Convert.ToInt32(dropQueries.SelectedValue) != 0)
                {
                    result = objQueryBL.DeleteQuery(Convert.ToInt32(dropQueries.SelectedValue),Convert.ToInt32(hdnLoginOrgId.Value),hdnLoginToken.Value.ToString());

                    Logger.Trace("Delete Click,delete query execute  ", Session["LoggedUserId"].ToString());
                    if (result == true)
                    {
                        lblMsg.Text = "Deleted Query Successfully";
                        lblMsg.ForeColor = System.Drawing.Color.Green;
                        BindQueries();
                        plhClauses.Controls.Clear();
                        Clear();
                    }
                }
                else
                {
                    lblMsg.Text = "Select a query to delete";
                    lblMsg.ForeColor = System.Drawing.Color.Red;
                }
                Logger.Trace("BindQueries() executed from delete click ", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public DataSet GetTemplatefieldDetails()
        {
            DataSet dsTemplateFields = new DataSet();
            try
            {
                Logger.Trace("GetTemplatefieldDetails started", Session["LoggedUserId"].ToString());
                if (Session["hdnProjectTypeId"] != null && Session["hdnDepartmentId"] != null)
                {
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    SearchFilter objFilter = new SearchFilter();
                    objFilter.CurrOrgId = loginUser.LoginOrgId;
                    objFilter.CurrUserId = loginUser.UserId;
                    objFilter.DocumentTypeName = Session["hdnProjectTypeId"] != null ? Session["hdnProjectTypeId"].ToString() : "0";
                    objFilter.DepartmentID = Session["hdnDepartmentId"] != null ? Convert.ToInt32(Session["hdnDepartmentId"]) : 0;
                    dsTemplateFields = new TemplateBL().GetTemplateDetails(objFilter);
                }
                Logger.Trace("GetTemplatefieldDetails finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return dsTemplateFields;
        }

        protected void EditQuery(object sender, EventArgs e)
        {
            QueryBL objQueryBL = new QueryBL();
            DataSet dsQuery = new DataSet();
            DataSet dsQueryClause = new DataSet();
            DataSet dsTemplate = new DataSet();
            DataSet dsDrop = new DataSet();
            DynamicControlBL objDynamicControlBL = new DynamicControlBL();
            try
            {
                Logger.Trace("EditQuery started", Session["LoggedUserId"].ToString());
                ViewState["QueryClauses"] = new List<QueryClause>();
                plhClauses.Controls.Clear();//Clear the existing controls binded

                string strControlNames = string.Empty;
                //Get query name based on selected query
                dsQuery = objQueryBL.GetQueryById(Convert.ToInt32(dropQueries.SelectedValue),Convert.ToInt32(hdnLoginOrgId.Value),hdnLoginToken.Value.ToString());

                if (dsQuery != null && dsQuery.Tables.Count > 0)
                {
                    if (dsQuery.Tables[0].Rows.Count > 0)
                    {
                        Logger.Trace("Editquery with total records retrieved : " + dsQuery.Tables[0].Rows.Count, Session["LoggedUserId"].ToString());
                        txtQueryName.Text = dsQuery.Tables[0].Rows[0]["Query_vQueryName"].ToString();
                        chkGlobalQuery.Checked = Convert.ToBoolean(dsQuery.Tables[0].Rows[0]["Query_bIsPublic"]);
                        //Get saved query details
                        dsQueryClause = objQueryBL.GetSavedQuery(Convert.ToInt32(dropQueries.SelectedValue),Convert.ToInt32(hdnLoginOrgId.Value),hdnLoginToken.Value);

                        if (dsQuery.Tables[0].Rows[0]["Query_vType"].ToString() == "Full Text Search")
                        {
                            cbxFullText.Checked = true;
                        }
                        else
                        {
                            cbxFullText.Checked = false;
                        }

                        if (dsQueryClause != null && dsQueryClause.Tables.Count > 0 && dsQueryClause.Tables[0].Rows.Count > 0)
                        {
                            Logger.Trace("Editquery queryclauses retrieved properly: ", Session["LoggedUserId"].ToString());
                            Session["dtQueryClause"] = dsQueryClause.Tables[0];
                            RecreateQueryClause();
                            btnSaveQuery.Enabled = true;
                        }

                    }
                }
                Logger.Trace("EditQuery finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void storeclauseValue()
        {
            DataTable dtClause = new DataTable();
            DataRow dr;
            dtClause.Columns.Add("BoolOperator");
            dtClause.Columns.Add("FieldName");
            dtClause.Columns.Add("ComparisonOperator");
            dtClause.Columns.Add("FieldValue");
            dtClause.Columns.Add("ObjectType");
            dr = dtClause.NewRow();
            dr["BoolOperator"] = "";
            dr["FieldName"] = "";
            dr["ComparisonOperator"] = "";
            dr["FieldValue"] = "";
            dr["ObjectType"] = "TextBox";

            dtClause.Rows.Add(dr);
            Session["dtClause"] = dtClause;

        }

        public void BindDocumentType(string loginOrgId, string loginToken)
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Logger.Trace("BindDocumentType started", Session["LoggedUserId"].ToString());
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
                if (Convert.ToInt32(Request.QueryString["PId"]) != 0)
                {
                    cmbDocumentType.SelectedValue = Request.QueryString["PId"];
                }
                Logger.Trace("BindDocumentType finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void BindDepartments(string loginOrgId, string loginToken)
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
                if (Convert.ToInt32(Request.QueryString["PId"]) != 0 && Convert.ToInt32(Request.QueryString["DprtId"]) != 0)
                {
                    cmbDepartment.SelectedValue = Request.QueryString["DprtId"];

                }
             


            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        /// <summary>
        /// Load the dropdown with saved queries.
        /// </summary>
        void BindQueries()
        {
            QueryBL objQueryBL = new QueryBL();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            DataSet ds = new DataSet();
            try
            {
                Logger.Trace("BindQueries started", Session["LoggedUserId"].ToString());
                ds = objQueryBL.GetQueriesByUserName(Convert.ToInt32(cmbDocumentType.SelectedValue),Convert.ToInt32(hdnLoginOrgId.Value),hdnLoginToken.Value);
                dropQueries.DataSource = ds;
                dropQueries.DataValueField = "Query_iId";
                dropQueries.DataTextField = "QueryName";
                dropQueries.DataBind();
                Logger.Trace("BindQueries finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }

        /// <summary>
        /// This method executes a query and displays the results.
        /// </summary>
        public string QueryWhereClause()
        {
            string strQuery = string.Empty;
            DataTable dtQueryclause = new DataTable();
            Logger.Trace("BuildQuery() executed properly ", Session["LoggedUserId"].ToString());


            try
            {
                List<QueryClause> queryclauses = GetQueryClauses();

                // Build Command Text
                bool IsFirstClause = true;
                StringBuilder commandBuilder = new StringBuilder();
                foreach (QueryClause queryclause in queryclauses)
                {
                    if (!cbxFullText.Checked)
                    {
                        switch (queryclause.ComparisonOperator)
                        {
                            case "=":
                                commandBuilder.AppendFormat(" {0} {1} {2} '{3}'", queryclause.BooleanOperator, queryclause.FieldName, queryclause.ComparisonOperator, queryclause.FieldValue);
                                break;
                            case "LIKE":
                                commandBuilder.AppendFormat(" {0} {1} {2} '%{3}%'", queryclause.BooleanOperator, queryclause.FieldName, queryclause.ComparisonOperator, queryclause.FieldValue);
                                break;
                            case "NOT LIKE":
                                commandBuilder.AppendFormat(" {0} {1} {2} '%{3}%'", queryclause.BooleanOperator, queryclause.FieldName, queryclause.ComparisonOperator, queryclause.FieldValue);
                                break;
                            case "&gt;":
                            case ">":
                                commandBuilder.AppendFormat(" {0} {1} {2} '{3}'", queryclause.BooleanOperator, queryclause.FieldName, queryclause.ComparisonOperator, queryclause.FieldValue);
                                break;
                            case "&lt;":
                            case "<":
                                commandBuilder.AppendFormat(" {0} {1} {2} '{3}'", queryclause.BooleanOperator, queryclause.FieldName, queryclause.ComparisonOperator, queryclause.FieldValue);
                                break;
                            case "SOUNDEX":
                                commandBuilder.AppendFormat(" {0} {1} {2} ('{3}')", queryclause.BooleanOperator, queryclause.FieldName, queryclause.ComparisonOperator, queryclause.FieldValue);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // Full text
                        if (IsFirstClause)
                            commandBuilder.AppendFormat(queryclause.FieldValue);
                        else
                            commandBuilder.AppendFormat(" {0} {1}", queryclause.BooleanOperator, queryclause.FieldValue);
                    }
                    IsFirstClause = false;
                }
                strQuery = commandBuilder.ToString();
                Logger.Trace("whereclause value " + strQuery, Session["LoggedUserId"].ToString());

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }



            return strQuery;
        }

        /// <summary>
        /// Function to get the clauses details
        /// </summary>
        /// <returns></returns>
        private List<QueryClause> GetQueryClauses()
        {
            List<QueryClause> queryClauses = new List<QueryClause>();

            try
            {
                if (Session["TemplateFields"] != null)
                {
                    DataTable queryFields = (DataTable)Session["dtQueryClause"];

                    if (queryFields != null && queryFields.Rows.Count > 0)
                    {
                        QueryClause queryClause = null;
                        int slNo = 0;
                        foreach (DataRow dr in queryFields.Rows)
                        {
                            slNo++;

                            DropDownList ddlBoolOperator = (DropDownList)plhClauses.FindControl("ddlBoolOperator_Row_" + slNo.ToString());
                            DropDownList ddlQueryCondition = (DropDownList)plhClauses.FindControl("ddlQueryCondition_Row_" + slNo.ToString());
                            DropDownList ddlFieldNames = (DropDownList)plhClauses.FindControl("ddlFieldNames_Row_" + slNo.ToString());
                            DropDownList ddlFieldValue = (DropDownList)plhClauses.FindControl("ddlFieldValue_Row_" + slNo.ToString());
                            TextBox txtFieldValue = (TextBox)plhClauses.FindControl("txtFieldValue_Row_" + slNo.ToString());

                            string strFieldValue = string.Empty;
                            string ObjectType = string.Empty;

                            if (ddlFieldValue.Visible)
                            {
                                ObjectType = "DropDownList";
                                strFieldValue = ddlFieldValue.SelectedItem.Text;
                            }
                            else
                            {
                                ObjectType = "TextBox";
                                strFieldValue = txtFieldValue.Text;
                            }

                            queryClause = new QueryClause(ddlBoolOperator.SelectedValue, ddlFieldNames.SelectedValue, ddlQueryCondition.SelectedValue, strFieldValue, SqlDbType.NVarChar, false, ObjectType);
                            queryClauses.Add(queryClause);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

            return queryClauses;
        }

        #endregion
    }
}