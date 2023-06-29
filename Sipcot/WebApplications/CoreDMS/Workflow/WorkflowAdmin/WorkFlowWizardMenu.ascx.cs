using System;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class WorkFlowWizardMenu : System.Web.UI.UserControl
    {
        public string ActiveItemName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ActiveItemName == "Process")
            {
                mnuWFWizard.Items[0].Selected = true;
            }
            else if (ActiveItemName == "Work Flow")
            {
                mnuWFWizard.Items[1].Selected = true;
            }
            else if (ActiveItemName == "Stages")
            {
                mnuWFWizard.Items[2].Selected = true;
            }
            else if (ActiveItemName == "Fields")
            {
                mnuWFWizard.Items[3].Selected = true;
            }
            else if (ActiveItemName == "Users")
            {
                mnuWFWizard.Items[4].Selected = true;
            }
            else if (ActiveItemName == "Status")
            {
                mnuWFWizard.Items[5].Selected = true;
            }
            else if (ActiveItemName == "Notification")
            {
                mnuWFWizard.Items[6].Selected = true;
            }
            
        }
    }
}