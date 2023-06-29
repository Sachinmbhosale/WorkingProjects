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

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class DocumentQualityCheck : PageBase
    {
        #region DocumentDownloadDetails
        //General Declarations
        protected string AfterDot = "";
        public string pageRights = string.Empty;
        
        protected bool IsMaintag = false;
        public string TodayDate = DateTime.Today.ToString("yyyy-MM-dd");

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
                if (ViewState["ModifiedCount"] != null)
                {
                    return (int)ViewState["ModifiedCount"];
                }
                else
                    return 0;
            }
            set { ViewState["ModifiedCount"] = value; }
        }

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

        protected void Page_Load(object sender, EventArgs e)
        {
            IsMaintag = false;
            CheckAuthentication();
            btnInedxSave.Attributes.Add("onclick", "javascript:return validate()");
            // btnCommitChanges.Attributes.Add("onclick", "javascript:return validateUpdate()");
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            pageRights = GetPageRights();
            hdnPageRights.Value = pageRights;
            PDFViewer1.AnnotationToolbarVisible = pageRights.ToLower().Contains("annotate");

            if (!IsPostBack)
            {
                // Get watermark from querytring and store into session
                Session["Watermark"] = Request.QueryString["Watermark"] != null ? Request.QueryString["Watermark"].ToString() : string.Empty;
                ddlStatusRemarks.Enabled = false;
                LoadStatusDropdown();
                ApplyPageRights(pageRights, this.Form.Controls);
                hdnUploaded.Value = "FALSE";
                if (Request.QueryString["depid"] != null && Request.QueryString["docid"] != null && Request.QueryString["id"] != null)
                {
                    //To load Index fields
                    hdntempPagecount.Value = "1";
                    this.NumberOfControls = 0;
                    GetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value);
                    LoadMainTag();
                    InitializeDocumentView();
                    UnlockDocument();
                    //Get the annoatation for saving in datatable for delete purpose
                    GetAnnotateDocumentDetails();
                    if (Convert.ToInt32(Request.QueryString["MainTagId"]) != 0)
                    {
                        Logger.Trace("loading pages dropdown for tagged pages started", Session["LoggedUserId"].ToString());
                        IsMaintag = true;
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
                        IsMaintag = false;
                    }


                    //To load Index fields


                    string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];
                    UploadDocBL bl = new UploadDocBL();
                    SearchFilter filter = new SearchFilter();
                    string action = "Viewcount";
                    filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
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

                    if (NewPageNo > InsertPageNoPosition)
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

                if (dsFieldNames.Tables[0].Rows[i]["TemplateFields_vName"].ToString() == strControlName)
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
                Tab.BorderColor = "red";
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
                    ViewState["DocumentData"] = Ds.Tables[0];

                    SearchFilter objFilter = new SearchFilter();

                    objFilter.CurrOrgId = loginUser.LoginOrgId;
                    objFilter.CurrUserId = loginUser.UserId;
                    objFilter.DocumentTypeName = currentRow["DocName"].ToString();
                    objFilter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                    DataSet TemplateDs = new TemplateBL().GetTemplateDetails(objFilter);

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
                                objCnltxt.ID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                objCnltxt.CssClass = Getcssclass("txt");
                                TD2.Controls.Add(objCnltxt);
                                objCnltxt.Width = Unit.Pixel(157);
                                objCnltxt.Height = Unit.Pixel(16);
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
                                    calext.ID = "cal_" + TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
                                    calext.TargetControlID = TemplateDs.Tables[0].Rows[i]["TemplateFields_vName"].ToString();
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
                                objCnl.Height = Unit.Pixel(24);
                                objCnl.Width = Unit.Pixel(165);
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
                            ddlBind.SelectedValue = "0";
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                Logger.Trace("Exception : " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

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
                    LoadRemarksDropdowns(ddlStatusRemarks, Rs.ResultDS.Tables[0]);
                    ddlStatusRemarks.Enabled = true;
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
                Filter.TotalPages = Convert.ToInt32(hdnPagesCount.Value);
                Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                pnlControls.Enabled = true;
            }
            else if (ddldocumentview.SelectedValue == "Tagged View")
            {
                action = "GetPagesTaggedView";
                if (IsMaintag == true)
                {
                    Filter.MainTagID = Convert.ToInt32(Request.QueryString["MainTagId"]);

                    Filter.SubTagID = Convert.ToInt32(Request.QueryString["SubTagId"]);


                }
                else
                {
                    Filter.MainTagID = Convert.ToInt32(cmbMainTag.SelectedValue);
                    Filter.SubTagID = Convert.ToInt32(cmbSubTag.SelectedValue);
                }
                Filter.TotalPages = Convert.ToInt32(hdnPagesCount.Value);
                Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                pnlControls.Enabled = false;

            }
            else if (ddldocumentview.SelectedValue == "NonTagged View")
            {


                action = "GetPagesNonTaggedView";
                Filter.TotalPages = Convert.ToInt32(hdnPagesCount.Value);
                Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                pnlControls.Enabled = false;
            }
            else if (ddldocumentview.SelectedValue == "FullText View")
            {
                action = "GetPagesFullText";
                if (hdnPagesCount.Value != null && Request.QueryString["id"] != null && Session["FullTextClause"] != null)
                {
                    if (hdnPagesCount.Value != string.Empty)
                    {
                        Filter.TotalPages = Convert.ToInt32(hdnPagesCount.Value);
                    }

                    Filter.CurrentDocumentId = Convert.ToInt32(Request.QueryString["id"]);
                    Filter.WhereClause = Session["FullTextClause"].ToString();
                    pnlControls.Enabled = false;
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
            //else
            //{
            Logger.Trace("getAvailablePages Finished", Session["LoggedUserId"].ToString());
            //    ddldocumentview.SelectedValue = "Original View";

            //}
            return dt;
        }

        //To load PDF in Iframe
        public void InitializeDocumentView()
        {
            // getAvailablePages();
            string result = "Success";
            string TagEnabled = "False";
            string src = string.Empty;
            Logger.Trace("loadfileiniframe started", Session["LoggedUserId"].ToString());
            string TempFolder = ConfigurationManager.AppSettings["TempWorkFolder"];

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
                    Logger.Trace("loadfileiniframe Exception:" + ex.Message, Session["LoggedUserId"].ToString());
                }

                if (Directory.Exists(destinationFile.ToLower()))
                {
                    DirectoryInfo di = new DirectoryInfo(destinationFile);
                    int count = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;
                    hdnPagesCount.Value = count.ToString();
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
                        }

                        drpPageCount.Items.Clear();
                        drpPageCount.Items.Add(new ListItem("Before", "1"));
                        for (int i = 2; i < count; i++)
                        {
                            drpPageCount.Items.Add(new ListItem(i.ToString(), i.ToString()));
                        }
                        drpPageCount.Items.Add(new ListItem("After", count.ToString()));

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
            Logger.Trace("loadfileiniframe finished", Session["LoggedUserId"].ToString());
        }

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

        private void Searchagain()
        {
            string SearchCriteria = string.Empty;
            if (Request.QueryString["Page"] == "CheckerDashBoard")
            {
                SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
                Response.Redirect("CheckerDashboard.aspx?Search=" + SearchCriteria);
            }
            else
            {
                SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
                Response.Redirect("MakerDashboard.aspx?Search=" + SearchCriteria);
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Searchagain();
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
                                PDFMerging.MergeFiles(Path_MergedFile, source, Position, "add");

                                // Extract merged file again
                                Logger.Trace("Splitting pdf file.", Session["LoggedUserId"].ToString());
                                count = new Image2Pdf().ExtractPages(Path_MergedFile, TempFolder + "\\" + Encryptdata(lblRefID.Text));
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
                            }

                            // Set session variable values
                            Session["TotalPageCount"] = count;
                            Session["Ext"] = UploadedFileExt;
                            Session["Encryptdata"] = Encryptdata(lblRefID.Text) + UploadedFileExt;
                            Session["TempFileLocation"] = Path_MergedFile;

                            int InsertedPagesCount = count - splittedfilescount;
                            UpdateAnnotatedPageAfterInsert(Position, InsertedPagesCount);

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
            Logger.Trace("btnReloadiFrame_Click Started", Session["LoggedUserId"].ToString());
            ReloadIframe();
            LoadImageToViewer(string.Empty);
            Logger.Trace("btnReloadiFrame_Click finished", Session["LoggedUserId"].ToString());
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
                    drpPageCount.Items.Add(new ListItem("Before", "1"));
                    for (int i = 2; i < count; i++)
                    {
                        drpPageCount.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    }
                    drpPageCount.Items.Add(new ListItem("After", count.ToString()));
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
                Session["TempFileLocation"] = Path_MergedFile;
                Session["TotalPageCount"] = PagesCount;
                Session["Ext"] = FileExtension;
            }
            else
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('No page is available to delete')", true);
            }
            Logger.Trace("deletepages Finished", Session["LoggedUserId"].ToString());
        }

        public void SentMail()
        {
            DataTable data = new DataTable();
            StreamReader reader = null;
            string strMailmessage = string.Empty;
            string strFilepath = string.Empty;
            string strWebpath = string.Empty;
            try
            {
                Logger.Trace("SentMail started", Session["LoggedUserId"].ToString());

                UserBase loginUser = (UserBase)Session["LoggedUser"];


                reader = new StreamReader(Server.MapPath("~/Accounts/Checker.htm"));
                string readFile = reader.ReadToEnd();
                data = (DataTable)ViewState["DocumentData"];
                if (data != null && data.Rows.Count > 0)
                {
                    strMailmessage = readFile;
                    strMailmessage = strMailmessage.Replace("$$Project$$", data.Rows[0]["DocName"].ToString());
                    strMailmessage = strMailmessage.Replace("$$Department$$", data.Rows[0]["DeptName"].ToString());
                    strMailmessage = strMailmessage.Replace("$$Document Name$$", data.Rows[0]["OrgiFileName"].ToString());
                    strMailmessage = strMailmessage.Replace("$$Status$$", ddlDocStatus.SelectedItem.ToString());
                    strMailmessage = strMailmessage.Replace("$$RemarkStatus$$", ddlStatusRemarks.SelectedItem.ToString());
                    strMailmessage = strMailmessage.Replace("$$Remarks$$", txtoldremarks.Text + " " + txtRemarks.Text);
                    strMailmessage = strMailmessage.Replace("$$OrgName$$", "Writer Corporation");
                    strMailmessage = strMailmessage.Replace("$$Name$$", data.Rows[0]["UserFullName"].ToString().Trim());

                    MailHelper.SendMailMessage(loginUser.EmailId, data.Rows[0]["UserMailId"].ToString().Trim(), "", "", ddlDocStatus.SelectedItem.ToString(), strMailmessage);


                }
                Logger.Trace("SentMail finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("SentMail Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        #region commit changes
        protected void btnCommitChanges_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnCommitChanges_Click started", Session["LoggedUserId"].ToString());

            hdnUploaded.Value = "TRUE";

            int DocumentId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
            Session["hdnOrgFileLocation"] = hdnFileLocation.Value;
            // Maintain verion if document is modified
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
                    // Send mail to maker on document reject
                    if (ddlDocStatus.SelectedItem.ToString() == "Rejected")
                        SentMail();

                    // Delete temporary file on success save.
                    DeleteTempFiles();
                    Session["TempFileLocation"] = "";
                    Session["EncyptImgName"] = "";
                    Session["Ext"] = "";
                    ModifiedCount = 0;
                    Searchagain();
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
                string ActualFolder = ServerPath + Session["LoginOrgCode"].ToString() + "\\" + Session["LoggedUserId"].ToString() + "\\" + TodayDate + "\\";
                hdnUploaded.Value = "TRUE";

                if (!Directory.Exists(ActualFolder))
                {
                    Directory.CreateDirectory(ActualFolder);
                }

                if (Session["TempFileLocation"] != null && Session["TempFileLocation"].ToString() != string.Empty && Session["hdnOrgFileLocation"] != null && Session["hdnOrgFileLocation"].ToString() != string.Empty)
                {
                    Logger.Trace("Renaming document name to original file name", Session["LoggedUserId"].ToString());
                    //Renaming document name to original file name
                    FilePath = Session["TempFileLocation"].ToString();
                    FilepathToRename = FilePath.Substring(0, FilePath.LastIndexOf("\\")) + "\\" + Session["Encryptdata"].ToString();
                    Session["TempFileLocation"] = FilepathToRename;

                    // Rename file to actual filename (temp folder).
                    if (File.Exists(FilePath))
                    {
                        File.Copy(FilePath, FilepathToRename);//copy file to original file name in tempfolder
                    }
                    Logger.Trace("Zipping documents before moving documents from tempfolder to original location", Session["LoggedUserId"].ToString());
                    /*---Zipping documents before moving documents from tempfolder to original location---*/
                    Actual_FileName_TempFolder = Session["TempFileLocation"].ToString();
                    Zip_FileName_TempFolder = beforedot(Session["TempFileLocation"].ToString()) + ".zip";
                    Zip_FileName_ActaulFolder = ActualFolder + ".zip";
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
                    if (Directory.Exists(ActualFolder))
                    {
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
                DataSet dsPageNoMappings = new DataSet();
                int intVersion = 1;
                int intPageCount = hdnPagesCount.Value.Trim() == string.Empty ? 0 : Convert.ToInt32(hdnPagesCount.Value);
                int iProcId = Request.QueryString["id"] != null ? Convert.ToInt32(Request.QueryString["id"]) : 0;
                UserBase loginUser = (UserBase)Session["LoggedUser"];

                intVersion = Convert.ToInt32(ViewState["Version"]) + 1;
                if (iProcId > 0)
                {
                    string Remarks = ddlStatusRemarks.SelectedValue.ToString() == "--Select--" ? "0" : ddlStatusRemarks.SelectedValue.ToString();

                    if (Convert.ToInt32(ddlDocStatus.SelectedValue) == 0)
                    {
                        strValues = "UPLOAD_iModifiedBy=" + loginUser.UserId.ToString() + ","
                      + "UPLOAD_iStatusRemarks_ID=" + Remarks + ","
                      + "UPLOAD_vStatusCommemts=&apos;" + txtoldremarks.Text + (txtoldremarks.Text.Trim().Length > 0 ? Environment.NewLine + Environment.NewLine : string.Empty) + loginUser.UserName.ToString() + ":" + txtRemarks.Text + "&apos;,"
                      + "UPLOAD_vSearchKeyWords=&apos;" + txtKeyword.Text + "&apos;,"
                      + "UPLOAD_iPageCount=" + intPageCount + ","
                      + "UPLOAD_iOrgID=" + loginUser.LoginOrgId.ToString() + ","
                      + "UPLOAD_bIsUploaded=1,";
                    }
                    else
                    {
                        strValues = "UPLOAD_iModifiedBy=" + loginUser.UserId.ToString() + ","
                                              + "UPLOAD_iCurrentStatus_ID=" + ddlDocStatus.SelectedValue.ToString() + ","
                                              + "UPLOAD_iStatusRemarks_ID=" + Remarks + ","
                                              + "UPLOAD_vStatusCommemts=&apos;" + txtoldremarks.Text + (txtoldremarks.Text.Trim().Length > 0 ? Environment.NewLine + Environment.NewLine : string.Empty) + loginUser.UserName.ToString() + ":" + txtRemarks.Text + "&apos;,"
                                              + "UPLOAD_vSearchKeyWords=&apos;" + txtKeyword.Text + "&apos;,"
                                              + "UPLOAD_iPageCount=" + intPageCount + ","
                                              + "UPLOAD_iOrgID=" + loginUser.LoginOrgId.ToString() + ","
                                              + "UPLOAD_bIsUploaded=1,";
                    }


                    strQuery = "update upload set " + strValues.Substring(0, strValues.Length - 1) + " where UPLOAD_iProcessID = " + Convert.ToString(Request.QueryString["id"]);
                    xml = "<Table><row><query>" + strQuery + "</query></row></Table>";

                    //  Get annotation page mapping xml      
                    DataTable dtAnnotate = (DataTable)Session["AnnotationPageNo"];
                    dsPageNoMappings.Tables.Add(dtAnnotate.Copy());
                    xmlPageNoMappings = dsPageNoMappings.GetXml();
                    Uploadaction = "UpdateDocumentDetails";

                    //Updating upload table and annotation table 
                    result = objDocumentBL.ManageUploadedDocuments(xml, hdnLoginOrgId.Value, hdnLoginToken.Value, Uploadaction, iProcId,false, xmlPageNoMappings);

                    Logger.Trace("Save button click after saving or updating the result= " + result.Message, Session["LoggedUserId"].ToString());
                }
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
            return result;
        }
        #endregion

        /// <summary>
        /// Delete temporary files of current user session.
        /// </summary>
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

                // Delete splitted files directory
                Logger.Trace("Deleting splitted files directory.", Session["LoggedUserId"].ToString());
                Directory.Delete(TempFolder + Encryptdata(lblRefID.Text), true);

                //Delete JPG file directory
                Logger.Trace("Deleting splitted JPG files directory.", Session["LoggedUserId"].ToString());
                Directory.Delete(TempFolder + Encryptdata(lblRefID.Text) + "JPG", true);


            }
            catch (Exception ex)
            {
                Logger.Trace("Exception: " + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

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
                        Directory.Move(beforedot(hdnFileLocation.Value), ArchiveFolder + beforedot(hdnEncrpytFileName.Value));

                        Response.Redirect("DocumentDownload.aspx?action=Delete");
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
            Response.Redirect("~/Secure/Core/DocumentQualityCheck.aspx?id=" + id + "&docid=" + docid + "&depid=" + depid + "&active=" + active + "&PageNo" + PageNo + "&Search=" + SearchCriteria);
            Logger.Trace("btnDicardChanges_Click finished", Session["LoggedUserId"].ToString());
        }

        protected void btnDeletePages_Click(object sender, EventArgs e)
        {
            // ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "ScriptRegistration", "Initialisationdis();", true);
            Logger.Trace("btnDeletePages_Click started", Session["LoggedUserId"].ToString());
            DeletePage();
            //deletepages();
            ReloadIframe();
            Logger.Trace("btnDeletePages_Click finished", Session["LoggedUserId"].ToString());
        }

        protected void LoadMainTag()
        {
            if (afterdot(hdnFileName.Value).ToString().ToLower() == ".pdf" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tif" || afterdot(hdnFileName.Value).ToString().ToLower() == ".tiff")
            {
                DataSet ds = new DataSet();
                TemplateBL bl = new TemplateBL();
                SearchFilter filter = new SearchFilter();
                cmbMainTag.Attributes.Add("onchange", "javascript:return LoadSubTag(this);");
                filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                filter.DocumentTypeID = Convert.ToInt32(Request.QueryString["docid"]);
                ds = bl.GetTagDetails(filter, "MainTag", hdnLoginOrgId.Value, hdnLoginToken.Value);
                cmbMainTag.Enabled = true;
                cmbSubTag.Enabled = true;
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    cmbMainTag.Items.Clear();
                    cmbMainTag.DataSource = ds.Tables[0];
                    cmbMainTag.DataTextField = "TextField";
                    cmbMainTag.DataValueField = "ValueField";
                    cmbMainTag.DataBind();

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
        }

        protected void btnCommonSubmitSub2_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            TemplateBL bl = new TemplateBL();
            SearchFilter filter = new SearchFilter();
            filter.DocumentTypeID = Convert.ToInt32(cmbMainTag.SelectedValue);
            ds = bl.GetTagDetails(filter, "SubTag", hdnLoginOrgId.Value, hdnLoginToken.Value);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                cmbSubTag.Items.Clear();
                cmbSubTag.DataSource = ds.Tables[0];
                cmbSubTag.DataTextField = "TextField";
                cmbSubTag.DataValueField = "ValueField";
                cmbSubTag.DataBind();

            }
            else
            {
                cmbSubTag.Items.Clear();
            }

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
        }

        protected void btnprint_Click(object sender, EventArgs e)
        {
            string desfolder = "";
            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

            if (!Directory.Exists(TempFolder))
            {
                Directory.CreateDirectory(TempFolder);
            }

            // Merge splitted files into single file : Takes output file path and array of paths of input PDF files.
            string folderpath = TempFolder + Encryptdata(lblRefID.Text);
            if (folderpath.Trim().Length > 0)
            {
                string[] splittedFilesPath = null;
                if (Request.QueryString["PageNo"] == "0")
                {
                    splittedFilesPath = Directory.GetFiles(folderpath, "*.pdf");
                }
                else
                {
                    splittedFilesPath = new string[DDLDrop.Items.Count];
                    for (int i = 0; i < DDLDrop.Items.Count; i++)
                    {
                        splittedFilesPath[i] = folderpath + "\\" + DDLDrop.Items[i].Text + ".pdf";
                    }
                }
                if (splittedFilesPath != null && splittedFilesPath.Length > 0)
                {
                    desfolder = TempFolder + "\\" + Encryptdata(lblRefID.Text) + "print" + ".pdf";
                    // Sort files by filename(numbers)
                    NumericComparer ns = new NumericComparer(); // new object
                    Array.Sort(splittedFilesPath, ns);

                    PDFsharpmerging.MergeFiles(desfolder, splittedFilesPath);
                }
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "printPdf();", true);

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
            UnlockDocument();
        }

        protected void UnlockDocument()
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
                IsMaintag = true;
                getAvailablePages();
            }
            LoadImageToViewer(string.Empty);
            //  loadfileiniframe();
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

        protected void btnCallFromJavascript_Click(object sender, EventArgs e)
        {
            if (hdnAction.Value.ToString().ToUpper() == "SAVEANNOTATIONS")
            {
                SaveAnnotations();
                //Get the annoatation for saving in datatable for delete purpose
                GetAnnotateDocumentDetails();
            }
            else
                LoadImageToViewer(hdnAction.Value);
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
                        break;
                    case "PREVIOUS":
                        DDLDrop.SelectedIndex = DDLDrop.SelectedIndex - 1;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = tempagecount - 1;
                        break;
                    case "GOTO":
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = DDLDrop.SelectedIndex + 1;
                        break;
                    case "FIRST":
                        DDLDrop.SelectedIndex = 0;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = 1;
                        break;
                    case "LAST":
                        DDLDrop.SelectedIndex = DDLDrop.Items.Count - 1;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = DDLDrop.Items.Count;
                        break;
                    default:
                        DDLDrop.SelectedIndex = 0;
                        PageNo = Convert.ToInt32(DDLDrop.SelectedValue);
                        tempagecount = 1;
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
                string ImageName = beforedot(Session["EncyptImgName"].ToString());

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

        #endregion

        protected void btnInedxSave_Click(object sender, EventArgs e)
        {
            SaveIndexPannelDetails();
        }

        protected void defaultview()
        {
            ddldocumentview.SelectedValue = "Original View";
            getAvailablePages();

            LoadImageToViewer(string.Empty);
        }

        protected void btnddlchanged_Click(object sender, EventArgs e)
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
    }
}