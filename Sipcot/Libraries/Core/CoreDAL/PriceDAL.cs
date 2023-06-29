using System;
using System.Data;
using Lotex.EnterpriseSolutions.CoreBE;
using DataAccessLayer;


namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class PriceDAL : BaseDAL
    {
        public PriceDAL() { }

        public Results ManagePrice(Price objPrice, string action, string loginToken, int loginOrgId)
        {
            Results results = new Results();
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(12);

                dbManager.AddParameters(0, "@in_iPriceId", objPrice.PriceId);
                dbManager.AddParameters(1, "@in_iCustomerId", objPrice.CustomerId);
                dbManager.AddParameters(2, "@in_iDocumentTypeId", objPrice.DocumentTypeId);
                dbManager.AddParameters(3, "@in_vBillType", objPrice.BillType);
                dbManager.AddParameters(4, "@in_dCharges", objPrice.Charges);
                dbManager.AddParameters(5, "@in_vCurrency", objPrice.Currency);
                dbManager.AddParameters(6, "@in_vAction", action);
                dbManager.AddParameters(7, "@in_vLoginToken", loginToken);
                dbManager.AddParameters(8, "@in_iLoginOrgId", loginOrgId);

                dbManager.AddParameters(9, "@out_iErrorState", 0, ParameterDirection.Output);
                dbManager.AddParameters(10, "@out_iErrorSeverity", 0, ParameterDirection.Output);
                dbManager.AddParameters(11, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);

                results.ResultDS = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_ManagePrice");
                results.ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                results.ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));
                results.Message = Convert.ToString(dbManager.GetOutputParameterValue("@out_vMessage"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return results;
        }
    }
}
