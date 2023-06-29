using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;
using System.Collections.Generic;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class ManageDocumentType : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();
            SecurityBL bl = new SecurityBL();
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;
                if (Request.QueryString["action"] == null)
                {
                    LogOutAndRedirectionWithErrorMessge(string.Empty);
                }
                else
                {
                    LoadTemplateNames(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    GetDepartments(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    GetGroups(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                    string action = Request.QueryString["action"].ToString();
                    string PageName = string.Empty;
                    if (action.ToLower() == "add")
                    {
                        PageName = "DOCUMENT_TYPES_ADD";
                        hdnAction.Value = "AddDocumentType";
                        lblHeading.Text = "Add New Project Type";
                        hdnCurrentDocumentTypeId.Value = "0";
                        btnsearchagain.Visible = false;
                    }
                    else if (action.ToLower() == "edit")
                    {
                        PageName = "DOCUMENT_TYPES_SEARCH";
                        if (Request.QueryString["id"] == null)
                        {
                            LogOutAndRedirectionWithErrorMessge(string.Empty);
                        }
                        else
                        {
                            hdnCurrentDocumentTypeId.Value = Request.QueryString["id"].ToString();
                            hdnAction.Value = "EditDocumentType";
                            lblHeading.Text = "Edit Project Type";
                            GetDocumentTypeWithId(loginUser.LoginOrgId.ToString(), loginUser.LoginToken);
                        }
                    }
                }
            }
        }
        public void GetDocumentTypeWithId(string loginOrgId, string loginToken)
        {
            DocumentTypeBL bl = new DocumentTypeBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                Logger.Trace("GetDocumentTypeWithId Started", Session["LoggedUserId"].ToString());
                filter.CurrDocumentTypeId = Convert.ToInt32(hdnCurrentDocumentTypeId.Value);
                Results res = bl.SearchDocumentTypes(filter, hdnAction.Value, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.DocumentTypes != null)
                {
                    DocumentType docType = res.DocumentTypes[0];
                    txtDocumentType.Text = docType.DocumentTypeName;
                    txtDescription.Text = docType.Description;
                    if (docType.Active)
                    {
                        chkActive.Checked = true;
                    }
                    else
                    {
                        chkActive.Checked = false;
                    }
                    string arrGroupIds = docType.GroupIds.Trim(',');
                    if (arrGroupIds != string.Empty)
                    {
                        string[] grpIds = arrGroupIds.Split(',');
                        for (int i = 0; i < lstGroups.Items.Count; i++)
                        {
                            foreach (string id in grpIds)
                            {
                                if (lstGroups.Items[i].Value == id)
                                {
                                    lstSelGroups.Items.Add(lstGroups.Items[i]);
                                }
                            }
                        }
                    }

                    SetDepartmentTemplate(res.DocumentTypes);
                }
                Logger.Trace("GetDocumentTypeWithId Finished", Session["LoggedUserId"].ToString());
            }
            catch (Exception ex)
            {
                Logger.Trace("Exception" + ex.Message, Session["LoggedUserId"].ToString());
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
            }
        }
        public void GetDepartments(string loginOrgId, string loginToken)
        {

            DepartmentBL bl = new DepartmentBL();
            SearchFilter filter = new SearchFilter();
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            try
            {
                string action = "DepartmetForManageProjectType";
                Results res = bl.SearchDepartments(filter, action, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Departments != null)
                {
                    DDLDepartment.Items.Clear();
                    DDLDepartment.Items.Add(new ListItem("<Select>", "0"));
                    gblDepartments = res.Departments;
                    foreach (Department dp in res.Departments)
                    {
                        DDLDepartment.Items.Add(new ListItem(dp.DepartmentName, dp.DepartmentId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }
        List<Template> gblTemplates = new List<Template>();
        List<Department> gblDepartments = new List<Department>();
        // int[,] arrTemplates=new int[,4];
        public void LoadTemplateNames(string loginOrgId, string loginToken)
        {
            TemplateBL bl = new TemplateBL();
            SearchFilter filter = new SearchFilter();
            Results res = bl.SearchTemplates(filter, "GetActiveTemplates", loginOrgId, loginToken);
            if (res.Templates != null)
            {
                gblTemplates = res.Templates;
                foreach (Template tpl in res.Templates)
                {
                    DDLtemplate.Items.Add(new ListItem(tpl.TemplateName, tpl.TemplateId.ToString()));
                }
            }
        }
        protected string strTemplateIDs = string.Empty;
        protected void SetDepartmentTemplate(List<DocumentType> DocumentTypes)
        {
            Logger.Trace("SetDepartmentTemplate Started", Session["LoggedUserId"].ToString());
            for (int i = 0; i < DocumentTypes.Count; i++)
            {
                DocumentType docType = DocumentTypes[i];
                strTemplateIDs += i + "," + docType.DepartmentId + "," + docType.DepartmentName + "," + docType.TemplateId + "," + docType.TemplateName + "," + docType.ArchivalD + "," + docType.WaterMarkT + "," + docType.Makerchecker + "#";
            }
            strTemplateIDs = strTemplateIDs.TrimStart('#');
            strTemplateIDs = strTemplateIDs.Remove(strTemplateIDs.LastIndexOf("#"));
            // strTemplateIDs = strTemplateIDs + "]";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "SetDepTemplate", "SetDepTemplate();", true);
            Logger.Trace("SetDepartmentTemplate Finished", Session["LoggedUserId"].ToString());
        }
        public void GetGroups(string loginOrgId, string loginToken)
        {
            GroupBL bl = new GroupBL();
            SearchFilter filter = new SearchFilter();
            try
            {
                string action = "SearchGroups";
                Results res = bl.SearchGroups(filter, action, loginOrgId, loginToken);
                if (res.ActionStatus == "SUCCESS" && res.Groups != null)
                {
                    //lstGroups.Items.Add(new ListItem("<Select>", "0"));
                    foreach (Group grp in res.Groups)
                    {
                        lstGroups.Items.Add(new ListItem(grp.GroupName, grp.GroupId.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                divMsg.InnerHtml = CoreMessages.GetMessages(hdnAction.Value, "ERROR");
                Logger.Trace("Exception:" + ex.Message, Session["LoggedUserId"].ToString());
            }
        }

        protected void LinkButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Secure/Core/DocumentTypes.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Secure/Core/DocumentTypes.aspx");
        }

        protected void btnsearchagain_Click(object sender, EventArgs e)
        {
            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            Response.Redirect("DocumentTypes.aspx?Search=" + SearchCriteria);
        }
    }
}
