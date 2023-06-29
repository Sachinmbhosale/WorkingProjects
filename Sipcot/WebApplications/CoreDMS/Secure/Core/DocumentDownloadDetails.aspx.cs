/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * ----------       -------------       ----------          ------------------------------------------------------------------
 * 2-28-2015        Gokuldas            DMS04-3400          Annotations should work in all browsers
 * 01-03-2015	    Gokuldas            DMS04-3417          After deleting a document, control should move to the Document Search page. 
 *                                                          and require confirmation on deleting a document
 * 20 Mar 2015      Yogeesha Naik       DMS04-3470          When “Tagged View” selected from “Document View” option, it is automatically 
 *                                                          going to “Original View” option
 * 20 Mar 2015      Yogeesha Naik       DMS04-3712	        Coding Standards, Revamp, Optimization, Naming Convention
 * 21 Mar 2015      Yogeesha Naik       DMS04-3459	        User rights issue prevents user for accessing Annotations options.
 * 04 Apr 2015      Sabina.K.V          DMS5-4101           If index field name contains a space then calendar/multi select will not work
 * 05 may 2015      Gokuldas.palapatta  DMS5-4232           Download of document with its original name.
 * 03 jun 2015      Gokuldas.palapatta  DMS5-4327           When a new page is inserted at either middle or end of a document, the insertion is not working; the previous page is overriding the inserted page.(changed in javascript)
 * 15 06 2015       gokuldas.palapatta  DMS5-4370	        Version Control has been implemented, but system does not have the menu to view/retrieve earlier versions of the document.( a new button and function added in java script to redirect
 *15 06 2015       gokuldas.palapatta  	DMS5-4377           Version is not updating when pages are added a deleted to/from a document.
 *15 06 2015       gokuldas.palapatta   DMS5-4371		    Clicking the Search Again button after visiting the History page, takes user back to a blank History page

====================================================================================================================================== */

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Data;
using System.Configuration;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using OfficeConverter;
using System.Net;
using AjaxControlToolkit;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class DocumentDownloadDetails : PageBase
    {

        #region DocumentDownloadDetails
        DocTagBL BL = new DocTagBL();
        #region Variable declaration

        protected string AfterDot = "";
        public string pageRights = string.Empty;
        //DMS04-3459M -removed constant keyword
        public string PageName = string.Empty;
        //DMS04-3470D
        //protected bool IsMaintag = false;
        public string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
        DataTable TotalTagPages = null;
        int pageno = 0;
        #endregion Variabledeclaration

        #region Property declaration
        //To save the dynamically created index fields count
        protected int NumberOfControls
        {
            get { return (int)ViewState["NumControls"]; }
            set { ViewState["NumControls"] = value; }
        }

        // No of times document modified
        protected int ModifiedCount
        {
            get
            {
                if (Session["ModifiedCount"] != null)
                {
                    return (int)Session["ModifiedCount"];
                }
                else
                    return 0;
            }
            set { Session["ModifiedCount"] = value; }
        }
        //To update the annoted pages in annotation table
        protected DataTable AnnotationPageNo
        {
            get
            {
                if (ViewState["AnnotationPageNo"] != null)
                {
                    return (DataTable)ViewState["AnnotationPageNo"];
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("OldPageNo", typeof(int));
                    dt.Columns.Add("NewPageNo", typeof(int));
                    return dt;
                }
            }
            set { ViewState["AnnotationPageNo"] = value; }
        }
        //To update the tagged pages in tagmaster
        protected DataTable TaggedPageNo
        {
            get
            {
                if (ViewState["TaggedPageNo"] != null)
                {
                    return (DataTable)ViewState["TaggedPageNo"];
                }
                else
                {
                    DataTable dt = new DataTable();
                    dt.Columns.Add("OldPageNo", typeof(int));
                    dt.Columns.Add("NewPageNo", typeof(int));
                    return dt;
                }
            }
            set { ViewState["TaggedPageNo"] = value; }
        }

        #endregion Property declaration

        protected void Page_Load(object sender, EventArgs e)
        {

            //DMS04-3470D
            //IsMaintag = false;
            CheckAuthentication();
            btnInedxSave.Attributes.Add("onclick", "javascript:return validate()");
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            //DMS04-3459A - Pass main page name to get rights; as this page navigated by multiple pages such as Maker, Checker & Search
            PageName = PreviousPage;
            /*DMS5-4370	BS*/
            if (PreviousPage == "documenthistoryview" || PreviousPage == "documentdownloaddetails")
            {
                PageName = "documentdownloadsearch";
            }
            /*DMS5-4370	BE*/
            pageRights = GetPageRights(PageName);
            hdnPageRights.Value = pageRights;
            // Based on user right enable or disable annotation toolbar;
            if (ModifiedCount == 0)
                PDFViewer1.AnnotationToolbarVisible = pageRights.ToLower().Contains("annotate");

            if (!IsPostBack)
            {

                GetSginatureDetails();
                /* DMS5-4371 BS */
                if (PreviousPage != "documenthistoryview")
                {
                    Session["hdnSearchPageUrl"] = Request.ServerVariables["HTTP_REFERER"];
                }
                /* DMS5-4371 BE */
                // Get watermark from querytring and store into session
                Session["Watermark"] = Request.QueryString["Watermark"] != null ? Request.QueryString["Watermark"].ToString() : string.Empty;
                ApplyPageRights(pageRights, this.Form.Controls);
                hdnUploaded.Value = "FALSE";
                if (Request.QueryString["depid"] != null && Request.QueryString["docid"] != null && Request.QueryString["id"] != null)
                {
                    //To load Index fields8

                    hdntempPagecount.Value = "1";
                    this.NumberOfControls = 0;
                    GetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value);
                    LoadMainTag();

                    pnlControls.Enabled = false;
                    InitializeDocumentView();
                    if (Request.QueryString["Page"] == "MakerDashboard" || Request.QueryString["Page"] == "CheckerDashBoard")
                    {
                        RemarksPanel.Visible = true;
                        LockDocument();
                        LoadStatusDropdown();
                    }
                    else
                    {
                        RemarksPanel.Visible = false;
                    }

                    //Get the annoatation for saving in datatable for delete purpose
                    GetAnnotateDocumentDetails();
                    //loadfileiniframe();
                    //DMS04-3712D
                    //GetTaggedDocumentDetails();
                    //DMS04-3712A
                    UpdatePropertyTaggedPagesTracker();

                    if (Convert.ToInt32(Request.QueryString["MainTagId"]) != 0)
                    {
                        Logger.Trace("loading pages dropdown for tagged pages started", Session["LoggedUserId"].ToString());
                        //DMS04-3470D
                        //IsMaintag = true;
                        ddldocumentview.SelectedValue = "Tagged View";
                        getAvailablePages();
                        LoadImageToViewer(string.Empty);
                        Logger.Trace("loading pages dropdown for tagged pages completed", Session["LoggedUserId"].ToString());
                    }
                    //To load drop down pages with fulltext pages
                    else if (Session["FullTextClause"] != null)
                    {
                        Logger.Trace("loading pages dropdown for fulltext started", Session["LoggedUserId"].ToString());
                        ddldocumentview.SelectedValue = "FullText View";
                        getAvailablePages();
                        LoadImageToViewer(string.Empty);
                        Logger.Trace("loading pages dropdown for fulltext completed", Session["LoggedUserId"].ToString());
                    }
                    else
                    {
                        LoadImageToViewer(string.Empty);
                        //DMS04-3470D
                        //IsMaintag = false;
                    }


                    //To load Index fields
                    MailPanelFill();//Call to fill the mail panel with default value

                    string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                    UploadDocBL bl = new UploadDocBL();
                    SearchFilter filter = new SearchFilter();
                    string action = "Viewcount";
                    filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                    Session["ProcessId"] = filter.UploadDocID;//Used in pagebase class to release document
                    Results res = bl.UpdateTrackTableOnviewd(filter, action, hdnLoginOrgId.Value, hdnLoginToken.Value);

                }

                //Intialize Session Variables
                Session["TempFileLocation"] = "";
                Session["TempFileLocation"] = "";
                Session["Ext"] = "";
            }

            else
            {
                GetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value);

            }
            ManageTagview();


            //To enable buttons 
            btnDicardChanges.Enabled = false;
            drpDeleteCount.Attributes.Add("onChange", "javascript:return drpDeleteChange();");
            DDLDrop.Attributes.Add("onChange", "javascript:return gotoPage();");
            ddldocumentview.Attributes.Add("onChange", "javascript:return DDLDocumentViewChanged();");
            DDLTagpagecount.Attributes.Add("onChange", "javascript:return gotoPageTag();");

            btnPrevDoc.Enabled = true;
            btnNextDoc.Enabled = true;


        }

        public string Getcssclass(string cntrl)
        {
            string csclass = string.Empty;
            if (cntrl == "drp")
            {
                csclass = "ComboStyle";
            }
            else if (cntrl == "txt")
            {
                csclass = "TextBoxStyle";
            }
            else
            {
                csclass = "LabelStyle";
            }
            return csclass;
        }

        protected string GetDBFieldName(DataSet dsFieldNames, string strControlName)
        {
            string strFieldName = string.Empty;
            for (int i = 0; i < dsFieldNames.Tables[0].Rows.Count; i++)
            {

                if (dsFieldNames.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty) == strControlName.Replace(" ", string.Empty))
                {
                    strFieldName = dsFieldNames.Tables[0].Rows[i]["TemplateFields_vDBFld"].ToString();
                }

            }

            Logger.Trace("GetDBFieldName the DBFld " + strFieldName, Session["LoggedUserId"].ToString());

            return strFieldName;
        }

        public void SaveIndexPannelDetails()
        {
            int fldcount = 0;
            string XML = string.Empty;
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            string strQuery = string.Empty;
            string strColumns = string.Empty;
            string strValues = string.Empty;

            SearchFilter filter = new SearchFilter();
            string action = "ExportDocumentType";
            filter.DocumentTypeID = Convert.ToInt32(Request.QueryString["docid"]);
            filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);

            string xml = string.Empty;
            string Uploadaction = string.Empty;

            DataSet dsFieldNames = new BatchUploadBL().GetHeadersForBatchUpload(filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
            foreach (Control c in pnlIndexpro.Controls)
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
                                            strValues += GetDBFieldName(dsFieldNames, txt.ID) + "=&apos;" + txt.Text + "&apos;,";

                                            fldcount++;
                                        }
                                        else if (control is DropDownList)
                                        {
                                            DropDownList drop = control as DropDownList;
                                            if (drop.SelectedItem.Text.ToLower() != "--select--")
                                            {
                                                strValues += GetDBFieldName(dsFieldNames, drop.ID) + "=&apos;" + drop.SelectedItem + "&apos;,";


                                            }

                                            fldcount++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            strQuery = "update upload set " + strValues.Substring(0, strValues.Length - 1) + " where UPLOAD_iProcessID = " + Convert.ToString(Request.QueryString["id"]);
            xml = "<Table><row><query>" + strQuery + "</query></row></Table>";
            Uploadaction = "UpdateIndexfields";
            int ReturnCode = 1;
            int iProcId = Convert.ToInt32(Request.QueryString["id"]);
            ReturnCode = new DocumentBL().ManageDocumentUpload(xml, string.Empty, hdnLoginOrgId.Value, hdnLoginToken.Value, Uploadaction, iProcId);
            if (ReturnCode == 0)
            {
                divMsg.InnerHtml = "Index fields values saved succesfully";
                lblResult.Text = "Index fields values saved succesfully";
                rtrnsucess.Text = "Index fields values saved succesfully";
                Logger.Trace("Index fields values saved succesfully", Session["LoggedUserId"].ToString());
                //Response.Write("<script>alert('Error Occured While Saving Document!');</script>");
            }
            else
            {
                divMsg.InnerHtml = "Index fields values not saved succesfully";
            }
        }

        public void GetTemplateDetails(string loginOrgId, string loginToken)
        {

            pnlIndexpro.Controls.Clear();
            this.NumberOfControls = 0;

            hdnCountControls.Value = "";
            hdnIndexMinLen.Value = "";
            hdnIndexNames.Value = "";
            hdnMandatory.Value = "";

            TemplateBL bl = new TemplateBL();
            UploadDocBL b2 = new UploadDocBL();
            SearchFilter filter = new SearchFilter();
            DataSet Ds = new DataSet();
            try
            {
                Logger.Trace("GetTemplateDetails Started", Session["LoggedUserId"].ToString());
                //If active = 0 means the document is archived. So disabling delete and update buttons
                if (Convert.ToInt32(Request.QueryString["active"]) == 0)
                {
                    btnDelete.Enabled = false;
                    btnUpdate.Enabled = false;
                }
                string indexname = string.Empty;
                string sortOreder = string.Empty;
                bool IActivive;
                HtmlTable Tab = new HtmlTable();

                Tab.Attributes.Add("style", "Border:Solid 1 black;");
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                string action = "GetUploadDocumentDetailsForDownload";
                filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);

                Ds = b2.GetUploadDocumentDetails(filter, action, hdnLoginOrgId.Value, hdnLoginToken.Value);

                if (Ds != null && Ds.Tables.Count > 0 && Ds.Tables[0].Rows.Count > 0)
                {
                    DataRow currentRow = Ds.Tables[0].Rows[0];
                    Logger.Trace("GetTemplateDetails total record count:" + Ds.Tables[0].Rows.Count, Session["LoggedUserId"].ToString());
                    lblDocType.Text = currentRow["DocName"].ToString();
                    lblDept.Text = currentRow["DeptName"].ToString();
                    txtKeyword.Text = currentRow["Keywords"].ToString();

                    lblVersion.Text = currentRow["DocVersion"].ToString();

                    ViewState["Version"] = currentRow["DocVersion"].ToString(); /* DMS5-4377 A*/
                    ViewState["DocumentData"] = Ds.Tables[0];

                    filter.CurrOrgId = loginUser.LoginOrgId;
                    filter.CurrUserId = loginUser.UserId;
                    filter.DocumentTypeName = currentRow["DocName"].ToString();
                    filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                    DataSet TemplateDs = new TemplateBL().GetTemplateDetails(filter);

                    lblRefID.Text = currentRow["RefId"].ToString();
                    {
                        hdnFileName.Value = currentRow["OrgiFileName"].ToString();
                        hdnEncrpytFileName.Value = currentRow["EncrDocName"].ToString();
                        Session["EncyptImgName"] = hdnEncrpytFileName.Value;
                        hdnFileLocation.Value = currentRow["phyFilepath"].ToString();

                        hdnCountControls.Value = "";

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
                            HtmlTableCell TD3 = new HtmlTableCell();
                            HtmlTableCell TD4 = new HtmlTableCell();
                            TD1.Width = "30px";
                            TD2.Width = "30px";

                            TD4.Width = "30px";

                            // Add label text
                            if (!string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Label", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "Button", StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "HiddenField", StringComparison.OrdinalIgnoreCase) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                            {
                                Label objCnl = new Label();
                                objCnl.ID = "lbl" + i;
                                objCnl.Text = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString() + ":";
                                objCnl.CssClass = Getcssclass("lbl");
                                objCnl.AssociatedControlID = "";
                                if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                {
                                    TD1.Controls.Add(objCnl);
                                }
                                else
                                {
                                    TD3.Controls.Add(objCnl);
                                }
                            }
                            #region textbox control
                            if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "TextBox", StringComparison.OrdinalIgnoreCase))
                            {
                                TextBox objCnltxt = new TextBox();
                                // objCnltxt.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bPostback"]);
                                //DMS5-4101M replaced the space with empty string
                                objCnltxt.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                objCnltxt.CssClass = Getcssclass("txt");
                                TD2.Controls.Add(objCnltxt);
                                objCnltxt.Width = Unit.Pixel(157);
                                objCnltxt.Text = Ds.Tables[0].Rows[0][TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString()].ToString();
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


                                    CalendarExtender calext = new CalendarExtender();
                                    //DMS5-4101M replaced the space with empty string
                                    calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString().Replace(" ", string.Empty);
                                    calext.Format = "dd/MM/yyyy";
                                    TD2.Controls.Add(calext);


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
                                Logger.Trace("Create controls,Load all Textbox dynamic", Session["LoggedUserId"].ToString());

                            }
                            #endregion

                            #region dropdownlist control
                            if (string.Equals(TemplateDs.Tables[0].Rows[i]["TemplateFields_cObjType"].ToString(), "DropDownList", StringComparison.OrdinalIgnoreCase))
                            {

                                DropDownList objCnl = new DropDownList();

                                objCnl.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                objCnl.CssClass = Getcssclass("drp");
                                objCnl.Width = Unit.Pixel(150);
                                int TemplatefldId = Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iId"]);
                                //new code for loading only correspondig values//
                                string Iaction = string.Empty;
                                Iaction = "LoadMain";
                                if (hdnMainvalueid.Value != "")
                                {
                                    TemplatefldId = System.Convert.ToInt16(hdnMainvalueid.Value);
                                    Iaction = "LoadSub";
                                    hdnMainvalueid.Value = "";
                                }


                                PopulateDropdown(objCnl, TemplatefldId, Iaction);//function to populate dropdownlist
                                //new code for loading only correspondig values//
                                objCnl.SelectedIndex = objCnl.Items.IndexOf(objCnl.Items.FindByText(Ds.Tables[0].Rows[0][TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString()].ToString()));
                                objCnl.AutoPostBack = Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bHaschild"]);

                                if (Convert.ToBoolean(TemplateDs.Tables[0].Rows[i]["TemplateFields_bHaschild"]))
                                {
                                    hdnMainvalueid.Value = objCnl.SelectedValue;
                                }

                                if (objCnl.AutoPostBack)
                                {
                                    objCnl.SelectedIndexChanged += new EventHandler(DynamicDropdownList_SelectedIndexChanged);


                                }


                                if (Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iParentId"]) == 0)
                                {
                                    TD2.Controls.Add(objCnl);
                                }
                                else
                                {

                                    TD4.Controls.Add(objCnl);
                                }
                                Logger.Trace("Createcontrols,Load all Dropdownlist dynamic", Session["LoggedUserId"].ToString());
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
                            Tab.Attributes.Add("CssClass", "tabledesign");
                            this.NumberOfControls++;
                            //For javascript validation
                            if (hdnIndexNames.Value.Length == 0)
                            {
                                hdnCountControls.Value = NumberOfControls.ToString();
                                hdnControlNames.Value = indexname;
                                hdnIndexNames.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                hdnIndexMinLen.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                                hdnIndexType.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                                hdnIndexDataType.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                                hdnMandatory.Value = TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                            }
                            else
                            {
                                hdnCountControls.Value = NumberOfControls.ToString();
                                hdnControlNames.Value = hdnControlNames.Value + '|' + indexname;
                                hdnIndexNames.Value = hdnIndexNames.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                hdnIndexType.Value = hdnIndexType.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vInputType"].ToString();
                                hdnIndexDataType.Value = hdnIndexDataType.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vDBType"].ToString();
                                if (!string.IsNullOrEmpty(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) && Convert.ToInt32(TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString()) != 0)
                                {
                                    hdnIndexMinLen.Value = hdnIndexMinLen.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_iMin"].ToString();
                                }
                                else
                                {
                                    hdnIndexMinLen.Value = hdnIndexMinLen.Value + "|" + "0";
                                }
                                hdnMandatory.Value = hdnMandatory.Value + "|" + TemplateDs.Tables[0].Rows[i]["TemplateFields_bMandatory"].ToString().ToLower();//UMF - Add
                            }
                        }
                        pnlIndexpro.Controls.Add(Tab);
                        pnlIndexpro.Visible = true;

                    }
                }
                Logger.Trace("GetTemplateDetails Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("GetTemplateDetails Exception: " + ex.Message, Session["LoggedUserId"].ToString());

            }
        }

        /// <summary>
        /// Populate dynamic dropdownlist
        /// </summary>
        /// <param name="drp"></param>
        /// <param name="TemplateId"></param>
        public void PopulateDropdown(DropDownList drp, int TemplateId, string Action)
        {
            DynamicControlBL objDynamicControlBL = new DynamicControlBL();
            DataSet ds = new DataSet();
            try
            {
                ds = objDynamicControlBL.DynamicPopulateDropdown(TemplateId, Action);
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

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        //To Create index fields dynamically

        /// <summary>
        /// Fill the panel of mail details with default value
        /// </summary>
        /// 
        public void DynamicDropdownList_SelectedIndexChanged(object sender, EventArgs e)
        {

            DynamicControlBL objDynamicControlBL = new DynamicControlBL();
            int TemplateFldId = 0;
            DataSet ds = new DataSet();
            try
            {
                DropDownList ddl = (DropDownList)sender;
                DropDownList ddlBind = (DropDownList)pnlIndexpro.FindControl(ddl.ID + "_sub");//Get the sub dropdown id from panel.

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
                            // ddlBind.SelectedValue = "0";
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        /// <summary>
        /// to get the details for mail sending
        /// </summary>
        public void MailPanelFill()
        {
            DataTable data = new DataTable();
            StreamReader reader = null;
            string strMailmessage = string.Empty;
            string strFilepath = string.Empty;
            string strWebpath = string.Empty;
            try
            {
                Logger.Trace("MailPanelFill started", Session["LoggedUserId"].ToString());
                strWebpath = System.Configuration.ConfigurationManager.AppSettings["FileWeb"];//Virtual path of document to download
                //string strToken = hdnLoginToken.Value;
                string strToken = Path.GetFileNameWithoutExtension(hdnFileLocation.Value);

                strFilepath = '"' + strWebpath + "?Token=" + strToken + '"';//Website link               
                reader = new StreamReader(Server.MapPath("~/Accounts/DownloadMailFormat.htm"));
                string readFile = reader.ReadToEnd();
                data = (DataTable)ViewState["DocumentData"];
                if (data != null && data.Rows.Count > 0)
                {
                    strMailmessage = readFile;
                    strMailmessage = strMailmessage.Replace("$$Project$$", data.Rows[0]["DocName"].ToString());
                    strMailmessage = strMailmessage.Replace("$$Department$$", data.Rows[0]["DeptName"].ToString());
                    strMailmessage = strMailmessage.Replace("$$Document Name$$", data.Rows[0]["OrgiFileName"].ToString());
                    strMailmessage = strMailmessage.Replace("$$Link$$", strFilepath);
                    strMailmessage = strMailmessage.Replace("$$OrgName$$", "Writer Corporation");
                    strMailmessage = strMailmessage.Replace("$$Name$$", "");
                    txtMessage.Text = HtmlHelper.StripHTML(strMailmessage);//Call to function that convert html to plain text
                    txtsubject.Text = data.Rows[0]["OrgiFileName"].ToString();
                }
                Logger.Trace("MailPanelFill finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("MailPanelFill Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        /// <summary>
        /// to copy all files from a directory same as the directory.copy()
        /// </summary>
        /// <param name="Src"></param>
        /// <param name="Dst"></param>
        public static void copyDirectory(string Src, string Dst)
        {
            String[] Files;
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
                Dst += Path.DirectorySeparatorChar;
            if (!Directory.Exists(Dst)) Directory.CreateDirectory(Dst);
            Files = Directory.GetFileSystemEntries(Src, "*.pdf");
            foreach (string Element in Files)
            {
                try
                {
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
                catch (Exception)
                {
                    continue;
                }
            }
        }

        public void LoadDropDown(DataSet ds)
        {
            if (afterdot(hdnFileName.Value).ToString().ToLower() == ".pdf" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tif" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tiff")
            {

                DDLDrop.DataSource = ds.Tables[0];
                DDLDrop.DataTextField = "PageNumbers";
                DDLDrop.DataValueField = "PageNumbers";
                DDLDrop.DataBind();
            }
        }

        public void LoadRemarksDropdowns(DropDownList ddlList, DataTable DT)
        {
            if (DT.Rows.Count > 0)
            {
                ddlList.DataSource = DT;
                ddlList.DataTextField = "Value";
                ddlList.DataValueField = "ID";
                ddlList.DataBind();
            }
        }

        protected void ddlDocStatus_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                SearchFilter Filter = new SearchFilter();
                DocumentBL BL = new DocumentBL();
                Results Rs = new Results();
                string action = string.Empty;
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                action = "GetRemarksStatusDropDown";
                Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                Filter.GenStatusID = Convert.ToInt16(ddlDocStatus.SelectedValue);
                Rs = BL.GetDocumetRemarksAndStatus(Filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                if (Rs.ResultDS.Tables[0].Rows.Count > 1)
                {
                    ddlStatusRemarks.Enabled = true;
                    LoadRemarksDropdowns(ddlStatusRemarks, Rs.ResultDS.Tables[0]);

                }
                LoadImageToViewer(string.Empty);
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        /// <summary>
        /// To load Remarks section as per the document Status.
        /// </summary>
        public void LoadStatusDropdown()
        {
            try
            {
                Logger.Trace("LoadStatusDropdown Started", Session["LoggedUserId"].ToString());
                DataTable dt = new DataTable();

                SearchFilter Filter = new SearchFilter();
                DocumentBL BL = new DocumentBL();
                Results Rs = new Results();
                string action = string.Empty;
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                string pageName = Request.QueryString["Page"] != null ? Request.QueryString["Page"].ToString() : string.Empty;
                if (pageName == "CheckerDashBoard")
                {
                    //Load intitial dropdown
                    action = "GetDocumentStatusDropdownForChecker";
                }
                else
                {
                    action = "GetDocumentStatusDropdownForMaker";
                }
                //Loading Status Dropdown
                Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                Rs = BL.GetDocumetRemarksAndStatus(Filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                if (Rs.ResultDS.Tables[0].Rows.Count > 0)
                {

                    LoadRemarksDropdowns(ddlDocStatus, Rs.ResultDS.Tables[0]);
                    action = "GetDocumentStatus";
                    Rs = BL.GetDocumetRemarksAndStatus(Filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    if (Rs.ResultDS.Tables[0].Rows.Count > 0)
                    {
                        ddlDocStatus.SelectedIndex = ddlDocStatus.Items.IndexOf(ddlDocStatus.Items.FindByValue(Rs.ResultDS.Tables[0].Rows[0]["aStatus"].ToString()));
                        if (ddlDocStatus.SelectedIndex > 0)
                        {
                            //Loading Rmemarks Status Dropdown
                            action = "GetRemarksStatusDropDown";

                            Filter.GenStatusID = Convert.ToInt16(ddlDocStatus.SelectedValue);
                            Rs = BL.GetDocumetRemarksAndStatus(Filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                            if (Rs.ResultDS.Tables[0].Rows.Count > 0)
                            {
                                LoadRemarksDropdowns(ddlStatusRemarks, Rs.ResultDS.Tables[0]);
                                action = "GetDocumentStatusRemarks";
                                //Taking the Value from upload table Rmemarks Status Dropdown
                                Rs = BL.GetDocumetRemarksAndStatus(Filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                                if (Rs.ResultDS.Tables[0].Rows.Count > 0)
                                {
                                    ddlStatusRemarks.SelectedIndex = ddlStatusRemarks.Items.IndexOf(ddlStatusRemarks.Items.FindByValue(Rs.ResultDS.Tables[0].Rows[0]["aStatus"].ToString()));
                                }
                            }

                        }

                    }
                }
                //Taking remarks text and binding to the label
                action = "GetRemarksText";
                Rs = BL.GetDocumetRemarksAndStatus(Filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                if (Rs.ResultDS.Tables[0].Rows.Count > 0)
                {
                    txtoldremarks.Text = Rs.ResultDS.Tables[0].Rows[0]["Comments"].ToString();
                }
                Logger.Trace("LoadStatusDropdown completed", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        public DataTable getAvailablePages()
        {

            Logger.Trace("getAvailablePages Started", Session["LoggedUserId"].ToString());
            DataTable dt = new DataTable();
            //to get the available pages 
            SearchFilter Filter = new SearchFilter();
            DocumentBL BL = new DocumentBL();
            Results Rs = new Results();
            string action = string.Empty;
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            if (ddldocumentview.SelectedValue == "Original View")
            {
                action = "GetPagesOrginalView";
                if (hdnPagesCount.Value != string.Empty && hdnPagesCount.Value != null)
                {
                    Filter.TotalPages = Convert.ToInt32(hdnPagesCount.Value);
                    Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                    txttagpages.Text = "";
                }

                pnlControls.Enabled = true;
            }
            else if (ddldocumentview.SelectedValue == "Tagged View")
            {
                action = "GetPagesTaggedView";


                //DMS04-3470D
                //if (IsMaintag == true)
                //DMS04-3470A
                string mainTagId = Request.QueryString["MainTagId"] != null ? Convert.ToString(Request.QueryString["MainTagId"]) : "0";
                //DMS04-3470A
                if (mainTagId.Length > 0 && Convert.ToInt32(mainTagId) > 0)
                {
                    Filter.MainTagID = Convert.ToInt32(Request.QueryString["MainTagId"]);

                    Filter.SubTagID = Convert.ToInt32(Request.QueryString["SubTagId"]);
                    txttagpages.Text = "";

                }
                else
                {
                    Filter.MainTagID = Convert.ToInt32(cmbMainTag.SelectedValue);

                    //Filter.SubTagID = Convert.ToInt32(cmbSubTag.SelectedValue);//TaskWDMS-S-5046 D
                    //TaskWDMS-S-5046 BS A
                    if (cmbSubTag.SelectedValue == "0")
                    {
                        Filter.SubTagID = Convert.ToInt32(hdnsubtagvalue.Value);
                        if (Filter.SubTagID <= 0)
                        {
                            Filter.SubTagID = Convert.ToInt32(hdnsubtagvalue1.Value);
                        }
                    }
                    else
                    {
                        Filter.SubTagID = Convert.ToInt32(cmbSubTag.SelectedValue);
                    }
                    hdnsubtagvalue.Value = "0";
                    hdnsubtagvalue1.Value = "0";
                    //TaskWDMS-S-5046 BS A
                }

                // DMS04-3470BS
                // DMS04-3470BE

                Filter.TotalPages = hdnPagesCount.Value.Length > 0 ? Convert.ToInt32(hdnPagesCount.Value) : 0;
                Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                pnlControls.Enabled = false;
                txttagpages.Text = "";
                ddldocumentview.SelectedValue = "Tagged View";

            }
            else if (ddldocumentview.SelectedValue == "NonTagged View")
            {


                action = "GetPagesNonTaggedView";
                Filter.TotalPages = Convert.ToInt32(hdnPagesCount.Value);
                Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                pnlControls.Enabled = false;
                txttagpages.Text = "";
            }
            else if (ddldocumentview.SelectedValue == "FullText View")
            {
                action = "GetPagesFullText";
                if (hdnPagesCount.Value != null && Request.QueryString["id"] != null && Session["FullTextClause"] != null)
                {
                    if (hdnPagesCount.Value != string.Empty)
                    {
                        Filter.TotalPages = Convert.ToInt32(hdnPagesCount.Value);
                        txttagpages.Text = "";
                    }

                    Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                    Filter.WhereClause = Session["FullTextClause"].ToString();
                    pnlControls.Enabled = false;
                    txttagpages.Text = "";
                }
                else
                {
                    Logger.Trace("Error in binding the drop down of full text pages dropdown", Session["LoggedUserId"].ToString());
                }

            }

            Rs = BL.GetAvailablePagesForDocumentView(Filter, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
            if (Rs.ResultDS != null && Rs.ResultDS.Tables.Count > 0 && Rs.ResultDS.Tables[0].Rows.Count > 0)
            {
                dt = Rs.ResultDS.Tables[0];
                LoadDropDown(Rs.ResultDS);
            }
            else
            {

                //Logger.Trace("getAvailablePages Finished", Session["LoggedUserId"].ToString());
                //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "alert('Page not tagged under selected main tag & sub tag.')", true);

                //ddldocumentview.SelectedValue = "Original View";


            }
            return dt;
        }

        //To load PDF in Iframe
        public void InitializeDocumentView()
        {
            // getAvailablePages();
            string result = "Success";
            string TagEnabled = "False";
            string src = string.Empty;
            Logger.Trace("InitializeDocumentView started", Session["LoggedUserId"].ToString());
            string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
            int count = 0;

            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }
            string filepath = beforedot(hdnFileLocation.Value).ToLower();

            if (Directory.Exists(filepath.ToLower()))
            {
                string sourceFile = filepath;
                string destinationFile = TempFolder + beforedot(hdnEncrpytFileName.Value);
                try
                {
                    copyDirectory(sourceFile, destinationFile);
                }
                catch (Exception ex)
                {
                    Logger.Trace("InitializeDocumentView Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                }

                if (Directory.Exists(destinationFile.ToLower()))
                {
                    /*changed*/
                    DirectoryInfo di = new DirectoryInfo(destinationFile);
                    count = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                    hdnPagesCount.Value = count.ToString();

                    /*changed*/
                    TagEnabled = "True";
                    if ((afterdot(hdnEncrpytFileName.Value).ToLower() == ".pdf" || afterdot(hdnEncrpytFileName.Value).ToLower() == ".tif") && (Convert.ToInt32(Request.QueryString["PageNo"]) == 0))
                    {
                        if (Convert.ToInt32(Request.QueryString["active"]) == 0)
                        {
                            pnlControls.Visible = false;
                        }
                        else
                        {
                            pnlControls.Visible = true;/*---Div for adding and deleting the records from pdf---*/
                            pnlControls.Enabled = false;
                        }

                        drpPageCount.Items.Clear();
                        drpPageCount.Items.Add(new ListItem("Before", "0"));
                        //if (Session["PagesCount"] != null)
                        //{
                        //    count = Convert.ToInt32(Session["PagesCount"]);
                        //    Session["PagesCount"] =null;
                        //}
                        //else
                        //{


                        //}
                        int x = 0;
                        for (int i = 0; i < count; i++)
                        {
                            x = i + 1;
                            drpPageCount.Items.Add(new ListItem(x.ToString(), x.ToString()));
                        }
                        x = count + 1;
                        drpPageCount.Items.Add(new ListItem("After", x.ToString()));

                        drpDeleteCount.Items.Clear();
                        drpDeleteCount.Items.Add(new ListItem("<Select>", "0"));
                        if (count > 1)
                        {
                            for (int j = 1; j <= count; j++)
                            {
                                drpDeleteCount.Items.Add(new ListItem(j.ToString(), j.ToString()));
                            }
                        }

                    }
                    else
                    {
                        pnlControls.Visible = false;
                    }
                    if (TagEnabled == "True")
                    {
                        TagPanel.Visible = true;

                        if (Convert.ToInt32(Request.QueryString["PageNo"]) != 0)
                        {
                            TotalDiv.Attributes.Add("style", "visibility:hidden;");
                            SearchFilter Filter = new SearchFilter();
                            DocumentBL DocBL = new DocumentBL();
                            Filter.MainTagID = Convert.ToInt32(Request.QueryString["MainTagId"]);
                            if (Request.QueryString["SubTagId"] != null && Request.QueryString["SubTagId"] != "")
                            {
                                Filter.SubTagID = Convert.ToInt32(Request.QueryString["SubTagId"]);
                            }
                            Filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                            Results Res = DocBL.SearchDocuments(Filter, "GetTaggedPages", hdnLoginOrgId.Value, hdnLoginToken.Value);
                            DDLDrop.Items.Clear();
                            foreach (IndexField dp in Res.IndexFields)
                            {
                                DDLDrop.Items.Add(new ListItem(dp.ListItemId.ToString(), dp.ListItemId.ToString()));
                            }

                        }
                        else
                        {
                            TotalDiv.Attributes.Add("style", "visibility:visible;");
                            //To load page numbers
                            int pagecount = count;
                            DDLDrop.Items.Clear();
                            for (int i = 0; i < pagecount; i++)
                            {
                                DDLDrop.Items.Insert(i, (i + 1).ToString());
                            }
                        }
                    }
                    else
                    {
                        TagPanel.Visible = false;
                    }
                    if (result == "Success")
                    {
                        src = GetSrc("Handler") + destinationFile;
                    }
                    else
                    {
                        src = GetSrc("PreviewNotAvailable");
                    }

                }

            }

            else
            {
                src = GetSrc("PreviewNotAvailable");
            }
            hdnPDFPathForObject.Value = src;

            // Call set image
            //  LoadImageToViewer(string.Empty);
            Logger.Trace("InitializeDocumentView finished", Session["LoggedUserId"].ToString());
        }

        #region download functions

        protected void DownloadWithAnnotation()
        {
            try
            {
                Logger.Trace("DownloadWithAnnotation started", Session["LoggedUserId"].ToString());

                #region variable declaration and initialization
                // Downloading file path 
                string DownloadFilepath = string.Empty;
                // Temp folder path to store splitted files
                string TempWorkFolderPath = string.Empty;
                // Splitted files original folder path
                string SplittedFilesOriginalFolderPath = string.Empty;
                // Temp folder path to store converted JPG images
                string TempWorkFolderPathForJPG = string.Empty;
                // Each splitted pdf file path
                string SourcePdfFilepath = string.Empty;
                // File path for JPG file
                string TargetFilepath = string.Empty;

                // Get splitted files folder path from hidden variable
                SplittedFilesOriginalFolderPath = IOUtility.beforedot(hdnFileLocation.Value);
                // Create temp work folder path for the current file
                TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + IOUtility.beforedot(hdnEncrpytFileName.Value);
                // Create temp work folder path to store JPG files of current file
                TempWorkFolderPathForJPG = TempWorkFolderPath + "JPEG\\";
                DownloadFilepath = TempWorkFolderPath + ".pdf";
                #endregion

                // If directory doesn't exists create
                if (!Directory.Exists(TempWorkFolderPathForJPG))
                    Directory.CreateDirectory(TempWorkFolderPathForJPG);

                // Copy files from original location to temporary work folder
                copyDirectory(SplittedFilesOriginalFolderPath, TempWorkFolderPath);

                string WatermarkText = Session["Watermark"] != null ? Session["Watermark"].ToString() : string.Empty;

                #region Loop through splitted pdf images, convert to jpg and apply watermark on it
                if (WatermarkText != null)
                {
                    for (int k = 1; k <= DDLDrop.Items.Count; k++)
                    {
                        SourcePdfFilepath = TempWorkFolderPath + "\\" + k.ToString() + ".pdf";
                        TargetFilepath = TempWorkFolderPathForJPG + k.ToString() + ".jpeg";

                        Logger.Trace("Converting PDF to JPG and applying watermark on it", Session["LoggedUserId"].ToString());
                        try
                        {
                            new ConvertPdf2Image().ConvertPDFtoHojas(SourcePdfFilepath, TempWorkFolderPathForJPG, k.ToString(), WatermarkText);
                        }
                        catch { }

                        Logger.Trace("Converting JPG back to pdf.", Session["LoggedUserId"].ToString());
                        new Image2Pdf().tiff2PDF(TargetFilepath, TempWorkFolderPath, true, k.ToString());
                    }
                }
                #endregion

                #region Generate image from base64string and convert to PDF
                // Get the annotated images from database 
                int DocumentId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                Results results = GetAnnotations(DocumentId);
                if (results != null && results.ResultDS != null && results.ResultDS.Tables.Count > 0 && results.ResultDS.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < results.ResultDS.Tables[0].Rows.Count; i++)
                    {
                        string DocWithAnnotation = results.ResultDS.Tables[0].Rows[i]["xImage"].ToString().Split(',')[1];
                        string PageNo = results.ResultDS.Tables[0].Rows[i]["PageNo"].ToString();
                        new ConvertingBase64ToImage().LoadImage(DocWithAnnotation, TempWorkFolderPathForJPG + PageNo + ".jpeg");
                        new Image2Pdf().tiff2PDF(TempWorkFolderPathForJPG + PageNo + ".jpeg", TempWorkFolderPath, true, PageNo);
                    }
                }
                #endregion

                // Merge documents and create single pdf file
                PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getAvailablePages(), DDLDrop.Items.Count);

                // Download file to physical path
                DownloadFile(DownloadFilepath);

                //updating track table                
                TrackActivity("UpdateTrackTableOnDownload");
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        private bool DownloadFile(string FilePath)
        {
            bool Status = false;

            /* DMS5-4232 BS */
            string FileExt = afterdot(Path.GetFileName(FilePath));
            string Filename = beforedot(hdnFileName.Value) + FileExt;
            /* DMS5-4232 BE */
            try
            {
                Response.ContentType = "application/octet-stream";
                Response.AppendHeader("Content-Disposition", "attachment;filename=" + Filename);
                Response.TransmitFile(FilePath);
                Response.Flush();
                Status = true;
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return Status;
        }

        private void DownloadWithoutAnn(bool Watrmark)
        {
            try
            {
                Logger.Trace("DownloadWithoutAnn Started", Session["LoggedUserId"].ToString());
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string ActualFolder = hdnFileLocation.Value;
                int index = ActualFolder.LastIndexOf("\\");
                ActualFolder = ActualFolder.Substring(0, index) + "\\";
                UploadDocBL bl = new UploadDocBL();
                SearchFilter filter = new SearchFilter();
                ConvertPdf2Image convertPdftoImage = new ConvertPdf2Image();
                ConvertingBase64ToImage Converttoimage = new ConvertingBase64ToImage();
                string action = "UpdateTrackTableOnDownload";
                filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                Results res = bl.UpdateTrackTableOnDownload(filter, action, hdnLoginOrgId.Value, hdnLoginToken.Value);
                //Unzip the document for downloading
                FileZip.Unzip(ActualFolder + beforedot(hdnEncrpytFileName.Value) + ".zip");
                string filepath = string.Empty;
                string destinationFile = string.Empty;
                string TempImageFolder = string.Empty;
                string Watermark = string.Empty;
                string currentfilepath = string.Empty;
                destinationFile = TempFolder + beforedot(hdnEncrpytFileName.Value);
                TempImageFolder = destinationFile + "JPEG\\";
                if (!Directory.Exists(TempImageFolder))
                {
                    Directory.CreateDirectory(TempImageFolder);
                }
                copyDirectory(ActualFolder, destinationFile);
                try
                {
                    // for checking watermark is required or not
                    if (Watrmark)
                    {
                        //checking for full download or as per the selecetd pages 
                        if (ddldocumentview.SelectedValue == "Original View")
                        {
                            Watermark = Session["Watermark"] != null ? Session["Watermark"].ToString() : string.Empty;
                            if (Watermark.Length > 0)
                            {
                                for (int k = 1; k <= DDLDrop.Items.Count; k++)
                                {
                                    convertPdftoImage.ConvertPDFtoHojas(destinationFile + "\\" + k.ToString() + ".pdf", TempImageFolder, k.ToString(), Watermark);
                                    currentfilepath = new Image2Pdf().tiff2PDF(TempImageFolder + k.ToString() + ".jpeg", destinationFile, true, k.ToString());
                                }
                                PDFMerging.MergeFilesDownload(destinationFile + ".pdf", TempFolder + beforedot(hdnEncrpytFileName.Value), getAvailablePages(), DDLDrop.Items.Count);
                                filepath = destinationFile + ".pdf"; ;
                            }
                        }
                        else
                        {

                            Watermark = Session["Watermark"] != null ? Session["Watermark"].ToString() : string.Empty;
                            if (Watermark.Length > 0)
                            {
                                for (int k = 1; k <= DDLDrop.Items.Count; k++)
                                {
                                    convertPdftoImage.ConvertPDFtoHojas(destinationFile + "\\" + k.ToString() + ".pdf", TempImageFolder, k.ToString(), Watermark);
                                    currentfilepath = new Image2Pdf().tiff2PDF(TempImageFolder + k.ToString() + ".jpeg", destinationFile, true, k.ToString());
                                }
                                PDFMerging.MergeFilesDownload(destinationFile + ".pdf", TempFolder + beforedot(hdnEncrpytFileName.Value), getAvailablePages(), DDLDrop.Items.Count);
                                filepath = destinationFile + ".pdf"; ;
                            }


                        }


                    }
                    else
                    {
                        if (ddldocumentview.SelectedValue == "Original View")
                        {
                            filepath = hdnFileLocation.Value;
                        }
                        else
                        {

                            PDFMerging.MergeFilesDownload(destinationFile + ".pdf", TempFolder + beforedot(hdnEncrpytFileName.Value), getAvailablePages(), DDLDrop.Items.Count);
                            filepath = destinationFile + ".pdf";
                        }
                    }
                    Response.ContentType = "application/octet-stream";
                    Response.AppendHeader("Content-Disposition", "attachment;filename=" + filepath);
                    Response.TransmitFile(filepath);
                    Response.Flush();

                }
                catch (Exception ex)
                {
                    Logger.Trace("btnDownload_Click Exception in using response for downloading: " + ex.Message, Session["LoggedUserId"].ToString());

                }
                if (File.Exists(hdnFileLocation.Value))
                {
                    // Deleting the unzipped file no need to zipped folder no merging
                    File.Delete(hdnFileLocation.Value);
                }

                Logger.Trace("DownloadWithoutAnn Finished", Session["LoggedUserId"].ToString());

            }
            catch (Exception ex)
            {
                Logger.Trace("DownloadWithoutAnn Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        //protected void btnDownload_Click(object sender, EventArgs e)
        //{
        //    Logger.Trace("Downloading original document.", Session["LoggedUserId"].ToString());

        //    #region variable declaration and initialization

        //    string DownloadFilepath = string.Empty;

        //    // Original File full path
        //    string OriginalFilePath = hdnFileLocation.Value;
        //    // File Name
        //    string FileName = Path.GetFileNameWithoutExtension(hdnFileLocation.Value);
        //    // File Extension
        //    string FileExtension = Path.GetExtension(hdnFileLocation.Value);

        //    // Temp folder path to store splitted files
        //    string TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + @"\" + FileName;
        //    // Splitted files original folder path
        //    string SplittedFilesOriginalFolderPath = IOUtility.beforedot(hdnFileLocation.Value);

        //    #endregion

        //    if (ddldocumentview.SelectedValue == "Original View")
        //    {
        //        string OriginalFilePathZip = OriginalFilePath.Replace(FileExtension, ".zip");
        //        string ZippedFilePath = TempWorkFolderPath + ".zip";
        //        File.Copy(OriginalFilePathZip, ZippedFilePath, true);


        //        if (File.Exists(ZippedFilePath))
        //        {
        //            //Unzip the document for downloading: if the file is in zipped format
        //            Logger.Trace("Unzipping file. Filepath: " + ZippedFilePath, Session["LoggedUserId"].ToString());
        //            FileZip.Unzip(ZippedFilePath);
        //        }
        //        string Filepath = ConfigurationManager.AppSettings["TempWorkFolder"] + Path.GetFileName(hdnFileLocation.Value);
        //        if (afterdot(hdnFileLocation.Value).ToLower() == ".pdf")
        //        {


        //            OriginalFilePath = GetDigitalSignatureDocumentDetails(Filepath, TempWorkFolderPath);
        //            if (lblsignatureDetails.Text.Length > 2)
        //            {
        //                string src = GetSrc("Handler") + OriginalFilePath;
        //                src = src.Replace("\\", "//").ToLower();

        //                string redirect = "<script>window.open('" + src + "');</script>";
        //                Response.Write(redirect);

        //            }


        //        }
        //        else
        //        {
        //            OriginalFilePath = Filepath;
        //        }


        //        DownloadFilepath = OriginalFilePath;
        //    }
        //    else // Only available pages from pages dropdown
        //    {
        //        // Copy files to temp work folder
        //        Logger.Trace("Copying splitted files from: " + SplittedFilesOriginalFolderPath + " to temporary work folder: " + TempWorkFolderPath, Session["LoggedUserId"].ToString());
        //        copyDirectory(SplittedFilesOriginalFolderPath, TempWorkFolderPath);

        //        DownloadFilepath = TempWorkFolderPath + ".pdf";
        //        Logger.Trace("Merging files: " + DownloadFilepath, Session["LoggedUserId"].ToString());
        //        PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getAvailablePages(), DDLDrop.Items.Count);
        //        DownloadFile(DownloadFilepath);
        //    }

        //    // Download file to physical path
        //    if (lblsignatureDetails.Text.Length < 2)
        //    {
        //        DownloadFile(DownloadFilepath);
        //    }
        //    else
        //    {

        //        LoadImageToViewer(DDLDrop.SelectedValue.ToString());
        //    }

        //    //updating track table                
        //    TrackActivity("UpdateTrackTableOnDownload");

        //    Logger.Trace("Downloading original document completed.", Session["LoggedUserId"].ToString());
        //}


        protected void btnDownload_Click(object sender, EventArgs e)
        {
            Logger.Trace("Downloading original document.", Session["LoggedUserId"].ToString());

            #region variable declaration and initialization

            string DownloadFilepath = string.Empty;

            // Original File full path
            string OriginalFilePath = hdnFileLocation.Value;
            // File Name
            string FileName = Path.GetFileNameWithoutExtension(hdnFileLocation.Value);
            // File Extension
            string FileExtension = Path.GetExtension(hdnFileLocation.Value);

            // Temp folder path to store splitted files
            string TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + @"\" + FileName;
            // Splitted files original folder path
            string SplittedFilesOriginalFolderPath = IOUtility.beforedot(hdnFileLocation.Value);

            #endregion

            //if (ddldocumentview.SelectedValue == "Original View")
            //{
            //    string OriginalFilePathZip = OriginalFilePath.Replace(FileExtension, ".zip");
            //    string ZippedFilePath = TempWorkFolderPath + ".zip";
            //    File.Copy(OriginalFilePathZip, ZippedFilePath, true);


            //    if (File.Exists(ZippedFilePath))
            //    {
            //        //Unzip the document for downloading: if the file is in zipped format
            //        Logger.Trace("Unzipping file. Filepath: " + ZippedFilePath, Session["LoggedUserId"].ToString());
            //        FileZip.Unzip(ZippedFilePath);
            //    }
            //    string Filepath = ConfigurationManager.AppSettings["TempWorkFolder"] + Path.GetFileName(hdnFileLocation.Value);
            //    if (afterdot(hdnFileLocation.Value).ToLower() == ".pdf")
            //    {


            //        OriginalFilePath = GetDigitalSignatureDocumentDetails(Filepath, TempWorkFolderPath);
            //        if (lblsignatureDetails.Text.Length > 2)
            //        {
            //            string src = GetSrc("Handler") + OriginalFilePath;
            //            src = src.Replace("\\", "//").ToLower();

            //            string redirect = "<script>window.open('" + src + "');</script>";
            //            Response.Write(redirect);

            //        }


            //    }
            //    else
            //    {
            //        OriginalFilePath = Filepath;
            //    }


            //    DownloadFilepath = OriginalFilePath;
            //}
            //else // Only available pages from pages dropdown
            //{
            // Copy files to temp work folder
            Logger.Trace("Copying splitted files from: " + SplittedFilesOriginalFolderPath + " to temporary work folder: " + TempWorkFolderPath, Session["LoggedUserId"].ToString());
            copyDirectory(SplittedFilesOriginalFolderPath, TempWorkFolderPath);

            DownloadFilepath = TempWorkFolderPath + ".pdf";
            Logger.Trace("Merging files: " + DownloadFilepath, Session["LoggedUserId"].ToString());
            PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getAvailablePages(), DDLDrop.Items.Count);
            DownloadFile(DownloadFilepath);
            //}

            // Download file to physical path
            //if (lblsignatureDetails.Text.Length < 2)
            //{
            //    DownloadFile(DownloadFilepath);
            //}
            //else
            //{

            //    LoadImageToViewer(DDLDrop.SelectedValue.ToString());
            //}

            //updating track table                
            TrackActivity("UpdateTrackTableOnDownload");

            Logger.Trace("Downloading original document completed.", Session["LoggedUserId"].ToString());
        }


        ////*******************************************************************///


        //protected void btnDownload_Click(object sender, EventArgs e)
        //{
        //    Logger.Trace("Downloading original document.", Session["LoggedUserId"].ToString());

        //    #region variable declaration and initialization
        //    // Downloading file path 
        //    string DownloadFilepath = string.Empty;
        //    // Temp folder path to store splitted files
        //    string TempWorkFolderPath = string.Empty;
        //    // Splitted files original folder path
        //    string SplittedFilesOriginalFolderPath = string.Empty;
        //    // Temp folder path to store converted JPG images
        //    string TempWorkFolderPathForJPG = string.Empty;
        //    // Each splitted pdf file path
        //    string SourcePdfFilepath = string.Empty;
        //    // File path for JPG file
        //    string TargetFilepath = string.Empty;

        //    // Get splitted files folder path from hidden variable
        //    SplittedFilesOriginalFolderPath = IOUtility.beforedot(hdnFileLocation.Value);
        //    // Create temp work folder path for the current file
        //    TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + IOUtility.beforedot(hdnEncrpytFileName.Value);
        //    // Create temp work folder path to store JPG files of current file
        //    TempWorkFolderPathForJPG = TempWorkFolderPath + "JPEG\\";
        //    DownloadFilepath = TempWorkFolderPath + ".pdf";
        //    #endregion

        //    // If directory doesn't exists create
        //    if (!Directory.Exists(TempWorkFolderPathForJPG))
        //        Directory.CreateDirectory(TempWorkFolderPathForJPG);

        //    // Copy files from original location to temporary work folder
        //    copyDirectory(SplittedFilesOriginalFolderPath, TempWorkFolderPath);



        //    // Merge documents and create single pdf file
        //    PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getAvailablePages(), DDLDrop.Items.Count);

        //    // Download file to physical path
        //    DownloadFile(DownloadFilepath);
        //    //updating track table                
        //    TrackActivity("UpdateTrackTableOnDownload");

        //    Logger.Trace("Downloading original document completed.", Session["LoggedUserId"].ToString());
        //}

        //protected void btnDownload_Click(object sender, EventArgs e)
        //{
        //    Logger.Trace("Downloading original document.", Session["LoggedUserId"].ToString());

        //    #region variable declaration and initialization

        //    string DownloadFilepath = string.Empty;

        //    // Original File full path
        //    string OriginalFilePath = hdnFileLocation.Value;
        //    // File Name
        //    string FileName = Path.GetFileNameWithoutExtension(hdnFileLocation.Value);
        //    // File Extension
        //    string FileExtension = Path.GetExtension(hdnFileLocation.Value);

        //    // Temp folder path to store splitted files
        //    string TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + @"\" + FileName;
        //    // Splitted files original folder path
        //    string SplittedFilesOriginalFolderPath = IOUtility.beforedot(hdnFileLocation.Value);

        //    #endregion

        //    if (ddldocumentview.SelectedValue == "Original View")
        //    {
        //        //string OriginalFilePathZip = OriginalFilePath.Replace(FileExtension, ".zip");
        //        //string ZippedFilePath = TempWorkFolderPath + ".zip";
        //        //File.Copy(OriginalFilePathZip, ZippedFilePath, true);


        //        //if (File.Exists(ZippedFilePath))
        //        //{
        //        //    //Unzip the document for downloading: if the file is in zipped format
        //        //    Logger.Trace("Unzipping file. Filepath: " + ZippedFilePath, Session["LoggedUserId"].ToString());
        //        //    FileZip.Unzip(ZippedFilePath);
        //        //}
        //        string Filepath = ConfigurationManager.AppSettings["TempWorkFolder"] + Path.GetFileName(hdnFileLocation.Value);
        //        if (afterdot(hdnFileLocation.Value).ToLower() == ".pdf")
        //        {


        //            OriginalFilePath = GetDigitalSignatureDocumentDetails(Filepath, TempWorkFolderPath);
        //            if (lblsignatureDetails.Text.Length > 2)
        //            {
        //                string src = GetSrc("Handler") + OriginalFilePath;
        //                src = src.Replace("\\", "//").ToLower();

        //                string redirect = "<script>window.open('" + src + "');</script>";
        //                Response.Write(redirect);

        //            }


        //        }
        //        else
        //        {
        //            OriginalFilePath = Filepath;
        //        }


        //        DownloadFilepath = OriginalFilePath;
        //    }
        //    else // Only available pages from pages dropdown
        //    {
        //        // Copy files to temp work folder
        //        Logger.Trace("Copying splitted files from: " + SplittedFilesOriginalFolderPath + " to temporary work folder: " + TempWorkFolderPath, Session["LoggedUserId"].ToString());
        //        copyDirectory(SplittedFilesOriginalFolderPath, TempWorkFolderPath);

        //        DownloadFilepath = TempWorkFolderPath + ".pdf";
        //        Logger.Trace("Merging files: " + DownloadFilepath, Session["LoggedUserId"].ToString());
        //        PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getAvailablePages(), DDLDrop.Items.Count);
        //        DownloadFile(DownloadFilepath);
        //    }

        //    // Download file to physical path
        //    if (lblsignatureDetails.Text.Length < 2)
        //    {
        //        DownloadFile(DownloadFilepath);
        //    }
        //    else
        //    {

        //        LoadImageToViewer(DDLDrop.SelectedValue.ToString());
        //        LoadTagImageToViewer(DDLTagpagecount.SelectedValue.ToString());
        //    }

        //    //updating track table                
        //    TrackActivity("UpdateTrackTableOnDownload");

        //    Logger.Trace("Downloading original document completed.", Session["LoggedUserId"].ToString());
        //}




        #endregion download functions

        private bool TrackActivity(string Action)
        {
            Logger.Trace("Updating activity track table for the action: " + Action, Session["LoggedUserId"].ToString());
            bool Status = false;

            try
            {
                UploadDocBL bl = new UploadDocBL();
                SearchFilter filter = new SearchFilter();
                filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                bl.UpdateTrackTableOnDownload(filter, Action, hdnLoginOrgId.Value, hdnLoginToken.Value);
                Status = true;
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return Status;
        }
        /// <summary>
        /// to direct to specific page when search again
        /// </summary>
        private void SearchAgain()
        {
            //search agin code by ronit
            //string searchCriteria = Session["newsearchback"].ToString();
            //Session["srchagain"] = "yes";
            //ends here
            //string searchCriteria = Request.Url.Query;

            string searchCriteria = Request.QueryString["id"] + "|" + Request.QueryString["docid"] + "|" + Request.QueryString["depid"];
            //DMS04-3459D
            /*
            string SearchCriteria = string.Empty;
            //DeleteTempFiles();
            if (Request.QueryString["Page"] == "DocumentDownload")
            {
                SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
                Response.Redirect("DocumentDownload.aspx?Search=" + SearchCriteria);
            }
            else if (Request.QueryString["Page"] == "MakerDashboard")
            {
                SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
                Response.Redirect("MakerDashboard.aspx?Search=" + SearchCriteria);
            }
            else if (Request.QueryString["Page"] == "CheckerDashBoard")
            {
                SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
                Response.Redirect("CheckerDashboard.aspx?Search=" + SearchCriteria);
            }
            else
            {
                SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
                Response.Redirect("DocumentDownloadSearch.aspx?Search=" + SearchCriteria);
            }
             * DMS5-4371 BS
             */
            //string searchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            if (PreviousPage == "documenthistoryview")
            {

                //Session["hdnSearchPageUrl"] = Request.ServerVariables["HTTP_REFERER"];
                Response.Redirect(Session["hdnSearchPageUrl"].ToString() + "?Search=" + searchCriteria);
                Session["hdnSearchPageUrl"] = "";
            }
            else
            {
                if (PreviousPage == "documentdownloaddetails")
                {
                    PreviousPage = "documentdownloadsearch";
                }
                //DMS04-3459BS -- Simplified code

                if (PreviousPage.Length > 0)
                    Response.Redirect(PreviousPage + ".aspx?Search=" + searchCriteria);
            }
            //DMS5-4371 BE
            //DMS04-3459BE
        }

        //Search Again Button
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            SearchAgain();
        }

        protected void btnUpdate_Click1(object sender, EventArgs e)
        {

            DeleteTempFiles();
            //Newly added for maintaining versions of document
            Logger.Trace("btnUpdate_Click1 Started", Session["LoggedUserId"].ToString());

            //To update Uploaded document
            Response.Redirect("DocumentUpload.aspx?id=" + Request.QueryString["id"] + "&docid=" + Request.QueryString["docid"] + "&depid=" + Request.QueryString["depid"] + "&ArchivedAction=UploadNewVersion");
            divMsg.InnerHtml = "Error Uploading Document";
            //}

            Logger.Trace("btnUpdate_Click1 Finished", Session["LoggedUserId"].ToString());
            /*----------end-------------------*/

        }


        protected void AsyncFileUpload1_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            Session["overload"] = "0";
            if (Convert.ToInt32(e.FileSize) > 2.4e+7) // use same condition in client side code
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Maximum file size allowed 24 MB')", true);
                Logger.Trace("Exception : ", Session["LoggedUserId"].ToString());
                AsyncFileUpload1.Dispose();
                Session["overload"] = "1";


                return;
            }
            if (hdnUploaded.Value == "FALSE")
            {
                try
                {
                    Logger.Trace("Document Download Details aspx - Started Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());

                    string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                    string ActualFolder = hdnFileLocation.Value;
                    int index = ActualFolder.LastIndexOf("\\");
                    ActualFolder = ActualFolder.Substring(0, index) + "\\";
                    string UploadedFileExt = afterdot(AsyncFileUpload1.FileName).ToLower();

                    if (!Directory.Exists(TempFolder))
                    {
                        Directory.CreateDirectory(TempFolder);
                    }

                    if (afterdot(hdnEncrpytFileName.Value).ToLower() != UploadedFileExt)
                    {
                        Session["SameExt"] = "FALSE";
                    }
                    else
                    {
                        Session["SameExt"] = "TRUE";
                        if (AsyncFileUpload1.HasFile)
                        {
                            int count = 0;

                            #region pdf and tif merge

                            string Path_FileToMerge = TempFolder + Encryptdata(lblRefID.Text) + UploadedFileExt;
                            ModifiedCount = ModifiedCount + 1; // Increment document modified count
                            string Path_MergedFile = TempFolder + Encryptdata(lblRefID.Text) + ModifiedCount.ToString() + UploadedFileExt;
                            string Path_ExistingFile = string.Empty;
                            string Path_Originalsplittedfile = TempFolder + Encryptdata(lblRefID.Text);
                            int Position = Convert.ToInt32(drpPageCount.SelectedValue.ToString());

                            //To get the count of documents after insert or delete
                            DirectoryInfo di = new DirectoryInfo(Path_Originalsplittedfile);
                            int splittedfilescount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;

                            AsyncFileUpload1.SaveAs(Path_FileToMerge);

                            if (File.Exists(ActualFolder + @"\" + beforedot(hdnEncrpytFileName.Value) + ".zip"))
                                FileZip.Unzip(ActualFolder + @"\" + beforedot(hdnEncrpytFileName.Value) + ".zip");

                            Logger.Trace("Uploaded file saved to " + Path_FileToMerge, Session["LoggedUserId"].ToString());

                            if (Session["TempFileLocation"].ToString().Length > 0)
                            {
                                // Second merge 
                                Path_ExistingFile = Session["TempFileLocation"].ToString();  //Previously merged file avalable in temporary folder.
                            }
                            else
                            {
                                // First Merge
                                Path_ExistingFile = hdnFileLocation.Value; // First time take file from original(uploaded) location.                                
                            }

                            string[] source = new string[] { Path_ExistingFile, Path_FileToMerge };

                            if (afterdot(hdnEncrpytFileName.Value).ToLower() == ".pdf" && UploadedFileExt == ".pdf")
                            {
                                // Merge Files  
                                Logger.Trace("Merging pdf file.", Session["LoggedUserId"].ToString());
                                if (drpPageCount.SelectedItem.Text == "After")
                                {
                                    PDFMerging.MergeFilesForbulk(Path_MergedFile, source);
                                }
                                else
                                {
                                    PDFMerging.MergeFiles(Path_MergedFile, source, Position, "add");
                                }

                                // Extract merged file again
                                Logger.Trace("Splitting pdf file.", Session["LoggedUserId"].ToString());
                                count = new Image2Pdf().ExtractPages(Path_MergedFile, TempFolder + "\\" + Encryptdata(lblRefID.Text));
                                //Session["PagesCount"] = count;
                            }
                            else if (afterdot(hdnEncrpytFileName.Value).ToLower() == ".tif" && UploadedFileExt == ".tif")
                            {
                                string pos = drpPageCount.SelectedItem.ToString();
                                // Merge Files
                                Logger.Trace("Merging tif file.", Session["LoggedUserId"].ToString());
                                TiffUtil.TiffUtil.mergeTiffPages(source, Path_MergedFile, Position - 1, pos);

                                // Extract merged file again
                                Logger.Trace("Splitting tif file.", Session["LoggedUserId"].ToString());
                                count = new Image2Pdf().tiff2PDF(Session["TempFileLocation"].ToString(), TempFolder + "\\" + Encryptdata(lblRefID.Text), true);
                                //Session["PagesCount"] = count;
                            }

                            // Set session variable values
                            Session["TotalPageCount"] = count;
                            Session["Ext"] = UploadedFileExt;
                            Session["Encryptdata"] = Encryptdata(lblRefID.Text) + UploadedFileExt;
                            Session["TempFileLocation"] = Path_MergedFile;

                            int InsertedPagesCount = count - splittedfilescount;
                            //update annotation property with new pageno
                            UpdateAnnotatedPageAfterInsert(Position, InsertedPagesCount);
                            //Update tagmaster property with new pageno
                            UpdateTaggedPageAfterInsert(Position, InsertedPagesCount);

                            getAvailablePages();
                            Logger.Trace("Deleting temporary files.", Session["LoggedUserId"].ToString());
                            // Delete files uploaded for merge
                            if (File.Exists(Path_FileToMerge))
                                File.Delete(Path_FileToMerge);

                            if (File.Exists(Path_ExistingFile)) // If first merge, delete from original location else from temporary location
                                File.Delete(Path_ExistingFile);

                            #endregion
                        }

                    }

                    hdnUploaded.Value = "TRUE";
                    Logger.Trace("Document Download Details aspx Finished Saving Uploaded Document in AsyncFileUpload1_UploadedComplete ", Session["LoggedUserId"].ToString());
                }
                catch (Exception ex)
                {
                    Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
                }
            }
        }

        protected void btnReloadiFrame_Click(object sender, EventArgs e)
        {
            if (Session["overload"].ToString() == "1") // use same condition in client side code
            {
                //Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Maximum file size allowed 24 MB')", true);
                //Logger.Trace("Exception : ", Session["LoggedUserId"].ToString());
                AsyncFileUpload1.Dispose();
                return;
            }
            else
            {
                Logger.Trace("btnReloadiFrame_Click Started", Session["LoggedUserId"].ToString());
                ReloadIframe();
                LoadImageToViewer(string.Empty);
                Logger.Trace("btnReloadiFrame_Click finished", Session["LoggedUserId"].ToString());
            }
        }

        public void ReloadIframe()
        {

            Logger.Trace("ReloadIframe Started", Session["LoggedUserId"].ToString());
            if (Session["SameExt"].ToString() == "FALSE")
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "alert('Same type of file can only be attached.')", true);
            }
            else
            {

                UpdatePanel7.Update();
                btnSaveTag.Enabled = false;

                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string src = string.Empty;
                int count = 0;
                string destinationFile = TempFolder + beforedot(hdnEncrpytFileName.Value);
                DirectoryInfo di = new DirectoryInfo(destinationFile);
                count = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;

                switch (Convert.ToString(Session["Ext"]))
                {

                    case ".pdf":
                        src = GetSrc("Handler") + TempFolder + Encryptdata(lblRefID.Text);
                        hdnPDFPathForObject.Value = src;
                        LoadImageToViewer(string.Empty);
                        break;
                    case ".tif":
                        src = GetSrc("Handler") + TempFolder + Encryptdata(lblRefID.Text);
                        //src = GetSrc("Handler") + TempFolder + Encryptdata(lblRefID.Text) + FileCount + ".pdf";
                        hdnPDFPathForObject.Value = src;
                        LoadImageToViewer(string.Empty);
                        break;
                    default:
                        break;
                }


                if (count == 0)
                {
                    drpDeleteCount.Enabled = false;
                    drpPageCount.Enabled = false;
                }
                else
                {
                    drpDeleteCount.Enabled = true;
                    drpPageCount.Enabled = true;
                    drpPageCount.Items.Clear();
                    drpDeleteCount.Items.Clear();
                    DDLDrop.Items.Clear();
                    drpPageCount.Items.Clear();
                    drpPageCount.Items.Add(new ListItem("Before", "0"));
                    int x = 0;
                    for (int i = 0; i < count; i++)
                    {
                        x = i + 1;
                        drpPageCount.Items.Add(new ListItem(x.ToString(), x.ToString()));
                    }
                    x = count + 1;
                    drpPageCount.Items.Add(new ListItem("After", x.ToString()));
                    drpDeleteCount.Items.Clear();
                    drpDeleteCount.Items.Add(new ListItem("<Select>", "0"));
                    if (count > 1)
                    {
                        for (int j = 1; j <= count; j++)
                        {
                            drpDeleteCount.Items.Add(new ListItem(j.ToString(), j.ToString()));
                        }
                    }
                    for (int i = 0; i < count; i++)
                    {
                        DDLDrop.Items.Insert(i, (i + 1).ToString());

                    }
                    DDLDrop.SelectedIndex = 0;

                }

            }
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Initialisation();", true);
            Logger.Trace("ReloadIframe Finished", Session["LoggedUserId"].ToString());
        }
        /// <summary>
        /// delete the page from document on delete click
        /// </summary>
        public void DeletePage()
        {
            Logger.Trace("deletepages Started", Session["LoggedUserId"].ToString());
            Session["SameExt"] = "TRUE";
            string Path_ExistingFile = string.Empty;
            string Path_MergedFile = string.Empty;
            string Path_FileToMerge = string.Empty;
            string destinationFile = string.Empty;
            string Path_TempFileLocation = string.Empty;
            string sourcepath = string.Empty;
            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
            string ActualFolder = hdnFileLocation.Value;
            string FileExtension = afterdot(hdnEncrpytFileName.Value).ToLower();
            int index = ActualFolder.LastIndexOf("\\");
            int PagesCount = 0;
            int DeletedPageNo = 0;

            ActualFolder = ActualFolder.Substring(0, index) + "\\";
            Session["Encryptdata"] = hdnEncrpytFileName.Value;
            if (drpDeleteCount.SelectedItem.Value != "0")
            {
                if (Convert.ToString(Session["TempFileLocation"]).Length > 0)
                {
                    Path_ExistingFile = Session["TempFileLocation"].ToString();
                }
                else
                {
                    Path_ExistingFile = hdnFileLocation.Value;
                }
                Logger.Trace("Path_ExistingFile value:" + Path_ExistingFile, Session["LoggedUserId"].ToString());
                ModifiedCount = ModifiedCount + 1; // Increment document modified count
                Path_MergedFile = TempFolder + Encryptdata(lblRefID.Text) + ModifiedCount.ToString() + FileExtension;
                Path_FileToMerge = Path_ExistingFile;
                destinationFile = TempFolder + beforedot(hdnEncrpytFileName.Value);
                Path_TempFileLocation = TempFolder + Encryptdata(lblRefID.Text);

                //Unzip document for merging          
                if (File.Exists(ActualFolder + @"\" + beforedot(hdnEncrpytFileName.Value) + ".zip"))
                    FileZip.Unzip(ActualFolder + @"\" + beforedot(hdnEncrpytFileName.Value) + ".zip");
                Logger.Trace("Unzipped the file:", Session["LoggedUserId"].ToString());
                if (FileExtension == ".pdf")
                {
                    // Remove page from pdf
                    PDFMerging.deleteMergeFiles(Path_MergedFile, Path_FileToMerge, Convert.ToInt32(drpDeleteCount.SelectedValue.ToString()), "delete");
                    Logger.Trace("Merging completed after deleting for pdf files", Session["LoggedUserId"].ToString());
                }
                else if (FileExtension == ".tif")
                {
                    // Remove page from tif
                    string[] source = new string[] { Path_ExistingFile };
                    TiffUtil.TiffUtil.deleteTiffPages(source, Path_MergedFile, Convert.ToInt32(drpDeleteCount.SelectedValue) - 1);
                    Logger.Trace("Merging completed after deleting for tiff files", Session["LoggedUserId"].ToString());
                }

                sourcepath = Path_TempFileLocation + "\\" + drpDeleteCount.SelectedValue.ToString() + ".pdf";
                File.Delete(sourcepath);
                int.TryParse(drpDeleteCount.SelectedValue, out DeletedPageNo);

                PagesCount = new DirectoryInfo(destinationFile).GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                //Session["PagesCount"] = PagesCount;
                try
                {
                    if (DeletedPageNo != PagesCount)
                    {
                        // Loop through splitted pdf and rename accordingly
                        for (int i = DeletedPageNo; i <= PagesCount; i++)
                        {
                            string OldPageName = (i + 1).ToString();
                            string NewPageName = i.ToString();
                            File.Move(Path_TempFileLocation + "\\" + OldPageName + ".pdf", destinationFile + "\\" + NewPageName + ".pdf");
                        }
                        Logger.Trace("File moved with updated pages:", Session["LoggedUserId"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                }
                //Update the Annotation table with updated pageno and replace the deleted page with null
                UpdateAnnotatedPageAfterDelete(DeletedPageNo);

                //Update the tagged pages in taggmaster with updated pageno and replace the deleted page with null
                UpdateTaggedPageAfterDelete(DeletedPageNo);
                Session["TempFileLocation"] = Path_MergedFile;
                Session["TotalPageCount"] = PagesCount;
                Session["Ext"] = FileExtension;

                getAvailablePages();
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('No page is available to delete')", true);
            }
            Logger.Trace("deletepages Finished", Session["LoggedUserId"].ToString());
        }


        #region commit changes
        protected void btnCommitChanges_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnCommitChanges_Click started", Session["LoggedUserId"].ToString());
            btnSaveTag.Enabled = true;
            hdnUploaded.Value = "TRUE";

            int DocumentId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            Session["hdnOrgFileLocation"] = hdnFileLocation.Value;
            // Maintain verion if document is modified
            Logger.Trace("btnCommitChanges_Click modified count:" + ModifiedCount, Session["LoggedUserId"].ToString());
            if (ModifiedCount > 0)
            {
                VersionManagement(DocumentId, "PageModification");

            }

            if (RelocateFileToOriginalLocation())
            {
                Results result = SaveDocumentDetailsToDB();

                if (result.ErrorState == 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "alert('" + result.Message + "');", true);
                    if (ddlDocStatus.SelectedItem.ToString() == "Indexing Completed")
                    {
                        ReleaseDocment();
                    }
                    // Send mail to maker on document reject
                    if (ddlDocStatus.SelectedItem.ToString() == "Rejected")
                        MailPanelFill();

                    // Delete temporary file on success save.
                    DeleteTempFiles();

                    //to remove the signature when it is got editted//
                    if (hdnIsDigitallySigned.Value == "True")
                    {
                        if (ModifiedCount > 0)
                        {

                            string action = "RemoveDigitalSignature";
                            string signatureXML = CreateXML();
                            DocumentBL objDocumentBL = new DocumentBL();
                            result = objDocumentBL.ManageDigitalSignature(action, "", signatureXML, hdnLoginOrgId.Value, hdnLoginToken.Value);
                        }
                    }
                    Session["TempFileLocation"] = "";
                    Session["EncyptImgName"] = "";
                    Session["Ext"] = "";
                    ModifiedCount = 0;
                    //SearchAgain(); /*TaskWDMS-S-5058 D*/ 
                    /*TaskWDMS-S-5058 BS A*/
                    InitializeDocumentView();
                    LoadImageToViewer(string.Empty);
                    getAvailablePages();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "LoadPage", "gotoPage();", true);
                    /*TaskWDMS-S-5058  BE A*/


                }
                else if (result.ErrorState == 1)
                {
                    hdnUploaded.Value = "FALSE";
                    Response.Write("<script>alert('" + result.Message + "');</script>");
                }
            }
            Logger.Trace("btnCommitChanges_Click finished", Session["LoggedUserId"].ToString());
        }

        /// <summary>
        /// Moving file from temp location to original location
        /// </summary>
        private bool RelocateFileToOriginalLocation()
        {
            bool status = false;
            try
            {
                Logger.Trace("Started moving Uploaded Document in SaveDocumentChanges from commit changes", Session["LoggedUserId"].ToString());

                TodayDate = DateTime.Today.ToString("yyyy-MM-dd");

                string EncryptedFileName = string.Empty;
                string FilePath = string.Empty;
                string FilepathToRename = string.Empty;
                string Actual_FileName_TempFolder = string.Empty;
                string Zip_FileName_TempFolder = string.Empty;
                string Zip_FileName_ActaulFolder = string.Empty;
                string ServerPath = ConfigurationManager.AppSettings["ImageLocation"];
                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                string ActualFolder = string.Empty; // ServerPath + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\";
                hdnUploaded.Value = "TRUE";

                if (Session["TempFileLocation"] != null && Session["TempFileLocation"].ToString() != string.Empty && Session["hdnOrgFileLocation"] != null && Session["hdnOrgFileLocation"].ToString() != string.Empty)
                {
                    ActualFolder = Convert.ToString(Session["hdnOrgFileLocation"]);
                    ActualFolder = ActualFolder.Substring(0, ActualFolder.LastIndexOf("\\")) + "\\";

                    if (!Directory.Exists(ActualFolder))
                        Directory.CreateDirectory(ActualFolder);

                    Logger.Trace("Renaming document name to original file name", Session["LoggedUserId"].ToString());
                    //Renaming document name to original file name
                    FilePath = Session["TempFileLocation"].ToString();
                    FilepathToRename = FilePath.Substring(0, FilePath.LastIndexOf("\\")) + "\\" + Session["Encryptdata"].ToString();
                    Session["TempFileLocation"] = FilepathToRename;

                    // Rename file to actual filename (temp folder).

                    if (FilePath != FilepathToRename)
                    {
                        if (File.Exists(FilePath))
                        {
                            File.Delete(FilepathToRename);
                            File.Copy(FilePath, FilepathToRename);//copy file to original file name in tempfolder
                        }

                    }
                    Logger.Trace("Zipping documents before moving documents from tempfolder to original location", Session["LoggedUserId"].ToString());
                    /*---Zipping documents before moving documents from tempfolder to original location---*/
                    Actual_FileName_TempFolder = Session["TempFileLocation"].ToString();
                    Zip_FileName_TempFolder = beforedot(Session["TempFileLocation"].ToString()) + ".zip";

                    // Zip file
                    FileZip.Zip(Actual_FileName_TempFolder, true);

                    /*---Moving zipped documents from tempfolder to original location---*/
                    if (File.Exists(Zip_FileName_TempFolder))
                    {
                        File.Copy(Zip_FileName_TempFolder, beforedot(Session["hdnOrgFileLocation"].ToString()) + ".zip", true);
                    }
                    Logger.Trace("moving the modified splitted file from tempfolder to original folder", Session["LoggedUserId"].ToString());
                    //moving the modified splitted file from tempfolder to original folder
                    EncryptedFileName = beforedot(Session["EncyptImgName"].ToString());
                    if (Directory.Exists(ActualFolder + EncryptedFileName))
                    {
                        Directory.Delete(ActualFolder + EncryptedFileName, true);

                        /*bug fix*/
                        copyDirectory(TempFolder + EncryptedFileName, ActualFolder + EncryptedFileName);
                    }
                }
                status = true;
                Logger.Trace("Finished Saving Uploaded Document in Save Button Click", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                hdnUploaded.Value = "FALSE";
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return status;
        }
        /// <summary>
        /// update the database with document modification
        /// </summary>
        /// <returns></returns>
        private Results SaveDocumentDetailsToDB()
        {
            Results result = null;
            DocumentBL objDocumentBL = new DocumentBL();
            try
            {
                string xml = string.Empty;
                string Uploadaction = string.Empty;
                string strQuery = string.Empty;
                string strColumns = string.Empty;
                string strValues = string.Empty;
                string xmlPageNoMappings = string.Empty;
                string xmlTagPageNoMaping = string.Empty;
                DataSet dsPageNoMappings = new DataSet();
                DataSet dsTagPageNoMapping = new DataSet();
                int intVersion = Convert.ToInt32(ViewState["Version"]);
                hdnPagesCount.Value = DDLDrop.Items.Count.ToString();
                int intPageCount = hdnPagesCount.Value.Trim() == string.Empty ? 0 : Convert.ToInt32(hdnPagesCount.Value);
                //int intPageCount = 129;
                int iProcId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                if (ModifiedCount > 0)
                {
                    intVersion = Convert.ToInt32(ViewState["Version"]) + 1;
                }
                if (iProcId > 0)
                {
                    //DMS5-4377 M
                    strValues = "UPLOAD_iModifiedBy=" + loginUser.UserId.ToString() + "," + "UPLOAD_vDocVersion = " + intVersion + ","
                        + (ddlDocStatus.SelectedValue != "0" ? ("UPLOAD_iCurrentStatus_ID=" + ddlDocStatus.SelectedValue.ToString() + ",") : string.Empty)  // update only if status is selected
                        + (ddlStatusRemarks.SelectedValue != "0" ? ("UPLOAD_iStatusRemarks_ID=" + ddlStatusRemarks.SelectedValue.ToString() + ",") : string.Empty)  // update only if remarks is selected
                        + ("UPLOAD_vStatusCommemts=&apos;" + txtoldremarks.Text + (txtoldremarks.Text.Trim().Length > 0 ? Environment.NewLine + Environment.NewLine : string.Empty) + loginUser.UserName.ToString() + ":" + txtRemarks.Text + "&apos;,")
                        + "UPLOAD_vSearchKeyWords=&apos;" + txtKeyword.Text + "&apos;,"
                        + "UPLOAD_iPageCount=" + intPageCount + ","
                        + "UPLOAD_iOrgID=" + loginUser.LoginOrgId.ToString() + ","
                        + "UPLOAD_bIsUploaded=1,";

                    strQuery = "update upload set " + strValues.Substring(0, strValues.Length - 1) + " where UPLOAD_iProcessID = " + Convert.ToString(Request.QueryString["id"]);
                    xml = "<Table><row><query>" + strQuery + "</query></row></Table>";

                    //  Get annotation page mapping xml      
                    DataTable dtAnnotate = (DataTable)Session["AnnotationPageNo"];
                    dsPageNoMappings.Tables.Add(dtAnnotate.Copy());
                    xmlPageNoMappings = dsPageNoMappings.GetXml();

                    //Get tagmaster page mapping xml
                    DataTable dtTags = (DataTable)Session["TaggedPageNo"];
                    dsTagPageNoMapping.Tables.Add(dtTags.Copy());
                    xmlTagPageNoMaping = dsTagPageNoMapping.GetXml();

                    Uploadaction = "UpdateDocumentDetails";

                    //Updating upload table and annotation table 
                    result = objDocumentBL.ManageUploadedDocuments(xml, hdnLoginOrgId.Value, hdnLoginToken.Value, Uploadaction, iProcId, false, xmlPageNoMappings, xmlTagPageNoMaping);

                    Logger.Trace("Save button click after saving or updating the result= " + result.Message, Session["LoggedUserId"].ToString());

                    //if (Session["deltagpgno"] != "")
                    //{
                    //    deletetag();
                    //}

                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: hiii" + ex.Message, Session["LoggedUserId"].ToString());
            }
            return result;
        }
        #endregion


        protected void deletetag()
        {
            try
            {
                string processid = Request.QueryString["id"];
                string maintagid = cmbMainTag.SelectedValue.Trim();
                string subtagid = cmbSubTag.SelectedValue.Trim();
                string connectionstring = ConfigurationManager.ConnectionStrings["SqlServerConnString"].ConnectionString.ToString();
                SqlConnection con = new SqlConnection(connectionstring);
                con.Open();
                var cmd = con.CreateCommand();
                cmd.CommandText = "USP_ModifiedDocDeleteTag";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@SP_DOCPROCESSID", processid);
                cmd.Parameters.AddWithValue("@SP_MainTagID", maintagid);
                cmd.Parameters.AddWithValue("@SP_SubTagID", subtagid);
                cmd.Parameters.AddWithValue("@SP_Pagenumbers", drpDeleteCount.SelectedValue.Trim());
                cmd.ExecuteScalar();

            }
            catch (Exception ex) { }
        }

        protected void btnUploadNewVersion_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnUploadNewVersion_Click started", Session["LoggedUserId"].ToString());
            if (Convert.ToString(Session["Ext"]).Length > 0)
            {
                hdnUploaded.Value = "TRUE";
                Session["hdnOrgFileLocation"] = hdnFileLocation.Value;
                Response.Redirect("DocumentUpload.aspx?id=" + Request.QueryString["id"] + "&docid=" + Request.QueryString["docid"] + "&depid=" + Request.QueryString["depid"] + "&ArchivedAction=PageModification");
                DeleteTempFiles();
            }
            else
            {
                //Added newly for enchancment for reloading viewer on page refresh
                DocumentDetails.Attributes.Add("style", "display:block;");
                LoadImageToViewer(string.Empty);
                lblResult.Text = "No changes have been made to the existing document!";
                lblResult.Visible = true;
                lblResult.ForeColor = System.Drawing.Color.Red;

            }
            Logger.Trace("btnUploadNewVersion_Click finished", Session["LoggedUserId"].ToString());
        }

        public void DeleteTempFiles()
        {
            try
            {
                //Delete Temp Files
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                string FileExtension = Session["Ext"].ToString().ToLower();

                // Delete original file; first copied.
                Logger.Trace("Deleteing original file; first copied.", Session["LoggedUserId"].ToString());
                File.Delete(TempFolder + Encryptdata(lblRefID.Text) + FileExtension);

                //Delete zip file
                Logger.Trace("Deleteing zip file", Session["LoggedUserId"].ToString());
                File.Delete(TempFolder + Encryptdata(lblRefID.Text) + ".zip");

                // Delete all modified versions documents
                Logger.Trace("Deleting all modified versions documents", Session["LoggedUserId"].ToString());
                for (int i = 1; i <= ModifiedCount + 1; i++)
                {
                    File.Delete(TempFolder + Encryptdata(lblRefID.Text) + i + FileExtension);
                }

                // Delete splitted files directory; below code is commented because splitted files required to view document
                // Logger.Trace("Deleting splitted files directory.", Session["LoggedUserId"].ToString());
                // Directory.Delete(TempFolder + Encryptdata(lblRefID.Text), true);

                //Delete JPG file directory
                Logger.Trace("Deleting splitted JPG files directory.", Session["LoggedUserId"].ToString());
                Directory.Delete(TempFolder + Encryptdata(lblRefID.Text) + "JPG", true);


            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void DeleteDirectory(string iDirectory)
        {
            try
            {
                Directory.Delete(iDirectory);
            }

            catch
            {

            }

        }
        /// <summary>
        /// delete the whole document
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click1(object sender, EventArgs e)
        {
            Logger.Trace("btnDelete_Click1 Started", Session["LoggedUserId"].ToString());

            string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");
            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
            string ArchiveFolder = System.Configuration.ConfigurationManager.AppSettings["ArchiveFolder"] + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\";

            string FileExtension = afterdot(hdnFileLocation.Value);
            string FilePathToDelete = hdnFileLocation.Value.ToLower().Replace(FileExtension, ".zip");
            string FilePathToArchve = ArchiveFolder + hdnEncrpytFileName.Value.ToLower().Replace(FileExtension, ".zip");

            if (!Directory.Exists(ArchiveFolder))
            {
                Directory.CreateDirectory(ArchiveFolder);
            }
            try
            {
                if (File.Exists(FilePathToDelete))
                {
                    // Copy file to archive location
                    File.Copy(FilePathToDelete, FilePathToArchve, true);

                    // Update database
                    UploadDocBL bl = new UploadDocBL();
                    SearchFilter filter = new SearchFilter();
                    string action = "ArchiveDocument";
                    filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                    filter.DocPhyPath = FilePathToArchve.ToLower().Replace(".zip", FileExtension);
                    filter.DocVirPath = "\\Writer\\ArchivedDocuments\\" + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\" + hdnEncrpytFileName.Value;
                    Results res = bl.DiscardUploadDocument(filter, action, hdnLoginOrgId.Value, hdnLoginToken.Value, "Delete Document");
                    if (res.ActionStatus == "SUCCESS")
                    {
                        // Delete file
                        File.Delete(FilePathToDelete);

                        // Move splitted files to archive location
                        copyDirectory(beforedot(hdnFileLocation.Value), ArchiveFolder + beforedot(hdnEncrpytFileName.Value));
                        //  Directory.Move(beforedot(hdnFileLocation.Value), ArchiveFolder + beforedot(hdnEncrpytFileName.Value));

                        DeleteDirectory(beforedot(hdnFileLocation.Value));
                        SearchAgain();
                        //Response.Redirect("DocumentDownloadSearch.aspx?action=Delete");
                    }
                    else
                    {
                        divMsg.InnerHtml = "Error Discarding Document";
                    }
                    Logger.Trace("btnDelete_Click1 Finished", Session["LoggedUserId"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("btnDelete_Click1 Exception" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnDicardChanges_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnDicardChanges_Click started", Session["LoggedUserId"].ToString());
            //Delete Temp Files
            if (Convert.ToString(Session["Ext"]).Length > 0)
            {
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                switch (Convert.ToString(Session["Ext"]).ToLower())
                {
                    case ".pdf":
                        File.Delete(TempFolder + Encryptdata(lblRefID.Text) + ".pdf");
                        File.Delete(TempFolder + Encryptdata(lblRefID.Text) + "1.pdf");
                        for (int i = 1; i <= Convert.ToInt32(Session["TotalPageCount"]); i++)
                        {
                            File.Delete(TempFolder + Encryptdata(lblRefID.Text) + "\\" + i + ".pdf");
                        }

                        break;
                    case ".tif":
                        File.Delete(TempFolder + Encryptdata(lblRefID.Text) + ".tif");
                        File.Delete(TempFolder + Encryptdata(lblRefID.Text) + "1.tif");
                        for (int i = 1; i <= Convert.ToInt32(Session["TotalPageCount"]); i++)
                        {
                            File.Delete(TempFolder + Encryptdata(lblRefID.Text) + "\\" + i + ".tif");
                        }
                        break;
                    default: break;

                }
            }

            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            string id = Request.QueryString["id"] != null ? Request.QueryString["id"].ToString() : string.Empty;
            string docid = Request.QueryString["docid"] != null ? Request.QueryString["docid"].ToString() : string.Empty;
            string depid = Request.QueryString["depid"] != null ? Request.QueryString["depid"].ToString() : string.Empty;
            string active = Request.QueryString["active"] != null ? Request.QueryString["active"].ToString() : string.Empty;
            string PageNo = Request.QueryString["PageNo"] != null ? Request.QueryString["PageNo"].ToString() : string.Empty;
            Response.Redirect("~/Secure/Core/DocumentDownloadDetails.aspx?id=" + id + "&docid=" + docid + "&depid=" + depid + "&active=" + active + "&PageNo" + PageNo + "&Search=" + SearchCriteria);
            Logger.Trace("btnDicardChanges_Click finished", Session["LoggedUserId"].ToString());
        }

        protected void btnDeletePages_Click(object sender, EventArgs e)
        {
            // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Initialisationdis();", true);
            Logger.Trace("btnDeletePages_Click started", Session["LoggedUserId"].ToString());
            DeletePage();
            DisableAnnotationToolbar();
            //delete tag
            deletetag();
            //var i = drpDeleteCount.SelectedValue.Trim();
            //if (Session["deltagpgno"] == "")
            //    Session["deltagpgno"] = i;
            //else
            //{
            //    Session["deltagpgno"] = Session["deltagpgno"].ToString() + "," + i;
            //}
            //change ends
            ReloadIframe();
            Logger.Trace("btnDeletePages_Click finished", Session["LoggedUserId"].ToString());
        }

        protected void btnSaveTag_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnSaveTag_Click started", Session["LoggedUserId"].ToString());

            //DMS04-3470A -- To refresh tagged pages dropdown
            getAvailablePages();
            txttagpages.Text = "";
            //DMS04-3712D
            //GetTaggedDocumentDetails();
            //DMS04-3712A
            UpdatePropertyTaggedPagesTracker();
            Logger.Trace("btnSaveTag_Click finished", Session["LoggedUserId"].ToString());
        }

        protected void btndeletetag_Click(object sender, EventArgs e)
        {
            Logger.Trace("btndeletetag_Click started", Session["LoggedUserId"].ToString());

            //DMS04-3470A -- To refresh tagged pages dropdown
            getAvailablePages();

            //DMS04-3712D
            //GetTaggedDocumentDetails();
            //DMS04-3712A
            UpdatePropertyTaggedPagesTracker();
            Logger.Trace("btndeletetag_Click finished", Session["LoggedUserId"].ToString());
        }

        protected void LoadMainTag()
        {
            if (afterdot(hdnFileName.Value).ToString().ToLower() == ".pdf" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tif" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tiff")
            {
                DataSet ds = new DataSet();
                TemplateBL bl = new TemplateBL();
                SearchFilter filter = new SearchFilter();
                cmbMainTag.Attributes.Add("onchange", "javascript:return LoadSubTag(this);");
                cmbSubTag.Attributes.Add("onchange", "javascript:return LoadPagesForTag(this);");
                filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                filter.DocumentTypeID = Convert.ToInt32(Request.QueryString["docid"]);
                //ds = bl.GetTagDetails(filter, "MainTag", hdnLoginOrgId.Value, hdnLoginToken.Value);
                ds = bl.GetTaggedMainTag(Convert.ToString(Request.QueryString["id"]));
                cmbMainTag.Enabled = true;
                cmbSubTag.Enabled = true;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    cmbMainTag.Items.Clear();
                    cmbMainTag.DataSource = ds.Tables[0];
                    cmbMainTag.DataTextField = "TextField";
                    cmbMainTag.DataValueField = "ValueField";
                    cmbMainTag.DataBind();
                    if (ds.Tables[0].Rows.Count > 1)
                    {
                        cmbMainTag.SelectedIndex = 1;
                    }
                }
                else
                {
                    cmbMainTag.Items.Clear();
                }
            }
            else
            {
                cmbMainTag.Enabled = false;
                cmbSubTag.Enabled = false;
            }

            LoadSubtag();
        }

        protected void LoadSubtag()
        {
            DataSet ds = new DataSet();
            TemplateBL bl = new TemplateBL();
            ds = bl.GetTaggedSubTag(Convert.ToString(Request.QueryString["id"]), cmbMainTag.SelectedValue);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                cmbSubTag.Items.Clear();
                cmbSubTag.DataSource = ds.Tables[0];
                cmbSubTag.DataTextField = "TextField";
                cmbSubTag.DataValueField = "ValueField";
                cmbSubTag.DataBind();
                cmbSubTag.SelectedIndex = -1;
                if (ds.Tables[0].Rows.Count > 1)
                {
                    cmbSubTag.SelectedIndex = 1;
                }

            }
            else
            {
                cmbSubTag.Items.Clear();
            }
        }

        protected void btnCommonSubmitSub2_Click(object sender, EventArgs e)
        {
            LoadSubtag();
            hdnsubtagvalue.Value = "0";// //TaskWDMS-S-5046 A
            DataSet ds = new DataSet();
            TemplateBL bl = new TemplateBL();
            SearchFilter filter = new SearchFilter();
            filter.DocumentTypeID = Convert.ToInt32(cmbMainTag.SelectedValue);
            //ds = bl.GetTagDetails(filter, "SubTag", hdnLoginOrgId.Value, hdnLoginToken.Value);


            //DMS04-3470D
            if (ddldocumentview.SelectedValue == "Tagged View")
            {
                DataTable dt = new DataTable();

                dt = getAvailablePages();
                if (dt.Rows.Count > 0)
                {
                    LoadImageToViewer(string.Empty);
                }
                else
                {
                    defaultview();
                }


            }


            //ds = bl.GetTaggedSubTag(Convert.ToString(Request.QueryString["id"]), Convert.ToString(filter.DocumentTypeID));
            //ds = bl.GetTaggedSubTag(Convert.ToString(Request.QueryString["id"]), cmbMainTag.SelectedValue);
            //if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //{
            //    cmbSubTag.Items.Clear();
            //    cmbSubTag.DataSource = ds.Tables[0];
            //    cmbSubTag.DataTextField = "TextField";
            //    cmbSubTag.DataValueField = "ValueField";
            //    cmbSubTag.DataBind();
            //    cmbSubTag.SelectedIndex = -1;
            //    if (ds.Tables[0].Rows.Count > 1)
            //    {
            //        cmbSubTag.SelectedIndex = 1;
            //    }

            //}
            //else
            //{
            //    cmbSubTag.Items.Clear();
            //}

        }
        protected void btnCommonSubmitSub3_Click(object sender, EventArgs e)
        {

            if (ddldocumentview.SelectedValue == "Tagged View")
            {
                DataTable dt = new DataTable();
                dt = getAvailablePages();
                if (dt.Rows.Count > 0)
                {
                    LoadImageToViewer(string.Empty);
                }
                else
                {
                    defaultTagview();
                }

            }
            LoadSubtag();
        }

        public static string getRootWebSitePathVal()
        {
            string _location = HttpContext.Current.Request.Url.ToString();
            int applicationNameIndex = _location.IndexOf("/", _location.IndexOf("://") + 3);
            string applicationName = _location.Substring(0, applicationNameIndex) + '/';
            int webFolderIndex = _location.IndexOf('/', _location.IndexOf(applicationName) + applicationName.Length);
            string webFolderFullPath = _location.Substring(0, webFolderIndex);

            //If develoment environement WEB folder won't be there
            webFolderFullPath = webFolderFullPath.Replace("/Accounts", "");
            //DMS04-3777M - used replace for "Secure" too (first letter upper case)
            webFolderFullPath = webFolderFullPath.Replace("/secure", "").Replace("/Secure", "");
            webFolderFullPath = webFolderFullPath.Replace("/Workflow", "");

            return webFolderFullPath;
        }
      


        //protected void btnprint_Click(object sender, EventArgs e)
        //{
        //    string desfolder = "";
        //    string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

        //    if (!Directory.Exists(TempFolder))
        //    {
        //        Directory.CreateDirectory(TempFolder);
        //    }

        //    // Merge splitted files into single file : Takes output file path and array of paths of input PDF files.
        //    string folderpath = TempFolder + Encryptdata(lblRefID.Text);
        //    if (folderpath.Trim().Length > 0)
        //    {
        //        string[] splittedFilesPath = null;
        //        if (Request.QueryString["PageNo"] == "0")
        //        {
        //            splittedFilesPath = Directory.GetFiles(folderpath, "*.pdf");
        //        }
        //        else
        //        {
        //            splittedFilesPath = new string[DDLDrop.Items.Count];
        //            for (int i = 0; i < DDLDrop.Items.Count; i++)
        //            {
        //                splittedFilesPath[i] = folderpath + "\\" + DDLDrop.Items[i].Text + ".pdf";
        //            }
        //        }
        //        if (splittedFilesPath != null && splittedFilesPath.Length > 0)
        //        {
        //            desfolder = TempFolder + "\\" + Encryptdata(lblRefID.Text) + "print" + ".pdf";
        //            // Sort files by filename(numbers)
        //            NumericComparer ns = new NumericComparer(); // new object
        //            Array.Sort(splittedFilesPath, ns);

        //            PDFsharpmerging.MergeFiles(desfolder, splittedFilesPath);
        //        }
        //    }
        //    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "printPdf();", true);
        //   // ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "printPdf('" + desfolder + "');", true);

        //}

        protected void btnSendMail_Click(object sender, EventArgs e)
        {
            string strMailbody = string.Empty;
            string strSubject = string.Empty;
            string messagevalue = string.Empty;
            string strlink = string.Empty;
            string strWeblink = string.Empty;
            string strWebpath = string.Empty;

            string strFilepath = string.Empty;
            string strDownloadWebLink = string.Empty;
            string strFolderpath = string.Empty;
            string strFullpath = string.Empty;
            string strDestinationfile = string.Empty;

            int orgId = 0;
            int ErrorState = 0;
            int ErrorSeverity = 0;
            DocumentMailBL objDocumentMailBL = new DocumentMailBL();
            try
            {
                Logger.Trace("btnSendMail_Click started", Session["LoggedUserId"].ToString());
                strWebpath = System.Configuration.ConfigurationManager.AppSettings["DocumentPath"];//Virtual path of document to download
                string strToken = hdnLoginToken.Value;

                string FileName = Path.GetFileNameWithoutExtension(hdnFileLocation.Value);

                string strMailtext = HtmlHelper.ParseText(txtMessage.Text, false);//Converting Message in mail body to html
                strMailbody = "<html><body>$$mailbody$$</body><html>";
                strMailbody = strMailbody.Replace("$$mailbody$$", strMailtext);
                strSubject = txtsubject.Text;
                strlink = System.Configuration.ConfigurationManager.AppSettings["FileWeb"];
                string strFilename = hdnEncrpytFileName.Value.Replace("\\", "");
                string strDownloadlink = strWebpath + @"/" + strToken + "//" + strFilename;
                orgId = Convert.ToInt32(Session["OrgID"]);

                //Check for the file path exists
                strFolderpath = System.Configuration.ConfigurationManager.AppSettings["DocumentPhysicalPath"];//Physical path of document
                strDownloadWebLink = System.Configuration.ConfigurationManager.AppSettings["FileWeb"];//Virtual path of document to download

                strFullpath = beforedot(hdnFileLocation.Value).ToLower();//Full path of file
                string ActualFolder = hdnFileLocation.Value;
                int index = ActualFolder.LastIndexOf("\\");
                ActualFolder = ActualFolder.Substring(0, index) + "\\";

                if (Directory.Exists(strFullpath.ToLower()))
                {
                    strDestinationfile = strFolderpath + strToken;
                    if (!Directory.Exists(strDestinationfile))
                    {
                        Directory.CreateDirectory(strDestinationfile);
                    }
                }
                //Random random = new Random();
                //int randomNumber = random.Next(1, 1000);
                objDocumentMailBL.SendDocumentMail("Insert", 0, txtMailTo.Text, strSubject, strMailbody, FileName, strDownloadlink, strToken, orgId, out messagevalue, out ErrorState, out ErrorSeverity);
                if (ErrorState == 0)
                {
                    // Implementation changed: DB mail changes to C# mail
                    // Send mail from c# itself, mail inforamtion is saved in DB for audit pupose
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    MailHelper.SendMailMessage(loginUser.EmailId, txtMailTo.Text, string.Empty, string.Empty, strSubject, strMailbody);

                    //code change by ronit


                    // File Extension
                    string FileExtension = Path.GetExtension(hdnFileLocation.Value);
                    string SplittedFilesOriginalFolderPath = IOUtility.beforedot(hdnFileLocation.Value);
                    // Temp folder path to store splitted files
                    string TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + @"\" + FileName;
                    string TempWorkFolderfullPath = ConfigurationManager.AppSettings["TempWorkFolder"] + @"\" + FileName + FileExtension;
                    string DownloadFilepath = TempWorkFolderPath + ".pdf";
                    copyDirectory(SplittedFilesOriginalFolderPath, TempWorkFolderPath);

                    PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getAvailablePages(), DDLDrop.Items.Count);
                    //ends here

                    //FileZip.Unzip(ActualFolder + @"\" + beforedot(hdnEncrpytFileName.Value) + ".zip");
                    try
                    {
                        if (File.Exists(TempWorkFolderfullPath))
                            File.Copy(TempWorkFolderfullPath, strDestinationfile + "\\" + hdnEncrpytFileName.Value, true);//Copy the file to new location
                    }
                    catch (Exception)
                    {
                        Logger.Trace("btnSendMail_Click file copy error", Session["LoggedUserId"].ToString());

                    }


                    lblMesg.Visible = true;
                    lblMesg.ForeColor = System.Drawing.Color.Green;
                    lblMesg.Text = messagevalue;
                    txtMessage.Text = string.Empty;
                    txtMailTo.Text = string.Empty;
                    txtsubject.Text = string.Empty;
                    txtName.Text = string.Empty;
                }
                else if (ErrorState == 1)
                {
                    lblMesg.Visible = true;
                    lblMesg.ForeColor = System.Drawing.Color.Red;
                    lblMesg.Text = messagevalue;
                }
                else
                {
                    lblMesg.Visible = true;
                    lblMesg.ForeColor = System.Drawing.Color.Red;
                    lblMesg.Text = messagevalue;
                }
            }
            catch (Exception ex)
            {
                lblMesg.Visible = true;
                lblMesg.ForeColor = System.Drawing.Color.Red;
                lblMesg.Text = ex.Message;
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void ManageTagview()
        {
            if (afterdot(hdnFileName.Value).ToString().ToLower() == ".pdf" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tif" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tiff")
            {
                cmbMainTag.Enabled = true;
                cmbSubTag.Enabled = true;
            }
            else
            {
                cmbMainTag.Enabled = false;
                cmbSubTag.Enabled = false;
            }
        }

        protected void btnLock_Click(object sender, EventArgs e)
        {

            LockDocument();
        }

        protected void LockDocument()
        {
            pnlIndexpro.Controls.Clear();
            this.NumberOfControls = 0;
            lblRefID.Text = "";
            hdnCountControls.Value = "";
            hdnIndexMinLen.Value = "";
            hdnIndexNames.Value = "";
            hdnMandatory.Value = "";

            GetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value);
            if (Convert.ToInt32(Request.QueryString["MainTagId"]) != 0)
            {
                //DMS04-3470D
                //IsMaintag = true;
                getAvailablePages();
            }
            LoadImageToViewer(string.Empty);

            //   generatecontrols();
            UploadDocBL objUploadDocBL = new UploadDocBL();
            int userId, processId;
            DataTable data = new DataTable();
            DataSet dsValue = new DataSet();
            int orgId = 0;
            string token = string.Empty;
            string message = string.Empty;
            int ErrorState = 0;
            int ErrorSeverity = 0;
            try
            {
                Logger.Trace("btnLock_Click started", Session["LoggedUserId"].ToString());
                userId = Convert.ToInt32(Session["LoggedUserId"]);
                data = (DataTable)ViewState["DocumentData"];
                orgId = Convert.ToInt32(Session["OrgID"]);
                token = hdnLoginToken.Value;
                Session["Token"] = hdnLoginToken.Value;
                pnlControls.Enabled = true;
                processId = Convert.ToInt32(Request.QueryString["id"]);
                if (processId != 0)
                {
                    dsValue = objUploadDocBL.UpdateDocumentStatusForLock("Lock Document", processId, userId, token, orgId, out message, out ErrorState, out ErrorSeverity);
                    // GetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value);
                }

                if (ErrorState == 0)
                {
                    DocumentDetails.Attributes.Add("style", "display:block;");
                    pnlIndexpro.Attributes.Add("style", "display:block;");
                    DocumentDetails.Enabled = true;
                    LoadImageToViewer(string.Empty);
                    lblResult.Visible = false;
                }
                else
                {
                    lblResult.Visible = true;
                    lblResult.Text = message;
                    lblResult.ForeColor = System.Drawing.Color.Red;
                    LoadImageToViewer(string.Empty);
                }

                Logger.Trace("btnLock_Click finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void btnDownloadWithAnn_Click(object sender, EventArgs e)
        {
            DownloadWithAnnotation();
        }

        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            if (hdnAction.Value.ToString().ToUpper() == "SAVEANNOTATIONS")
            {
                SaveAnnotations();
                //Get the annoatation for saving in datatable for delete purpose
                GetAnnotateDocumentDetails();
            }
            else
            {
                LoadImageToViewer(hdnAction.Value);
            }
        }
        protected void btnCallFromJavascriptTag_Click(object sender, EventArgs e)
        {
            LoadTagImageToViewer(hdnAction.Value);
        }

        #endregion

        #region viewer helper methods

        private string convertPDFToImage(string Filepath, string EncryptedFilepath, int RequestingPageNo)
        {
            string CurrentFilepath = string.Empty, filepath = string.Empty;

            try
            {
                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                if (TempFolder.Length > 0)
                {
                    TempFolder += EncryptedFilepath + "JPG";
                }

                //CurrentFilepath = obj.GetJpegPageFromPdf(Filepath, TempFolder, RequestingPageNo.ToString());

                // Get watermark for the document from session
                string Watermark = Session["Watermark"] != null ? Session["Watermark"].ToString() : string.Empty;
                if (Watermark.Length > 0)
                {
                    Logger.Trace("Converting PDF to JPEG along with watermark. Watermark text: " + Watermark, Session["LoggedUserId"].ToString());
                    // Convert pdf to JPEG and apply watermark
                    CurrentFilepath = new ConvertPdf2Image().ConvertPDFtoHojas(Filepath, TempFolder, RequestingPageNo.ToString(), Watermark);
                }
                else
                {
                    Logger.Trace("Converting PDF to JPEG without applying watermark.", Session["LoggedUserId"].ToString());
                    // Convert pdf to JPEG 
                    CurrentFilepath = new ConvertPdf2Image().ConvertPDFtoHojas(Filepath, TempFolder, RequestingPageNo.ToString(), Watermark);
                }

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return CurrentFilepath;
        }

        private void LoadImageToViewer(string Action)
        {
            try
            {
                string curtxttagpages = txttagpages.Text;
                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                int pagecount = Convert.ToInt32(DDLDrop.Items.Count);
                string src = string.Empty;
                int tempagecount = Convert.ToInt16(hdntempPagecount.Value);
                Logger.Trace("LoadImageToViewer started", Session["LoggedUserId"].ToString());
                int CurrentPage = 1;

                if (hdnPageNo.Value.Length > 0)

                    CurrentPage = Convert.ToInt32(hdnPageNo.Value);

                Logger.Trace("Setting image to viewer. Action: " + hdnAction.Value.ToString() + " Page count: " + pagecount + " Current Page:" + CurrentPage, Session["LoggedUserId"].ToString());

                int PageNo = 0;

                switch (Action.ToUpper())
                {
                    case "NEXT":
                        DDLDrop.SelectedIndex = DDLDrop.SelectedIndex + 1;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = tempagecount + 1;

                        txttagpages.Text = curtxttagpages + "," + PageNo;

                        break;

                    case "PREVIOUS":
                        DDLDrop.SelectedIndex = DDLDrop.SelectedIndex - 1;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = tempagecount - 1;


                        if (txttagpages.Text.Contains(","))
                        {
                            List<string> str = new List<string>();
                            str.AddRange(txttagpages.Text.Split(','));
                            str.RemoveAt(str.Count - 1);
                            txttagpages.Text = string.Join(",", str);
                        }
                        else
                            txttagpages.Text = DDLDrop.SelectedValue;

                        break;

                    case "GOTO":
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = DDLDrop.SelectedIndex + 1;

                        txttagpages.Text = DDLDrop.SelectedValue;

                        break;

                    case "FIRST":
                        DDLDrop.SelectedIndex = 0;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = 1;

                        txttagpages.Text = DDLDrop.SelectedValue;

                        break;
                    case "LAST":
                        DDLDrop.SelectedIndex = DDLDrop.Items.Count - 1;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = DDLDrop.Items.Count;

                        txttagpages.Text = DDLDrop.SelectedValue;

                        break;
                    case "THUMB":
                        DDLDrop.SelectedIndex = Convert.ToInt32(hdnThumbPageNo.Value) - 1;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = Convert.ToInt32(DDLDrop.SelectedValue);

                        txttagpages.Text = DDLDrop.SelectedValue;

                        break;
                    default:
                        DDLDrop.SelectedIndex = 0;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = 1;

                        txttagpages.Text = DDLDrop.SelectedValue;

                        break;
                }
                hdntempPagecount.Value = tempagecount.ToString();
                if (PageNo > 0 && tempagecount <= pagecount)
                {
                    hdnPageNo.Value = PageNo.ToString();
                    DDLDrop.SelectedValue = PageNo.ToString();
                }
                else
                    PageNo = CurrentPage;

                string PdfPath = TempFolder + beforedot(Session["EncyptImgName"].ToString()) + "\\" + PageNo.ToString() + ".pdf";

                string OriginalFilePath = hdnFileLocation.Value;

                string strPdfPath = OriginalFilePath;



                string ImageName = beforedot(Session["EncyptImgName"].ToString());

                // code by rakesh shinde on 10-05-2017

                //  PDFViewer1.filepath = hdnFileLocation.Value;

                // 05-06-2023 
                // pdfFrame1.Attributes.Add("src", "../../pdf.js/web/viewer.html?f=" + strPdfPath);



             
              //  string src = string.Empty;
                src = GetSrc("Handler") + strPdfPath + "#toolbar=1";
                pdfFrame1.Attributes.Add("src", src);
                // PDFViewer1.filepath = PdfPath;
                // 05-06-2023 

                Session["pdfpath"] = PdfPath;
                Logger.Trace("Converting PDF to JPEG. PDF Path: " + PdfPath + " Image Name: " + ImageName + " Page No: " + PageNo, Session["LoggedUserId"].ToString());
                string CurrentFilepath = convertPDFToImage(PdfPath, ImageName, PageNo);

                if (CurrentFilepath.Length > 0)
                {
                    src = GetSrc("Handler") + CurrentFilepath;
                    src = src.Replace("\\", "//").ToLower();

                    Logger.Trace("Setting image to viewer using RegisterStartupScript: " + src, Session["LoggedUserId"].ToString());
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "loadImageAndAnnotations", "loadImageAndAnnotations('" + src + "');", true);

                    //PDFViewer1.Load += new EventHandler(PDFViewer1.JustPostbakUserControl);
                }
                else
                {
                    Logger.Trace("convertPDFToImage method returned file path as empty.", Session["LoggedUserId"].ToString());
                }
                //Load Annotations
                if (hdnAction.Value.ToString().ToUpper() != "SAVEANNOTATIONS")
                {
                    Logger.Trace("Invoke method to get annotations from databnase.", Session["LoggedUserId"].ToString());
                    GetAnnotations();
                    //Logger.Trace("Invoke method to load annotations to viewer.", Session["LoggedUserId"].ToString());
                    //LoadAnnotations();
                }

            }
            catch
            {

            }
        }

        private void LoadTagImageToViewer(string Action)
        {
            try
            {
                string curtxttagpages = txttagpages.Text;
                string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];
                int pagecount = Convert.ToInt32(DDLTagpagecount.Items.Count);
                string src = string.Empty;
                int tempagecount = Convert.ToInt16(hdntempPagecount.Value);
                Logger.Trace("LoadImageToViewer started", Session["LoggedUserId"].ToString());
                int CurrentPage = 1;

                if (hdnPageNo.Value.Length > 0)

                    CurrentPage = Convert.ToInt32(hdnPageNo.Value);

                Logger.Trace("Setting image to viewer. Action: " + hdnAction.Value.ToString() + " Page count: " + pagecount + " Current Page:" + CurrentPage, Session["LoggedUserId"].ToString());

                int PageNo = 0;

                switch (Action.ToUpper())
                {
                    case "NEXT":
                        DDLTagpagecount.SelectedIndex = DDLTagpagecount.SelectedIndex + 1;
                        PageNo = Convert.ToInt32(DDLTagpagecount.SelectedValue);
                        tempagecount = tempagecount + 1;

                        txttagpages.Text = curtxttagpages + "," + PageNo;

                        break;

                    case "PREVIOUS":
                        DDLTagpagecount.SelectedIndex = DDLTagpagecount.SelectedIndex - 1;
                        PageNo = Convert.ToInt32(DDLTagpagecount.SelectedValue);
                        tempagecount = tempagecount - 1;


                        if (txttagpages.Text.Contains(","))
                        {
                            List<string> str = new List<string>();
                            str.AddRange(txttagpages.Text.Split(','));
                            str.RemoveAt(str.Count - 1);
                            txttagpages.Text = string.Join(",", str);
                        }
                        else
                            txttagpages.Text = DDLTagpagecount.SelectedValue;

                        break;

                    case "GOTO":
                        PageNo = Convert.ToInt32(DDLTagpagecount.SelectedValue);
                        tempagecount = DDLTagpagecount.SelectedIndex + 1;

                        txttagpages.Text = DDLTagpagecount.SelectedValue;

                        break;

                    case "FIRST":
                        DDLTagpagecount.SelectedIndex = 0;
                        PageNo = Convert.ToInt32(DDLTagpagecount.SelectedValue);
                        tempagecount = 1;

                        txttagpages.Text = DDLTagpagecount.SelectedValue;

                        break;
                    case "LAST":
                        DDLTagpagecount.SelectedIndex = DDLTagpagecount.Items.Count - 1;
                        PageNo = Convert.ToInt32(DDLTagpagecount.SelectedValue);
                        tempagecount = DDLTagpagecount.Items.Count;

                        txttagpages.Text = DDLTagpagecount.SelectedValue;

                        break;
                    case "THUMB":
                        DDLTagpagecount.SelectedIndex = Convert.ToInt32(hdnThumbPageNo.Value) - 1;
                        PageNo = Convert.ToInt32(DDLTagpagecount.SelectedValue);
                        tempagecount = Convert.ToInt32(DDLTagpagecount.SelectedValue);

                        txttagpages.Text = DDLTagpagecount.SelectedValue;

                        break;
                    default:
                        DDLTagpagecount.SelectedIndex = 0;
                        PageNo = Convert.ToInt32(DDLTagpagecount.SelectedValue);
                        tempagecount = 1;

                        txttagpages.Text = DDLTagpagecount.SelectedValue;

                        break;
                }
                hdntempPagecount.Value = tempagecount.ToString();
                if (PageNo > 0 && tempagecount <= pagecount)
                {
                    hdnPageNo.Value = PageNo.ToString();
                    DDLTagpagecount.SelectedValue = PageNo.ToString();
                }
                else
                    PageNo = CurrentPage;

                string PdfPath = TempFolder + beforedot(Session["EncyptImgName"].ToString()) + "\\" + PageNo.ToString() + ".pdf";
                string ImageName = beforedot(Session["EncyptImgName"].ToString());

                // code by rakeh shinde on 10-05-2017

                PDFViewer1.filepath = hdnFileLocation.Value;

                //PDFViewer1.filepath = PdfPath;

                Logger.Trace("Converting PDF to JPEG. PDF Path: " + PdfPath + " Image Name: " + ImageName + " Page No: " + PageNo, Session["LoggedUserId"].ToString());
                string CurrentFilepath = convertPDFToImage(PdfPath, ImageName, PageNo);

                if (CurrentFilepath.Length > 0)
                {
                    src = GetSrc("Handler") + CurrentFilepath;
                    src = src.Replace("\\", "//").ToLower();

                    Logger.Trace("Setting image to viewer using RegisterStartupScript: " + src, Session["LoggedUserId"].ToString());
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "loadImageAndAnnotations", "loadImageAndAnnotations('" + src + "');", true);

                    //PDFViewer1.Load += new EventHandler(PDFViewer1.JustPostbakUserControl);
                }
                else
                {
                    Logger.Trace("convertPDFToImage method returned file path as empty.", Session["LoggedUserId"].ToString());
                }
                //Load Annotations
                if (hdnAction.Value.ToString().ToUpper() != "SAVEANNOTATIONS")
                {
                    Logger.Trace("Invoke method to get annotations from databnase.", Session["LoggedUserId"].ToString());
                    GetAnnotations();
                    //Logger.Trace("Invoke method to load annotations to viewer.", Session["LoggedUserId"].ToString());
                    //LoadAnnotations();
                }

            }
            catch
            {

            }
        }

        #region Tags

        /// <summary>
        /// Update property - tagged pages 
        /// </summary>
        //DMS04-3712D
        //public void GetTaggedDocumentDetails()
        //DMS04-3712A
        public void UpdatePropertyTaggedPagesTracker()
        {
            int DocumentId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;

            //DMS04-3712D
            //Results results = GetTagsForUpdatePages(DocumentId);//Get the annoatation for saving in datatable for delete purpose

            //DMS04-3712A
            Results results = GetTaggedPages(DocumentId);//Get the tagged pages

            if (results != null && results.ResultDS != null && results.ResultDS.Tables.Count > 0 && results.ResultDS.Tables[0].Rows.Count > 0)
            {
                TaggedPageNo = results.ResultDS.Tables[0];//binding the annotated document details into datatable property
            }
            Session["TaggedPageNo"] = TaggedPageNo;
        }

        //DMS04-3712D
        //private Results GetTagsForUpdatePages(int DocumentId) 
        //DMS04-3712A
        private Results GetTaggedPages(int DocumentId)
        {
            SearchFilter SearchFilterBE = new SearchFilter();
            DocumentBL objTags = new DocumentBL();

            string action = "GetTaggedPages";
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            SearchFilterBE.UploadDocID = DocumentId;

            Logger.Trace("Calling database to get annotated images. DocumentId: " + SearchFilterBE.UploadDocID + ", Action: " + action, Session["LoggedUserId"].ToString());
            return objTags.ManageTagDetails(SearchFilterBE, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
        }

        public void UpdateTaggedPageAfterDelete(int deletedPageNo)
        {
            int pageNo = 0;
            if (TaggedPageNo.Rows.Count > 0)
            {
                for (int i = 0; i < TaggedPageNo.Rows.Count; i++)
                {
                    if (TaggedPageNo.Rows[i]["NewPageNo"].ToString() != string.Empty)
                    {
                        //updating the Annotation Datatable NewPageNo value which is equal to deleted page with null value
                        if (Convert.ToInt32(TaggedPageNo.Rows[i]["NewPageNo"]) == deletedPageNo)
                        {
                            TaggedPageNo.Rows[i]["NewPageNo"] = DBNull.Value;
                            continue;
                        }
                        //Updating other pages coming after deleted page by decrementing the value by 1
                        if (Convert.ToInt32(TaggedPageNo.Rows[i]["NewPageNo"]) > deletedPageNo)
                        {
                            pageNo = Convert.ToInt32(TaggedPageNo.Rows[i]["NewPageNo"]);
                            TaggedPageNo.Rows[i]["NewPageNo"] = pageNo - 1;
                        }
                    }

                }
            }
            Session["TaggedPageNo"] = TaggedPageNo;
        }

        public void UpdateTaggedPageAfterInsert(int InsertPageNoPosition, int InsertedPagesCount)
        {
            if (TaggedPageNo.Rows.Count > 0)
            {
                //Updating other pages with existing page number plus addedpage number.
                for (int i = 0; i < TaggedPageNo.Rows.Count; i++)
                {
                    int NewPageNo = TaggedPageNo.Rows[i]["NewPageNo"] != DBNull.Value ? Convert.ToInt32(TaggedPageNo.Rows[i]["NewPageNo"]) : 0;

                    if (NewPageNo >= InsertPageNoPosition)
                    {
                        TaggedPageNo.Rows[i]["NewPageNo"] = NewPageNo + InsertedPagesCount;
                    }
                }

                // Insert newly added pages
                DataRow drAnnotate = null;
                for (int counter = 1; counter <= InsertedPagesCount; counter++)
                {
                    drAnnotate = TaggedPageNo.NewRow();
                    drAnnotate["NewPageNo"] = InsertPageNoPosition + counter;
                    TaggedPageNo.Rows.Add(drAnnotate);
                }
            }

            Session["TaggedPageNo"] = TaggedPageNo;
        }
        #endregion Tags

        #region Annotation

        private void DisableAnnotationToolbar()
        {
            // Disable annotation toolbar if document is edited i.e., added or deleted pages.
            if (ModifiedCount > 0)
            {
                PDFViewer1.AnnotationToolbarVisible = false;
                PDFViewer1.JustPostbakUserControl(null, null);
            }
        }

        protected void SaveAnnotations()
        {
            Logger.Trace("Saving annotations to database.", Session["LoggedUserId"].ToString());
            Annotations AnnotationsBE = new Annotations();
            AnnotationsBL objAnnotations = new AnnotationsBL();
            string action = string.Empty;
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            Results results = new Results();
            Logger.Trace("Hidden variable action: " + hdnAction.Value.ToString().ToUpper(), Session["LoggedUserId"].ToString());
            if (hdnAnnotaionXML.Value.Length > 0)
            {
                AnnotationsBE.DocumentId = Convert.ToInt32(Request.QueryString["id"]);
                AnnotationsBE.PageNo = Convert.ToInt32(hdnPageNo.Value);
                AnnotationsBE.XAnnotations = WebUtility.HtmlDecode(hdnAnnotaionXML.Value.ToString());

                AnnotationsBE.xDocumentWithAnnotations = WebUtility.HtmlDecode(hdnAnnotionwithDoc.Value.ToString());
                action = "SaveAnnotations";

                Logger.Trace("Document Id: " + AnnotationsBE.DocumentId + " Page No: " + AnnotationsBE.PageNo + " Action: SaveAnnotations.", Session["LoggedUserId"].ToString());

                results = objAnnotations.ManageAnnotatations(AnnotationsBE, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);

                hdnAnnotaionXML.Value = "";
                hdnAnnotionwithDoc.Value = "";

                // Load Annotations from database
                if (results != null && results.ResultDS != null && results.ResultDS.Tables.Count > 0 && results.ResultDS.Tables[0].Rows.Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Success", "alert('Annotations saved successfully.');", true);
                    Logger.Trace("Database returned dataset which has " + results.ResultDS.Tables[0].Rows.Count + " rows.", Session["LoggedUserId"].ToString());

                }
            }
        }

        private void GetAnnotations()
        {
            Logger.Trace("Get annotations from database.", Session["LoggedUserId"].ToString());
            Annotations AnnotationsBE = new Annotations();
            AnnotationsBL objAnnotations = new AnnotationsBL();
            string action = string.Empty;
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            Results results = new Results();

            AnnotationsBE.DocumentId = Convert.ToInt32(Request.QueryString["id"]);
            AnnotationsBE.PageNo = Convert.ToInt32(hdnPageNo.Value);
            Logger.Trace("Document Id: " + AnnotationsBE.DocumentId + " Page No: " + AnnotationsBE.PageNo, Session["LoggedUserId"].ToString());

            if (AnnotationsBE.DocumentId > 0 && AnnotationsBE.PageNo > 0)
            {
                Logger.Trace("Invoking method to get annotation data from database. Action: GetAnnotations", Session["LoggedUserId"].ToString());
                action = "GetAnnotations";
                results = objAnnotations.ManageAnnotatations(AnnotationsBE, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);

                hdnAnnotaionXML.Value = "";

                // Load Annotations from database
                if (results != null && results.ResultDS != null && results.ResultDS.Tables.Count > 0 && results.ResultDS.Tables[0].Rows.Count > 0)
                {
                    Logger.Trace("Database returned dataset which has " + results.ResultDS.Tables[0].Rows.Count + " rows.", Session["LoggedUserId"].ToString());
                    string xmlAnnotations = WebUtility.HtmlEncode(results.ResultDS.Tables[0].Rows[0]["Anotations"].ToString());
                    if (xmlAnnotations.Length > 0)
                    {
                        Logger.Trace("Annotation has value. Length: " + xmlAnnotations.Length, Session["LoggedUserId"].ToString());
                        hdnAnnotaionXML.Value = xmlAnnotations;
                    }
                }
            }
        }

        private Results GetAnnotations(int DocumentId)
        {
            Annotations AnnotationsBE = new Annotations();
            AnnotationsBL objAnnotations = new AnnotationsBL();

            string action = "GetAnnotationsForDownload";
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            AnnotationsBE.DocumentId = DocumentId;

            Logger.Trace("Calling database to get annotated images. DocumentId: " + AnnotationsBE.DocumentId + ", Action: " + action, Session["LoggedUserId"].ToString());
            return objAnnotations.ManageAnnotatations(AnnotationsBE, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
        }

        private Results GetAnnotationForUpdatePages(int DocumentId)
        {
            Annotations AnnotationsBE = new Annotations();
            AnnotationsBL objAnnotations = new AnnotationsBL();

            string action = "GetAnnotatedPages";
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            AnnotationsBE.DocumentId = DocumentId;

            Logger.Trace("Calling database to get annotated images. DocumentId: " + AnnotationsBE.DocumentId + ", Action: " + action, Session["LoggedUserId"].ToString());
            return objAnnotations.ManageAnnotatations(AnnotationsBE, action, loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
        }



        private void LoadAnnotations()
        {
            Logger.Trace("Loading annotations to the viewer.", Session["LoggedUserId"].ToString());
            string xmlAnnotations = WebUtility.HtmlDecode(hdnAnnotaionXML.Value);
            if (xmlAnnotations.Length > 0)
            {
                Logger.Trace("Annotation has value. Length: " + xmlAnnotations.Length, Session["LoggedUserId"].ToString());
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "annLoad", "annLoad('" + xmlAnnotations + "');", true);
            }
        }

        /// <summary>
        /// Get the annoatation for saving in datatable for delete purpose
        /// </summary>
        public void GetAnnotateDocumentDetails()
        {
            int DocumentId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            Results results = GetAnnotationForUpdatePages(DocumentId);//Get the annoatation for saving in datatable for delete purpose
            if (results != null && results.ResultDS != null && results.ResultDS.Tables.Count > 0 && results.ResultDS.Tables[0].Rows.Count > 0)
            {
                AnnotationPageNo = results.ResultDS.Tables[0];//binding the annotated document details into datatable property
            }
            Session["AnnotationPageNo"] = AnnotationPageNo;
        }

        /// <summary>
        /// Updating the AnnotationPageNo Datatable property with null value for deleted page
        /// </summary>
        /// <param name="deletedPageNo"></param>
        public void UpdateAnnotatedPageAfterDelete(int deletedPageNo)
        {
            int pageNo = 0;
            if (AnnotationPageNo.Rows.Count > 0)
            {
                for (int i = 0; i < AnnotationPageNo.Rows.Count; i++)
                {
                    if (AnnotationPageNo.Rows[i]["NewPageNo"].ToString() != string.Empty)
                    {
                        //updating the Annotation Datatable NewPageNo value which is equal to deleted page with null value
                        if (Convert.ToInt32(AnnotationPageNo.Rows[i]["NewPageNo"]) == deletedPageNo)
                        {
                            AnnotationPageNo.Rows[i]["NewPageNo"] = DBNull.Value;
                            continue;
                        }
                        //Updating other pages coming after deleted page by decrementing the value by 1
                        if (Convert.ToInt32(AnnotationPageNo.Rows[i]["NewPageNo"]) > deletedPageNo)
                        {
                            pageNo = Convert.ToInt32(AnnotationPageNo.Rows[i]["NewPageNo"]);
                            AnnotationPageNo.Rows[i]["NewPageNo"] = pageNo - 1;
                        }
                    }

                }
            }
            Session["AnnotationPageNo"] = AnnotationPageNo;
        }

        /// <summary>
        /// update the AnnotationPageNo datatable with newly added pages after delete
        /// </summary>
        public void UpdateAnnotatedPageAfterInsert(int InsertPageNoPosition, int InsertedPagesCount)
        {
            if (AnnotationPageNo.Rows.Count > 0)
            {
                //Updating other pages with existing page number plus addedpage number.
                for (int i = 0; i < AnnotationPageNo.Rows.Count; i++)
                {
                    int NewPageNo = AnnotationPageNo.Rows[i]["NewPageNo"] != DBNull.Value ? Convert.ToInt32(AnnotationPageNo.Rows[i]["NewPageNo"]) : 0;

                    if (NewPageNo >= InsertPageNoPosition)
                    {
                        AnnotationPageNo.Rows[i]["NewPageNo"] = NewPageNo + InsertedPagesCount;
                    }
                }

                // Insert newly added pages
                DataRow drAnnotate = null;
                for (int counter = 1; counter <= InsertedPagesCount; counter++)
                {
                    drAnnotate = AnnotationPageNo.NewRow();
                    drAnnotate["NewPageNo"] = InsertPageNoPosition + counter;
                    AnnotationPageNo.Rows.Add(drAnnotate);
                }
            }

            Session["AnnotationPageNo"] = AnnotationPageNo;
        }

        #endregion Annotation

        protected void btnInedxSave_Click(object sender, EventArgs e)
        {
            SaveIndexPannelDetails();
        }

        protected void defaultview()
        {
            //ddldocumentview.SelectedValue = "Original View";
            //getAvailablePages();

            LoadImageToViewer(string.Empty);

        }


        protected void defaultTagview()
        {
            //ddldocumentview.SelectedValue = "Tagged View";
            //getAvailablePages();

            LoadImageToViewer(string.Empty);
        }

        protected void btnddlchanged_Click(object sender, EventArgs e)
        {
            hdnsubtagvalue.Value = "0"; //TaskWDMS-S-5046 BS A
            DataTable dt = new DataTable();
            dt = getAvailablePages();
            if (dt.Rows.Count > 0)
            {
                LoadImageToViewer(string.Empty);
            }
            else
            {
                defaultview();
            }
        }

        #endregion

        #region Certficate Adding
        /// <summary>
        /// Certificate upload compete
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AsyncFileUpload2_UploadedComplete(object sender, AjaxControlToolkit.AsyncFileUploadEventArgs e)
        {
            try
            {
                Logger.Trace("AsyncFileUpload2_UploadedComplete Started:-", Session["LoggedUserId"].ToString());
                string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                Session["CertficateName"] = AsyncFileUpload2.FileName;
                Session["TempCertficatePath"] = TempFolder + AsyncFileUpload2.FileName;
                if (!Directory.Exists(TempFolder))
                {
                    Directory.CreateDirectory(TempFolder);
                }
                if (AsyncFileUpload2.HasFile)
                {
                    AsyncFileUpload2.SaveAs(TempFolder + AsyncFileUpload2.FileName);

                }
                Logger.Trace("AsyncFileUpload2_UploadedComplete Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }
        /// <summary>
        /// for getting Signature details
        /// </summary>
        protected void GetSginatureDetails()
        {

            try
            {

                Logger.Trace("GetSginatureDetails Started:-", Session["LoggedUserId"].ToString());
                Results result = null;
                DocumentBL objDocumentBL = new DocumentBL();
                int DocumentId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                if (DocumentId > 0)
                {
                    result = objDocumentBL.GetSignatureDetails("GetSignatureDetails", DocumentId, hdnLoginOrgId.Value, hdnLoginToken.Value);
                    if (result.ResultDS.Tables[0].Rows.Count > 0)
                    {
                        string Auther = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vAuthor"].ToString();
                        string Date = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_iCreatedDate"].ToString();
                        hdnIsDigitallySigned.Value = "True";

                        //lblsignatureDetails.Text = "Document is digitally signed By " + Auther + " on " + Date;
                    }
                    else
                    {
                        hdnIsDigitallySigned.Value = "False";
                    }
                }
                Logger.Trace("GetSginatureDetails Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        /// <summary>
        /// Get Signature Detils for applying to the document
        /// </summary>
        /// <param name="Filepath"></param>
        /// <param name="OutputPath"></param>
        /// <returns></returns>

        protected string GetDigitalSignatureDocumentDetails(string Filepath, string OutputPath)
        {
            string FilepathWithSignature = string.Empty;
            try
            {
                Logger.Trace("GetDigitalSignatureDocumentDetails Started:-", Session["LoggedUserId"].ToString());
                Results result = null;
                DocumentBL objDocumentBL = new DocumentBL();
                int DocumentId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                FilepathWithSignature = OutputPath + "SignedDoc.Pdf";
                if (File.Exists(FilepathWithSignature))
                {
                    File.Delete(FilepathWithSignature);
                }
                result = objDocumentBL.GetSignatureDetails("GetSignatureDetails", DocumentId, hdnLoginOrgId.Value, hdnLoginToken.Value);
                if (result.ResultDS.Tables[0].Rows.Count > 0)
                {
                    Cert myCert = null;
                    myCert = new Cert(result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vCertificatePhysicalPath"].ToString(), result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vCertificatePassword"].ToString());
                    //Adding Meta Datas
                    MetaData MyMD = new MetaData();
                    MyMD.Author = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vAuthor"].ToString();
                    MyMD.Title = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vTitle"].ToString();
                    MyMD.Subject = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vSubject"].ToString();
                    MyMD.Keywords = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vKeywords"].ToString();
                    MyMD.Creator = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vCreator"].ToString();
                    MyMD.Producer = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vProducer"].ToString();

                    PDFSigner pdfs = new PDFSigner(Filepath, FilepathWithSignature, myCert, MyMD);
                    string Reason = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vReason"].ToString();
                    string Contacttext = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vContact"].ToString();
                    string Locationtext = result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_vLocation"].ToString();
                    bool SigVisible;
                    if (result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_bIsVisible"].ToString().Trim().Length > 2)
                    {
                        SigVisible = Convert.ToBoolean(result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_bIsVisible"].ToString());
                    }
                    else
                    {
                        SigVisible = Convert.ToBoolean(Convert.ToInt32(result.ResultDS.Tables[0].Rows[0]["DigitalSignatureDetails_bIsVisible"].ToString()));
                    }

                    pdfs.Sign(Reason, Contacttext, Locationtext, SigVisible);

                }
                else
                {
                    FilepathWithSignature = Filepath;
                }
                Logger.Trace("GetDigitalSignatureDocumentDetails Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return FilepathWithSignature;
        }
        /// <summary>
        /// For validating the certificate
        /// </summary>
        /// <param name="CerFilepath"></param>
        /// <returns></returns>
        protected bool ValidateCertificate(string CerFilepath)
        {
            bool Valid = false;
            Cert myCert = null;
            try
            {
                Logger.Trace("ValidateCertificate Started:-", Session["LoggedUserId"].ToString());
                myCert = new Cert(CerFilepath, txtpassword.Text.ToString());
                Valid = true;
                Logger.Trace("ValidateCertificate Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return Valid;

        }
        /// <summary>
        /// make  xml and return
        /// </summary>
        /// <returns></returns>
        protected string CreateXML()
        {
            string xml = "<Signature>";
            try
            {
                Logger.Trace("CreateXML Started:-", Session["LoggedUserId"].ToString());
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                string UploadId = string.Empty;


                string Password = txtpassword.Text;
                string Author = txtauther.Text;
                string Title = txtTitle.Text;
                string Subject = txtsubject.Text;
                string Keywords = txtkeywods.Text;
                string Creator = txtcreater.Text;
                string Producer = txtproducer.Text;
                string Reason = txtReason.Text;
                string Contact = txtcontact.Text;
                string Location = txtlocation.Text;
                string Createdby = loginUser.UserId.ToString();
                string Isvisible = chkvisibleSignature.Checked == true ? "1" : "0";




                UploadId = Request.QueryString["id"] != null ? Request.QueryString["id"].ToString() : "0"; ;
                xml += "<SignatureDetails><DigitalSignatureDetails_iDcoumentID>" + UploadId + "</DigitalSignatureDetails_iDcoumentID>"
                    + "<DigitalSignatureDetails_vCertificatePassword>" + Password + "</DigitalSignatureDetails_vCertificatePassword>"
                    + "<DigitalSignatureDetails_vAuthor>" + Author + "</DigitalSignatureDetails_vAuthor>"
                    + "<DigitalSignatureDetails_vTitle>" + Title + "</DigitalSignatureDetails_vTitle>"
                    + "<DigitalSignatureDetails_vSubject>" + Subject + "</DigitalSignatureDetails_vSubject>"
                    + "<DigitalSignatureDetails_vKeywords>" + Keywords + "</DigitalSignatureDetails_vKeywords>"
                    + "<DigitalSignatureDetails_vCreator>" + Creator + "</DigitalSignatureDetails_vCreator>"
                    + "<DigitalSignatureDetails_vProducer>" + Producer + "</DigitalSignatureDetails_vProducer>"
                    + "<DigitalSignatureDetails_vReason>" + Reason + "</DigitalSignatureDetails_vReason>"
                    + "<DigitalSignatureDetails_vContact>" + Contact + "</DigitalSignatureDetails_vContact>"
                    + "<DigitalSignatureDetails_vLocation>" + Location + "</DigitalSignatureDetails_vLocation>"
                    + "<DigitalSignatureDetails_iCreatedby>" + Createdby + "</DigitalSignatureDetails_iCreatedby>"
                    + "<DigitalSignatureDetails_bIsVisible>" + Isvisible + "</DigitalSignatureDetails_bIsVisible></SignatureDetails>";

                xml += "</Signature>";
                Logger.Trace("CreateXML Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return xml;

        }
        /// <summary>
        /// for removing the signature details
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        protected void btnRemoveDigitalSignature_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnRemoveDigitalSignature_Click Started:-", Session["LoggedUserId"].ToString());
                string action = "RemoveDigitalSignature";
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                string signatureXML = CreateXML();
                Results result = null;
                DocumentBL objDocumentBL = new DocumentBL();
                result = objDocumentBL.ManageDigitalSignature(action, "", signatureXML, hdnLoginOrgId.Value, hdnLoginToken.Value);
                if (result.ResultDS.Tables[0].Rows[0]["ActionStatus"].ToString() == "SUCCESS")
                {

                    chkvisibleSignature.Checked = true;
                    hdnIsDigitallySigned.Value = "False";
                    // lblsignatureDetails.Text = "";
                    LoadImageToViewer(string.Empty);
                }
                Logger.Trace("btnRemoveDigitalSignature_Click Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        /// <summary>
        /// for adding signature
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnadd_Click(object sender, EventArgs e)
        {
            try
            {

                Logger.Trace("btnadd_Click Started:-", Session["LoggedUserId"].ToString());
                if (ValidateCertificate(Session["TempCertficatePath"].ToString()))
                {
                    string action = "InsertDigitalSignature";
                    string OrginalCertificateFilePath = string.Empty;
                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    string CertificateFolder = System.Configuration.ConfigurationManager.AppSettings["CertificateFolder"];
                    if (!Directory.Exists(CertificateFolder))
                    {
                        Directory.CreateDirectory(CertificateFolder);
                    }
                    string CertificateFinalFolder = CertificateFolder + loginUser.UserOrgId.ToString() + "\\" + loginUser.UserId.ToString() + "\\";
                    if (!Directory.Exists(CertificateFinalFolder))
                    {
                        Directory.CreateDirectory(CertificateFinalFolder);
                    }
                    OrginalCertificateFilePath = CertificateFinalFolder + Session["CertficateName"].ToString();
                    File.Copy(Session["TempCertficatePath"].ToString(), OrginalCertificateFilePath, true);
                    string signatureXML = CreateXML();

                    Results result = null;
                    DocumentBL objDocumentBL = new DocumentBL();
                    result = objDocumentBL.ManageDigitalSignature(action, OrginalCertificateFilePath, signatureXML, hdnLoginOrgId.Value, hdnLoginToken.Value);
                    if (result.ResultDS.Tables[0].Rows.Count > 0)
                    {
                        if (result.ResultDS.Tables[0].Rows[0]["ActionStatus"].ToString() == "SUCCESS")
                        {

                            chkvisibleSignature.Checked = true;
                            LoadImageToViewer(string.Empty);
                            divMsg.Style.Add("color", "green");
                            divMsg.InnerHtml = "Document/s signed successfully.";
                            GetSginatureDetails();
                        }
                    }
                    else
                    {
                        divMsg.Style.Add("color", "Red");
                        divMsg.InnerHtml = "Failed to Add the certificate";
                    }

                }
                else
                {
                    divMsg.Style.Add("color", "Red");
                    divCertMsg.InnerHtml = "Certificate or password is invalid";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowApplySignaturePopUP", "ShowApplySignaturePopUP();", true);
                }
                Logger.Trace("btnadd_Click Completed:-", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }

        }

        #endregion
        public void loadfileiniframe()
        {
            string result = "Success";
            string src = string.Empty;
            DDLDrop.Items.Clear();
            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
            //  DocumentBL DocBL = new DocumentBL();
            // TotalTagPages= DocBL.SearchDocuments(Filter, "GetTaggedPages", hdnLoginOrgId.Value, hdnLoginToken.Value);

            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }
            string filepath = beforedot(hdnFileLocation.Value).ToLower();
            if (Directory.Exists(filepath.ToLower()))
            {
                string sourceFile = filepath;
                string destinationFile = TempFolder + beforedot(hdnEncrpytFileName.Value);
                copyDirectory(sourceFile, destinationFile);

                if (Directory.Exists(destinationFile.ToLower()))
                {
                    DirectoryInfo di = new DirectoryInfo(destinationFile);
                    int count = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                    //To load page numbers
                    int pagecount = count;
                    Hidtotalpagecount.Value = "";
                    Hidtotalpagecount.Value = Convert.ToString(pagecount);

                    if (Convert.ToInt32(Request.QueryString["PageNo"]) != 0 && cmbSubTag.SelectedValue != "0")
                    {

                        SearchFilter Filter = new SearchFilter();
                        DocumentBL DocBL = new DocumentBL();
                        Filter.MainTagID = Convert.ToInt32(Request.QueryString["MainTagId"]);
                        if (Request.QueryString["SubTagId"] != null && Request.QueryString["SubTagId"] != "")
                        {
                            Filter.SubTagID = Convert.ToInt32(Request.QueryString["SubTagId"]);
                        }
                        Filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                        Results Res = DocBL.SearchDocuments(Filter, "GetTaggedPages", hdnLoginOrgId.Value, hdnLoginToken.Value);
                        //foreach (IndexField dp in Res.IndexFields)
                        //{
                        //    DDLDrop.Items.Add(new ListItem(dp.ListItemId.ToString(), dp.ListItemId.ToString()));
                        //}

                        for (int i = 0; i < pagecount; i++)
                        {

                            DDLDrop.Items.Insert(i, (i + 1).ToString());
                        }


                    }
                    else
                    {
                        // lbluntagpage.Text = Convert.ToString(pagecount) + " Pages"; 
                        for (int i = 0; i < pagecount; i++)
                        {

                            DDLDrop.Items.Insert(i, (i + 1).ToString());
                        }
                        drpPageCount.Items.Clear();
                        drpPageCount.Items.Add(new ListItem("<Select>", "0"));
                        for (int i = 1; i < count; i++)
                        {
                            drpPageCount.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        }
                        drpPageCount.Items.Add(new ListItem(count.ToString(), count.ToString()));
                        drpDeleteCount.Items.Clear();
                        drpDeleteCount.Items.Add(new ListItem("<Select>", "0"));
                        if (count > 1)
                        {
                            for (int j = 1; j <= count; j++)
                            {
                                drpDeleteCount.Items.Add(new ListItem(j.ToString(), j.ToString()));
                            }
                        }


                    }

                    if (Request.QueryString["id"] != "")
                    {
                        TotalTagPages = null;

                        //lbltotalnoofPages.Text = "";
                        lbluntagpage.Text = "";
                        lbltotaltagepage.Text = "";
                        DDLTagpagecount.Items.Clear();
                        int TagePagescount = 0;
                        //if (Request.QueryString["MainTagId"] != null && Request.QueryString["SubTagId"] != null)
                        //{
                        TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), pageno, Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));
                        //}
                        //else
                        //{
                        //    TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), pageno, Convert.ToInt32(cmbMainTag.SelectedValue), Convert.ToInt32(cmbSubTag.SelectedValue));
                        //}

                        TagePagescount = TotalTagPages.Rows.Count;
                        if (TotalTagPages.Rows.Count > 0)
                        {
                            for (int k = 0; k < TotalTagPages.Rows.Count; k++)
                            {
                                DDLTagpagecount.Items.Add(TotalTagPages.Rows[k][0].ToString());
                                DDLDrop.Items.Remove(TotalTagPages.Rows[k][0].ToString());


                            }
                            DDLDrop.SelectedIndex = -1;
                            if (Convert.ToString(TotalTagPages.Rows[0][3]) != null || Convert.ToString(TotalTagPages.Rows[0][3]) != "")
                            {
                                if (Convert.ToInt32(Hidtotalpagecount.Value) >= TagePagescount)
                                {

                                    lbluntagpage.Text = Convert.ToString(Convert.ToInt32(Hidtotalpagecount.Value) - TagePagescount) + " Pages";
                                    if (lbluntagpage.Text.Contains("-"))
                                    {
                                        lbluntagpage.Text = "0" + " Pages";
                                    }

                                }
                                else
                                {


                                    lbluntagpage.Text = Convert.ToString(TagePagescount - Convert.ToInt32(Hidtotalpagecount.Value)) + " Pages";
                                    if (lbluntagpage.Text.Contains("-"))
                                    {
                                        lbluntagpage.Text = "0" + " Pages";
                                    }

                                }
                            }
                            else
                            {
                                lbluntagpage.Text = "0";
                                if (lbluntagpage.Text.Contains("-"))
                                {
                                    lbluntagpage.Text = "0" + " Pages";
                                }
                            }

                            lbltotaltagepage.Text = Convert.ToString(TagePagescount + " Pages");
                            //lbltotalPagess.Text = Convert.ToString(Hidtotalpagecount.Value);

                        }
                        //}
                    }

                    if (result == "Success")
                    {
                        src = GetSrc("Handler") + destinationFile;
                    }
                    else
                    {
                        src = GetSrc("PreviewNotAvailable");
                    }

                }

            }

            else
            {
                src = GetSrc("PreviewNotAvailable");
            }
            hdnPDFPathForObject.Value = src;
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "loadpdf();", true);
        }



        protected void btnNextDoc_Click(object sender, EventArgs e)
        {
            if ((Session["dtSearcData"] != null) && (Request.QueryString["id"] != null))
            {
                string Id = Request.QueryString["id"].ToString().Trim();
                DataTable dtSearchData = (DataTable)Session["dtSearcData"];
                dtSearchData.PrimaryKey = new[] { dtSearchData.Columns["UPLOAD_iProcessID"] };
                int index = dtSearchData.Rows.IndexOf(dtSearchData.Rows.Find(Id));
                if (dtSearchData.Rows.Count == index + 1)
                    index = 0;
                else
                    index++;

                DataRow dr = dtSearchData.Rows[index];
                string view = dr["View"].ToString().Replace('$', '&');
                string totalCount = dtSearchData.Compute("COUNT(UPLOAD_iProcessID)", "").ToString();
                lblRecords.Text = string.Format("Showing {0} out of {1} search results.", index, totalCount);
                Response.Redirect(string.Format("DocumentDownloadDetails.aspx?{0}", view));

            }
        }

        protected void btnPrevDoc_Click(object sender, EventArgs e)
        {
            if ((Session["dtSearcData"] != null) && (Request.QueryString["id"] != null))
            {
                string Id = Request.QueryString["id"].ToString().Trim();
                DataTable dtSearchData = (DataTable)Session["dtSearcData"];
                dtSearchData.PrimaryKey = new[] { dtSearchData.Columns["UPLOAD_iProcessID"] };
                int index = dtSearchData.Rows.IndexOf(dtSearchData.Rows.Find(Id));
                if (index == 0)
                    index = dtSearchData.Rows.Count - 1;
                else
                    index--;

                DataRow dr = dtSearchData.Rows[index];
                string view = dr["View"].ToString().Replace('$', '&');
                string totalCount = dtSearchData.Compute("COUNT(UPLOAD_iProcessID)", "").ToString();
                lblRecords.Text = string.Format("Showing {0} out of {1} search results.", index, totalCount);
                Response.Redirect(string.Format("DocumentDownloadDetails.aspx?{0}", view));

            }
        }

        protected void btnBackToSearch_Click(object sender, EventArgs e)
        {
            string doctType = hdnLoginOrgId.Value;
            string deptId = Request.QueryString["depid"];
            Response.Redirect("DocumentDownloadSearch.aspx?s=1&dt=" + doctType + "&dp=" + deptId, true);
        }

        public DataTable getSelectedPages()
        {

            Logger.Trace("getAvailablePages Started", Session["LoggedUserId"].ToString());
            DataTable dt = new DataTable();
            //to get the available pages 
            SearchFilter Filter = new SearchFilter();
            DocumentBL BL = new DocumentBL();
            Results Rs = new Results();
            string action = string.Empty;
            UserBase loginUser = (UserBase)Session["LoggedUser"];

                      

            string[] strPageNo = TextBox2.Text.Split(',');
            // string[] strPageNo = (ArrayList)Session["SplitPages"];

            Convert.ToString(Session["SplitPages "]);// TextBox2.Text.Split(',');

            dt.Clear();
            dt.Columns.Add("DocumentId");
            dt.Columns.Add("TotalPages");
            dt.Columns.Add("PageNumbers");
            //foreach (ListItem ltItem in ddlPDAdd.Items)
            //{
            //    if (ltItem.Text != "")
            //    {
            //        DataRow dtRow = dt.NewRow();
            //        dtRow["DocumentId"] = "0001";
            //        dtRow["TotalPages"] = HidSelectedPages.Value;
            //        dtRow["PageNumbers"] = ltItem.Text;
            //        dt.Rows.Add(dtRow);
            //    }

            foreach (string ltItem in strPageNo)
            {
                if (ltItem.ToString() != "")
                {
                    DataRow dtRow = dt.NewRow();
                    dtRow["DocumentId"] = "0001";
                    dtRow["TotalPages"] = HidSelectedPages.Value;
                    dtRow["PageNumbers"] = ltItem.ToString();
                    dt.Rows.Add(dtRow);
                }
            }





            // DataTable dt = new DataTable();



            // dt = Rs.ResultDS.Tables[0];
            //   LoadDropDown(Rs.ResultDS);


            return dt;
        }
        protected void btnDownloadSelected_Click(object sender, EventArgs e)
        {
            Logger.Trace("Downloading original document.", Session["LoggedUserId"].ToString());

            #region variable declaration and initialization

            string DownloadFilepath = string.Empty;

            // Original File full path
            string OriginalFilePath = hdnFileLocation.Value;
            // File Name
            string FileName = Path.GetFileNameWithoutExtension(hdnFileLocation.Value);
            // File Extension
            string FileExtension = Path.GetExtension(hdnFileLocation.Value);

            // Temp folder path to store splitted files
            string TempWorkFolderPath = ConfigurationManager.AppSettings["TempWorkFolder"] + @"\" + FileName;
            // Splitted files original folder path
            string SplittedFilesOriginalFolderPath = IOUtility.beforedot(hdnFileLocation.Value);

            #endregion

            // Copy files to temp work folder
            Logger.Trace("Copying splitted files from: " + SplittedFilesOriginalFolderPath + " to temporary work folder: " + TempWorkFolderPath, Session["LoggedUserId"].ToString());
            copyDirectory(SplittedFilesOriginalFolderPath, TempWorkFolderPath);

            DownloadFilepath = TempWorkFolderPath + ".pdf";
            HidSelectedPages.Value = ddlPDAdd.Items.Count.ToString();
            Logger.Trace("Merging files: " + DownloadFilepath, Session["LoggedUserId"].ToString());
            //PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getSelectedPages(), ddlPDAdd.Items.Count);
            string[] strPageNo = TextBox2.Text.Split(',');
            
            string strw = string.Empty;
            foreach (string author in strPageNo)
            {
                if (author.Contains("-"))
                {
                    string[] Splitpageno = author.Split('-');
                    int startindex = Convert.ToInt32(Splitpageno[0]);
                    int endindex = Convert.ToInt32(Splitpageno[1]);

                    for (int k = startindex; k <= endindex; k++)
                    {
                        strw += Convert.ToString(k) + ",";
                    }
                }
                else
                {
                    strw += Convert.ToString(author[0]) + ",";
                }
            }
            string strSplitPages = strw.Substring(0, strw.Length - 1);
           // Session["SplitPages "] = strSplitPages;

            TextBox2.Text = strSplitPages;
            int lenght = strw.Replace(",", "").Length;


            //Array.Sort(strPageNo);
            int maxpagenumer = Convert.ToInt32(strPageNo.GetValue(strPageNo.Length - 1));
            DirectoryInfo di = new DirectoryInfo(SplittedFilesOriginalFolderPath);
            int pagecount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;

            if (maxpagenumer <= pagecount)
            {

                PDFMerging.MergeFilesDownload(DownloadFilepath, TempWorkFolderPath, getSelectedPages(), lenght);
                DownloadFile(DownloadFilepath);
                LoadImageToViewer(DDLDrop.SelectedValue.ToString());
            }
            else
            {
                LoadImageToViewer(DDLDrop.SelectedValue.ToString());
                // err.Text = "Page Number should be less or equal to Total Page count";
                //ScriptManager.RegisterStartupScript(Page,Page.GetType(),"Message", "alert(Page Number should be less or equal to Total Page count);",true);

            }
            //}
            //TrackActivity("UpdateTrackTableOnDownload");

            //Logger.Trace("Downloading original document completed.", Session["LoggedUserId"].ToString());
            // Download file to physical path
            ////if (lblsignatureDetails.Text.Length < 2)
            ////{
            ////    // DownloadFile(DownloadFilepath);

            ////}
            ////else
            ////{

            ////    LoadImageToViewer(DDLDrop.SelectedValue.ToString());
            ////}

            //updating track table                

        }

        protected void btnprint_Click(object sender, EventArgs e)

        {
            Logger.Trace("Print Method", Session["LoggedUserId"].ToString());
            //  iFrameDocView.Attributes.Add("src", Session["pdfpath"].ToString());
           // Logger.Trace(Session["pdfpath"].ToString(), Session["LoggedUserId"].ToString());

            //  System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "Script", "PrintPdf('" + Session["pdfpath"].ToString() + "');", true);

            string src = string.Empty;
            src = getRootWebSitePathVal();
            src = src + "/GenericHandler.ashx?f=";
            src = src + Session["pdfpath"].ToString() + "#toolbar=1";

            src = src.Replace(@"\", "/");



            System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "displayalertmessage", "window.open('" + src + "','_newtab');", true);

            Logger.Trace(src, Session["LoggedUserId"].ToString());
            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "printPdf('" + desfolder + "');", true);
            ////try
            ////{
            ////    Logger.Trace("Try opening", Session["LoggedUserId"].ToString());
            ////    PdfDocument doc1 = new PdfDocument();
            ////   // doc1.LoadFromFile(Session["pdfpath"].ToString());

            ////    doc1.LoadFromFile(@"E:\Writer\TempWorkFolder\R1JfRklfODg3XzMwMF8xMDAwNg==\1.pdf");
            ////    Logger.Trace("Session", Session["LoggedUserId"].ToString());
            ////    // doc1.l
            ////    //   lblCurrentPath.Text = Session["pdfpath"].ToString();

            ////    //System.Windows.Forms.PrintDialog printDialog = new System.Windows.Forms.PrintDialog();

            ////    // printDialog.ShowDialog.

            ////    // printDialog.ShowDialog(owner);

            ////    //if (printDialog.ShowDialog(owner) == System.Windows.Forms.DialogResult.OK)

            ////    //{
            ////        try
            ////        {
            ////        Logger.Trace("calling doc1.Print", Session["LoggedUserId"].ToString());
            ////        Logger.Trace(doc1.PrintSettings.PrinterName.ToString(), Session["LoggedUserId"].ToString());

            ////        doc1.Print();
            ////            Logger.Trace("Printdialog Invoke", Session["LoggedUserId"].ToString());
            ////        }
            ////        catch (Exception ex)
            ////        {

            ////            Logger.Trace(ex.Message, Session["LoggedUserId"].ToString());
            ////            //System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "displayalertmessage", "alert('" + ex.Message + "');", true);
            ////        }

            ////    //}



            ////}
            ////catch (Exception ex)


            ////{
            ////    //lblCurrentPath.Text = ex.Message;
            ////    Logger.Trace(ex.Message + "hh", Session["LoggedUserId"].ToString());

            ////    System.Web.UI.ScriptManager.RegisterStartupScript(this, GetType(), "displayalertmessage", "alert('" + ex.Message + "');", true);

            ////}
            //string desfolder = "";
            //string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

            //if (!Directory.Exists(TempFolder))
            //{
            //    Directory.CreateDirectory(TempFolder);
            //}

            //// Merge splitted files into single file : Takes output file path and array of paths of input PDF files.
            //string folderpath = TempFolder + Encryptdata(lblRefID.Text);
            //if (folderpath.Trim().Length > 0)
            //{
            //    string[] splittedFilesPath = null;
            //    if (Request.QueryString["PageNo"] == "0")
            //    {
            //        splittedFilesPath = Directory.GetFiles(folderpath, "*.pdf");
            //    }
            //    else
            //    {
            //        splittedFilesPath = new string[DDLDrop.Items.Count];
            //        for (int i = 0; i < DDLDrop.Items.Count; i++)
            //        {
            //            splittedFilesPath[i] = folderpath + "\\" + DDLDrop.Items[i].Text + ".pdf";
            //        }
            //    }
            //    if (splittedFilesPath != null && splittedFilesPath.Length > 0)
            //    {
            //        desfolder = TempFolder + "\\" + Encryptdata(lblRefID.Text) + "1" + ".pdf";
            //        // Sort files by filename(numbers)
            //        NumericComparer ns = new NumericComparer(); // new object
            //        Array.Sort(splittedFilesPath, ns);

            //        PDFsharpmerging.MergeFiles(desfolder, splittedFilesPath);
            //    }
            //}

            //ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "printPdf('" + desfolder + "');", true);

        }
    }
}