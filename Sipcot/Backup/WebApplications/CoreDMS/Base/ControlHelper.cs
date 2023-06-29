/* ============================================================================  
Author     : Yogeesha Naik
Create date: 17 Apr 2015
===============================================================================  
** Change History   
** Date:          Author:            Issue ID           Description:  
** ----------   -------------       ----------          ----------------------------
 * 17 Apr 2015  Yogeesha Naik        DMS5-3947          Rewritten most of the methods related to dynamic controls (made generic)                                 
=============================================================================== */

using System;
using System.Web.UI.WebControls;
using WorkflowBAL;
using Lotex.EnterpriseSolutions.CoreBE;
using WorkflowBLL.Classes;

namespace Lotex.EnterpriseSolutions.WebUI
{
    public class ControlHelper : PageBase
    {
        WorkflowDataEntryBLL ObjDataEntry = new WorkflowDataEntryBLL();

        #region bind methods
        public void BindDropDownList(DropDownList ddl, int masterTypeId)
        {
            try
            {
                DBResult Objresult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
                ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
                ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
                ObjDataEntry.LoginToken = loginUser.LoginToken;
                ObjDataEntry.WorkflowDataEntryMasterTypeId = masterTypeId;

                Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetDropDownData");
                ddl.DataSource = Objresult.dsResult;
                ddl.DataTextField = "WorkflowMasterValues_vName";
                ddl.DataValueField = "WorkflowMasterValues_iId";
                ddl.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BindCheckBoxList(CheckBoxList ddl, int masterTypeId)
        {
            try
            {
                DBResult Objresult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
                ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
                ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
                ObjDataEntry.LoginToken = loginUser.LoginToken;
                ObjDataEntry.WorkflowDataEntryMasterTypeId = masterTypeId;

                Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetDropDownData");
                ddl.DataSource = Objresult.dsResult;
                ddl.DataTextField = "WorkflowMasterValues_vName";
                ddl.DataValueField = "WorkflowMasterValues_iId";
                ddl.DataBind();

                ddl.Items.RemoveAt(0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BindChildDropDownList(DropDownList ddl, int masterTypeId, int masterTypeVal)
        {
            try
            {
                DBResult Objresult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
                ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
                ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
                ObjDataEntry.LoginToken = loginUser.LoginToken;
                ObjDataEntry.WorkflowDataEntryMasterTypeId = masterTypeId;
                ObjDataEntry.WorkflowDataEntryParentId = masterTypeVal;

                Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetChildDropDownData");
                /*Modified by gokul 01 -03- 2015*/
                ddl.Items.Clear();
                ddl.SelectedIndex = -1;
                ddl.SelectedValue = null;
                ddl.ClearSelection();
                /*Modified by gokul 01 -03- 2015*/

                ddl.DataSource = Objresult.dsResult.Tables[0];
                ddl.DataTextField = "WorkflowMasterValues_vName";
                ddl.DataValueField = "WorkflowMasterValues_iId";
                ddl.DataBind();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void BindChildCheckBoxList(CheckBoxList ddl, int masterTypeId, int masterTypeVal)
        {
            try
            {
                DBResult Objresult = new DBResult();
                UserBase loginUser = (UserBase)Session["LoggedUser"];
                ObjDataEntry.WorkflowDataEntryProcessId = Convert.ToInt32(Session["ProcessId"]);
                ObjDataEntry.WorkflowDataEntryWorkflowId = Convert.ToInt32(Session["WorkflowId"]);
                ObjDataEntry.WorkflowDataEntryStageId = Convert.ToInt32(Session["StageId"]);
                ObjDataEntry.LoginOrgId = loginUser.LoginOrgId;
                ObjDataEntry.LoginToken = loginUser.LoginToken;
                ObjDataEntry.WorkflowDataEntryMasterTypeId = masterTypeId;
                ObjDataEntry.WorkflowDataEntryParentId = masterTypeVal;

                Objresult = ObjDataEntry.ManageWorkflowDataEntry(ObjDataEntry, "GetChildDropDownData");
                ddl.DataSource = Objresult.dsResult;
                ddl.DataTextField = "WorkflowMasterValues_vName";
                ddl.DataValueField = "WorkflowMasterValues_iId";
                try
                {
                    ddl.DataBind();
                }
                catch
                { }

                ddl.Items.RemoveAt(0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
