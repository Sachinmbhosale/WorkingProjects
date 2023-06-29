using System;
using System.Data;
using DataAccessLayer;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class AppDAL : BaseDAL
    {
        public AppDAL() { }

        public string getApplicatonData(string keyword)
        {
            string appData = string.Empty;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(1);
                dbManager.AddParameters(0, "@in_vKeyword", keyword);

                object objstatus = dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_GetApplicationData");
                if ((string)objstatus != string.Empty)
                {
                    appData = (string)objstatus;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            return appData;
        }

        public bool setApplicatonData(string keyword, string value)
        {
            bool successFlag = false;

            IDBManager dbManager = new DBManager(ConfiguredDataProvider, DbConnectionString);
            try
            {
                dbManager.Open();
                dbManager.CreateParameters(2);

                dbManager.AddParameters(0, "@in_vKeyword", keyword);
                dbManager.AddParameters(1, "@in_vValue", value);

                object objstatus = dbManager.ExecuteScalar(CommandType.StoredProcedure, "USP_SetApplicationData");
                if ((bool)objstatus)
                {
                    successFlag = (bool)objstatus;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }
            return successFlag;
        }
    }
}
