/* ============================================================================  
Author     : 
Create date: 
===============================================================================  
** Change History   
** Date:          Author:            Issue ID                         Description:  
** ----------   -------------       ----------                  ----------------------------
** 03 Dec 2013          Mandatory   (UMF)                Set Mandatory for index fields
 *
 * Modified
 * 28/02/2015   Sabina              DMS5-3498           Added extra fields in stagefields
 * 02 Jul  2015 Sharath             DMS5-4423           Data Captured Form usage
=============================================================================== */
using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowStageField : WorkflowBase
    {

        #region ConstructorLogic
        public WorkflowStageField()
        {
            WorkflowStageFieldsId = 0;
            WorkflowStageFieldsProcessId = 0;
            WorkflowStageFieldsWorkflowId = 0;
            WorkflowStageFieldsStageId = 0;
            WorkflowStageFieldsName = string.Empty;
            WorkflowStageFieldsTable = string.Empty;
            WorkflowStageFieldsDBFld = string.Empty;
            WorkflowStageFieldsDBType = string.Empty;
            WorkflowStageFieldsMasterType = 0;
            WorkflowStageFieldsParentId = 0;
            WorkflowStageFieldsLabelText = string.Empty;
            WorkflowStageFieldsCOMSource = string.Empty;
            WorkflowStageFieldsCOMSaveFld = 0;
            WorkflowStageFieldsCOMValueFld = string.Empty;
            WorkflowStageFields_vCOMTextFld = string.Empty;
            WorkflowStageFields_vClientScript = string.Empty;
            WorkflowStageFields_vClientEvent = string.Empty;
            WorkflowStageFields_iLength = 0;
            WorkflowStageFields_iWidth = 0;
            WorkflowStageFields_iHeight = 0;
            WorkflowStageFields_iLeft = 0;
            WorkflowStageFields_vFont = string.Empty;
            WorkflowStageFields_iFontSize = 0;
            WorkflowStageFields_bBold = false;
            WorkflowStageFields_vValidationSQL = string.Empty;
            WorkflowStageFields_bProceed = false;
            WorkflowStageFields_ShowMsgIfNotFound = string.Empty;
            WorkflowStageFields_iMin = 0;
            WorkflowStageFields_iMax = 0;
            WorkflowStageFields_cPadChar = string.Empty;
            WorkflowStageFields_iPadMax = 0;
            WorkflowStageFields_vInputType = string.Empty;
            WorkflowStageFields_bActive = false;
            WorkflowStageFields_bMandatory = false;
            WorkflowStageFields_bDisplay = false;
            WorkflowStageFields_bEditable = false;
            WorkflowStageFields_bPostback = false;
            MapFieldsToTemplate_iSortOrder = 0;
            WorkflowStageFields_bHaschild = false;
            // DMS5-3498A
            WorkflowStageFields_iX1 = 0;
            WorkflowStageFields_iX2 = 0;
            WorkflowStageFields_iY1 = 0;
            WorkflowStageFields_iY2 = 0;
            WorkflowStageFields_iPageNo = 0;
            WorkflowStageFields_iImageHeight = 0;
            WorkflowStageFields_iImageWidth = 0;
            WorkflowStageFields_vFields = string.Empty;
            WorkflowStageFields_vFormula = string.Empty;
            WorkflowStageFields_bShowInDasboard = false; /*	DMS04-3877 -A */
            //DMS5-4423 BS          
            WorkflowStageFields_bShowRemarks = false;
            //DMS5-4423 BE
        }
        #endregion

        #region Properties
        public int WorkflowStageFieldsId { get; set; }
        public int WorkflowStageFieldsProcessId { get; set; }
        public int WorkflowStageFieldsWorkflowId { get; set; }
        public int WorkflowStageFieldsStageId { get; set; }
        public string WorkflowStageFieldsName { get; set; }
        public string WorkflowStageFieldsTable { get; set; }
        public string WorkflowStageFieldsDBFld { get; set; }
        public string WorkflowStageFieldsDBType { get; set; }
        public string WorkflowStageFieldsObjType { get; set; }
        public int WorkflowStageFieldsMasterType { get; set; }
        public int WorkflowStageFieldsParentId { get; set; }
        public string WorkflowStageFieldsLabelText { get; set; }
        public string WorkflowStageFieldsCOMSource { get; set; }
        public int WorkflowStageFieldsCOMSaveFld { get; set; }
        public string WorkflowStageFieldsCOMValueFld { get; set; }
        public string WorkflowStageFields_vCOMTextFld { get; set; }
        public string WorkflowStageFields_vClientScript { get; set; }
        public string WorkflowStageFields_vClientEvent { get; set; }
        public int WorkflowStageFields_iLength { get; set; }
        public int WorkflowStageFields_iWidth { get; set; }
        public int WorkflowStageFields_iHeight { get; set; }
        public int WorkflowStageFields_iLeft { get; set; }
        public int WorkflowStageFields_iTop { get; set; }
        public string WorkflowStageFields_vFont { get; set; }
        public int WorkflowStageFields_iFontSize { get; set; }
        public bool WorkflowStageFields_bBold { get; set; }
        public string WorkflowStageFields_vValidationSQL { get; set; }
        public bool WorkflowStageFields_bProceed { get; set; }
        public string WorkflowStageFields_ShowMsgIfNotFound { get; set; }
        public int WorkflowStageFields_iMin { get; set; }
        public int WorkflowStageFields_iMax { get; set; }
        public string WorkflowStageFields_cPadChar { get; set; }
        public int WorkflowStageFields_iPadMax { get; set; }
        public string WorkflowStageFields_vInputType { get; set; }
        public bool WorkflowStageFields_bActive { get; set; }
        public bool WorkflowStageFields_bMandatory { get; set; }
        public bool WorkflowStageFields_bDisplay { get; set; }
        public bool WorkflowStageFields_bEditable { get; set; }
        public bool WorkflowStageFields_bPostback { get; set; }
        public int MapFieldsToTemplate_iSortOrder { get; set; }
        public bool WorkflowStageFields_bHaschild { get; set; }
        public int MyProperty { get; set; }
        // DMS5-3498A
        public int WorkflowStageFields_iX1 { get; set; }
        public int WorkflowStageFields_iX2 { get; set; }
        public int WorkflowStageFields_iY1 { get; set; }
        public int WorkflowStageFields_iY2 { get; set; }
        public int WorkflowStageFields_iPageNo { get; set; }
        public int WorkflowStageFields_iImageHeight { get; set; }
        public int WorkflowStageFields_iImageWidth { get; set; }
        public string WorkflowStageFields_vFields { get; set; }
        public string WorkflowStageFields_vFormula { get; set; }
        public bool WorkflowStageFields_bShowInDasboard { get; set; } /*	DMS04-3877 -A */
     // DMS5-4423 BS    
        public bool  WorkflowStageFields_bShowRemarks{ get; set; }
        //DMS5-4423 BE


        #endregion
        public DBResult ManageStageFields(WorkflowStageField Properties, string Action)
        {
            DBResult objDBResult = new DBResult();
            DataSet ds = new DataSet();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(58);
                dbManager.AddParameters(0, "@in_iId", Properties.WorkflowStageFieldsId);
                dbManager.AddParameters(1, "@in_iProcessId", Properties.WorkflowStageFieldsProcessId);
                dbManager.AddParameters(2, "@in_iWorkflowId", Properties.WorkflowStageFieldsWorkflowId);
                dbManager.AddParameters(3, "@in_iStageId", Properties.WorkflowStageFieldsStageId);
                dbManager.AddParameters(4, "@in_vName", Properties.WorkflowStageFieldsName);
                dbManager.AddParameters(5, "@in_vTable", Properties.WorkflowStageFieldsTable);
                dbManager.AddParameters(6, "@in_vDBFld", Properties.WorkflowStageFieldsDBFld);
                dbManager.AddParameters(7, "@in_vDBType", Properties.WorkflowStageFieldsDBType);
                dbManager.AddParameters(8, "@in_cObjType", Properties.WorkflowStageFieldsObjType);
                dbManager.AddParameters(9, "@in_iMasterType", Properties.WorkflowStageFieldsMasterType);
                dbManager.AddParameters(10, "@in_iParentId", Properties.WorkflowStageFieldsParentId);
                dbManager.AddParameters(11, "@in_vLabelText", Properties.WorkflowStageFieldsLabelText);
                dbManager.AddParameters(12, "@in_vCOMSource", Properties.WorkflowStageFieldsCOMSource);
                dbManager.AddParameters(13, "@in_iCOMSaveFld", Properties.WorkflowStageFieldsCOMSaveFld);
                dbManager.AddParameters(14, "@in_vCOMValueFld", Properties.WorkflowStageFieldsCOMValueFld);
                dbManager.AddParameters(15, "@in_vCOMTextFld", Properties.WorkflowStageFields_vCOMTextFld);
                dbManager.AddParameters(16, "@in_vClientScript", Properties.WorkflowStageFields_vClientScript);
                dbManager.AddParameters(17, "@in_vClientEvent", Properties.WorkflowStageFields_vClientEvent);
                dbManager.AddParameters(18, "@in_iLength", Properties.WorkflowStageFields_iLength);
                dbManager.AddParameters(19, "@in_iWidth", Properties.WorkflowStageFields_iWidth);
                dbManager.AddParameters(20, "@in_iHeight", Properties.WorkflowStageFields_iHeight);
                dbManager.AddParameters(21, "@in_iLeft", Properties.WorkflowStageFields_iLeft);
                dbManager.AddParameters(22, "@in_iTop", Properties.WorkflowStageFields_iTop);
                dbManager.AddParameters(23, "@in_vFont", Properties.WorkflowStageFields_vFont);
                dbManager.AddParameters(24, "@in_iFontSize", Properties.WorkflowStageFields_iFontSize);
                dbManager.AddParameters(25, "@in_bBold", Properties.WorkflowStageFields_bBold);
                dbManager.AddParameters(26, "@in_vValidationSQL", Properties.WorkflowStageFields_vValidationSQL);
                dbManager.AddParameters(27, "@in_bProceed", Properties.WorkflowStageFields_bProceed);
                dbManager.AddParameters(28, "@in_ShowMsgIfNotFound", Properties.WorkflowStageFields_ShowMsgIfNotFound);
                dbManager.AddParameters(29, "@in_iMin", Properties.WorkflowStageFields_iMin);
                dbManager.AddParameters(30, "@in_iMax", Properties.WorkflowStageFields_iMax);
                dbManager.AddParameters(31, "@in_cPadChar", Properties.WorkflowStageFields_cPadChar);
                dbManager.AddParameters(32, "@in_iPadMax", Properties.WorkflowStageFields_iPadMax);
                dbManager.AddParameters(33, "@in_vInputType", Properties.WorkflowStageFields_vInputType);
                dbManager.AddParameters(34, "@in_bActive", Properties.WorkflowStageFields_bActive);
                dbManager.AddParameters(35, "@in_bMandatory", Properties.WorkflowStageFields_bMandatory);
                dbManager.AddParameters(36, "@in_bDisplay", Properties.WorkflowStageFields_bDisplay);
                dbManager.AddParameters(37, "@in_bPostback", Properties.WorkflowStageFields_bPostback);
                dbManager.AddParameters(38, "@in_iSortOrder", Properties.MapFieldsToTemplate_iSortOrder);
                dbManager.AddParameters(39, "@in_bHaschild", Properties.WorkflowStageFields_bHaschild);
                dbManager.AddParameters(40, "@in_vAction", Action);
                dbManager.AddParameters(41, "@in_vLoginToken", Properties.LoginToken);
                dbManager.AddParameters(42, "@in_iLoginOrgId", Properties.LoginOrgId);
                dbManager.AddParameters(43, "@in_iX1", Properties.WorkflowStageFields_iX1);
                dbManager.AddParameters(44, "@in_iX2", Properties.WorkflowStageFields_iX2);
                dbManager.AddParameters(45, "@in_iY1", Properties.WorkflowStageFields_iY1);
                dbManager.AddParameters(46, "@in_iY2", Properties.WorkflowStageFields_iY2);
                dbManager.AddParameters(47, "@in_iPageNo", Properties.WorkflowStageFields_iPageNo);
                dbManager.AddParameters(48, "@in_iImageHeight", Properties.WorkflowStageFields_iImageHeight);
                dbManager.AddParameters(49, "@in_iImageWidth", Properties.WorkflowStageFields_iImageWidth);
                dbManager.AddParameters(50, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(51, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(52, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(53, "@in_bEditable", Properties.WorkflowStageFields_bEditable);
                dbManager.AddParameters(54, "@in_vFields", Properties.WorkflowStageFields_vFields);
 				dbManager.AddParameters(55, "@in_vFormula", Properties.WorkflowStageFields_vFormula);
                dbManager.AddParameters(56, "@in_bShowInDashBoard", Properties.WorkflowStageFields_bShowInDasboard);  /*	DMS04-3877 -A */              
                dbManager.AddParameters(57, "@in_bShowRemarks", Properties.WorkflowStageFields_bShowRemarks); //DMS5-4423 
                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageStageFields");

                objDBResult.dsResult = ds;
                string errState = dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorState").ToString().Trim();
                string errSev = dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim() == "" ? "0" : dbManager.GetOutputParameterValue("@out_iErrorSeverity").ToString().Trim();
                objDBResult.ErrorState = Convert.ToInt32(errState);
                objDBResult.ErrorSeverity = Convert.ToInt32(errSev);
                objDBResult.Message = dbManager.GetOutputParameterValue("@out_vMessage").ToString().Trim();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }


            //string StroredProcedure = "";
            //SqlParameter[] Parameters = new SqlParameter[47];

            //Parameters[0] = new SqlParameter { ParameterName = "@in_iId", Value = Properties.WorkflowStageFieldsId };
            //Parameters[1] = new SqlParameter { ParameterName = "@in_iProcessId", Value = Properties.WorkflowStageFieldsProcessId };
            //Parameters[2] = new SqlParameter { ParameterName = "@in_iWorkflowId", Value = Properties.WorkflowStageFieldsWorkflowId };
            //Parameters[3] = new SqlParameter { ParameterName = "@in_iStageId", Value = Properties.WorkflowStageFieldsStageId };
            //Parameters[4] = new SqlParameter { ParameterName = "@in_vName", Value = Properties.WorkflowStageFieldsName };
            //Parameters[5] = new SqlParameter { ParameterName = "@in_vTable", Value = Properties.WorkflowStageFieldsTable };
            //Parameters[6] = new SqlParameter { ParameterName = "@in_vDBFld", Value = Properties.WorkflowStageFieldsDBFld };
            //Parameters[7] = new SqlParameter { ParameterName = "@in_vDBType", Value = Properties.WorkflowStageFieldsDBType };
            //Parameters[8] = new SqlParameter { ParameterName = "@in_cObjType", Value = Properties.WorkflowStageFieldsObjType };
            //Parameters[9] = new SqlParameter { ParameterName = "@in_iMasterType", Value = Properties.WorkflowStageFieldsMasterType };
            //Parameters[10] = new SqlParameter { ParameterName = "@in_iParentId", Value = Properties.WorkflowStageFieldsParentId };
            //Parameters[11] = new SqlParameter { ParameterName = "@in_vLabelText", Value = Properties.WorkflowStageFieldsLabelText };
            //Parameters[12] = new SqlParameter { ParameterName = "@in_vCOMSource", Value = Properties.WorkflowStageFieldsCOMSource };
            //Parameters[13] = new SqlParameter { ParameterName = "@in_iCOMSaveFld", Value = Properties.WorkflowStageFieldsCOMSaveFld };
            //Parameters[14] = new SqlParameter { ParameterName = "@in_vCOMValueFld", Value = Properties.WorkflowStageFieldsCOMValueFld };
            //Parameters[15] = new SqlParameter { ParameterName = "@in_vCOMTextFld", Value = Properties.WorkflowStageFields_vCOMTextFld };
            //Parameters[16] = new SqlParameter { ParameterName = "@in_vClientScript", Value = Properties.WorkflowStageFields_vClientScript };
            //Parameters[17] = new SqlParameter { ParameterName = "@in_vClientEvent", Value = Properties.WorkflowStageFields_vClientEvent };
            //Parameters[18] = new SqlParameter { ParameterName = "@in_iLength", Value = Properties.WorkflowStageFields_iLength };
            //Parameters[19] = new SqlParameter { ParameterName = "@in_iWidth", Value = Properties.WorkflowStageFields_iWidth };
            //Parameters[20] = new SqlParameter { ParameterName = "@in_iHeight", Value = Properties.WorkflowStageFields_iHeight };
            //Parameters[21] = new SqlParameter { ParameterName = "@in_iLeft", Value = Properties.WorkflowStageFields_iLeft };
            //Parameters[22] = new SqlParameter { ParameterName = "@in_iTop", Value = Properties.WorkflowStageFields_iTop };
            //Parameters[23] = new SqlParameter { ParameterName = "@in_vFont", Value = Properties.WorkflowStageFields_vFont };
            //Parameters[24] = new SqlParameter { ParameterName = "@in_iFontSize", Value = Properties.WorkflowStageFields_iFontSize };
            //Parameters[25] = new SqlParameter { ParameterName = "@in_bBold", Value = Properties.WorkflowStageFields_bBold };
            //Parameters[26] = new SqlParameter { ParameterName = "@in_vValidationSQL", Value = Properties.WorkflowStageFields_vValidationSQL };
            //Parameters[27] = new SqlParameter { ParameterName = "@in_bProceed", Value = Properties.WorkflowStageFields_bProceed };
            //Parameters[28] = new SqlParameter { ParameterName = "@in_ShowMsgIfNotFound", Value = Properties.WorkflowStageFields_ShowMsgIfNotFound };
            //Parameters[29] = new SqlParameter { ParameterName = "@in_iMin", Value = Properties.WorkflowStageFields_iMin };
            //Parameters[30] = new SqlParameter { ParameterName = "@in_iMax", Value = Properties.WorkflowStageFields_iMax };
            //Parameters[31] = new SqlParameter { ParameterName = "@in_cPadChar", Value = Properties.WorkflowStageFields_cPadChar };
            //Parameters[32] = new SqlParameter { ParameterName = "@in_iPadMax", Value = Properties.WorkflowStageFields_iPadMax };
            //Parameters[33] = new SqlParameter { ParameterName = "@in_vInputType", Value = Properties.WorkflowStageFields_vInputType };
            //Parameters[34] = new SqlParameter { ParameterName = "@in_bActive", Value = Properties.WorkflowStageFields_bActive };
            //Parameters[35] = new SqlParameter { ParameterName = "@in_bMandatory", Value = Properties.WorkflowStageFields_bMandatory };
            //Parameters[36] = new SqlParameter { ParameterName = "@in_bDisplay", Value = Properties.WorkflowStageFields_bDisplay };
            //Parameters[37] = new SqlParameter { ParameterName = "@in_bPostback", Value = Properties.WorkflowStageFields_bPostback };
            //Parameters[38] = new SqlParameter { ParameterName = "@in_iSortOrder", Value = Properties.MapFieldsToTemplate_iSortOrder };
            //Parameters[39] = new SqlParameter { ParameterName = "@in_bHaschild", Value = Properties.WorkflowStageFields_bHaschild };
            //Parameters[40] = new SqlParameter { ParameterName = "@in_vAction", Value = Action };
            //Parameters[41] = new SqlParameter { ParameterName = "@in_vLoginToken", Value = Properties.LoginToken };
            //Parameters[42] = new SqlParameter { ParameterName = "@in_iLoginOrgId", Value = Properties.LoginOrgId };
            //Parameters[43] = new SqlParameter { ParameterName = "@out_iErrorState", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            //Parameters[44] = new SqlParameter { ParameterName = "@out_iErrorSeverity", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
            //Parameters[45] = new SqlParameter { ParameterName = "@out_vMessage", SqlDbType = SqlDbType.NVarChar, Direction = ParameterDirection.Output, Size = 250 };
            //Parameters[46] = new SqlParameter { ParameterName = "@in_bEditable", Value = Properties.WorkflowStageFields_bEditable};

            //objDBResult = objDBHelper.ExecuteDataset(StroredProcedure, Parameters);
            return objDBResult;
        }
    }
}
