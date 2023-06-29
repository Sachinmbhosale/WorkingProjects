using System;
using System.Data;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DynamicControlsDAL : BaseDAL
    {
       /// <summary>
       /// Get values to populate the dropdownlist
       /// </summary>
       /// <param name="Templated"></param>
       /// <returns></returns>
        public DataSet DynamicPopulateDropdown(int Templated,string action)
        {
            DataSet dsDetails = new DataSet();


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(2);
                dbManager.AddParameters(0, "@in_iTemplateFldId", Templated);
                dbManager.AddParameters(1, "@in_vAction", action);

                dsDetails = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_DynamicPopulateDropdown");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return dsDetails;


            /*SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            SqlDataAdapter sqlda = new SqlDataAdapter();
            DataSet dsDetails = new DataSet();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "USP_DynamicPopulateDropdown";
                dbCmd.Parameters.Add("@in_iTemplateFldId", SqlDbType.Int).Value = Templated;
                dbCmd.Parameters.Add("@in_vAction", SqlDbType.VarChar).Value = action;
                sqlda.SelectCommand = dbCmd;
                sqlda.Fill(dsDetails);

            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return dsDetails;*/
        }
        /// <summary>
        /// Load dropdown based on value sub dropdown loading
        /// </summary>
        /// <param name="Templated"></param>
        /// <returns></returns>
        public DataSet DynamicLoadDropdownBasedOnValue(int Templated)
        {

            DataSet dsDetails = new DataSet();


            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@in_iTemplateId", Templated);


                dsDetails = dbManager.ExecuteDataSet(CommandType.StoredProcedure, "USP_DynamicLoadDropdownBasedOnValue");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return dsDetails;




            /*SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            SqlDataAdapter sqlda = new SqlDataAdapter();
            DataSet dsDetails = new DataSet();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "USP_DynamicLoadDropdownBasedOnValue";
                dbCmd.Parameters.Add("@in_iTemplateId", SqlDbType.Int).Value = Templated;
                sqlda.SelectCommand = dbCmd;
                sqlda.Fill(dsDetails);
            }
            catch (Exception)
            {
                
                throw;
            }
            finally
            {
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                CloseConnection(dbCon);
            }
            return dsDetails;*/
        }
    }
}
