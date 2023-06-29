using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Lotex.EnterpriseSolutions.WebUI.Secure.Core
{
    public partial class DocumentTag : PageBase
    {
        //protected List<string> restwo;
        DataSet restwo = new DataSet();
        DataTable TotalTagPages = null;
        int pageno = 0;
        int TagPages = 0;
        DocTagBL BL = new DocTagBL();
        Results Results = new Results();
        string FinalResult = string.Empty;
        string ReturnMsg = string.Empty;

        protected int NumberOfControls
        {
            get { return (int)ViewState["NumControls"]; }
            set { ViewState["NumControls"] = value; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
            hdnLoginToken.Value = loginUser.LoginToken;
            hdnPageId.Value = "0";
            if (!IsPostBack)
            {
                lblmsg.Text = string.Empty;
                if (Request.QueryString["depid"] != null && Request.QueryString["docid"] != null && Request.QueryString["id"] != null)
                {
                    //To load Index fields
                    this.NumberOfControls = 0;
                    GetMainTag(hdnLoginOrgId.Value, hdnLoginToken.Value);
                    GetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value);
                    if (Request.QueryString["PageNo"] != null)
                    {
                        hdnPageNo.Value = Request.QueryString["PageNo"].ToString();
                    }
                }
                DDLDrop.Attributes.Add("onChange", "javascript:return gotoPage();");
                DDLTagpagecount.Attributes.Add("onChange", "javascript:return gotoTagPage();");
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Script", "gotoTagPage();", true);

                if (Request.QueryString["id"] != "")
                {
                    TotalTagPages = null;

                    lbltotalnoofPages.Text = "";
                    lbluntagpage.Text = "";
                    lbltotaltagepage.Text = "";

                    int TagePagescount = 0;



                    TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), pageno, Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));

                    DDLTagpagecount.Items.Clear();
                    TagePagescount = TotalTagPages.Rows.Count;
                    if (TotalTagPages.Rows.Count > 0)
                    {
                        for (int k = 0; k < TotalTagPages.Rows.Count; k++)
                        {
                            DDLTagpagecount.Items.Add(TotalTagPages.Rows[k][0].ToString());
                            DDLDrop.Items.Remove(TotalTagPages.Rows[k][0].ToString());
                        }

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
                            lbluntagpage.Text = "0" + " Pages";
                            if (lbluntagpage.Text.Contains("-"))
                            {
                                lbluntagpage.Text = "0" + " Pages";
                            }
                            //cmbMainTag.Enabled = false;
                            //cmbSubTag.Enabled = false;
                        }

                        if (lbluntagpage.Text == "0 Pages")
                        {
                            //cmbMainTag.Enabled = false;
                            //cmbSubTag.Enabled = false;
                        }
                        lbltotaltagepage.Text = Convert.ToString(TagePagescount + " Pages");

                    }
                }

            }
            else
            {

                PostBackGetTemplateDetails(hdnLoginOrgId.Value, hdnLoginToken.Value);

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "gotoPage();", true);
                //"loadpdf();", true);

            }

        }


        protected void cmbMainTag_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cmbMainTag.SelectedIndex > 0)
                {

                    lblmsg.Text = string.Empty;
                    GetSubTag(hdnLoginOrgId.Value, hdnLoginToken.Value);
                }
                else
                    cmbSubTag.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
            }
        }
        protected void cmbSubTag_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                lblmsg.Text = string.Empty;
                if (cmbSubTag.SelectedIndex > 0)
                {
                    List<string> SearchData = new List<string>();

                    SearchData.Add(Request.QueryString["id"]);
                    SearchData.Add(Request.QueryString["docid"]);
                    SearchData.Add(Request.QueryString["depid"]);
                    SearchData.Add(cmbMainTag.SelectedValue.Trim());
                    SearchData.Add(cmbSubTag.SelectedValue.Trim());
                    SearchData.Add(txtPages.Text.Trim());

                    string Pagecount = BL.GetTagPages(SearchData);
                    if (Pagecount != "" && Pagecount != string.Empty)
                    {
                        txtPages.Text = Pagecount;
                    }
                    else
                    {
                        txtPages.Text = string.Empty;
                    }

                }
                //DDLTagpagecount.Items.Clear();
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
            }
        }
        public void GetMainTag(string loginOrgId, string loginToken, string LoadMain = "true")
        {
            if (Request.QueryString["depid"] != null && Request.QueryString["docid"] != null)
            {

                DataSet ds = new DataSet();
                TemplateBL bl = new TemplateBL();
                SearchFilter filter = new SearchFilter();

                try
                {

                    if (LoadMain == "true")
                    {

                        filter.DocumentTypeID = Convert.ToInt32(Request.QueryString["docid"]);
                        filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                        ds = bl.GetTagDetails(filter, "MainTag", loginOrgId, loginToken);
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            cmbMainTag.Items.Clear();
                            cmbMainTag.DataSource = ds.Tables[0];
                            cmbMainTag.DataTextField = "TextField";
                            cmbMainTag.DataValueField = "ValueField";
                            cmbMainTag.DataBind();
                            cmbMainTag.SelectedValue = Request.QueryString["MainTagId"];
                            cmbMainTag_SelectedIndexChanged(null, null);
                            cmbSubTag.SelectedValue = Request.QueryString["SubTagId"];
                            cmbSubTag_SelectedIndexChanged(null, null);
                        }
                        else
                        {
                            cmbMainTag.Items.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.TraceErrorLog(ex.ToString());
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);

                }
            }

        }

        public void GetMainTageonTag(string loginOrgId, string loginToken, string LoadMain = "true")
        {

            DataSet ds1 = new DataSet();
            TemplateBL bl = new TemplateBL();
            SearchFilter filter = new SearchFilter();

            if (LoadMain == "true")
            {

                filter.DocumentTypeID = Convert.ToInt32(Request.QueryString["docid"]);
                filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                ds1 = bl.GetTagDetails(filter, "MainTag", loginOrgId, loginToken);
                if (ds1 != null && ds1.Tables.Count > 0 && ds1.Tables[0].Rows.Count > 0)
                {
                    cmbMainTag.Items.Clear();
                    cmbMainTag.DataSource = ds1.Tables[0];
                    cmbMainTag.DataTextField = "TextField";
                    cmbMainTag.DataValueField = "ValueField";
                    cmbMainTag.DataBind();
                    cmbMainTag.SelectedValue = Request.QueryString["MainTagId"];
                    cmbMainTag_SelectedIndexChanged(null, null);
                    cmbSubTag.SelectedValue = Request.QueryString["SubTagId"];
                    cmbSubTag_SelectedIndexChanged(null, null);
                }
                else
                {
                    cmbMainTag.Items.Clear();
                }
            }
        }

        public void GetSubTag(string loginOrgId, string loginToken, string LoadMain = "true")
        {
            if (Request.QueryString["depid"] != null && Request.QueryString["docid"] != null)
            {

                DataSet ds = new DataSet();
                TemplateBL bl = new TemplateBL();
                SearchFilter filter = new SearchFilter();

                try
                {

                    if (LoadMain == "true")
                    {

                        filter.DocumentTypeID = Convert.ToInt32(cmbMainTag.SelectedValue);
                        ds = bl.GetTagDetails(filter, "SubTag", loginOrgId, loginToken);
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
                    }
                }
                catch (Exception ex)
                {
                    Logger.TraceErrorLog(ex.ToString());
                    Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
                }
            }

        }

        public void PostBackGetTemplateDetails(string loginOrgId, string loginToken)
        {
            Logger.Trace("GetTemplateDetails Started", "TraceStatus");

            bool IActivive;
            string indexname = string.Empty;
            string sortOreder = string.Empty;
            string orginalorder = string.Empty;
            HtmlTable tb = new HtmlTable();
            Label lbl;
            Label lbl1;
            Label lbl2;
            Label lbl3;
            Label lbl4;
            HtmlTableRow tr;
            HtmlTableCell td;
            HtmlTableCell td1;
            HtmlTableCell td2;
            TemplateBL bl = new TemplateBL();
            UploadDocBL b2 = new UploadDocBL();
            SearchFilter filter = new SearchFilter();

            try
            {

                string action = "GetTemplateDetailsForDownload";
                string subtype = string.Empty;
                filter.DocumentTypeID = Convert.ToInt32(Request.QueryString["docid"]);
                filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                Results res = bl.GetTemplateDetails(filter, action, loginOrgId, loginToken);

                //To get uploaded document details using ProcessID
                action = "GetUploadDocumentDetails";
                filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                restwo = b2.GetUploadDocumentDetails(filter, action, loginOrgId, loginToken);
                lblDocType.Text = restwo.Tables[0].Rows[0][139].ToString(); //restwo[1];
                lblDept.Text = restwo.Tables[0].Rows[0][140].ToString();
                hdnFileName.Value = restwo.Tables[0].Rows[0][3].ToString();
                hdnFileLocation.Value = restwo.Tables[0].Rows[0][6].ToString();
                hdnEncrpytFileName.Value = restwo.Tables[0].Rows[0][4].ToString();
                Session["hdnOrgFileLocation"] = restwo.Tables[0].Rows[0][6].ToString();

                Session["Count"] = "0";
                if (res.ActionStatus == "SUCCESS" && res.IndexFields != null)
                {
                    int i = 18;
                    foreach (IndexField tf in res.IndexFields)
                    {
                        lbl = new Label();
                        lbl1 = new Label();
                        lbl2 = new Label();
                        lbl3 = new Label();
                        lbl4 = new Label();
                        td = new HtmlTableCell();
                        td1 = new HtmlTableCell();
                        td2 = new HtmlTableCell();
                        tr = new HtmlTableRow();


                        lbl.ID = "lbl_" + NumberOfControls.ToString();
                        lbl.Text = tf.IndexName + ":";
                        indexname = tf.IndexName;
                        indexname = indexname.Replace(" ", "");
                        IActivive = Convert.ToBoolean(tf.ActiveIndex);
                        sortOreder = tf.SortOrder.ToString();
                        lbl.CssClass = Getcssclass(IActivive, "lbl");
                        td.Controls.Add(lbl);
                        if (tf.EntryType == "Multiple Field Selection")
                        {
                            filter.CurrTemplateId = tf.Indexidentity;
                            restwo = b2.GetUploadDocumentDetails(filter, action, loginOrgId, loginToken);

                            lbl2.ID = indexname;
                            lbl2.Text = "  " + restwo.Tables[0].Rows[0][i].ToString(); //restwo[i];
                            lbl2.CssClass = Getcssclass(IActivive, "lbl");
                            td1.Controls.Add(lbl2);

                            lbl3.ID = "lbl_0" + NumberOfControls.ToString();
                            lbl3.Text = "  " + restwo.Tables[0].Rows[0][i + 1].ToString(); //restwo[i + 1];
                            lbl3.CssClass = Getcssclass(IActivive, "lbl");
                            td2.Controls.Add(lbl3);
                            i++;
                        }
                        else
                        {
                            lbl1.ID = indexname;
                            lbl1.Text = "  " + restwo.Tables[0].Rows[0][i].ToString(); //restwo[i];
                            lbl1.CssClass = Getcssclass(IActivive, "lbl");
                            td1.Controls.Add(lbl1);
                        }
                        tr.ID = sortOreder;
                        tr.Cells.Add(td);
                        tr.Cells.Add(td1);
                        tr.Cells.Add(td2);
                        tb.Rows.Add(tr);
                        tb.Attributes.Add("CssClass", "tabledesign");
                        this.NumberOfControls++;
                        i++;
                    }
                    hdnCountControls.Value = NumberOfControls.ToString();
                    HtmlTable tempTab = new HtmlTable();
                    HtmlTableRow[] tra = new HtmlTableRow[tb.Rows.Count];
                    for (int k = 0; k < tb.Rows.Count; k++)
                    {
                        HtmlTableRow dr = tb.Rows[k];
                        tra[Convert.ToInt32(dr.ID) - 1] = dr;

                    }
                    for (int r = 0; r < tra.Length; r++)
                    {
                        tempTab.Rows.Add(tra[r]);

                    }

                    pnlIndexpro.Controls.Add(tempTab);

                    //string filePath = hdnFileLocation.Value;
                    //string src = string.Empty;
                    //src = GetSrc("Handler") + filePath + "#toolbar=1";
                    //frame1.Attributes.Add("src", src);


                    //To load PDF in iframe
                    //loadfileiniframe();
                }
                Logger.Trace("GetTemplateDetails Finished", "TraceStatus");

            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
                Logger.TraceErrorLog("function GetTemplateDetails caught ex;" + ex.Message.ToString());
            }
        }

        public void GetTemplateDetails(string loginOrgId, string loginToken)
        {
            Logger.Trace("GetTemplateDetails Started", "TraceStatus");

            bool IActivive;
            string indexname = string.Empty;
            string sortOreder = string.Empty;
            string orginalorder = string.Empty;
            HtmlTable tb = new HtmlTable();
            Label lbl;
            Label lbl1;
            Label lbl2;
            Label lbl3;
            Label lbl4;
            HtmlTableRow tr;
            HtmlTableCell td;
            HtmlTableCell td1;
            HtmlTableCell td2;
            TemplateBL bl = new TemplateBL();
            UploadDocBL b2 = new UploadDocBL();
            SearchFilter filter = new SearchFilter();

            try
            {

                string action = "GetTemplateDetailsForDownload";
                string subtype = string.Empty;
                filter.DocumentTypeID = Convert.ToInt32(Request.QueryString["docid"]);
                filter.DepartmentID = Convert.ToInt32(Request.QueryString["depid"]);
                Results res = bl.GetTemplateDetails(filter, action, loginOrgId, loginToken);

                //To get uploaded document details using ProcessID
                action = "GetUploadDocumentDetails";
                filter.UploadDocID = Convert.ToInt32(Request.QueryString["id"]);
                restwo = b2.GetUploadDocumentDetails(filter, action, loginOrgId, loginToken);

                lblDocType.Text = restwo.Tables[0].Rows[0][139].ToString(); //restwo[1];
                lblDept.Text = restwo.Tables[0].Rows[0][140].ToString();
                hdnFileName.Value = restwo.Tables[0].Rows[0][3].ToString();
                hdnFileLocation.Value = restwo.Tables[0].Rows[0][6].ToString();
                hdnEncrpytFileName.Value = restwo.Tables[0].Rows[0][4].ToString();
                Session["hdnOrgFileLocation"] = restwo.Tables[0].Rows[0][6].ToString();

                Session["Count"] = "0";
                if (res.ActionStatus == "SUCCESS" && res.IndexFields != null)
                {
                    int i = 17;
                    foreach (IndexField tf in res.IndexFields)
                    {
                        lbl = new Label();
                        lbl1 = new Label();
                        lbl2 = new Label();
                        lbl3 = new Label();
                        lbl4 = new Label();
                        td = new HtmlTableCell();
                        td1 = new HtmlTableCell();
                        td2 = new HtmlTableCell();
                        tr = new HtmlTableRow();
                        tf.ActiveIndex = "True";

                        lbl.ID = "lbl_" + NumberOfControls.ToString();
                        lbl.Text = tf.IndexName + ":";
                        indexname = tf.IndexName;
                        indexname = indexname.Replace(" ", "");
                        IActivive = Convert.ToBoolean(tf.ActiveIndex);
                        //sortOreder = tf.SortOrder.ToString();
                        sortOreder = (i - 16).ToString();
                        lbl.CssClass = Getcssclass(IActivive, "lbl");
                        td.Controls.Add(lbl);
                        if (tf.EntryType == "Multiple Field Selection")
                        {
                            filter.CurrTemplateId = tf.Indexidentity;
                            restwo = b2.GetUploadDocumentDetails(filter, action, loginOrgId, loginToken);

                            lbl2.ID = indexname;
                            lbl2.Text = "  " + restwo.Tables[0].Rows[0][i].ToString(); //restwo[i];
                            lbl2.CssClass = Getcssclass(IActivive, "lbl");
                            td1.Controls.Add(lbl2);

                            //lbl3.ID = "lbl_0" + NumberOfControls.ToString();
                            //lbl3.Text = "  " + restwo.Tables[0].Rows[0][i + 1].ToString(); //restwo[i + 1];
                            //lbl3.CssClass = Getcssclass(IActivive, "lbl");
                            //td2.Controls.Add(lbl3);
                            //i++;
                        }
                        else
                        {
                            lbl1.ID = indexname;
                            lbl1.Text = "  " + restwo.Tables[0].Rows[0][i].ToString(); //restwo[i];
                            lbl1.CssClass = Getcssclass(IActivive, "lbl");
                            td1.Controls.Add(lbl1);
                        }
                        tr.ID = sortOreder;
                        tr.Cells.Add(td);
                        tr.Cells.Add(td1);
                        tr.Cells.Add(td2);
                        tb.Rows.Add(tr);
                        tb.Attributes.Add("CssClass", "tabledesign");
                        this.NumberOfControls++;
                        i++;
                    }
                    hdnCountControls.Value = NumberOfControls.ToString();
                    HtmlTable tempTab = new HtmlTable();
                    HtmlTableRow[] tra = new HtmlTableRow[tb.Rows.Count];
                    for (int k = 0; k < tb.Rows.Count; k++)
                    {
                        HtmlTableRow dr = tb.Rows[k];
                        tra[Convert.ToInt32(dr.ID) - 1] = dr;

                    }
                    for (int r = 0; r < tra.Length; r++)
                    {
                        tempTab.Rows.Add(tra[r]);

                    }

                    pnlIndexpro.Controls.Add(tempTab);

                    //string filePath = hdnFileLocation.Value;
                    //string src = string.Empty;
                    //src = GetSrc("Handler") + filePath + "#toolbar=1";
                    //frame1.Attributes.Add("src", src);


                    //To load PDF in iframe
                    loadfileiniframe();
                }
                Logger.Trace("GetTemplateDetails Finished", "TraceStatus");

            }
            catch (Exception ex)
            {
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
                Logger.TraceErrorLog("function GetTemplateDetails caught ex;" + ex.Message.ToString());
            }
        }

        public string Getcssclass(bool active, string cntrl)
        {
            string csclass = string.Empty;
            if (cntrl == "drp")
            {
                if (active == true)
                {
                    csclass = "ComboStyle";
                }
                else
                {
                    csclass = "ComboStylehidden";
                }
            }
            else if (cntrl == "txt")
            {
                if (active == true)
                {
                    csclass = "TextBoxStyle";
                }
                else
                {
                    csclass = "TextBoxStylehidden";
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

        protected void btnClearTag_Click(object sender, EventArgs e)
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
                cmd.CommandText = "USP_ClearMainSubTag";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@processid", processid);
                cmd.Parameters.AddWithValue("@maintagid", maintagid);
                cmd.Parameters.AddWithValue("@subtagid", subtagid);
                cmd.ExecuteScalar();
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('Tag Deleted Successfully...')", true);
                lblmsg.Text = "Tag Deleted Successfully...";
                txtPages.Text = "";
                //get total tag untag
                if (Request.QueryString["id"] != "")
                {
                    TotalTagPages = null;

                    lbltotalnoofPages.Text = "";
                    lbluntagpage.Text = "";
                    lbltotaltagepage.Text = "";
                    DDLTagpagecount.Items.Clear();
                    DDLDrop.Items.Clear();
                    int TagePagescount = 0;


                    TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), pageno, Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));

                    DDLTagpagecount.Items.Clear();
                    TagePagescount = TotalTagPages.Rows.Count;

                    int totpcount = Convert.ToInt32(Hidtotalpagecount.Value);
                    for (int i = 0; i < totpcount; i++)
                    {
                        DDLDrop.Items.Insert(i, (i + 1).ToString());
                    }

                    //for (int k = 0; k < TotalTagPages.Rows.Count; k++)
                    //{
                    //    DDLTagpagecount.Items.Add(TotalTagPages.Rows[k][0].ToString());
                    //    DDLDrop.Items.Remove(TotalTagPages.Rows[k][0].ToString());
                    //}

                    //if (Convert.ToString(TotalTagPages.Rows[0][3]) != null || Convert.ToString(TotalTagPages.Rows[0][3]) != "")
                    //{
                    //    if (Convert.ToInt32(Hidtotalpagecount.Value) >= TagePagescount)
                    //    {
                    //        lbluntagpage.Text = Convert.ToString(Convert.ToInt32(Hidtotalpagecount.Value) - TagePagescount) + " Pages";
                    //        if (lbluntagpage.Text.Contains("-"))
                    //        {
                    //            lbluntagpage.Text = "0" + " Pages";
                    //        }
                    //    }
                    //    else
                    //    {
                    //        lbluntagpage.Text = Convert.ToString(TagePagescount - Convert.ToInt32(Hidtotalpagecount.Value)) + " Pages";
                    //        if (lbluntagpage.Text.Contains("-"))
                    //        {
                    //            lbluntagpage.Text = "0" + " Pages";
                    //        }
                    //    }
                    //}
                    //else
                    //{
                    //    lbluntagpage.Text = "0" + " Pages";
                    //    if (lbluntagpage.Text.Contains("-"))
                    //    {
                    //        lbluntagpage.Text = "0" + " Pages";
                    //    }
                    //}
                    //lbltotaltagepage.Text = Convert.ToString(TagePagescount + " Pages");
                    
                }
                
                //ends here
            }
            catch (Exception ex) { }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                List<string> SearchData = new List<string>();


                int pagecount = 0;
                string filepath = BL.GetFilePath(Convert.ToInt32(Request.QueryString["id"]), Convert.ToInt32(Request.QueryString["docid"]), Convert.ToInt32(Request.QueryString["depid"]));
                filepath = beforedot(filepath).ToLower();
                DirectoryInfo di = new DirectoryInfo(filepath);
                pagecount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;

                SearchData.Add(Request.QueryString["id"]);
                SearchData.Add(Request.QueryString["docid"]);
                SearchData.Add(Request.QueryString["depid"]);
                SearchData.Add(cmbMainTag.SelectedValue.Trim());
                SearchData.Add(cmbSubTag.SelectedValue.Trim());
                SearchData.Add(txtPages.Text.Trim());
                SearchData.Add(pagecount.ToString());


                Results = BL.DeleteData(SearchData, Session["LoggedUserId"].ToString(), hdnLoginToken.Value);

                if (Results.Message == "SUCCESS")
                {
                    Results = BL.SaveDetails(SearchData, Session["LoggedUserId"].ToString(), hdnLoginToken.Value);

                    ReturnMsg = Results.Message.ToString();


                    if (Request.QueryString["id"] != "")
                    {
                        TotalTagPages = null;

                        lbltotalnoofPages.Text = "";
                        lbluntagpage.Text = "";
                        lbltotaltagepage.Text = "";
                        DDLTagpagecount.Items.Clear();
                        DDLDrop.Items.Clear();
                        int TagePagescount = 0;

                        TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), pageno, Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));

                        DDLTagpagecount.Items.Clear();
                        TagePagescount = TotalTagPages.Rows.Count;
                        if (TotalTagPages.Rows.Count > 0)
                        {
                            int totpcount = Convert.ToInt32(Hidtotalpagecount.Value);
                            for (int i = 0; i < totpcount; i++)
                            {
                                DDLDrop.Items.Insert(i, (i + 1).ToString());
                            }

                            for (int k = 0; k < TotalTagPages.Rows.Count; k++)
                            {
                                DDLTagpagecount.Items.Add(TotalTagPages.Rows[k][0].ToString());
                                DDLDrop.Items.Remove(TotalTagPages.Rows[k][0].ToString());
                            }

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
                                lbluntagpage.Text = "0" + " Pages";
                                if (lbluntagpage.Text.Contains("-"))
                                {
                                    lbluntagpage.Text = "0" + " Pages";
                                }
                            }
                            lbltotaltagepage.Text = Convert.ToString(TagePagescount + " Pages");

                        }
                    }
                    lblmsg.Text = ReturnMsg;
                    txtPages.Text = string.Empty;
                    cmbMainTag.SelectedIndex = -1;
                    cmbSubTag.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
            }
        }
        //protected void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        UserBase loginUser = (UserBase)Session["LoggedUser"];
        //        List<string> SearchData = new List<string>();


        //        int pagecount = 0;
        //        string filepath = BL.GetFilePath(Convert.ToInt32(Request.QueryString["id"]), Convert.ToInt32(Request.QueryString["docid"]), Convert.ToInt32(Request.QueryString["depid"]));
        //        filepath = beforedot(filepath).ToLower();
        //        DirectoryInfo di = new DirectoryInfo(filepath);
        //        pagecount = di.GetFiles("*.pdf", SearchOption.AllDirectories).Length;

        //        SearchData.Add(Request.QueryString["id"]);
        //        SearchData.Add(Request.QueryString["docid"]);
        //        SearchData.Add(Request.QueryString["depid"]);
        //        SearchData.Add(cmbMainTag.SelectedValue.Trim());
        //        SearchData.Add(cmbSubTag.SelectedValue.Trim());
        //        SearchData.Add(txtPages.Text.Trim());
        //        SearchData.Add(pagecount.ToString());


        //        Results = BL.DeleteData(SearchData, Session["LoggedUserId"].ToString(), hdnLoginToken.Value);

        //        if (Results.Message == "SUCCESS")
        //        {
        //            Results = BL.SaveDetails(SearchData, Session["LoggedUserId"].ToString(), hdnLoginToken.Value);

        //            ReturnMsg = Results.Message.ToString();
        //            //Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Alert", "alert('" + ReturnMsg + "')", true);
        //            //return;
        //            lblmsg.Text = ReturnMsg;
        //            txtPages.Text = string.Empty;
        //            cmbMainTag.SelectedIndex = -1;
        //            cmbSubTag.SelectedIndex = -1;

        //        }

        //        if (Request.QueryString["id"] != "")
        //        {
        //            TotalTagPages = null;

        //            lbltotalnoofPages.Text = "";
        //            lbluntagpage.Text = "";
        //            lbltotaltagepage.Text = "";
        //            DDLTagpagecount.Items.Clear();
        //            int TagePagescount = 0;

        //            TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), pageno, Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));

        //            TagePagescount = TotalTagPages.Rows.Count;
        //            if (TotalTagPages.Rows.Count > 0)
        //            {
        //                for (int k = 0; k < TotalTagPages.Rows.Count; k++)
        //                {
        //                    DDLTagpagecount.Items.Add(TotalTagPages.Rows[k][0].ToString());

        //                    DDLDrop.Items.Remove(TotalTagPages.Rows[k][0].ToString());

        //                }

        //                if (Convert.ToString(TotalTagPages.Rows[0][3]) != null || Convert.ToString(TotalTagPages.Rows[0][3]) != "")
        //                {
        //                    if (Convert.ToInt32(Hidtotalpagecount.Value) >= TagePagescount)
        //                    {

        //                        lbluntagpage.Text = Convert.ToString(Convert.ToInt32(Hidtotalpagecount.Value) - TagePagescount) + " Pages";
        //                    }
        //                    else
        //                    {
        //                        lbluntagpage.Text = Convert.ToString(TagePagescount - Convert.ToInt32(Hidtotalpagecount.Value)) + " Pages";
        //                    }
        //                }
        //                else
        //                {
        //                    lbluntagpage.Text = "0" + " Pages";
        //                }

        //                lbltotaltagepage.Text = Convert.ToString(TagePagescount + " Pages");

        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.TraceErrorLog(ex.ToString());
        //        Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
        //    }
        //}
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
                catch (Exception ex)
                {
                    continue;
                }
            }
        }
        public void loadfileiniframe()
        {
            string result = "Success";
            string src = string.Empty;
            DDLDrop.Items.Clear();

            string TempFolder = System.Configuration.ConfigurationManager.AppSettings["TempWorkFolder"];

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
                    int pagecount = count;
                    for (int i = 0; i < pagecount; i++)
                    {
                        DDLDrop.Items.Insert(i, (i + 1).ToString());
                    }
                    Hidtotalpagecount.Value = Convert.ToString(pagecount);

                    if (Request.QueryString["id"] != "")
                    {
                        TotalTagPages = null;

                        lbltotalnoofPages.Text = "";
                        lbluntagpage.Text = "";
                        lbltotaltagepage.Text = "";

                        int TagePagescount = 0;
                        TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), pageno, Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));
                        DDLTagpagecount.Items.Clear();
                        TagePagescount = TotalTagPages.Rows.Count;
                        if (TotalTagPages.Rows.Count > 0)
                        {
                            for (int k = 0; k < TotalTagPages.Rows.Count; k++)
                            {
                                DDLTagpagecount.Items.Add(TotalTagPages.Rows[k][0].ToString());

                                DDLDrop.Items.Remove(TotalTagPages.Rows[k][0].ToString());


                            }

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
                                //cmbMainTag.Enabled = false;
                                //cmbSubTag.Enabled = false;
                                if (lbluntagpage.Text.Contains("-"))
                                {
                                    lbluntagpage.Text = "0" + " Pages";
                                }
                            }

                            lbltotaltagepage.Text = Convert.ToString(TagePagescount + " Pages");
                            if (lbluntagpage.Text == "0 Pages")
                            {
                                //cmbMainTag.Enabled = false;
                                //cmbSubTag.Enabled = false;
                            }
                        }
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
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "Script", "loadpdf();", true);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {

            try
            {
                string search = "docid=" + Request.QueryString["docid"] + "&depid=" + Request.QueryString["depid"] + "&MainTagId=" + Request.QueryString["MainTagId"] + "&SubTagId=" + Request.QueryString["SubTagId"] + "&FileBarcode=" + Request.QueryString["FileBarcode"] + "&LoanNo=" + Request.QueryString["LoanNo"];
                Response.Redirect("DocumentDownloadSearch.aspx?" + search);
            }
            catch (Exception ex)
            {
                Logger.TraceErrorLog(ex.ToString());
                Page.ClientScript.RegisterClientScriptBlock(typeof(Page), "Error", "alert('" + ex.ToString() + "')", true);
            }
        }

        protected void DDLTagpagecount_SelectedIndexChanged(object sender, EventArgs e)
        {
            string aa = DDLTagpagecount.SelectedValue;
            TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), Convert.ToInt32(DDLTagpagecount.SelectedValue), Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));
            GetMainTageonTag(hdnLoginOrgId.Value, hdnLoginToken.Value);
            if (TotalTagPages.Rows.Count > 0)
            {
                cmbMainTag.SelectedValue = TotalTagPages.Rows[0][1].ToString();

                GetSubTag(hdnLoginOrgId.Value, hdnLoginToken.Value);
                cmbSubTag.SelectedValue = TotalTagPages.Rows[0][2].ToString();

                Hidmaintag.Value = cmbMainTag.SelectedValue;
                Hidsubtag.Value = cmbSubTag.SelectedValue;
                //   DDLTagpagecount.Attributes.Add("onChange", "javascript:return gotoTagPage();");

            }

        }
        protected void btnpostback_Click(object sender, EventArgs e)
        {
            TotalTagPages = BL.Gettotaltagpages(Convert.ToInt32(Request.QueryString["id"]), Convert.ToInt32(DDLTagpagecount.SelectedValue), Convert.ToInt32(Request.QueryString["MainTagId"]), Convert.ToInt32(Request.QueryString["SubTagId"]));
            GetMainTageonTag(hdnLoginOrgId.Value, hdnLoginToken.Value);
            if (TotalTagPages.Rows.Count > 0)
            {
                //cmbMainTag.SelectedValue = TotalTagPages.Rows[0][1].ToString();

                //GetSubTag(hdnLoginOrgId.Value, hdnLoginToken.Value);
                //cmbSubTag.SelectedValue = TotalTagPages.Rows[0][2].ToString();
                //Hidmaintag.Value = cmbMainTag.SelectedValue;
                //Hidsubtag.Value = cmbSubTag.SelectedValue;
                //DDLTagpagecount.Attributes.Add("onChange", "javascript:return gotoTagPage();");
            }

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
                Response.Redirect(string.Format("DocumentTag.aspx?{0}", view));

            }
        }



    }
}