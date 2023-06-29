/****                                    *****
 ****   HOW YOU CAN RUN IT, SAMPLE CODE  *****
 ****                                    *****                       
public class RunSP
{
/// <summary>
/// Run Store Procedure.
/// </summary>
/// <param name="cnnStr">Connection String of SQL server</param>
public static void RunSP(string cnnStr)
{
//Create a array list to store parameter(s) with
//parameter name, value & data type.
ArrayList arLst = new ArrayList();
//Now send array list name, value & varchar, persingle
//call of that function will create one stored parameter.
SP.spArgumentsCollection(arLst, "@nextName", "Suman Biswas", "varchar");
SP.spArgumentsCollection(arLst, "@id", "2", "int");
//Now run stored procedure.
SP.RunStoredProcedure(cnnStr, "UpdateName", arLst);
}
}

*/


using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using GenServiceDataContracts;
using System.Reflection;
using System.Configuration;
using DataAccessLayer;

namespace StoredProcedureLib
{
    /// <summary>
    /// This class helps to built SP argument. It can use directly to any SP.
    /// </summary>
    public class SP
    {
        private static string ConnectionString = string.Empty;
        private static string GetConnectionStrings(string Key)
        {
            ConnectionStringSettingsCollection settings =
                ConfigurationManager.ConnectionStrings;

            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    if (cs.Name == Key)
                    {
                        ConnectionString = cs.ConnectionString;
                    }
                }
            }
            return ConnectionString;
        }

        public static DataProvider ConfiguredDataProvider
        {
            get
            {
                string DatabaseSystem = ConfigurationManager.AppSettings["DatabaseSystem"];
                switch (DatabaseSystem.ToUpper())
                {
                    default:
                    case "SQLSERVER":
                        return DataProvider.SqlServer;
                    case "MYSQL":
                        return DataProvider.MySql;
                }
            }
        }

        /// <summary>
        /// Run a stored procedure of Select SQL type.
        /// </summary>
        /// <param name="dbConnStr">Connection String to connect Sql Server</param>
        /// <param name="ds">DataSet which will return after filling Data</param>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="spPramArrList">Parameters in ArrayList</param>
        /// <returns>Return DataSet after filing data by SQL.</returns>
        public static DBResult RunStoredProcedure(DBResult Result, string dbConnStr, DataSet ds, string spName, string spPramList)
        {
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, GetConnectionStrings(dbConnStr));
            try
            {
                dbManager.Open();

                string spPramName = string.Empty;
                string spPramValue = string.Empty;
                string spPramDataType = string.Empty;

                //Split Parameters list
                string[] asParameterSets = null;
                if (spPramList.Trim().Length > 0)
                {
                    asParameterSets = spPramList.Split(']');
                }

                int paramCount = asParameterSets.Length - 1;

                dbManager.CreateParameters(paramCount);
                for (int i = 0; i < paramCount; i++)
                {
                    //Split Parameter set
                    string[] asParameters = null;
                    if (asParameterSets[i].Trim().Length > 0)
                    {
                        asParameters = asParameterSets[i].Split('|');
                    }

                    spPramName = asParameters[0].Replace("[", "");
                    spPramValue = asParameters[1];
                    spPramDataType = asParameters[2].Replace("]", "");
                    if (ConfiguredDataProvider== DataProvider.SqlServer)
                    {
                       dbManager.AddParameters(i, spPramName, spPramValue,ParameterDirection.Input); 
                    }
                    else if(ConfiguredDataProvider==DataProvider.MySql)
                    {
                     dbManager.AddParameters(i, spPramName, spPramValue,ParameterDirection.InputOutput);
                    }
                }
                //#region default/output parameters
                //dbManager.AddParameters(paramCount, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                //dbManager.AddParameters(paramCount + 1, "@out_iErrorState", 0, DbType.Int32, 10, ParameterDirection.Output);
                //dbManager.AddParameters(paramCount + 2, "@out_iErrorSeverity", 0, DbType.Int32, 10, ParameterDirection.Output);
                //#endregion

                Result.ResultDS = dbManager.ExecuteDataSet(CommandType.StoredProcedure, spName);

                try
                {
                    Result.ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                    Result.ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));
                    Result.Msg = Convert.ToString(dbManager.GetOutputParameterValue("@out_vMessage"));
                }
                catch { }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }

            /*
            SqlConnection conn = new SqlConnection(GetConnectionStrings(dbConnStr));
            conn.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = conn;
            cmd.CommandText = spName;

            string spPramName = "";
            string spPramValue = "";
            string spPramDataType = "";

            //Split Parameters list
            string[] asParameterSets = null;
            if (spPramList.Trim().Length > 0)
            {
                asParameterSets = spPramList.Split(']');
            }

            for (int i = 0; i < asParameterSets.Length - 1; i++)
            {
                //Split Parameter set
                string[] asParameters = null;
                if (asParameterSets[i].Trim().Length > 0)
                {
                    asParameters = asParameterSets[i].Split('|');
                }

                spPramName = asParameters[0].Replace("[", "");
                spPramValue = asParameters[1];
                spPramDataType = asParameters[2].Replace("]", "");

                SqlParameter pram = null;
                #region SQL DB TYPE AND VALUE ASSIGNMENT
                switch (spPramDataType.ToUpper())
                {
                    case "BIGINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.BigInt);
                        pram.Value = spPramValue;
                        break;

                    case "BINARY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Binary);
                        pram.Value = spPramValue;
                        break;

                    case "BIT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Bit);
                        pram.Value = spPramValue;
                        break;

                    case "CHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Char);
                        pram.Value = spPramValue;
                        break;

                    case "DATETIME":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.DateTime);
                        pram.Value = spPramValue;
                        break;

                    case "DECIMAL":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Decimal);
                        pram.Value = spPramValue;
                        break;

                    case "FLOAT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Float);
                        pram.Value = spPramValue;
                        break;

                    case "IMAGE":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Image);
                        pram.Value = spPramValue;
                        break;

                    case "INT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Int);
                        pram.Value = spPramValue;
                        break;

                    case "MONEY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Money);
                        pram.Value = spPramValue;
                        break;

                    case "NCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NChar);
                        pram.Value = spPramValue;
                        break;

                    case "NTEXT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NText);
                        pram.Value = spPramValue;
                        break;

                    case "NVARCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NVarChar);
                        pram.Value = spPramValue;
                        break;

                    case "REAL":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Real);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLDATETIME":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallDateTime);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallInt);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLMONEY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallMoney);
                        pram.Value = spPramValue;
                        break;

                    case "TEXT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Text);
                        pram.Value = spPramValue;
                        break;

                    case "TIMESTAMP":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Timestamp);
                        pram.Value = spPramValue;
                        break;

                    case "TINYINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.TinyInt);
                        pram.Value = spPramValue;
                        break;

                    case "UDT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Udt);
                        pram.Value = spPramValue;
                        break;

                    case "UMIQUEIDENTIFIER":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.UniqueIdentifier);
                        pram.Value = spPramValue;
                        break;

                    case "VARBINARY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.VarBinary);
                        pram.Value = spPramValue;
                        break;

                    case "VARCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.VarChar);
                        pram.Value = spPramValue;
                        break;

                    case "VARIANT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Variant);
                        pram.Value = spPramValue;
                        break;

                    case "XML":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Xml);
                        pram.Value = spPramValue;
                        break;

                    default:
                        throw new Exception("Invalid data type " + spPramDataType + ".");
                }
                #endregion
                pram.Direction = ParameterDirection.Input;
            }
            //#region default/output parameters
            //cmd.Parameters.Add("@out_vMessage", SqlDbType.NVarChar, 4000).Direction = ParameterDirection.Output;
            //cmd.Parameters.Add("@out_iErrorState", SqlDbType.Int).Direction = ParameterDirection.Output;
            //cmd.Parameters.Add("@out_iErrorSeverity", SqlDbType.Int).Direction = ParameterDirection.Output;
            //#endregion
            SqlDataAdapter adap = new SqlDataAdapter(cmd);

            adap.Fill(ds);
            Result.ResultDS = ds;
            //Result.Msg = cmd.Parameters["@out_vMessage"].Value != DBNull.Value ? cmd.Parameters["@out_vMessage"].Value.ToString() : "";
            //Result.ErrorSeverity = Convert.ToInt32(cmd.Parameters["@out_iErrorSeverity"].Value != DBNull.Value ? cmd.Parameters["@out_iErrorSeverity"].Value : 0);
            //Result.ErrorState = Convert.ToInt32(cmd.Parameters["@out_iErrorState"].Value != DBNull.Value ? cmd.Parameters["@out_iErrorState"].Value : 0);
            */
            return Result;

        }

        /// <summary>
        /// Run a stored procedure which will execure some nonquery SQL.
        /// </summary>
        /// <param name="dbConnStr">Connection String to connect Sql Server</param>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="spPramArrList">Parameters in a ArrayList</param>
        public static DBResult RunStoredProcedure(DBResult Result, string dbConnStr, string spName, string spPramList)
        {
            IDBManager dbManager = new DBManager(ConfiguredDataProvider, GetConnectionStrings(dbConnStr));
            try
            {
                dbManager.Open();

                string spPramName = string.Empty;
                string spPramValue = string.Empty;
                string spPramDataType = string.Empty;

                //Split Parameters list
                string[] asParameterSets = null;
                if (spPramList.Trim().Length > 0)
                {
                    asParameterSets = spPramList.Split(']');
                }
                int paramCount = asParameterSets.Length - 1;

                dbManager.CreateParameters(paramCount);
                for (int i = 0; i < paramCount; i++)
                {
                    //Split Parameter set
                    string[] asParameters = null;
                    if (asParameterSets[i].Trim().Length > 0)
                    {
                        asParameters = asParameterSets[i].Split('|');
                    }

                    spPramName = asParameters[0];
                    spPramValue = asParameters[1];
                    spPramDataType = asParameters[2];

                    dbManager.AddParameters(i, spPramName, spPramValue);
                }
                #region default/output parameters
                dbManager.AddParameters(paramCount, "@out_vMessage", string.Empty, DbType.String, 250, ParameterDirection.Output);
                dbManager.AddParameters(paramCount + 1, "@out_iErrorState", 0, DbType.Int32, 10, ParameterDirection.Output);
                dbManager.AddParameters(paramCount + 2, "@out_iErrorSeverity", 0, DbType.Int32, 10, ParameterDirection.Output);
                #endregion

                dbManager.ExecuteNonQuery(CommandType.StoredProcedure, spName);

                try
                {
                    Result.ErrorState = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorState"));
                    Result.ErrorSeverity = Convert.ToInt32(dbManager.GetOutputParameterValue("@out_iErrorSeverity"));
                    Result.Msg = Convert.ToString(dbManager.GetOutputParameterValue("@out_vMessage"));
                }
                catch { }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                dbManager.Dispose();
            }


            /*
            for (int i = 0; i < asParameterSets.Length; i++)
            {
                //Split Parameter set
                string[] asParameters = null;
                if (asParameterSets[i].Trim().Length > 0)
                {
                    asParameters = asParameterSets[i].Split('|');
                }

                spPramName = asParameters[0];
                spPramValue = asParameters[1];
                spPramDataType = asParameters[2];

                SqlParameter pram = null;
                #region SQL DB TYPE AND VALUE ASSIGNMENT
                switch (spPramDataType.ToUpper())
                {
                    case "BIGINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.BigInt);
                        pram.Value = spPramValue;
                        break;

                    case "BINARY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Binary);
                        pram.Value = spPramValue;
                        break;

                    case "BIT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Bit);
                        pram.Value = spPramValue;
                        break;

                    case "CHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Char);
                        pram.Value = spPramValue;
                        break;

                    case "DATETIME":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.DateTime);
                        pram.Value = spPramValue;
                        break;

                    case "DECIMAL":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Decimal);
                        pram.Value = spPramValue;
                        break;

                    case "FLOAT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Float);
                        pram.Value = spPramValue;
                        break;

                    case "IMAGE":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Image);
                        pram.Value = spPramValue;
                        break;

                    case "INT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Int);
                        pram.Value = spPramValue;
                        break;

                    case "MONEY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Money);
                        pram.Value = spPramValue;
                        break;

                    case "NCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NChar);
                        pram.Value = spPramValue;
                        break;

                    case "NTEXT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NText);
                        pram.Value = spPramValue;
                        break;

                    case "NVARCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.NVarChar);
                        pram.Value = spPramValue;
                        break;

                    case "REAL":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Real);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLDATETIME":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallDateTime);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallInt);
                        pram.Value = spPramValue;
                        break;

                    case "SMALLMONEY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.SmallMoney);
                        pram.Value = spPramValue;
                        break;

                    case "TEXT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Text);
                        pram.Value = spPramValue;
                        break;

                    case "TIMESTAMP":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Timestamp);
                        pram.Value = spPramValue;
                        break;

                    case "TINYINT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.TinyInt);
                        pram.Value = spPramValue;
                        break;

                    case "UDT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Udt);
                        pram.Value = spPramValue;
                        break;

                    case "UMIQUEIDENTIFIER":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.UniqueIdentifier);
                        pram.Value = spPramValue;
                        break;

                    case "VARBINARY":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.VarBinary);
                        pram.Value = spPramValue;
                        break;

                    case "VARCHAR":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.VarChar);
                        pram.Value = spPramValue;
                        break;

                    case "VARIANT":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Variant);
                        pram.Value = spPramValue;
                        break;

                    case "XML":
                        pram = cmd.Parameters.Add(spPramName, SqlDbType.Xml);
                        pram.Value = spPramValue;
                        break;

                    default:
                        throw new Exception("Invalid data type " + spPramDataType + ".");
                }
                #endregion
                pram.Direction = ParameterDirection.Input;
            }
            */
            return Result;
        }

    }


}