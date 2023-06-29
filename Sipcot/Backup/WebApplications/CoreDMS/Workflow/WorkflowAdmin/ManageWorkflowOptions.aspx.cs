/* ===================================================================================================================================
 * Author     : 
 * Create date: 
 * Description:  
======================================================================================================================================  
 * Change History   
 * Date:            Author:             Issue ID            Description:  
 * -----------------------------------------------------------------------------------------------------------------------------------

 * 17 Apr 2015      Gokul               DMS5-3946       Applying rights an permissions in all pages as per user group rights!
====================================================================================================================================== */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WorkflowBAL;
using WorkflowBLL.Classes;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using System.Drawing;
using System.Globalization;

namespace Lotex.EnterpriseSolutions.WebUI.Workflow.WorkflowAdmin
{
    public partial class ManageWorkflowOptions : PageBase
    {
        WorkflowOptions objWorkflowOption = new WorkflowOptions();
        public string pageRights = string.Empty; /* DMS5-3946 A */
       

        protected void Page_Load(object sender, EventArgs e)
        {
            CheckAuthentication();           
            pageRights = GetPageRights();/* DMS5-3946 A */
            //Start: ------------------SitePath Details ----------------------
            Label sitePath = (Label)Master.FindControl("SitePath");
            sitePath.Text = "Define >> Workflow Options";
            //End: ------------------SitePath Details -----------------------

            txtOOOStartDate.Attributes.Add("readonly","readonly");
            txtOOOEndDate.Attributes.Add("readonly","readonly");

            if (!Page.IsPostBack)
            {
                ApplyPageRights(pageRights, this.Form.Controls); /* DMS5-3946 A */
                if (Session["OutofOffice"] != null)
                {
                    chkOutofOffice.Checked = Convert.ToInt32(Session["OutofOffice"].ToString()) == 0 ? false : true;
                    FillOptionValues();
                }
                else
                {
                }
            }
        }

        protected void FillOptionValues()
        {
            try
            {
                DBResult dr = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                objWorkflowOption.LoginToken = loginUser.LoginToken;
                objWorkflowOption.LoginOrgId = loginUser.LoginOrgId;
                objWorkflowOption.WorkflowOptions_Action = "GetUserOutOfOfficePreference";
                objWorkflowOption.WorkflowOptions_OutOfOffice = 0;
                objWorkflowOption.WorkflowOptions_OutOfOfficeStartDate = string.Empty;
                objWorkflowOption.WorkflowOptions_OutOfOfficeEndDate = string.Empty;
                dr = objWorkflowOption.ManageWorkflowOptions(objWorkflowOption);

                if (chkOutofOffice.Checked)
                {
                    if (dr.ErrorState == 0 && dr.dsResult != null && dr.dsResult.Tables.Count > 0 && dr.dsResult.Tables[0].Rows.Count > 0)
                    {
                        txtOOOStartDate.Text = dr.dsResult.Tables[0].Rows[0]["USERS_dOutofOfficeStartDate"].ToString().Replace(":00 AM", "");
                        txtOOOEndDate.Text = dr.dsResult.Tables[0].Rows[0]["USERS_dOutofOfficeEndDate"].ToString().Replace(":00 AM", "");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnSaveOptions_Click(object sender, EventArgs e)
        {

            if (chkOutofOffice.Checked == false)
            {
                txtOOOStartDate.Text = string.Empty;
                txtOOOEndDate.Text = string.Empty;

            }
            else
            {
                DBResult objDBResult = new DBResult();
                CheckAuthentication();
                try
                {

                    string stDt;
                    string enDt;

                    try
                    {
                        stDt = txtOOOStartDate.Text.ToString().Trim();
                        enDt = txtOOOEndDate.Text.ToString().Trim();
                    }
                    catch
                    {
                        stDt = null;
                        enDt = null;
                    }

                    int iOutofOffice = chkOutofOffice.Checked == true ? 1 : 0;

                    UserBase loginUser = (UserBase)Session["LoggedUser"];
                    objWorkflowOption.LoginToken = loginUser.LoginToken;
                    objWorkflowOption.LoginOrgId = loginUser.LoginOrgId;
                    objWorkflowOption.WorkflowOptions_Action = "SaveUserOutOfOfficePreference";
                    objWorkflowOption.WorkflowOptions_OutOfOffice = iOutofOffice;
                    objWorkflowOption.WorkflowOptions_OutOfOfficeStartDate = stDt;
                    objWorkflowOption.WorkflowOptions_OutOfOfficeEndDate = enDt;
                    objWorkflowOption.ManageWorkflowOptions(objWorkflowOption);

                    if (objDBResult.ErrorState == 0)
                    {
                        lblMessage.Text = "User preferences are saved successfully.";
                        lblMessage.ForeColor = Color.Green;
                        Session["OutofOffice"] = chkOutofOffice.Checked == true ? "1" : "0";

                        FillOptionValues();
                    }
                    else
                    {
                        lblMessage.Text = "An error occured while saving the user preferences. Please contact administrator.";
                        lblMessage.ForeColor = Color.Red;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}