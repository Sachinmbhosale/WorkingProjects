using System;
using System.Data;
using WorkflowBAL;
using DataAccessLayer;

namespace WorkflowBLL.Classes
{
    public class WorkflowStatusMaster : WorkflowBase
   {      

       #region ConsructorLogic
       public WorkflowStatusMaster()
       {
           WorkflowStatusMasterStatusId = 0;
           WorkflowStatusMasterStatusName = string.Empty;
           WorkflowStatusMasterShortName = string.Empty;
           WorkflowStatusMasterDecription = string.Empty;
           WorkflowStatusMasterIsActive = false;
       }
       #endregion
       #region Default Properties
       public int WorkflowStatusMasterStatusId { get; set; }
       public string WorkflowStatusMasterStatusName { get; set; }
       public string WorkflowStatusMasterShortName { get; set; }
       public string WorkflowStatusMasterDecription { get; set; }
       public bool WorkflowStatusMasterIsActive { get; set; }
       #endregion

       #region SaveWorkflowStatusMaster
       public DBResult ManageWorkflowStatusMaster(WorkflowStatusMaster prop, string Actions)
       {
           DBResult objDBResult = new DBResult();
           DataSet ds = new DataSet();
           IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
           try
           {
               dbManager.Open();
               dbManager.CreateParameters(11);
               dbManager.AddParameters(0, "@in_iStatusId", prop.WorkflowStatusMasterStatusId);
               dbManager.AddParameters(1, "@in_vStatusName", prop.WorkflowStatusMasterStatusName);
               dbManager.AddParameters(2, "@in_vShortName", prop.WorkflowStatusMasterShortName);
               dbManager.AddParameters(3, "@in_vDescription", prop.WorkflowStatusMasterDecription);
               dbManager.AddParameters(4, "@in_bIsActive", prop.WorkflowStatusMasterIsActive);
               dbManager.AddParameters(5, "@in_vAction", Actions);
               dbManager.AddParameters(6, "@in_vLoginToken", prop.LoginToken);
               dbManager.AddParameters(7, "@in_iLoginOrgId", prop.LoginOrgId);
               dbManager.AddParameters(8, "@out_iErrorState", 0, ParameterDirection.Output);
               dbManager.AddParameters(9, "@out_iErrorSeverity", 0, ParameterDirection.Output);
               dbManager.AddParameters(10, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);


               ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Workflow_ManageStatusMaster");

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

           
           //DBResult objDBResult = new DBResult();

           //string StoredProcedure = "USP_Workflow_ManageStatusMaster";

           //SqlParameter [] Parameters = new SqlParameter[11];
           //Parameters[0] = new SqlParameter { ParameterName = "@in_iStatusId", Value = prop.WorkflowStatusMasterStatusId };
           //Parameters[1]= new SqlParameter{ParameterName="@in_vStatusName",Value=prop.WorkflowStatusMasterStatusName};
           //Parameters[2]= new SqlParameter{ParameterName="@in_vShortName",Value=prop.WorkflowStatusMasterShortName};
           //Parameters[3]= new SqlParameter{ParameterName="@in_vDescription",Value=prop.WorkflowStatusMasterDecription};
           //Parameters[4] = new SqlParameter { ParameterName = "@in_bIsActive", Value = prop.WorkflowStatusMasterIsActive };
           //Parameters[5] = new SqlParameter { ParameterName ="@in_vAction", Value = Actions };
           //Parameters[6]= new SqlParameter{ParameterName="@in_vLoginToken",Value=prop.LoginToken};
           //Parameters[7]= new SqlParameter{ParameterName="@in_iLoginOrgId",Value=prop.LoginOrgId};
           //Parameters[8]= new SqlParameter{ParameterName="@out_iErrorState",Direction=ParameterDirection.Output,SqlDbType=SqlDbType.Int};
           //Parameters[9]= new SqlParameter{ParameterName="@out_iErrorSeverity",Direction=ParameterDirection.Output,SqlDbType=SqlDbType.Int};
           // Parameters[10]= new SqlParameter{ParameterName="@out_vMessage",Direction=ParameterDirection.Output,SqlDbType=SqlDbType.NVarChar,Size=250 };
           // objDBResult = objDBHelper.ExecuteDataset(StoredProcedure, Parameters);
           return objDBResult;
       }
       #endregion

      
   }
}
