using System;
using Lotex.EnterpriseSolutions.CoreBE;
using Lotex.EnterpriseSolutions.CoreBL;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public partial class DptAddNew : PageBase
    {
        Department Entityobj = new Department();
        DepartmentBL Logicobj = new DepartmentBL();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            btnSubmit.Attributes.Add("onclick", "javascript:return DeptAddValidate();");
            if (!IsPostBack)
            {
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                hdnLoginOrgId.Value = loginUser.LoginOrgId.ToString();
                hdnLoginToken.Value = loginUser.LoginToken;

                string pageRights = GetPageRights();
                hdnPageRights.Value = pageRights;
                ApplyPageRights(pageRights, this.Form.Controls);
                if (Request.QueryString["id"] != null)
                {
                    if (Request.QueryString["action"] == "edit")
                    {
                        lblHeading.Text = "Edit Department ";

                    }
                    Results result = new Results();
                    Entityobj.DepartmentId = Convert.ToInt32(Request.QueryString["id"]);
                    // Action may be wrong
                    result = Logicobj.ManageDepartment(Entityobj, "GetDepartment", Convert.ToString(hdnLoginToken.Value), Convert.ToInt32(hdnLoginOrgId.Value));
                    txtDptName.Text = Convert.ToString(result.ResultDS.Tables[0].Rows[0][0]);
                    txtDescription.Text = Convert.ToString(result.ResultDS.Tables[0].Rows[0][1]);
                    txtHead.Text = Convert.ToString(result.ResultDS.Tables[0].Rows[0][2]);
                }
            }

        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Department.aspx");
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Results result = new Results();
            Logger.Trace("btnSubmit_Click Started", Session["LoggedUserId"].ToString());
            UserBase loginUser = (UserBase)Session["LoggedUser"];
            Entityobj.DepartmentName = txtDptName.Text.Trim();
            Entityobj.Description = Server.HtmlEncode(txtDescription.Text);
            Entityobj.Head = txtHead.Text;

            if (Request.QueryString["id"] != null)
            {
                Entityobj.DepartmentId = Convert.ToInt32(Request.QueryString["id"]);
                result = Logicobj.ManageDepartment(Entityobj, "UpdateDepartment", Convert.ToString(hdnLoginToken.Value), Convert.ToInt32(hdnLoginOrgId.Value));
            }
            else
            {
                result = Logicobj.ManageDepartment(Entityobj, "AddDepartment", Convert.ToString(hdnLoginToken.Value), Convert.ToInt32(hdnLoginOrgId.Value));
            }

            if (result.ErrorState == 0)
            {
                divMsg.Style.Add("color", "green");
                divMsg.InnerHtml = result.Message;
            }
            else
            {
                divMsg.Style.Add("color", "red");
                divMsg.InnerHtml = result.Message;
            }
            Logger.Trace("btnSubmit_Click Finished", Session["LoggedUserId"].ToString());
        }

        protected void btnsearchagain_Click(object sender, EventArgs e)
        {
            string SearchCriteria = Request.QueryString["Search"] != null ? Request.QueryString["Search"].ToString() : string.Empty;
            Response.Redirect("Department.aspx?Search=" + SearchCriteria);

        }
    }
}
