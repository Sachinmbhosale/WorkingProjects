using System;
using System.Data;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DocumentViewDAL : BaseDAL
    {
        public DataSet GetAllTreeViewData(string LoginToken, int LoginOrgId, string ToolTip, int NodeId, int ParentNodeId, int DocumentTypeId, int DepartmentId)
      {
          DataSet dsValue = new DataSet();


          IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
          try
          {
              dbManager.Open();
              dbManager.CreateParameters(7);
              dbManager.AddParameters(0, "@in_vLoginToken", LoginToken);
              dbManager.AddParameters(1, "@in_iLoginOrgId", LoginOrgId);
              dbManager.AddParameters(2, "@in_vToolTip", ToolTip);
              dbManager.AddParameters(3, "@in_iNodeId", NodeId);
              dbManager.AddParameters(4, "@in_iParentNodeId", ParentNodeId);
              dbManager.AddParameters(5, "@in_iDocumentTypeId", DocumentTypeId);
              dbManager.AddParameters(6, "@in_iDepartmentId", DepartmentId);
              dsValue = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_GetDocumentHierarchyForTreeview");
          }
          catch (Exception ex)
          {
              throw ex;
          }
          finally
          {
              dbManager.Dispose();
          }
          return dsValue;
        /*  SqlCommand sqlCmd = new SqlCommand();
          SqlConnection dbCon = null;
          SqlDataAdapter sqlda = new SqlDataAdapter();
          DataSet dsValue = new DataSet();
          try
          {
              dbCon = OpenConnection();
              sqlCmd.Connection = dbCon;
              sqlCmd.CommandType = CommandType.StoredProcedure;
              sqlCmd.CommandText = "USP_GetDocumentHierarchyForTreeview";
              sqlCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar).Value = LoginToken;
              sqlCmd.Parameters.Add("@in_iLoginOrgId", SqlDbType.Int).Value = LoginOrgId;
              sqlCmd.Parameters.Add("@in_vToolTip", SqlDbType.VarChar).Value = ToolTip;
              sqlCmd.Parameters.Add("@in_iNodeId", SqlDbType.Int).Value = NodeId;
              sqlCmd.Parameters.Add("@in_iParentNodeId", SqlDbType.Int).Value = ParentNodeId;
              sqlCmd.Parameters.Add("@in_iDocumentTypeId", SqlDbType.Int).Value = DocumentTypeId;
              sqlCmd.Parameters.Add("@in_iDepartmentId", SqlDbType.Int).Value = DepartmentId;
              sqlda.SelectCommand = sqlCmd;
              sqlda.Fill(dsValue);
          }
          catch (Exception)
          {
              
             
          }
          return dsValue;*/
      }
    }
}
