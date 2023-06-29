using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBL;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Text;
using System.Data;
using AjaxControlToolkit;
using System.Web.UI.HtmlControls;
using System.IO;
using Ionic.Zip;
using System.Configuration;
using System.Data.SqlClient;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class FullDocumentSearch : PageBase
    {

        public int ControlCount = 0;
        #region basicsearch

        protected bool bDynamicControlIndexChange = false;

        protected int NumberOfControls
        {
            get { return (int)ViewState["NumControls"]; }
            set { ViewState["NumControls"] = value; }
        }
        public const string PageName = "DOCUMENT_DOWNLOAD";
        #endregion
        #region page events

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");


            #region basicsearch
            CheckAuthentication();
            bDynamicControlIndexChange = hdnDynamicControlIndexChange_BS.Value == "1" ? true : false;
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId_BS.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken_BS.Value = loginUser.LoginToken;
            hdnPageId_BS.Value = "0";
            lblIndexProprties_BS.Visible = false;
            lblIndexProprties_OCR.Visible = false;
            //calendat specific
            cmbDocumentType_BS.Attributes.Add("onChange", "javascript:SetHiddenVal_BS('Static');");
            cmbDepartment_BS.Attributes.Add("onChange", "javascript:SetHiddenVal_BS('Static');");

            // DMSENH6-4657

            if (!IsPostBack)
            {
                /* DMS04-3447	  BS*/
                Session["FullTextClause"] = null;
                /* DMS04-3447	  BE*/
                if (ViewState["dt"] == null)
                {
                    DataTable dt = new DataTable();//Creating a Datatable for maintaining id and values of dynamic controls
                    ViewState["dt"] = dt;//Creating a viewstate for maintaining id and values of dynamic controls
                    dt.Columns.Add("DRPID", typeof(string)); //adding columns
                    dt.Columns.Add("SID", typeof(string));
                }

                GetDocumentType_BS(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                GetSearchType_BS(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                this.NumberOfControls = 0;
                hdnCountControls_BS.Value = Convert.ToString(0);

            }

            if (!IsPostBack && Request.QueryString["action"] == "Delete")
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Document Discarded Successfully.')", true);
            }

            #endregion
            try
            {
                cmbDocumentType.Attributes.Add("onChange", "javascript:SetHiddenVal('Static');");
                cmbDepartment.Attributes.Add("onChange", "javascript:SetHiddenVal('Static');");

                lblMsg.Text = string.Empty;
                if (!IsPostBack)
                {
                    UserBase loginUser1 = (UserBase)Session["LoggedUser"];
                    hdnLoginOrgId.Value = loginUser1.LoginOrgId.ToString();
                    hdnLoginToken.Value = loginUser1.LoginToken;

                    //Applying page rights
                    string pageRights = GetPageRights();
                    hdnPageRights.Value = pageRights;
                    string[] RightsArray = pageRights.Split(',');
                    string value = "ReadOCR";
                    int valuefound = Array.IndexOf(RightsArray, value);
                    if (valuefound > -1)
                    {
                        Search.Tabs[2].Visible = true;
                    }
                    //  ApplyPageRights(pageRights, this.Form.Controls);

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
                        hdnSearchString.Value = Request.QueryString["Search"].ToString();
                        string strData = Request.QueryString["Search"];
                        string[] strspliteddata = strData.Split('|');

                        //DMS5-4365 BS : Note: 6th index is SearchMode only when it's Advanced search querystring
                        if (strspliteddata.Length > 0 && strspliteddata[10].Equals("AdvancedSearch"))
                        {
                            // Advanced search 
                            //SearchOption, DocumentType, Department, Refid, Keywords, check, MainTagID, SubTagID, PageNo, RowsPerPage, SearchMode;
                            cmbDocumentType.SelectedValue = strspliteddata[1];
                            cmbDocumentType_SelectedIndexChanged(sender, e);
                            cmbDepartment.SelectedValue = strspliteddata[2];
                            cmbDepartment_SelectedIndexChanged(sender, e);

                            // Set selected tab index;
                            Search.ActiveTabIndex = 1;

                            // Load data for advanced search
                            btnSearchAgain_Click(null, null);
                        }
                        else
                        {
                            // Basic search 
                            //SearchOption, DocumentType, Department, Refid, Keywords, check, MainTagID, SubTagID, PageNo, RowsPerPage, SearchMode;

                            cmbDocumentType_BS.SelectedValue = strspliteddata[1];
                            cmbDocumentType_SelectedIndexChanged_BS(sender, e);

                            cmbDepartment_BS.SelectedValue = strspliteddata[2];
                            //Fill tag controls
                            GetTemplateDetails_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);

                            cmbMainTag_BS.SelectedValue = strspliteddata[6];
                            btnCommonSubmitSub2_Click_BS(sender, e);
                            cmbSubTag_BS.SelectedValue = strspliteddata[7];

                            // Set selected tab index;
                            Search.ActiveTabIndex = 0;
                        }
                        //DMS5-4365 BE
                    }
                    //DMSENH6-4713 BS
                    if (Request.QueryString["ReadOcr"] != null)
                    {
                        int DepartmentId = Convert.ToInt32(Decrypt(HttpUtility.UrlDecode(Request.QueryString["DepartmentId"])));
                        int projectId = Convert.ToInt32(Decrypt(HttpUtility.UrlDecode(Request.QueryString["ProjectId"])));
                        // Set selected tab index;
                        Search.ActiveTabIndex = 2;

                        cmbDocumentType_OCR.SelectedValue = Convert.ToString(projectId);
                        cmbDocumentType_SelectedIndexChanged_BS(sender, e);

                        cmbDepartment_OCR.SelectedValue = Convert.ToString(DepartmentId);
                        //Fill tag controls
                        GetTemplateDetails_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
                        btnPerformQueryClick(sender, e);


                    }
                    //DMSENH6-4713 BE
                    if (Session["dtQueryClause"] != null)
                    {
                        Session.Remove("dtQueryClause");
                    }

                    Results.Visible = false;
                    btnDeleteQuery.Enabled = false;
                    btnSaveQuery.Enabled = false;
                }

                if ((cmbDepartment_BS.SelectedValue != "0" && cmbDocumentType_BS.SelectedValue != "0")
                    || ((cmbDepartment_OCR.SelectedValue) != "0" && (cmbDocumentType_OCR.SelectedValue) != "0"))//DMSENH6-4657
                {
                    createControls_BS();
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
                            DataSet dsDrop = objQueryBL.ManageQueryClause("BindFieldValues", TemplateID, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);
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

                //DMSENH6-4657 BS
                if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0)
                {
                    filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType_OCR.SelectedValue);
                    filter.DepartmentID = Convert.ToInt32(cmbDepartment_OCR.SelectedValue);
                    filter.DocPageNo = Convert.ToInt32(hdnPageNo_OCR.Value);
                    filter.RowsPerPage = Convert.ToInt32(hdnRowsPerPage_OCR.Value);
                }
                //DMSENH6-4657 BE


                //SearchOption, DocumentType, Department, Refid, Keywords, check, MainTagID, SubTagID, PageNo, RowsPerPage, SearchMode;
                hdnSearchString.Value = filter.SearchOption + "|" + filter.DocumentTypeID + "|" + filter.DepartmentID
                    + "|0|0|0|0|0|" + filter.DocPageNo + "|" + filter.RowsPerPage + "|AdvancedSearch";

                string SearchAction = (cbxFullText.Checked == true) ? "SearchDocumentsUsingFullText" : "SearchDocuments";
                Logger.Trace("Before searching the details with whereclause : " + filter.WhereClause, Session["LoggedUserId"].ToString());
                results = bl.SearchDocuments(filter, SearchAction, hdnLoginOrgId.Value, hdnLoginToken.Value);

                //binding the div with tml table
                if (results.DocumentDownloads.Count > 0)
                {
                    // Set paging controls
                    hdnTotalRowCount.Value = results.DocumentDownloads[0].TotalRowcount.ToString();
                    hdnTotalRowCount_OCR.Value = results.DocumentDownloads[0].TotalRowcount.ToString();

                    Logger.Trace("btnPerformQueryClick search result record count " + results.DocumentDownloads.Count, Session["LoggedUserId"].ToString());

                    // Bind html table to div
                    if (Convert.ToInt32(cmbDepartment.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType.SelectedValue) != 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Success", "createPagingIndexes();", true);
                        paging.Visible = true;
                        divRecordCountText.InnerHtml = results.RecordCountText;
                        divPagingText.InnerHtml = results.PagingText;
                        divSearchResults.Visible = true;
                        divSearchResults.InnerHtml = results.DocumentDownloads[0].HtmlTable;
                        ViewState["GridValue"] = results.DocumentDownloads[0].HtmlTable;
                    }

                    //Bind html to OCR grid
                    if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0)
                    {
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Success", "createPagingIndexes();", true);
                        divRecordCountText_OCR.InnerHtml = results.RecordCountText;
                        divPagingText_OCR.InnerHtml = results.PagingText;
                        OCRGridContents.Visible = true;
                        paging_OCR.Visible = true;
                        GridOCR.DataSource = results.ResultDS.Tables[0];
                        GridOCR.DataBind();

                    }
                    //showing pAGIN OF OCR
                    if ((Convert.ToInt32(hdnTotalRowCount_OCR.Value) > 0)
                      && (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0))
                    {
                        paging_OCR.Attributes.Add("style", "Visibility:visible");
                    }

                    if ((Convert.ToInt32(hdnTotalRowCount.Value) > 0)
                        && (Convert.ToInt32(cmbDepartment.SelectedValue) != 0) && (Convert.ToInt32(cmbDocumentType.SelectedValue) != 0))
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
                        /* DMS04-3447	  BS*/
                        Session["FullTextClause"] = null;
                        /* DMS04-3447	  BE*/
                        paging.Attributes.Add("style", "Visibility:hidden");
                        divSearchResults.InnerHtml = "";
                        paging.Visible = false;
                        if ((Convert.ToInt32(cmbDepartment.SelectedValue) != 0) && (Convert.ToInt32(cmbDocumentType.SelectedValue) != 0))
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

                //SearchOption, DocumentType, Department, Refid, Keywords, check, MainTagID, SubTagID, PageNo, RowsPerPage, SearchMode;
                hdnSearchString.Value = filter.SearchOption + "|" + filter.DocumentTypeID + "|" + filter.DepartmentID
                    + "|0|0|0|0|0|" + filter.DocPageNo + "|" + filter.RowsPerPage + "|AdvancedSearch";//DMS5-4365 

                results = bl.SearchDocuments(filter, "SearchDocuments", hdnLoginOrgId.Value, hdnLoginToken.Value);

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

                        success = objQueryBL.SaveQuery(Convert.ToInt32(cmbDocumentType.SelectedValue), queryName, chkGlobalQuery.Checked, SearchType, queryClauses, Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);
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

                //disable dropdown for first count
                if (controlId == 1) //DMSENH6-4638
                {
                    ddlBoolOperator.Enabled = false;
                    ddlBoolOperator.SelectedIndex = 1;// DMSENH6-4719 
                }

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

            /* DMS04-3396 BS */
            if (cbxFullText.Checked == true)
            {
                txtFieldValue.Attributes.Add("OnKeyPress", "javascript:return CheckSpace(event);");
            }
            if (hdnSearchTextboxId.Value == "")
            {
                hdnSearchTextboxId.Value = "txtFieldValue_Row_" + controlId.ToString();
            }
            else
            {
                hdnSearchTextboxId.Value = hdnSearchTextboxId.Value + '|' + "txtFieldValue_Row_" + controlId.ToString();
            }
            /* DMS04-3396 BE */

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
                    result = objQueryBL.DeleteQuery(Convert.ToInt32(dropQueries.SelectedValue), Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);

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
                dsQuery = objQueryBL.GetQueryById(Convert.ToInt32(dropQueries.SelectedValue), Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);

                if (dsQuery != null && dsQuery.Tables.Count > 0)
                {
                    if (dsQuery.Tables[0].Rows.Count > 0)
                    {
                        Logger.Trace("Editquery with total records retrieved : " + dsQuery.Tables[0].Rows.Count, Session["LoggedUserId"].ToString());
                        txtQueryName.Text = dsQuery.Tables[0].Rows[0]["Query_vQueryName"].ToString();
                        chkGlobalQuery.Checked = Convert.ToBoolean(dsQuery.Tables[0].Rows[0]["Query_bIsPublic"]);
                        //Get saved query details
                        dsQueryClause = objQueryBL.GetSavedQuery(Convert.ToInt32(dropQueries.SelectedValue), Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);

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

                    //DMSENH6-4657 BS
                    cmbDocumentType_OCR.Items.Clear();
                    cmbDocumentType_OCR.Items.Add(new ListItem("<Select>", "0"));
                    //DMSENH6-4657 BE

                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                        cmbDocumentType_OCR.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));//DMSENH6-4657
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

                    //DMSENH6-4657 BS
                    cmbDepartment_OCR.Items.Clear();
                    cmbDepartment_OCR.Items.Add(new ListItem("<Select>", "0"));
                    //DMSENH6-4657 BE

                    foreach (Department dp in res.Departments)
                    {
                        cmbDepartment.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                        cmbDepartment_OCR.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                    }
                }
                if (Convert.ToInt32(Request.QueryString["PId"]) != 0 && Convert.ToInt32(Request.QueryString["DprtId"]) != 0)
                {
                    cmbDepartment.SelectedValue = Request.QueryString["DprtId"];

                }
                //if (Request.QueryString["Search"]!=null)
                //{
                //    string data = Request.QueryString["Search"];
                //    string[] strvalue = data.Split('|');
                //    cmbDepartment.SelectedValue = strvalue[2];
                //}


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
                ds = objQueryBL.GetQueriesByUserName(Convert.ToInt32(cmbDocumentType.SelectedValue), Convert.ToInt32(hdnLoginOrgId.Value), hdnLoginToken.Value);
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

        #region basicsearch
        private void createControls_BS()
        {
            Logger.Trace("createControls Started", Session["LoggedUserId"].ToString());
            string indexname = string.Empty;
            string sortOreder = string.Empty;
            bool IActivive;

            HtmlTable Tab = new HtmlTable();
            //Tab.Width = "550px";
            Tab.CellSpacing = 2;

            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SearchFilter objFilter = new SearchFilter();
            objFilter.CurrOrgId = loginUser.LoginOrgId;
            objFilter.CurrUserId = loginUser.UserId;
            objFilter.DocumentTypeName = cmbDocumentType_BS.SelectedItem.ToString();
            objFilter.DepartmentID = Convert.ToInt32(cmbDepartment_BS.SelectedValue);

            //DMSENH6-4657 BS
            if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0)
            {
                objFilter.DocumentTypeName = cmbDocumentType_OCR.SelectedItem.ToString();
                objFilter.DepartmentID = Convert.ToInt32(cmbDepartment_OCR.SelectedValue);
            }
            //DMSENH6-4657 BE

            DataSet TemplateDs = new TemplateBL().GetTemplateDetails(objFilter);


            pnlIndexpro_BS.Controls.Clear();
            pnlIndexpro_OCR.Controls.Clear();
            if ((Convert.ToInt32(cmbDepartment_BS.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_BS.SelectedValue) != 0)
                && (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0))//DMSENH6-4657
            {
                this.NumberOfControls = 0;
                txtRefid_BS.Text = "";
                hdnCountControls_BS.Value = "";
                hdnIndexMinLen_BS.Value = "";
                hdnIndexNames_BS.Value = "";
            }
            else
            {
                int ControlsCounter = -1; // if the control counter is even bind controls parallelly, i.e., 4 columns :[ lable:control label:control ] other wise 2 columns style
                HtmlTableRow TR = new HtmlTableRow();
                for (int i = 0; i < TemplateDs.Tables[0].Rows.Count; i++)
                {
                    IActivive = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bActive"]);
                    ++ControlsCounter; // Comment this line to bind controls in 2 coulumns stlye

                    if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        TR = new HtmlTableRow();
                    HtmlTableCell TD1 = new HtmlTableCell();
                    HtmlTableCell TD2 = new HtmlTableCell();

                    TD1.Width = "125px";
                    TD2.Width = "150px";
                    // Parellel controls binding
                    HtmlTableCell TD3 = new HtmlTableCell();
                    HtmlTableCell TD4 = new HtmlTableCell();

                    // Add label text
                    if (!string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Label", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Button", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "HiddenField", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                    {
                        Label objCnl = new Label();
                        objCnl.ID = "lbl" + i;
                        objCnl.CssClass = Getcssclass_BS(IActivive, "lbl");
                        objCnl.Text = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                        objCnl.AssociatedControlID = "";
                        if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        {
                            TD1.Controls.Add(objCnl);
                        }
                        else
                            TD3.Controls.Add(objCnl);
                    }
                    #region textbox control
                    if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "TextBox", StringComparison.OrdinalIgnoreCase))
                    {
                        TextBox objCnltxt = new TextBox();
                        objCnltxt.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                        //DMS5-4101M replaced the space with empty string
                        objCnltxt.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                        TD2.Controls.Add(objCnltxt);
                        objCnltxt.Width = Unit.Pixel(155);
                        objCnltxt.Height = Unit.Pixel(16);

                        if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Integer")
                        {
                            objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                            objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckNumericKey(event);");
                        }
                        else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Boolean")
                        {
                            objCnltxt.MaxLength = 1;
                            objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckBoolean(event);");
                        }
                        else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "DateTime")
                        {
                            objCnltxt.MaxLength = 10;
                            objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckDateFormat(event.keyCode,'" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString() + "');");

                            if (bDynamicControlIndexChange)
                            {
                                CalendarExtender calext = new CalendarExtender();
                                //DMS5-4101M replaced the space with empty string
                                calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                calext.Format = "dd/MM/yyyy";

                                TD2.Controls.Add(calext);
                            }
                        }
                        else
                        {
                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString()) != 0)
                            {
                                objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                            }
                            else
                            {
                                objCnltxt.MaxLength = 30;
                            }
                        }
                        Logger.Trace("createControls Loaded all Textbox dynamically", Session["LoggedUserId"].ToString());
                    }
                    #endregion

                    #region dropdownlist control
                    if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "DropDownList", StringComparison.OrdinalIgnoreCase))
                    {
                        DropDownList objCnl = new DropDownList();
                        objCnl.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                        //DMS5-4101M replaced the space with empty string
                        objCnl.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                        objCnl.CssClass = Getcssclass_BS(IActivive, "drp");
                        objCnl.Height = Unit.Pixel(30);
                        objCnl.Width = Unit.Pixel(162);
                        int TemplatefldId = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iId"]);
                        ViewState["TemplatefldId"] = TemplatefldId;
                        string Iaction = string.Empty;
                        if (TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"].ToString() == "0")
                        {
                            Iaction = "LoadMain";

                            PopulateDropdown_BS(objCnl, TemplatefldId, Iaction);//function to populate dropdownlist
                        }
                        else
                        {
                            objCnl.Items.Insert(0, "--Select--");
                        }
                        //DMS5-4111M it was calling SetHiddenVal instead of SetHiddenVal_BS
                        objCnl.Attributes.Add("onChange", "javascript:SetHiddenVal_BS('Dynamic');");

                        if (objCnl.AutoPostBack)
                        {
                            objCnl.SelectedIndexChanged += new EventHandler(DynamicDropdownList_SelectedIndexChanged_BS);
                        }

                        if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                        {
                            TD2.Controls.Add(objCnl);
                        }
                        else
                        {
                            TD4.Controls.Add(objCnl);
                        }
                        Logger.Trace("createControls Loaded all Dropdownlist dynamically", Session["LoggedUserId"].ToString());
                    }
                    #endregion
                    if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                    {
                        TR.Cells.Add(TD1);
                        TR.Cells.Add(TD2);
                    }
                    else
                    {
                        TR.Cells.Add(TD3);
                        TR.Cells.Add(TD4);
                    }

                    Tab.Rows.Add(TR);
                    //Tab.Attributes.Add("CssClass", "tabledesign");
                }

                if (Convert.ToInt32(cmbDepartment_BS.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_BS.SelectedValue) != 0)
                    pnlIndexpro_BS.Controls.Add(Tab);

                if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0)
                    pnlIndexpro_OCR.Controls.Add(Tab);
            }
            Logger.Trace("createControls Finished", Session["LoggedUserId"].ToString());
        }
        /// <summary>
        /// Populate dynamic dropdownlist
        /// </summary>
        /// <param name="drp"></param>
        /// <param name="TemplateId"></param>
        public void PopulateDropdown_BS(DropDownList drp, int TemplateId, string action)
        {
            DynamicControlBL objDynamicControlBL = new DynamicControlBL();
            DataSet ds = new DataSet();
            try
            {
                ds = objDynamicControlBL.DynamicPopulateDropdown(TemplateId, action);
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0] != null)
                    {
                        drp.DataSource = ds;

                        drp.DataTextField = "DataTextField";
                        drp.DataValueField = "DataValueField";
                        drp.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void DynamicDropdownList_SelectedIndexChanged_BS(object sender, EventArgs e)
        {

            bDynamicControlIndexChange = true;
            DynamicControlBL objDynamicControlBL = new DynamicControlBL();
            int TemplateFldId = 0;
            DataSet ds = new DataSet();
            try
            {
                DropDownList ddl = (DropDownList)sender;
                DropDownList ddlBind = (DropDownList)pnlIndexpro_BS.FindControl(ddl.ID + "_sub");//Get the sub dropdown id from panel.

                TemplateFldId = Convert.ToInt32(ddl.SelectedValue);
                ds = objDynamicControlBL.DynamicLoadDropdownBasedOnValue(TemplateFldId);
                if (ddlBind != null)
                {
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0] != null)
                        {
                            ddlBind.DataSource = ds;
                            ddlBind.DataTextField = "DataTextField";
                            ddlBind.DataValueField = "DataValueField";
                            ddlBind.DataBind();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetDepartments_BS(string loginOrgId, string loginToken)
        {
            DepartmentBL bl = new DepartmentBL();
            SearchFilter filter = new SearchFilter();
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType_BS.SelectedValue) == 0 ? Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) : Convert.ToInt32(cmbDocumentType_BS.SelectedValue);
            try
            {
                string action = "DepartmentsForUpload";
                Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.Departments != null)
                {
                    cmbDepartment_BS.Items.Clear();
                    cmbDepartment_BS.Items.Add(new ListItem("<Select>", "0"));

                    //DMSENH6-4657  BS
                    cmbDepartment_OCR.Items.Clear();
                    cmbDepartment_OCR.Items.Add(new ListItem("<Select>", "0"));
                    // DMSENH6-4657 BE
                    foreach (Department dp in res.Departments)
                    {
                        cmbDepartment_BS.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                        cmbDepartment_OCR.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString())); //DMSENH6-4657
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg_BS.InnerHtml = CoreMessages.GetMessages(hdnAction_BS.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetDocumentType_BS(string loginOrgId, string loginToken)
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "DocumentTypeForUpload";
                Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    cmbDocumentType_BS.Items.Clear();
                    cmbDocumentType_BS.Items.Add(new ListItem("<Select>", "0"));
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbDocumentType_BS.Items.Add(new ListItem(dp.DocumentTypeName, dp.DocumentTypeId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg_BS.InnerHtml = CoreMessages.GetMessages(hdnAction_BS.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetSearchType_BS(string loginOrgId, string loginToken)
        {

            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "SearchTypeForDownload";
                Results res = bl.SearchDocumentTypes(filter, action, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    cmbSearchOption_BS.Items.Clear();
                    foreach (DocumentType dp in res.DocumentTypes)
                    {
                        cmbSearchOption_BS.Items.Add(new ListItem(dp.DocumentTypeName, dp.Description));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg_BS.InnerHtml = CoreMessages.GetMessages(hdnAction_BS.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public void GetTemplateDetails_BS(string loginOrgId, string loginToken, string LoadMain = "true")
        {
            if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) == 0 || Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) == 0)
            {
                OCRGridContents.Visible = false;

            }
            if (Convert.ToInt32(cmbDepartment_BS.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_BS.SelectedValue) != 0)
            {
                DataSet ds = new DataSet();
                HtmlTable tb = new HtmlTable();
                TemplateBL bl = new TemplateBL();
                SearchFilter filter = new SearchFilter();

                try
                {
                    Logger.Trace("GetTemplateDetails started", Session["LoggedUserId"].ToString());
                    if (LoadMain == "true")
                    {
                        cmbMainTag_BS.Attributes.Add("onchange", "javascript:return LoadSubTag_BS(this);");
                        filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType_BS.SelectedValue);
                        filter.DepartmentID = Convert.ToInt32(cmbDepartment_BS.SelectedValue);
                        ds = bl.GetTagDetails(filter, "MainTag", loginOrgId, loginToken);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            cmbMainTag_BS.Items.Clear();
                            cmbMainTag_BS.DataSource = ds.Tables[0];
                            cmbMainTag_BS.DataTextField = "TextField";
                            cmbMainTag_BS.DataValueField = "ValueField";
                            cmbMainTag_BS.DataBind();
                            lblIndexProprties_BS.Visible = true;
                        }
                        else
                        {
                            cmbMainTag_BS.Items.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                }
            }
            //DMSENH6-4657 BS
            else if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0)
            {
                DataSet ds = new DataSet();
                HtmlTable tb = new HtmlTable();
                TemplateBL bl = new TemplateBL();
                SearchFilter filter = new SearchFilter();

                try
                {
                    Logger.Trace("GetTemplateDetails started", Session["LoggedUserId"].ToString());
                    if (LoadMain == "true")
                    {
                        cmbMainTag_OCR.Attributes.Add("onchange", "javascript:return LoadSubTag_BS(this);");
                        filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType_OCR.SelectedValue);
                        filter.DepartmentID = Convert.ToInt32(cmbDepartment_OCR.SelectedValue);
                        ds = bl.GetTagDetails(filter, "MainTag", loginOrgId, loginToken);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            cmbMainTag_OCR.Items.Clear();
                            cmbMainTag_OCR.DataSource = ds.Tables[0];
                            cmbMainTag_OCR.DataTextField = "TextField";
                            cmbMainTag_OCR.DataValueField = "ValueField";
                            cmbMainTag_OCR.DataBind();
                            lblIndexProprties_OCR.Visible = true;
                        }
                        else
                        {
                            cmbMainTag_OCR.Items.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                }
            }

            //DMSENH6-4657 BE
        }

        protected void cmbDocumentType_SelectedIndexChanged_BS(object sender, EventArgs e)
        {

            bDynamicControlIndexChange = false;
            GetDepartments_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            hdnCountControls_BS.Value = Convert.ToString(0);
            this.NumberOfControls = 0;
            GetTemplateDetails_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            pnlIndexpro_BS.Controls.Clear();
            generatecontrols_BS();
            hdnDBCOLMapping_BS.Value = getIndexFieldsAndColumnsList_BS();



        }

        protected void cmbDepartment_SelectedIndexChanged_BS(object sender, EventArgs e)
        {
            bDynamicControlIndexChange = false;
            hdnCountControls_BS.Value = Convert.ToString(0);
            this.NumberOfControls = 0;
            GetTemplateDetails_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            pnlIndexpro_BS.Controls.Clear();
            pnlIndexpro_OCR.Controls.Clear(); //DMSENH6-4718 
            generatecontrols_BS();

            hdnDBCOLMapping_BS.Value = getIndexFieldsAndColumnsList_BS();
        }

        protected void btnCommonSubmitSub_Click_BS(object sender, EventArgs e)
        {
            //GetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value, "false");
            getDynamicControlAfterPageLoad_BS();
            this.NumberOfControls = 0;
            pnlIndexpro_BS.Controls.Clear();
            generatecontrols_BS();


        }

        public string Getcssclass_BS(bool active, string cntrl)
        {
            string csclass = string.Empty;
            if (cntrl == "drp")
            {
                if (active == true)
                {
                    csclass = "";
                }
                else
                {
                    csclass = "";
                }
            }
            else if (cntrl == "txt")
            {
                if (active == true)
                {
                    csclass = "";
                }
                else
                {
                    csclass = "";
                }
            }
            else
            {

                if (active == true)
                {
                    csclass = "LabelStyle";
                }
                else
                {
                    csclass = "LabelStylehidden";
                }

            }


            return csclass;
        }

        public void generatecontrols_BS()
        {
            if ((Convert.ToInt32(cmbDepartment_BS.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_BS.SelectedValue) != 0)
                || (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0))//DMSENH6-4657
            {
                TemplateBL bl = new TemplateBL();
                SearchFilter filter = new SearchFilter();

                try
                {
                    Logger.Trace("generatecontrols Started", Session["LoggedUserId"].ToString());
                    string indexname = string.Empty;

                    string sortOreder = string.Empty;
                    bool IActivive;
                    HtmlTable Tab = new HtmlTable();
                    // Tab.Width = "550px";
                    Tab.CellSpacing = 2;
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    SearchFilter objFilter = new SearchFilter();
                    objFilter.CurrOrgId = loginUser.LoginOrgId;
                    objFilter.CurrUserId = loginUser.UserId;
                    objFilter.DocumentTypeName = cmbDocumentType_BS.SelectedItem.ToString();
                    objFilter.DepartmentID = Convert.ToInt32(cmbDepartment_BS.SelectedValue);

                    //DMSENH6-4657 BS
                    if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0)
                    {
                        objFilter.DocumentTypeName = cmbDocumentType_OCR.SelectedItem.ToString();
                        objFilter.DepartmentID = Convert.ToInt32(cmbDepartment_OCR.SelectedValue);
                    }
                    //DMSENH6-4657 BE

                    DataSet TemplateDs = new TemplateBL().GetTemplateDetails(objFilter);

                    UserBase loginUser1 = (UserBase)Session["LoggedUser"];
                    if ((cmbDepartment_BS.SelectedValue == "0" && cmbDocumentType_BS.SelectedValue == "0")
                        && (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) == 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) == 0))
                    {
                        pnlIndexpro_BS.Controls.Clear();
                        pnlIndexpro_OCR.Controls.Clear();
                        this.NumberOfControls = 0;
                        txtRefid_BS.Text = "";
                        hdnCountControls_BS.Value = "";
                        hdnIndexMinLen_BS.Value = "";
                        hdnIndexNames_BS.Value = "";
                    }
                    else
                    {
                        hdnIndexNames_BS.Value = "";
                        hdnIndexType_BS.Value = "";
                        hdnIndexDataType_BS.Value = "";
                        hdnIndexMinLen_BS.Value = "";
                        hdnIndexMinLen_BS.Value = "";
                        hdnCountControls_BS.Value = "";

                        int ControlsCounter = -1; // if the control counter is even bind controls parallelly, i.e., 4 columns :[ lable:control label:control ] other wise 2 columns style
                        HtmlTableRow TR = new HtmlTableRow();
                        for (int i = 0; i < TemplateDs.Tables[0].Rows.Count; i++)
                        {
                            IActivive = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bActive"]);
                            indexname = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                            ++ControlsCounter; // Comment this line to bind controls in 2 coulumns stlye                            
                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                TR = new HtmlTableRow();
                            HtmlTableCell TD1 = new HtmlTableCell();
                            HtmlTableCell TD2 = new HtmlTableCell();

                            TD1.Width = "125px";
                            TD2.Width = "150px";
                            // Parellel controls binding
                            HtmlTableCell TD3 = new HtmlTableCell();
                            HtmlTableCell TD4 = new HtmlTableCell();

                            // Add label text
                            if (!string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Label", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Button", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "HiddenField", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            {
                                Label objCnl = new Label();
                                objCnl.ID = "lbl" + i;
                                objCnl.Text = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                objCnl.CssClass = Getcssclass_BS(IActivive, "lbl");
                                objCnl.AssociatedControlID = "";
                                if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                {
                                    TD1.Controls.Add(objCnl);
                                }
                                else
                                    TD3.Controls.Add(objCnl);
                            }
                            #region textbox control
                            if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "TextBox", StringComparison.OrdinalIgnoreCase))
                            {
                                TextBox objCnltxt = new TextBox();
                                objCnltxt.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                                //DMS5-4101M replaced the space with empty string
                                objCnltxt.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                TD2.Controls.Add(objCnltxt);
                                objCnltxt.Width = Unit.Pixel(155);
                                objCnltxt.Height = Unit.Pixel(16);

                                if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Integer")
                                {
                                    objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                                    objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckNumericKey(event);");
                                }
                                else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "Boolen")
                                {
                                    objCnltxt.MaxLength = 1;
                                    objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckBoolean(event);");
                                }
                                else if (TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString() == "DateTime")
                                {
                                    objCnltxt.MaxLength = 10;
                                    objCnltxt.Attributes.Add("OnKeyPress", "javascript:return CheckDateFormat(event.keyCode,'" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString() + "');");

                                    if (!bDynamicControlIndexChange)
                                    {

                                        CalendarExtender calext = new CalendarExtender();
                                        //DMS5-4101M replaced the space with empty string
                                        calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                        calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                        calext.Format = "dd/MM/yyyy";

                                        TD2.Controls.Add(calext);
                                    }
                                }
                                else
                                {
                                    if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString()) != 0)
                                    {
                                        objCnltxt.MaxLength = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMax"].ToString());
                                    }
                                    else
                                    {
                                        objCnltxt.MaxLength = 30;
                                    }
                                }
                                Logger.Trace("GenerateControls:Loaded all Textbox dynamically", Session["LoggedUserId"].ToString());
                            }
                            #endregion
                            #region dropdownlist control
                            if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "DropDownList", StringComparison.OrdinalIgnoreCase))
                            {
                                DropDownList objCnl = new DropDownList();
                                objCnl.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                                //DMS5-4101M replaced the space with empty string
                                objCnl.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                objCnl.CssClass = Getcssclass_BS(IActivive, "drp");
                                objCnl.Height = Unit.Pixel(30);
                                objCnl.Width = Unit.Pixel(162);
                                int TemplatefldId = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iId"]);
                                ViewState["TemplatefldId"] = TemplatefldId;
                                string Iaction = string.Empty;
                                if (TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"].ToString() == "0")
                                {
                                    Iaction = "LoadMain";

                                    PopulateDropdown_BS(objCnl, TemplatefldId, Iaction);//function to populate dropdownlist
                                }
                                else
                                {
                                    objCnl.Items.Insert(0, "--Select--");
                                }

                                objCnl.SelectedIndexChanged += new EventHandler(DynamicDropdownList_SelectedIndexChanged_BS);
                                //DMS5-4111M it was calling SetHiddenVal instead of SetHiddenVal_BS
                                objCnl.Attributes.Add("onChange", "javascript:SetHiddenVal_BS('Dynamic');");

                                if (objCnl.AutoPostBack)
                                {
                                    objCnl.SelectedIndexChanged += new EventHandler(DynamicDropdownList_SelectedIndexChanged_BS);
                                }
                                if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                {
                                    TD2.Controls.Add(objCnl);
                                }
                                else
                                {
                                    TD4.Controls.Add(objCnl);
                                }

                                Logger.Trace("GenerateControls:Loaded all Dropdownlist dynamically", Session["LoggedUserId"].ToString());
                            }
                            #endregion

                            if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            {
                                TR.Cells.Add(TD1);
                                TR.Cells.Add(TD2);
                            }
                            else
                            {
                                TR.Cells.Add(TD3);
                                TR.Cells.Add(TD4);
                            }

                            Tab.Rows.Add(TR);

                            //For javascript validation
                            if (hdnIndexNames_BS.Value.Length == 0)
                            {
                                hdnControlNames_BS.Value = indexname;
                                hdnIndexNames_BS.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                hdnIndexMinLen_BS.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                                hdnIndexType_BS.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                                hdnIndexDataType_BS.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                                // hdnMandatory.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                            }
                            else
                            {
                                hdnControlNames_BS.Value = hdnControlNames_BS.Value + '|' + indexname;
                                hdnIndexNames_BS.Value = hdnIndexNames_BS.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                hdnIndexType_BS.Value = hdnIndexType_BS.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                                hdnIndexDataType_BS.Value = hdnIndexDataType_BS.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                                if (!string.IsNullOrEmpty(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) != 0)
                                {
                                    hdnIndexMinLen_BS.Value = hdnIndexMinLen_BS.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                                }
                                else
                                {
                                    hdnIndexMinLen_BS.Value = hdnIndexMinLen_BS.Value + "|" + "0";
                                }
                                //  hdnMandatory.Value = hdnMandatory.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                            }
                        }

                        if (Convert.ToInt32(cmbDepartment_BS.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_BS.SelectedValue) != 0)
                        {
                            pnlIndexpro_OCR.Controls.Clear();
                            pnlIndexpro_BS.Controls.Add(Tab);
                            pnlIndexpro_BS.Visible = true;
                        }

                        if (Convert.ToInt32(cmbDepartment_OCR.SelectedValue) != 0 && Convert.ToInt32(cmbDocumentType_OCR.SelectedValue) != 0)
                        {
                            pnlIndexpro_BS.Controls.Clear();
                            pnlIndexpro_OCR.Controls.Add(Tab);//DMSENH6-4657
                            pnlIndexpro_OCR.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    divMsg_BS.InnerHtml = CoreMessages.GetMessages(hdnAction_BS.Value, "ERROR");
                    Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                }
            }
        }

        //DMSENH6-4657 BS
        protected void GridOCR_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //// Hide unwanted rows
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
                {

                    e.Row.Cells[2].Style.Add("display", "none"); //total
                    e.Row.Cells[3].Style.Add("display", "none"); //view
                    e.Row.Cells[4].Style.Add("display", "none"); //maintagid
                    e.Row.Cells[5].Style.Add("display", "none");//subtagid
                    e.Row.Cells[7].Style.Add("display", "none"); //processid


                    e.Row.Cells[6].Style.Add("Width", "20px"); //serial nuimber
                    e.Row.Cells[6].Style.Add("text-align", "center"); //serial nuimber
                    e.Row.Cells[8].Style.Add("Width", "80px"); //File name
                    e.Row.Cells[18].Style.Add("Width", "30px"); //Tag status


                }

                if (e.Row.RowType == DataControlRowType.DataRow)
                {

                    string OCRDone = e.Row.Cells[18].Text == "Yes" ? e.Row.Cells[18].Text : "No";
                    ImageButton imgView = (ImageButton)e.Row.FindControl("imgOCRView");
                    if (!OCRDone.Equals("Yes"))
                    {
                        imgView.Visible = false;
                    }
                    else
                    {
                        imgView.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected void OCRView(object sender, EventArgs e)
        {
            GridViewRow grdrow = (GridViewRow)((ImageButton)sender).NamingContainer;
            string documentId = grdrow.Cells[7].Text; //process id
            string projectId = HttpUtility.UrlEncode(Encrypt(cmbDocumentType_OCR.SelectedValue));
            string departmentId = HttpUtility.UrlEncode(Encrypt(cmbDepartment_OCR.SelectedValue));
            string EdocumentId = HttpUtility.UrlEncode(Encrypt(documentId));
            string QueryString = "?DocumentID=" + (EdocumentId) + "&ProjectId=" + projectId + "&DepartmentId=" + departmentId;
            Response.Redirect("~/secure/Core/ReadOCRText.aspx" + QueryString);

        }
        //DMSENH6-4657 BE
        protected void getDynamicControlAfterPageLoad_BS()
        {
            string TXTTEXT;
            string TXTID;
            string DRPVALUE;
            string DRPID;

            foreach (Control c in pnlIndexpro_BS.Controls)
            {
                if (c is HtmlTable)
                {
                    HtmlTable td = (HtmlTable)c;
                    foreach (Control tc in td.Controls)
                    {
                        if (tc is HtmlTableRow)
                        {
                            HtmlTableRow row = (HtmlTableRow)tc;
                            foreach (Control trc in row.Controls)
                            {
                                if (trc is HtmlTableCell)
                                {
                                    HtmlTableCell cell = (HtmlTableCell)trc;
                                    //int count = this.NumberOfControls;

                                    foreach (Control control in cell.Controls)
                                    {
                                        if (control is TextBox)
                                        {
                                            TextBox txt = control as TextBox;

                                            TXTTEXT = txt.Text.ToString();
                                            TXTID = txt.ID;
                                            DataTable dt = (DataTable)ViewState["dt"];
                                            for (int i = dt.Rows.Count - 1; i >= 0; i--)
                                            {
                                                DataRow dr = dt.Rows[i];// checking for duplicated if exists delete and reenter textbox
                                                if (dr["DRPID"].ToString() == TXTID)
                                                {
                                                    dr.Delete();
                                                }
                                            }
                                            dt.Rows.Add(TXTID, TXTTEXT);//adding to the datatable
                                            ViewState["dt"] = dt;
                                        }
                                        else if (control is DropDownList)
                                        {
                                            DropDownList drop = control as DropDownList;
                                            DRPVALUE = drop.SelectedValue;
                                            DRPID = drop.ID;

                                            DataTable dt = (DataTable)ViewState["dt"];
                                            for (int i = dt.Rows.Count - 1; i >= 0; i--)
                                            {
                                                DataRow dr = dt.Rows[i];//checking for duplicated if exists delete and reenter textbox
                                                if (dr["DRPID"].ToString() == DRPID)
                                                {
                                                    dr.Delete();
                                                }
                                            }
                                            dt.Rows.Add(DRPID, DRPVALUE);//adding to the datatable
                                            ViewState["dt"] = dt;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void btnCommonSubmitSub2_Click_BS(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            TemplateBL bl = new TemplateBL();
            SearchFilter filter = new SearchFilter();
            filter.DocumentTypeID = Convert.ToInt32(cmbMainTag_BS.SelectedValue);
            ds = bl.GetTagDetails(filter, "SubTag", hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                cmbSubTag_BS.Items.Clear();
                cmbSubTag_BS.DataSource = ds.Tables[0];
                cmbSubTag_BS.DataTextField = "TextField";
                cmbSubTag_BS.DataValueField = "ValueField";
                cmbSubTag_BS.DataBind();
            }
            else
            {
                cmbSubTag_BS.Items.Clear();
            }
            GetTemplateDetails_BS(hdnLoginOrgId_BS.Value, hdnLoginToken_BS.Value, "false");
        }

        protected string getIndexFieldsAndColumnsList_BS()
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();

            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId_BS.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken_BS.Value = loginUser.LoginToken;

            string action = "ExportDocumentType";
            filter.DocumentTypeID = Convert.ToInt32(cmbDocumentType_BS.SelectedValue);
            filter.DepartmentID = Convert.ToInt32(cmbDepartment_BS.SelectedValue);

            string IndexFieldsList = string.Empty;

            try
            {
                DataSet dsFieldNames = new BatchUploadBL().GetHeadersForBatchUpload(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                if (dsFieldNames.Tables.Count > 0)
                {
                    for (int i = 0; i < dsFieldNames.Tables[0].Rows.Count; i++)
                    {
                        IndexFieldsList += (dsFieldNames.Tables[0].Rows[i]["TemplateFields_vName"].ToString()).Replace(" ", string.Empty) + "|" + dsFieldNames.Tables[0].Rows[i]["TemplateFields_vDBFld"].ToString() + "$";
                    }
                }

            }
            catch { throw new Exception(CoreMessages.GetMessages(hdnAction_BS.Value, "ERROR")); }
            return IndexFieldsList.Length > 0 ? IndexFieldsList.Substring(0, IndexFieldsList.Length - 1) : IndexFieldsList;
        }


        #endregion

        //DMSENH6-4657 BS
        //to make document dropdown defalut selection
        protected void Search_ActiveTabChanged(object sender, EventArgs e)
        {

            if (Search.ActiveTabIndex.Equals(0))
            {
                cmbDocumentType_OCR.SelectedIndex = 0;
                cmbDepartment_OCR.SelectedIndex = 0;
                cmbDocumentType.SelectedIndex = 0;
                cmbDepartment.SelectedIndex = 0;
            }
            else if (Search.ActiveTabIndex.Equals(1))
            {
                cmbDocumentType_BS.SelectedIndex = 0;
                cmbDepartment_BS.SelectedIndex = 0;
                cmbDocumentType_OCR.SelectedIndex = 0;
                cmbDepartment_OCR.SelectedIndex = 0;
            }

            else if (Search.ActiveTabIndex.Equals(2))
            {
                cmbDocumentType_BS.SelectedIndex = 0;
                cmbDepartment_BS.SelectedIndex = 0;
                cmbDocumentType.SelectedIndex = 0;
                cmbDepartment.SelectedIndex = 0;
            }
        }
        //DMSENH6-4657 BE

        //protected void fetchExcelData(object sender, EventArgs e)
        //{
        //    var reader = new StreamReader(AsyncFileUpload1.FileContent);
        //    List<string> listA = new List<string>();
        //    while (!reader.EndOfStream)
        //    {
        //        var line = reader.ReadLine();
        //        var values = line.Split(';');

        //        listA.Add(values[0]);
        //    }
        //    hdnControlNames_BS.Value = string.Join(",", listA.ToArray());

        //    //foreach (TextBox textBox in pnlIndexpro_BS.Controls.OfType<TextBox>())
        //    //{
        //    //    if (textBox.ID == "InvoiceNumber")
        //    //    {
        //    //        textBox.Text = string.Join(",", listA.ToArray());
        //    //    }
        //    //}

        //    //ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_InvoiceNumber.Text = string.Join(",", listA.ToArray());
        //    //((TextBox)pnlIndexpro_BS.Controls["InvoiceNumber"]).Text = string.Join(",", listA.ToArray());
        //    //pnlIndexpro_BS.InvoiceNumber.Text = string.Join(",", listA.ToArray());
        //}

        //protected void AsyncFileUpload2_UploadedComplete(object sender, AsyncFileUploadEventArgs e)
        //{
        //    string CSVFile = System.Configuration.ConfigurationManager.AppSettings["CSVFile"];
        //    string filepath = Path.GetFullPath(AsyncFileUpload2.FileName);
        //    string Filename = AsyncFileUpload2.FileName;
        //    AsyncFileUpload2.SaveAs(CSVFile + Filename);

        //    var reader = new StreamReader(AsyncFileUpload2.FileContent);
        //    List<string> listA = new List<string>();
        //    while (!reader.EndOfStream)
        //    {
        //        var line = reader.ReadLine();
        //        var values = line.Split(';');

        //        listA.Add(values[0]);
        //    }
        //    //pnlIndexpro_BS.Controls.Clear();
        //    //generatecontrols_BS();

        //    TextBox textBox = (TextBox)pnlIndexpro_BS.FindControl("InvoiceNumber");
        //    //pnlIndexpro_BS.Controls.Add(textBox);
        //    textBox.Text = string.Join(",", listA.ToArray());

        //    Indexupdatepanel_BS.Update();

        //    //ScriptManager.RegisterStartupScript(this, GetType(), "ChangeTextBoxText", "<script type='text/javascript'>$('#" + textBox + "').val('" + string.Join(",", listA.ToArray()) + "');</script>", false);

        //    //TextBox txtFieldValue = (TextBox)plhClauses.FindControl("txtFieldValue_Row_" + slNo.ToString());
        //    //hdnControlNames_BS.Value = string.Join(",", listA.ToArray());

        //    // TextBox textBox = Page.FindControl("ctl00_ContentPlaceHolder2_ContentPlaceHolder1_Search_NormalSearch_InvoiceNumber")<TextBox>();

        //    //foreach (TextBox textBox in pnlIndexpro_BS.Controls.OfType<TextBox>())
        //    //{
        //    //    if (textBox.ID == "InvoiceNumber")
        //    //    {
        //    //        textBox.Text = string.Join(",", listA.ToArray());
        //    //    }
        //    //}
        //}

        protected void Download(object sender, EventArgs e)
        {

            using (ZipFile zip = new ZipFile())
            {
                try
                {
                    string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                    SqlConnection con = new SqlConnection(connectionstring);

                    var doctype = cmbDocumentType_BS.SelectedValue;
                    var depttype = cmbDepartment_BS.SelectedValue;

                    //TextBox textBox = (TextBox)pnlIndexpro_BS.FindControl("InvoiceNumber");
                    //string mulinv = textBox.Text;

                    //string[] invarray = mulinv.Split(',');
                    List<string> listA = new List<string>();

                    foreach (GridViewRow gr in GridView1.Rows)
                    {
                        CheckBox chk = (CheckBox)gr.FindControl("chkRow");
                        if (chk.Checked)
                        {
                            listA.Add(gr.Cells[3].Text.ToString() + "~" + gr.Cells[4].Text.ToString());
                            string inv = gr.Cells[3].Text.ToString();

                        }
                    }


                    DataTable dt = new DataTable();
                    DataSet ds = new DataSet();
                    ds.Tables.Add(dt);
                    foreach (string inv in listA)
                    {
                        SqlCommand cmd = new SqlCommand("[dbo].[GetMulInvoices2]", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);

                        cmd.Parameters.Add(new SqlParameter("@UPLOAD_iDocType", doctype));
                        cmd.Parameters.Add(new SqlParameter("@UPLOAD_iProcessID", inv.Split('~')[1]));
                        cmd.Parameters.Add(new SqlParameter("@UPLOAD_vFld1", inv.Split('~')[0]));

                        sda.Fill(dt);

                    }

                    string filePath = "";
                    string filePath2 = "";
                    string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                    if (!Directory.Exists(TempFolder))
                    {
                        Directory.CreateDirectory(TempFolder);
                    }

                    List<string> pathList = new List<string>();
                    List<string> pdfpathList = new List<string>();

                    string extn = "";
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        filePath = ds.Tables[0].Rows[i]["UPLOAD_vDocPhysicalPath"].ToString();
                        filePath2 = beforedot(filePath) + ".zip" + "#" + System.IO.Path.GetExtension(filePath);
                        pathList.Add(filePath2);
                        pdfpathList.Add(filePath);

                        extn = System.IO.Path.GetExtension(filePath);
                        //zip.AddFile(filePath2, Server.MapPath("/tmp") + "/DownloadedFile.zip");

                    }
                    string Temppath = string.Empty;

                    string Temppath1 = Server.MapPath("\\tmp");
                    if (File.Exists(Temppath1 + "\\" + "Tempzip.zip"))
                    {
                        File.Delete(Temppath1 + "\\" + "Tempzip.zip");
                    }

                    if (Directory.Exists(Temppath1 + "\\" + "Tempzip"))
                    {
                        string[] filePathss = Directory.GetFiles(Temppath1 + "\\" + "Tempzip");
                        foreach (string filePaths in filePathss)
                            File.Delete(filePaths);

                        Directory.Delete(Temppath1 + "\\" + "Tempzip");
                    }



                    foreach (string pth in pathList)
                    {
                        string fname = Path.GetFileName(pth);
                        Temppath = Server.MapPath("\\tmp" + "\\Tempzip");
                        if (!Directory.Exists(Server.MapPath("\\tmp" + "\\Tempzip")))
                        {
                            Directory.CreateDirectory(Server.MapPath("\\tmp" + "\\Tempzip"));

                        }
                        FileZip.Unzip1(pth, Temppath);

                    }

                    ZipFile zip1 = new ZipFile(Temppath1 + "\\" + "Tempzip.zip");
                    zip1.AddDirectory(Temppath1 + "\\Tempzip");
                    zip1.Save();

                    //Response.ContentType = "application/zip";
                    //Response.AddHeader("Content-Disposition", "attachment;filename=" + "aAAAAA" + ".zip");
                    //Response.TransmitFile(Temppath1 + "\\Tempzip.zip");
                    //Response.End();

                    //foreach (string pdfpth in pdfpathList)
                    //{
                    //    //string fname = Path.GetFileName(pdfpth);
                    //    //zip.AddFile(pdfpth, Server.MapPath("/tmp" + fname));
                    //}

                    //string gpath = Server.MapPath("/tmp");
                    string sPath = Temppath1 + "\\Tempzip.zip";


                    //if (System.IO.File.Exists(sPath))
                    //    System.IO.File.Delete(sPath);

                    //File.Copy(filePath, sPath);

                    Session["zipFilePath"] = sPath;
                    Response.Redirect("Downloadnzip.ashx?path=" + sPath);

                    /*
                    Response.ContentType = "application/zip";
                    Response.AddHeader("Content-Disposition", "attachment;filename=DownloadedFile.zip");
                    string path = Server.MapPath("/tmp/") + "DownloadedFile.zip";


                    Response.TransmitFile(Server.MapPath("/tmp/") + "DownloadedFile.zip");
                    //Response.Flush();
                    Response.End();
                   
                    //-----------------
                    //Response.Clear();
                    //Response.AddHeader("Content-Disposition", "attachment; filename=DownloadedFile.zip");
                    //Response.ContentType = "application/zip";
                    //zip.Save(Response.OutputStream);
                    //Response.End();

                    */

                }
                catch
                {


                }


            }

        }

        //private void hideProgressBar()
        //{

        //    StringBuilder sb1 = new StringBuilder();
        //    //sb1.Append("$(document).ready(function(){ \n $('#statusPopup').hide(); \n });");

        //    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('hi')", true);
        //}

        //protected void DownloadAll(object sender, EventArgs e)
        //{

        //    using (ZipFile zip = new ZipFile())
        //    {
        //        try
        //        {
        //            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
        //            SqlConnection con = new SqlConnection(connectionstring);

        //            var doctype = cmbDocumentType_BS.SelectedValue;
        //            var depttype = cmbDepartment_BS.SelectedValue;

        //            //TextBox textBox = (TextBox)pnlIndexpro_BS.FindControl("InvoiceNumber");

        //            string[] mulinv = bulkinv;


        //            if (mulinv.Length > 0)
        //            {
        //                string[] invarray = bulkinv;

        //                DataTable dt = new DataTable();
        //                DataSet ds = new DataSet();
        //                ds.Tables.Add(dt);
        //                foreach (var inv in invarray)
        //                {
        //                    SqlCommand cmd = new SqlCommand("[dbo].[GetMulInvoices]", con);
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    SqlDataAdapter sda = new SqlDataAdapter(cmd);

        //                    cmd.Parameters.Add(new SqlParameter("@UPLOAD_iDocType", doctype));
        //                    cmd.Parameters.Add(new SqlParameter("@UPLOAD_iDepartment", depttype));
        //                    cmd.Parameters.Add(new SqlParameter("@UPLOAD_vFld1", inv));

        //                    sda.Fill(dt);

        //                }


        //                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        //                {

        //                    string filePath = ds.Tables[0].Rows[i]["UPLOAD_vDocPhysicalPath"].ToString();
        //                    zip.AddFile(filePath, "files");
        //                }
        //                Response.Clear();
        //                Response.AddHeader("Content-Disposition", "attachment; filename=DownloadedFile.zip");
        //                Response.ContentType = "application/zip";
        //                zip.Save(Response.OutputStream);
        //                Response.End();

        //            }
        //            else
        //            {
        //                emptyinv.Text = "No invoices found...";
        //                emptyinv.ForeColor = System.Drawing.Color.Red;
        //                emptyinv.Enabled = true;
        //                emptyinv.Visible = true;
        //            }

        //        }
        //        catch (Exception)
        //        {

        //            throw;
        //        }


        //    }

        //}

        protected void SearchNew(object sender, EventArgs e)
        {
            try
            {
                string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                SqlConnection con = new SqlConnection(connectionstring);
                //string param="";

                var SearchOption = cmbSearchOption_BS.SelectedValue;
                var DocumentType = cmbDocumentType_BS.SelectedValue;
                var Department = cmbDepartment_BS.SelectedValue;

                //var DocumentType = "1144";
                //var Department = "1079";

                var Refid = txtRefid_BS.Text;
                var Keywords = txtKeyword_BS.Text;
                var count = hdnCountControls_BS.Value;
                var archive = chkArchive_BS.Checked;
                var MainTagID = cmbMainTag_BS.SelectedValue;
                var SubTagID = cmbSubTag_BS.SelectedValue;
                var PageNo = hdnPageNo_BS.Value;
                var RowsPerPage = hdnRowsPerPage_BS.Value;

                var check = 1;
                if (archive == true)
                {
                    check = 0;
                }
                else
                {
                    check = 1;
                }

                string param = SearchOption + '|' + DocumentType + '|' + Department + '|' + Refid + '|' + Keywords + '|' + check + '|' + MainTagID + '|' + SubTagID + '|' + PageNo + '|' + RowsPerPage;

                string[] userdata = null;
                SearchFilter filter = new SearchFilter();
                DocumentBL bl = new DocumentBL();
                userdata = param.Split('|');
                filter.SearchOption = userdata[0];
                filter.DocumentTypeID = Convert.ToInt32(userdata[1]);
                filter.DepartmentID = Convert.ToInt32(userdata[2]);
                filter.Refid = userdata[3].Trim();
                filter.keywords = userdata[4].Trim();
                filter.Active = Convert.ToInt32(userdata[5].Trim());
                filter.MainTagID = Convert.ToInt32(userdata[6].Trim());
                filter.SubTagID = Convert.ToInt32(userdata[7].Trim());
                filter.DocPageNo = Convert.ToInt32(userdata[8].Trim());
                filter.RowsPerPage = Convert.ToInt32(userdata[9].Trim());
                filter.WhereClause = hdnSearchString.Value;


                SearchDocumentsNew(filter, "SearchDocuments", hdnLoginOrgId_BS.Value.ToString(), hdnLoginToken_BS.Value.ToString());
                //CallSearchDocuments2(hdnLoginOrgId_BS.Value.ToString(), hdnLoginToken_BS.Value.ToString(), "SearchDocuments", param, "", false);
                btndownloadall.Visible = true;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public Results SearchDocumentsNew(SearchFilter filter, string action, string loginOrgId, string loginToken)
        {
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            DataSet ds = new DataSet();

            int? MainTagID = null;
            MainTagID = filter.MainTagID > 1 ? filter.MainTagID : MainTagID;
            int? SubTagID = null;
            SubTagID = filter.SubTagID > 1 ? filter.SubTagID : SubTagID;


            string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
            SqlConnection con = new SqlConnection(connectionstring);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "GetDocumentSearchByorgID";
            cmd.CommandType = CommandType.StoredProcedure;


            cmd.Parameters.AddWithValue("@in_iDocID", filter.UploadDocID);
            cmd.Parameters.AddWithValue("@SP_DocTypeID", filter.DocumentTypeID);
            cmd.Parameters.AddWithValue("@in_iDepartmentID", filter.DepartmentID);
            cmd.Parameters.AddWithValue("@in_vSearchType", filter.SearchOption);
            cmd.Parameters.AddWithValue("@in_vRefID", filter.Refid);
            cmd.Parameters.AddWithValue("@in_vKeywords", filter.keywords);
            cmd.Parameters.AddWithValue("@in_iActive", filter.Active);
            cmd.Parameters.AddWithValue("@in_iMainTagID", MainTagID);
            cmd.Parameters.AddWithValue("@in_iSubTagID", SubTagID);
            cmd.Parameters.AddWithValue("@PageNo", filter.DocPageNo);
            cmd.Parameters.AddWithValue("@PageSize", filter.RowsPerPage);
            cmd.Parameters.AddWithValue("@in_vWhereClause", filter.WhereClause);
            cmd.Parameters.AddWithValue("@in_vAction", action);
            cmd.Parameters.AddWithValue("@in_vLoginToken", loginToken);
            cmd.Parameters.AddWithValue("@in_iLoginOrgId", loginOrgId);

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;

            da.Fill(ds);

            //ds = cmd.ExecuteDataSet();
            //results.ResultDS = ds;

            //DocumentDownload DocumentDownloadDetails = new DocumentDownload();
            ////DocumentDownloadDetails.HtmlTable = createHtmlTable(ds);
            //if (results.DocumentDownloads == null)
            //{
            //    results.DocumentDownloads = new List<DocumentDownload>();
            //}

            //DocumentDownloadDetails.RowsCount = ds.Tables[0].Rows.Count;
            //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    DocumentDownloadDetails.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
            //}

            //results.DocumentDownloads.Add(DocumentDownloadDetails);
            //GridView1.Columns[2].HeaderText = "Invoice";

            GridView1.DataSource = ds;
            GridView1.DataBind();



            for (int i = 0; i < GridView1.Rows.Count; i++)
            {

                for (int j = 0; j < GridView1.Rows[i].Cells.Count; j++)
                {
                    GridView1.HeaderRow.Cells[2].Visible = false;
                    GridView1.Rows[i].Cells[2].Visible = false;


                    //string encoded = GridView1.Rows[i].Cells[1].Text;
                    //GridView1.Rows[i].Cells[1].Text = Context.Server.HtmlDecode(encoded);

                    //GridView1.Rows[i].Cells[1].Text = "<a onclick='Redirect('" + encoded + "') '><img border='0' src = '../../Assets/Skin/Images/view.png' alt='view' /> </a>";

                }
            }
            //GridView1.Columns[3].Visible = false;

            return results;

            /* SqlDataAdapter da = new SqlDataAdapter();
             DataSet ds = new DataSet();

             SqlDataReader dbRdr = null;
             SqlCommand dbCmd = null;
             SqlConnection dbCon = null;
             Results results = new Results();
             results.ActionStatus = "SUCCESS";
             try
             {
                 dbCon = OpenConnection();
                 dbCmd = new SqlCommand();
                 dbCmd.Connection = dbCon;
                 dbCmd.CommandType = CommandType.StoredProcedure;

                 dbCmd.CommandText = "USP_SearchDocuments";
                 dbCmd.Parameters.Add("@in_iCurrUserId", SqlDbType.Int).Value = filter.CurrUserId;
                 if (filter.UploadDocID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDocID", SqlDbType.Int).Value = filter.UploadDocID;
                 }
                 if (filter.DocumentTypeID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDocuTypeID", SqlDbType.Int).Value = filter.DocumentTypeID;
                 }
                 if (filter.DepartmentID > 0)
                 {
                     dbCmd.Parameters.Add("@in_iDepartmentID", SqlDbType.Int).Value = filter.DepartmentID;
                 }
                 if (filter.SearchOption != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vSearchType ", SqlDbType.VarChar, 150).Value = filter.SearchOption;
                 }
                 if (filter.Refid != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vRefID ", SqlDbType.VarChar, 150).Value = filter.Refid;
                 }
                 if (filter.keywords != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vKeywords", SqlDbType.VarChar, 150).Value = filter.keywords;
                 }
                 if (filter.Active != 0)
                 {
                     dbCmd.Parameters.Add("@in_iActive", SqlDbType.Int).Value = filter.Active;
                 }
                 if (filter.MainTagID != 0)
                 {
                     dbCmd.Parameters.Add("@in_iMainTagID", SqlDbType.Int).Value = filter.MainTagID;
                 }
                 if (filter.SubTagID != 0)
                 {
                     dbCmd.Parameters.Add("@in_iSubTagID", SqlDbType.Int).Value = filter.SubTagID;
                 }
                 if (filter.DocPageNo != 0)
                 {
                     dbCmd.Parameters.Add("@PageNo", SqlDbType.Int).Value = filter.DocPageNo;
                 }
                 if (filter.RowsPerPage != 0)
                 {
                     dbCmd.Parameters.Add("@PageSize", SqlDbType.Int).Value = filter.RowsPerPage;
                 }
                 if (filter.WhereClause != string.Empty)
                 {
                     dbCmd.Parameters.Add("@in_vWhereClause", SqlDbType.NVarChar).Value = filter.WhereClause;
                 }

                 dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar, 30).Value = action;
                 //sesstion
                 dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                 dbCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = loginOrgId;



                 if (action == "GetTaggedPages")
                 {
                     dbRdr = dbCmd.ExecuteReader();

                     while (dbRdr.Read())
                     {
                         if (results.IndexFields == null)
                         {
                             results.IndexFields = new List<IndexField>();
                         }
                         IndexField Index = new IndexField();
                         Index.ListItemId = Convert.ToInt32(dbRdr["PageNo"]);
                         results.IndexFields.Add(Index);
                     }
                 }
                 else
                 {
                     da.SelectCommand = dbCmd;
                     da.Fill(ds);
                     results.ResultDS = ds;

                     DocumentDownload DocumentDownloadDetails = new DocumentDownload();
                     DocumentDownloadDetails.HtmlTable = createHtmlTable(ds);
                     if (results.DocumentDownloads == null)
                     {
                         results.DocumentDownloads = new List<DocumentDownload>();
                     }

                     DocumentDownloadDetails.RowsCount = ds.Tables[0].Rows.Count;
                     if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                     {
                         DocumentDownloadDetails.TotalRowcount = Convert.ToInt32(ds.Tables[0].Rows[0]["TotalRows"].ToString());
                     }

                     results.DocumentDownloads.Add(DocumentDownloadDetails);               
                 }
             }
             catch (Exception ex)
             {
                 results.Message = ex.ToString();
                 throw ex;
             }
             finally
             {
                 if (dbRdr != null)
                 {
                     dbRdr.Dispose();
                 }
                 if (dbCmd != null)
                 {
                     dbCmd.Dispose();
                 }
                 CloseConnection(dbCon);
             }
             return results;*/
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            SearchNew(sender, e);
        }
        
        

    }
}
