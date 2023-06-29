/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
** 06 Nov 2013    gokuldas           bug 1662 
** 03 Dec 2013    Pratheesh A       Mandatory and Display (MD)   Set Mandatory and Display option for index fields\
 * 13 feb 2014    Gokuldas          Index active and bug fix search with(501)
 * 28 feb 2015    Gokuldas.p        Bug DMS04-3381  

=============================================================================== */
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Data;
using System.Net;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ManageTemplate : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            if (!IsPostBack)
            {
                if (ViewState["dt"] == null)
                {
                    DataTable dt = new DataTable();//Creating a Datatable for maintaining id and values of dynamic controls
                    ViewState["dt"] = dt;//Creating a viewstate for maintaining id and values of dynamic controls
                    dt.Columns.Add("ListItemsId", typeof(string)); //adding columns
                    dt.Columns.Add("ListName", typeof(string));
                    dt.Columns.Add("ListParentID", typeof(string));
                    dt.Columns.Add("FieldName", typeof(string));

                }
                if (ViewState["tdt"] == null)
                {
                    DataTable tdt = new DataTable();//Creating a Datatable for maintaining id and values of dynamic controls
                    ViewState["tdt"] = tdt;//Creating a viewstate for maintaining id and values of dynamic controls
                    tdt.Columns.Add("TagItemsId", typeof(string)); //adding columns
                    tdt.Columns.Add("TagName", typeof(string));
                    tdt.Columns.Add("TagParentID", typeof(string));
                    tdt.Columns.Add("TemplateName", typeof(string));
                }
                hdnIsCopied.Value = "0";
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                if (Request.QueryString["action"] == null)
                {
                    LogOutAndRedirectionWithErrorMessge(string.Empty);
                }
                else
                {
                    string action = Request.QueryString["action"].ToString();
                    string PageName = string.Empty;
                    if (action.ToLower() == "add")
                    {
                        PageName = "TEMPLATE_ADD";
                        hdnAction.Value = "AddTemplate";
                        lblHeading.Text = "Add New Template";
                        hdnCurrentTemplateId.Value = "0";
                        hdnClearFileName.Value = "0";
                        btnsearchagain.Visible = false;
                    }
                    else if (action.ToLower() == "edit")
                    {

                        PageName = "TEMPLATE_SEARCH";
                        if (Request.QueryString["id"] == null)
                        {
                            LogOutAndRedirectionWithErrorMessge(string.Empty);
                        }
                        else
                        {
                            hdnCurrentTemplateId.Value = Request.QueryString["id"].ToString();
                            hdnAction.Value = "EditTemplate";
                            lblHeading.Text = "Edit Template";
                            hdnClearFileName.Value = "1";
                            GetTemplateWithID(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                        }
                    }
                    lstDataType.Items.Add(new ListItem("String", "String"));
                    lstDataType.Items.Add(new ListItem("Integer", "Integer"));
                    lstDataType.Items.Add(new ListItem("DateTime", "DateTime"));
                    lstDataType.Items.Add(new ListItem("Boolean", "Boolean"));

                }
                lstValue.Attributes.Add("onchange", "javascript:return OnMainListIndexChange();");

                lstMainTag.Attributes.Add("onchange", "javascript:return OnMainTagIndexChange();");
                ddlMaintag.Attributes.Add("onchange", "javascript:return OnMainTagDropIndexChange();");
                ddlsubtag.Attributes.Add("onchange", "javascript:return OnMainSubTagDropIndexChange();");

                lstDataType.Attributes.Add("onchange", "javascript:return Datatype();");
                txtFileName.Attributes.Add("readonly", "readonly");

            }
        }

        public void GetTemplateWithID(string loginOrgId, string loginToken)
        {
            TemplateBL bl = new TemplateBL();
            SearchFilter filter = new SearchFilter();
            DataSet ds = new DataSet();
            try
            {

                filter.CurrTemplateId = Convert.ToInt32(hdnCurrentTemplateId.Value);
                Results res = bl.SearchTemplates(filter, hdnAction.Value, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Templates != null)
                {

                    Template tpl = res.Templates[0];
                    txtTemplateName.Text = tpl.TemplateName;
                    txtFileName.Text = tpl.UploadFIleName;
                    if (tpl.UploadFIleNameSeperator == "")
                    {
                        tpl.UploadFIleNameSeperator = "--select--";
                        cmbSeperator.Items.FindByText(tpl.UploadFIleNameSeperator).Selected = true;
                        cmbSeperator.Enabled = true;
                    }
                    else
                    {
                        cmbSeperator.Items.FindByText(tpl.UploadFIleNameSeperator).Selected = true;
                        cmbSeperator.Enabled = false;
                    }


                    hdnClearFileName.Value = "1";
                    tpl.TemplateId = filter.CurrTemplateId;
                    if (tpl.Active)
                    {
                        chkActive.Checked = true;
                    }
                    else
                    {
                        chkActive.Checked = false;
                    }
                    IndexField index = new IndexField();
                    index.TemplateID = Convert.ToInt32(Request.QueryString["id"]);
                    lstMainTag.Items.Clear();
                    ddlMaintag.Items.Clear();
                    index.IsCopied = 0;
                    ds = bl.ManageListItems(index, "GetTagDetails", hdnLoginOrgId.Value, hdnLoginToken.Value);

                    if (ds != null && ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0][0].ToString() == "SUCCESS")
                        {
                            AddTagDetailsToDT(ds.Tables[1]);
                            DataSet Tds = new DataSet();
                            DataTable tdt = (DataTable)ViewState["tdt"];
                            Tds.Tables.Add(tdt);
                            //Encoded because XML was creating Error when used dirctly 
                            HdnTagdetails.Value = WebUtility.HtmlEncode(Tds.GetXml().ToString());
                         
                        }
                    }
                    txtTemplateName.Enabled = false;
                    GetIndexFieldsByTemplateId(tpl, loginOrgId, loginToken);
                }


            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception"+ex.Message, Session["LoggedUserId"].ToString());
            }

        }

        private void AddTagDetailsToDT(DataTable dt)
        {
            DataTable tdt = (DataTable)ViewState["tdt"];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tdt.Rows.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), txtTemplateName.Text.Trim().ToString());
            }

            ViewState["tdt"] = tdt;
            BindTagLisbox(GetDTForTag("0"), lstMainTag);
            txtMainTagValue.Text = "";
            BindTagDropdown(GetDTForTag("0"), ddlMaintag);
        }

        public void AddIndexListDetailsTODT(DataTable dt)
        {
            DataTable tdt = (DataTable)ViewState["dt"];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                tdt.Rows.Add(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString());
            }

            ViewState["dt"] = tdt;
        }

        public void GetIndexFieldsByTemplateId(Template tpl, string loginOrgId, string loginToken)
        {


            TemplateBL bl = new TemplateBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Logger.Trace("GetIndexFieldsByTemplateId Started", Session["LoggedUserId"].ToString());
                Results res = bl.GetTemplateFieldList(tpl, hdnAction.Value, loginOrgId, loginToken);

                if (res.ActionStatus == "SUCCESS" && res.Templates != null && res.Templates[0].IndexFields != null)
                {

                    string rightsText = string.Empty;
                    Template tpl1 = res.Templates[0];
                    int i = 0;
                    foreach (IndexField iField in tpl1.IndexFields)
                    {
                        i = i + 1;
                        rightsText += "#" + i.ToString() + "|" + iField.IndexName + "|" + iField.EntryType + "|" + iField.CharIndexDataType + "|";

                        if (iField.MinLength > 0)
                        {
                            rightsText += iField.MinLength;
                        }
                        else
                        {
                            rightsText += "";
                        }
                        if (iField.MaxLength > 0)
                        {
                            rightsText += "|" + iField.MaxLength;
                        }
                        else
                        {
                            rightsText += "|" + "";
                        }
                        rightsText += "|" + iField.Mandatory.ToLower() + "|" + iField.Display.ToLower() + '|' + iField.ActiveIndex.ToLower() + '|' + iField.OrginalOrder + '|' + iField.IndexFldId + '|' + iField.haschild + '|' + "NoDelete";
                        //MD -Add

                    }
                    if (rightsText != string.Empty)
                    {
                        rightsText = rightsText.TrimStart('#');
                        AddIndexListDetailsTODT(res.ResultDS.Tables[1]);
                        DataSet ds = new DataSet();
                        DataTable dt = (DataTable)ViewState["dt"];
                        ds.Tables.Add(dt);
                        //Encoded because XML was creating Error when used dirctly 
                        hdnIndexListValues.Value = WebUtility.HtmlEncode(ds.GetXml().ToString());
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowTemplateFieldList", "ShowTemplateFieldList('" + rightsText + "');", true);
                    }

                }
                Logger.Trace("GetIndexFieldsByTemplateId Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception"+ex.Message, Session["LoggedUserId"].ToString());
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Secure/Core/SearchTemplates.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            // Clear ListItem table's content of this session
            TemplateBL bl = new TemplateBL();
            IndexField filter = new IndexField();
            string MainAction = string.Empty;
            if (Request.QueryString["action"] != null && Request.QueryString["action"] == "add")
            {
                MainAction = "AddTemplate";
            }
            filter.ListItemId = 0;
            bl.ManageListItems(filter, "DeleteSessionData", hdnLoginOrgId.Value, hdnLoginToken.Value);

            // Redirect to SearchTemplates page
            Response.Redirect("~/Secure/Core/SearchTemplates.aspx");
        }

        public void bindlistbox(DataTable dt, String Action)
        {

            if (Action == "AddMainList" || Action == "DeleteMainItem" || Action == "LoadMainCategory")
            {
                lstValue.Items.Clear();
                ListValueSub.Items.Clear();
                lstValue.DataSource = dt;
                lstValue.DataTextField = "ListName";
                lstValue.DataValueField = "ListItemsId";
                lstValue.DataBind();
            }
            else if (Action == "AddSubList" || Action == "LoadSubCategory" || Action == "DeleteSubItem")
            {
                ListValueSub.Items.Clear();
                ListValueSub.DataSource = dt;
                ListValueSub.DataTextField = "ListName";
                ListValueSub.DataValueField = "ListItemsId";
                ListValueSub.DataBind();
            }



        }
        private void UpdateHasChild()
        {
            DataTable iDt = (DataTable)ViewState["dt"];
            DataTable dt = new DataTable();
            DataView dv = new DataView(iDt);
            dv.RowFilter = "([ListParentID] > '" + "0" + "') AND ([FieldName] = '" + txtIndexFieldName.Text.Trim().ToString() + "')";
            dt = dv.ToTable();
            if (dt.Rows.Count > 0)
            {
                hdnHaschild.Value = "1";
            }
            else
            {
                hdnHaschild.Value = "0";
            }
        }
        private DataTable GetDT(string action, string ParentId)
        {
            string FieldName = txtIndexFieldName.Text.ToString();
            DataTable iDt = (DataTable)ViewState["dt"];
            DataTable dt = new DataTable();
            DataView dv = new DataView(iDt);
            if (ParentId == string.Empty || ParentId == "")
            {
                ParentId = "0";
            }

            dv.RowFilter = "([ListParentID] = '" + ParentId + "') AND ([FieldName] = '" + FieldName + "')";
            dt = dv.ToTable();

            return dt;
        }

        public DataTable deletefromdt(DataTable dt, string ValueID, string action)
        {

            if (action == "DeleteMainItem")
            {
                var rows = dt.Select("ListItemsId = '" + ValueID + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
                var subrows = dt.Select("ListParentID = '" + ValueID + "'");
                foreach (var row in subrows)
                {
                    row.Delete();
                }
            }
            else if (action == "DeleteSubItem")
            {
                var rows = dt.Select("ListItemsId = '" + ValueID + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
            }
            else if (action == "DeleteIndexItem")
            {
                var rows = dt.Select("FieldName = '" + ValueID + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
            }

            return dt;
        }

        public void ManageIndexListBOx(string Action)
        {
            try
            {
                Logger.Trace("ManageIndexListBOx Started", Session["LoggedUserId"].ToString());
                int count = 0;
                string Mainvalue = string.Empty;
                DataSet ds = new DataSet();
                DataTable dt = (DataTable)ViewState["dt"];
                if (Action == "AddMainList")
                {
                    count = lstValue.Items.Count + 1;
                    ListItem MListItem = new ListItem(txtValue1.Text.Trim().ToString());
                    if (!lstValue.Items.Contains(MListItem))
                    {
                        dt.Rows.Add(txtIndexFieldName.Text.Trim().ToString() + count.ToString(), MListItem.ToString(), "0", txtIndexFieldName.Text.Trim().ToString());//adding to the datatable

                        ViewState["dt"] = dt;
                        bindlistbox(GetDT(Action, "0"), Action);
                        txtValue1.Text = "";
                    }


                }
                else if (Action == "AddSubList")
                {

                    count = ListValueSub.Items.Count + 1;
                    ListItem SListItem = new ListItem(txtSubValue.Text.Trim().ToString());
                    Mainvalue = lstValue.SelectedValue.ToString();

                    if (!ListValueSub.Items.Contains(SListItem))
                    {
                        dt.Rows.Add(lstValue.SelectedItem.Text.ToString() + "Sub_" + count.ToString(), SListItem.ToString(), Mainvalue, txtIndexFieldName.Text.Trim().ToString());//adding to the datatable

                        ViewState["dt"] = dt;

                        bindlistbox(GetDT(Action, Mainvalue), Action);
                        txtSubValue.Text = "";
                    }

                }
                else if (Action == "LoadMainCategory")
                {

                    bindlistbox(GetDT(Action, Mainvalue), Action);
                    txtSubValue.Text = "";
                }
                else if (Action == "LoadSubCategory")
                {
                    Mainvalue = lstValue.SelectedValue.ToString();
                    bindlistbox(GetDT(Action, Mainvalue), Action);
                    txtSubValue.Text = "";
                }
                else if (Action == "DeleteMainItem")
                {
                    ViewState["dt"] = deletefromdt(dt, lstValue.SelectedValue.ToString(), Action);
                    bindlistbox(GetDT(Action, Mainvalue), Action);
                    txtSubValue.Text = "";
                }
                else if (Action == "DeleteSubItem")
                {
                    Mainvalue = lstValue.SelectedValue.ToString();
                    ViewState["dt"] = deletefromdt(dt, ListValueSub.SelectedValue.ToString(), Action);
                    bindlistbox(GetDT(Action, Mainvalue), Action);
                    txtSubValue.Text = "";
                }
                else if (Action == "DeleteIndexItem")
                {

                    ViewState["dt"] = deletefromdt(dt, txtIndexFieldName.Text.Trim().ToString(), Action);
                    lstValue.Items.Clear();
                    ListValueSub.Items.Clear();
                    // txtSubValue.Text = "";
                }
                DataTable tdt = (DataTable)ViewState["dt"];
                ds.Tables.Add(dt);
                //Encoded because XML was creating Error when used dirctly 
                hdnIndexListValues.Value = WebUtility.HtmlEncode(ds.GetXml().ToString());

                UpdateHasChild();
                txtIndexFieldName.Text = "";
                Logger.Trace("ManageIndexListBOx Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
                
            }

        }
        private DataTable GetDTForTag(string ParentId)
        {
            string FieldName = txtTemplateName.Text.ToString();
            DataTable iDt = (DataTable)ViewState["tdt"];
            DataTable dt = new DataTable();
            DataView dv = new DataView(iDt);
            if (ParentId == string.Empty || ParentId == "")
            {
                ParentId = "0";
            }

            dv.RowFilter = "([TagParentID] = '" + ParentId + "') AND ([TemplateName] = '" + FieldName + "')";
            dt = dv.ToTable();

            return dt;
        }
        protected void BindTagDropdown(DataTable dt, DropDownList DropDownlist)
        {
            DropDownlist.Items.Clear();
            DropDownlist.DataSource = dt;
            DropDownlist.DataTextField = "TagName";
            DropDownlist.DataValueField = "TagItemsId";
            DropDownlist.DataBind();
            DropDownlist.Items.Insert(0, new ListItem("--Select--", "0"));
        }
        protected void BindTagLisbox(DataTable dt, ListBox listbox)
        {

            listbox.Items.Clear();
            listbox.DataSource = dt;
            listbox.DataTextField = "TagName";
            listbox.DataValueField = "TagItemsId";
            listbox.DataBind();
        }
        public DataTable deletefromTagdt(DataTable dt, string ValueID, string action)
        {

            if (action == "DeleteMainTag")
            {
                var rows = dt.Select("TagItemsId = '" + ValueID + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
                var subrows = dt.Select("TagParentID = '" + ValueID + "'");
                foreach (var row in subrows)
                {
                    row.Delete();
                }
            }
            else if (action == "DeleteSubTag")
            {
                var rows = dt.Select("TagItemsId = '" + ValueID + "'");
                foreach (var row in rows)
                {
                    row.Delete();
                }
            }


            return dt;
        }
        protected void ManageTagListbox(string Action)
        {
            Logger.Trace("ManageTagListbox Started", Session["LoggedUserId"].ToString());
            try
            {
                int count = 0;
                string Mainvalue = string.Empty;
                DataSet ds = new DataSet();
                DataTable dt = (DataTable)ViewState["tdt"];
                if (Action == "AddMainTag")
                {
                    count = lstMainTag.Items.Count + 1;
                    ListItem MListItem = new ListItem(txtMainTagValue.Text.Trim().ToString());
                    if (!lstMainTag.Items.Contains(MListItem))
                    {
                        dt.Rows.Add(txtTemplateName.Text.Trim().ToString() + count.ToString(), MListItem.ToString(), "0", txtTemplateName.Text.Trim().ToString());//adding to the datatable
                        ViewState["tdt"] = dt;
                        BindTagLisbox(GetDTForTag("0"), lstMainTag);
                        txtMainTagValue.Text = "";
                        BindTagDropdown(GetDTForTag("0"), ddlMaintag);
                    }
                }
                else if (Action == "AddSubTag")
                {
                    count = lstSubTag.Items.Count + 1;
                    ListItem SListItem = new ListItem(txtSubTagValue.Text.Trim().ToString());
                    Mainvalue = lstMainTag.SelectedValue.ToString();

                    if (!lstSubTag.Items.Contains(SListItem))
                    {
                        dt.Rows.Add(lstMainTag.SelectedItem.Text.ToString() + "Sub_" + count.ToString(), SListItem.ToString(), Mainvalue, txtTemplateName.Text.Trim().ToString());//adding to the datatable

                        ViewState["tdt"] = dt;
                        BindTagLisbox(GetDTForTag(Mainvalue), lstSubTag);

                        txtSubTagValue.Text = "";
                    }
                }
                else if (Action == "LoadSubTag")
                {
                    Mainvalue = lstMainTag.SelectedValue.ToString();
                    BindTagLisbox(GetDTForTag(Mainvalue), lstSubTag);
                    txtSubTagValue.Text = "";
                }
                else if (Action == "LoadMainTag")
                {

                    BindTagLisbox(GetDTForTag("0"), lstMainTag);
                    txtSubTagValue.Text = "";
                }
                else if (Action == "DeleteMainTag")
                {


                    ViewState["tdt"] = deletefromTagdt(dt, lstMainTag.SelectedValue.ToString(), Action);
                    BindTagLisbox(GetDTForTag("0"), lstMainTag);
                    txtMainTagValue.Text = "";
                    lstSubTag.Items.Clear();
                }
                else if (Action == "DeleteSubTag")
                {

                    Mainvalue = lstMainTag.SelectedValue.ToString();
                    ViewState["tdt"] = deletefromTagdt(dt, lstSubTag.SelectedValue.ToString(), Action);
                    BindTagLisbox(GetDTForTag(Mainvalue), lstSubTag);
                    txtMainTagValue.Text = "";
                }
                else if (Action == "LoadSubTagForTagNameChange")
                {
                    Mainvalue = ddlMaintag.SelectedValue.ToString();
                    BindTagDropdown(GetDTForTag(Mainvalue), ddlsubtag);
                }
                DataTable tdt = (DataTable)ViewState["tdt"];
                ds.Tables.Add(tdt);
                //Encoded because XML was creating Error when used dirctly 
                HdnTagdetails.Value = WebUtility.HtmlEncode(ds.GetXml().ToString());
                Logger.Trace("ManageTagListbox Finished", "TraceStatus");
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception:"+ex.Message, Session["LoggedUserId"].ToString());
                
            }

        }
        protected void btnManageList_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnManageList_Click Started", Session["LoggedUserId"].ToString());
                if (hdnListAction.Value.Length > 0)
                {
                    ManageIndexListBOx(hdnListAction.Value);
                }
                if (hdnTagAction.Value.Length > 0)
                {
                    ManageTagListbox(hdnTagAction.Value);
                }
                Logger.Trace("btnManageList_Click finished", Session["LoggedUserId"].ToString());

                hdnListAction.Value = "";
                hdnTagAction.Value = "";
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception"+ex.Message, Session["LoggedUserId"].ToString());
               
            }
        }

        protected void btnsearchagain_Click(object sender, EventArgs e)
        {
            Logger.Trace("btnsearchagain_Click started", "TraceStatus");
            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            Response.Redirect("SearchTemplates.aspx?Search=" + SearchCriteria);

            Logger.Trace("btnsearchagain_Click Finised", Session["LoggedUserId"].ToString());
        }

        protected void btntagnamesave_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btntagnamesave_Click Started", Session["LoggedUserId"].ToString());
                if (txtmaintag.Text.Length > 0)
                {
                    DataTable iDt = (DataTable)ViewState["tdt"];
                    foreach (DataRow row in iDt.Rows)
                    {

                        if (row["TagName"].ToString() == ddlMaintag.SelectedItem.Text.ToString() && row["TagItemsId"].ToString() == ddlMaintag.SelectedValue.ToString())
                        {
                            row["TagName"] = txtmaintag.Text;
                        }
                    }
                    ViewState["tdt"] = iDt;
                    BindTagDropdown(GetDTForTag("0"), ddlMaintag);
                    BindTagLisbox(GetDTForTag("0"), lstMainTag);
                    DataSet ds = new DataSet();
                    DataTable tdt = (DataTable)ViewState["tdt"];
                    ds.Tables.Add(tdt);
                    //Encoded because XML was creating Error when used dirctly 
                    HdnTagdetails.Value = WebUtility.HtmlEncode(ds.GetXml().ToString());
                    txtmaintag.Text = "";

                }
                Logger.Trace("btntagnamesave_Click Finised", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception" + ex.Message, Session["LoggedUserId"].ToString());
              
            }
        }

        protected void btnsubtagsave_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnsubtagsave_Click Started", Session["LoggedUserId"].ToString());
                if (txtsubtag.Text.Length > 0)
                {
                    DataTable iDt = (DataTable)ViewState["tdt"];
                    foreach (DataRow row in iDt.Rows)
                    {

                        if (row["TagName"].ToString() == ddlsubtag.SelectedItem.Text.ToString() && row["TagItemsId"].ToString() == ddlsubtag.SelectedValue.ToString())
                        {
                            row["TagName"] = txtsubtag.Text;
                        }
                    }
                    ViewState["tdt"] = iDt;
                    BindTagDropdown(GetDTForTag(ddlMaintag.SelectedValue.ToString()), ddlsubtag);
                    txtmaintag.Text = "";
                    DataSet ds = new DataSet();
                    DataTable tdt = (DataTable)ViewState["tdt"];
                    ds.Tables.Add(tdt);
                    //Encoded because XML was creating Error when used dirctly 
                    HdnTagdetails.Value = WebUtility.HtmlEncode(ds.GetXml().ToString());
                    txtmaintag.Text = "";
                    lstSubTag.Items.Clear();
                }
                Logger.Trace("btnsubtagsave_Click Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception"+ex.Message, Session["LoggedUserId"].ToString());
               
            }
        }

        protected void btnClearAll_Click(object sender, EventArgs e)
        {
            try
            {
                Logger.Trace("btnClearAll_Click Started", Session["LoggedUserId"].ToString());
                ListValueSub.Items.Clear();
                lstValue.Items.Clear();
                lstMainTag.Items.Clear();
                lstSubTag.Items.Clear();

                DataTable dt = new DataTable();//Creating a Datatable for maintaining id and values of dynamic controls
                ViewState["dt"] = dt;//Creating a viewstate for maintaining id and values of dynamic controls
                dt.Columns.Add("ListItemsId", typeof(string)); //adding columns
                dt.Columns.Add("ListName", typeof(string));
                dt.Columns.Add("ListParentID", typeof(string));
                dt.Columns.Add("FieldName", typeof(string));

                DataTable tdt = new DataTable();//Creating a Datatable for maintaining id and values of dynamic controls
                ViewState["tdt"] = tdt;//Creating a viewstate for maintaining id and values of dynamic controls
                tdt.Columns.Add("TagItemsId", typeof(string)); //adding columns
                tdt.Columns.Add("TagName", typeof(string));
                tdt.Columns.Add("TagParentID", typeof(string));
                tdt.Columns.Add("TemplateName", typeof(string));
                divMsg.Style.Add("color", "green");
                divMsg.InnerHtml = "Template Uploaded Successfully";
                Logger.Trace("btnClearAll_Click Finished", Session["LoggedUserId"].ToString());

            }
            catch (Exception ex)
            {
                Logger.Trace("Exception"+ex.Message, Session["LoggedUserId"].ToString());
               
            }
        }
    }
}
