using System;
using Lotex.EnterpriseSolutions.CoreBE;
using System.Data;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class AnnotationsDAL : BaseDAL
    {
        public DataSet GetUploadDocumentDetails(Annotations filter, string action, string loginOrgId, string loginToken)
        {
            DataSet ds = new DataSet();

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(8);

                dbManager.AddParameters(0, "@in_iDocumentId", filter.DocumentId);
                dbManager.AddParameters(1, "@in_iPageNo", filter.PageNo);
                dbManager.AddParameters(2, "@in_xAnnotations", filter.XAnnotations);
                dbManager.AddParameters(3, "@in_xDocumentWithAnnotations", filter.xDocumentWithAnnotations);
                dbManager.AddParameters(4, "@in_xPageNoMappings", filter.xPageNoMappings);
                dbManager.AddParameters(5, "@in_vAction", action);
                dbManager.AddParameters(6, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(7, "@in_iLoginOrgId", loginOrgId);

                ds = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_Manage_Annotations");

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return ds;
        }
       

    }
}
