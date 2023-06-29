using System;
using System.Web.UI;

namespace Lotex.EnterpriseSolutions.WebUI.UserControls
{
    public partial class ApplicationTabs : System.Web.UI.UserControl
    {
        public int EnableAdmin = 0;
        public int EnableDMS = 0;
        public int EnableWorkFlow = 0;
        public string ActiveApplication = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (EnableAdmin == 1)
                imgButtonAdmin.Enabled = true;
            else
                imgButtonAdmin.Enabled = false;

            if (EnableDMS == 1)
                imgButtonDMS.Enabled = true;
            else
                imgButtonDMS.Enabled = false;

            if (EnableWorkFlow == 1)
                imgButtonWorkFlow.Enabled = true;
            else
                imgButtonWorkFlow.Enabled = false;

            if (ActiveApplication.ToUpper() == "WORKFLOW")
            {
                imgButtonAdmin.ImageUrl = "../Workflow/images/header_admin_deselect.png";
                imgButtonDMS.ImageUrl = "../Workflow/images/header_dms_deselect.png";
                imgButtonWorkFlow.ImageUrl = "../Workflow/images/header_workflow_select.png";
            }
            else if (ActiveApplication.ToUpper() == "DMS")
            {
                imgButtonAdmin.ImageUrl = "../Workflow/images/header_admin_deselect.png";
                imgButtonDMS.ImageUrl = "../Workflow/images/header_dms_select.png";
                imgButtonWorkFlow.ImageUrl = "../Workflow/images/header_workflow_deselect.png";
            }
            else if (ActiveApplication.ToUpper() == "ADMIN")
            {
                imgButtonAdmin.ImageUrl = "../Workflow/images/header_admin_select.png";
                imgButtonDMS.ImageUrl = "../Workflow/images/header_dms_deselect.png";
                imgButtonWorkFlow.ImageUrl = "../Workflow/images/header_workflow_deselect.png";
            }
        }

        protected void imgButtonWorkFlow_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/Workflow/WorkflowAdmin/WorkFlowHome.aspx", false);
        }

        protected void imgButtonDMS_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/secure/Home.aspx", false);
        }
    }
}