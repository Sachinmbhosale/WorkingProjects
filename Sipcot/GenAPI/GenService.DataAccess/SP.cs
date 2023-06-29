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
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using GenServiceDataContractLibrary;

namespace GenService.Service
{
    /// <summary>
    /// This class helps to built SP argument. It can use directly to any SP.
    /// </summary>
    public class SP
    {
        string API_Key = string.Empty;
        public SP(string Token)
        {
            API_Key = Token;
        }

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

        /// <summary>
        /// Run a stored procedure of Select SQL type.
        /// </summary>
        /// <param name="dbConnStr">Connection String to connect Sql Server</param>
        /// <param name="ds">DataSet which will return after filling Data</param>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="spPramArrList">Parameters in ArrayList</param>
        /// <returns>Return DataSet after filing data by SQL.</returns>
        public ServiceData ExecuteDataSet(ServiceData Result, string dbConnStr, string spName)
        {
            Logger.logTrace("Get connection string.", API_Key);
            using (var conn = new SqlConnection(GetConnectionStrings(dbConnStr)))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = spName;
                cmd.Connection = conn;

                Logger.logTrace("Prepare command object from paramerts collection string.", API_Key);
                cmd = SetCommandParameters(ref cmd, Result.CustomInput);

                if (Result.Method != null && Result.Method.Length > 0)
                {
                    cmd.Parameters.Add("@in_vMethod", SqlDbType.VarChar, 100).Value = Result.Method;
                    Logger.logTrace("Parameter @in_vMethod added to the command object.", API_Key);
                }
                else
                    Logger.logTrace("Method is null or empty.", API_Key);

                // Result.Input will be received from client side which contains parameter and its values
                if (Result.Input != null && Result.Input.Length > 0)
                {
                    cmd.Parameters.Add("@in_xRequest", SqlDbType.Xml).Value = Result.Input;
                    Logger.logTrace("Parameter @in_xRequest added to the command object.", API_Key);
                }
                else
                    Logger.logTrace("Input is null or empty.", API_Key);

                // If API_Key doen't exist in parameter list add it specifically
                if (!cmd.Parameters.Contains("in_vToken"))
                {
                    cmd.Parameters.Add("@in_vToken", SqlDbType.VarChar, 100).Value = API_Key;
                    Logger.logTrace("API_Key added specifically.", API_Key);
                }

                // Add default output parameters
                Logger.logTrace("Adding deafult output parameters to command.", API_Key);
                if (!cmd.Parameters.Contains("out_iErrorState"))
                    cmd.Parameters.Add("@out_iErrorState", SqlDbType.Int).Direction = ParameterDirection.Output;
                if (!cmd.Parameters.Contains("out_iErrorSeverity"))
                    cmd.Parameters.Add("@out_iErrorSeverity", SqlDbType.Int).Direction = ParameterDirection.Output;
                if (!cmd.Parameters.Contains("out_vMessage"))
                    cmd.Parameters.Add("@out_vMessage", SqlDbType.NVarChar, 500).Direction = ParameterDirection.Output;

                SqlDataAdapter adap = new SqlDataAdapter(cmd);
                Logger.logTrace("Execute SqlDataAdapter.", API_Key);
                adap.Fill(Result.Resultsets = new DataSet());

                // Reading values of default output parameters
                Logger.logTrace("Reading values of default output parameters from command.", API_Key);
                if (cmd.Parameters.Contains("@out_iErrorState"))
                    Result.ErrorState = cmd.Parameters["@out_iErrorState"].Value != DBNull.Value ? (int)cmd.Parameters["@out_iErrorState"].Value : 0;
                if (cmd.Parameters.Contains("@out_iErrorSeverity"))
                    Result.ErrorSeverity = cmd.Parameters["@out_iErrorSeverity"].Value != DBNull.Value ?(int)cmd.Parameters["@out_iErrorSeverity"].Value:0;
                if (cmd.Parameters.Contains("@out_vMessage"))
                    Result.Message = cmd.Parameters["@out_vMessage"].Value != DBNull.Value ? cmd.Parameters["@out_vMessage"].Value.ToString():string.Empty;

                Logger.logTrace("Prepare output parameters and its values string.", API_Key);
                string outputParamValueList = ReadOutputPrarams(ref cmd);
                Result.OutputParamValues = outputParamValueList;

            }
            return Result;
        }

        /// <summary>
        /// Run a stored procedure which will execure some nonquery SQL.
        /// </summary>
        /// <param name="dbConnStr">Connection String to connect Sql Server</param>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="spPramArrList">Parameters in a ArrayList</param>
        public ServiceData ExecuteNonQuery(ServiceData Result, string dbConnStr, string spName)
        {
            Logger.logTrace("Get connection string.", API_Key);
            using (var conn = new SqlConnection(GetConnectionStrings(dbConnStr)))
            {
                Logger.logTrace("Open sql connection.", API_Key);
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.CommandText = spName;

                Logger.logTrace("Prepare command object from paramerts collection string.", API_Key);
                cmd = SetCommandParameters(ref cmd, Result.CustomInput);

                if (Result.Method != null && Result.Method.Length > 0)
                {
                    cmd.Parameters.Add("@in_vMethod", SqlDbType.Xml).Value = Result.Method;
                    Logger.logTrace("Parameter @in_vMethod added to the command object.", API_Key);
                }
                else
                    Logger.logTrace("Method is null or empty.", API_Key);

                // Result.Input will be received from client side which contains parameter and its values
                if (Result.Input != null && Result.Input.Length > 0)
                {
                    cmd.Parameters.Add("@in_xRequest", SqlDbType.Xml).Value = Result.Input;
                    Logger.logTrace("Parameter @in_xRequest added to the command object.", API_Key);
                }
                else
                    Logger.logTrace("Input is null or empty.", API_Key);

                // If API_Key doen't exist in parameter list add it specifically
                if (!cmd.Parameters.Contains("in_vToken"))
                {
                    cmd.Parameters.Add("@in_vToken", SqlDbType.VarChar, 100).Value = API_Key;
                    Logger.logTrace("API_Key added specifically.", API_Key);
                }

                // Add default output parameters
                Logger.logTrace("Adding deafult output parameters to command.", API_Key);
                if (!cmd.Parameters.Contains("out_iErrorState"))
                    cmd.Parameters.Add("@out_iErrorState", SqlDbType.Int).Direction = ParameterDirection.Output;
                if (!cmd.Parameters.Contains("out_iErrorSeverity"))
                    cmd.Parameters.Add("@out_iErrorSeverity", SqlDbType.Int).Direction = ParameterDirection.Output;
                if (!cmd.Parameters.Contains("out_vMessage"))
                    cmd.Parameters.Add("@out_vMessage", SqlDbType.NVarChar, 500).Direction = ParameterDirection.Output;

                Logger.logTrace("ExecuteNonQuery command.", API_Key);
                cmd.ExecuteNonQuery();

                // Reading values of default output parameters
                Logger.logTrace("Reading values of default output parameters from command.", API_Key);
                if (cmd.Parameters.Contains("@out_iErrorState"))
                    Result.ErrorState = cmd.Parameters["@out_iErrorState"].Value != DBNull.Value ? (int)cmd.Parameters["@out_iErrorState"].Value : 0;
                if (cmd.Parameters.Contains("@out_iErrorSeverity"))
                    Result.ErrorSeverity = cmd.Parameters["@out_iErrorSeverity"].Value != DBNull.Value ? (int)cmd.Parameters["@out_iErrorSeverity"].Value : 0;
                if (cmd.Parameters.Contains("@out_vMessage"))
                    Result.Message = cmd.Parameters["@out_vMessage"].Value != DBNull.Value ? cmd.Parameters["@out_vMessage"].Value.ToString() : string.Empty;

                Logger.logTrace("Prepare output parameters and its values string.", API_Key);
                string outputParamValueList = ReadOutputPrarams(ref cmd);

                Result.OutputParamValues = outputParamValueList;
            } return Result;
        }

        private static SqlCommand SetCommandParameters(ref SqlCommand cmd, string spPramList)
        {
            string spPramName = string.Empty;
            string spPramValue = string.Empty;
            string spPramDataType = string.Empty;
            string spPramDirection = string.Empty;

            //Split Parameters list
            string[] asParameterSets = null;
            if (spPramList != null && spPramList.Trim().Length > 0)
            {
                asParameterSets = spPramList.Split(']');
            }

            if (asParameterSets != null)
                for (int i = 0; i < asParameterSets.Length; i++)
                {
                    spPramName = string.Empty;
                    spPramValue = string.Empty;
                    spPramDataType = string.Empty;
                    spPramDirection = string.Empty;

                    //Split Parameter set
                    string[] asParameters = null;
                    if (asParameterSets[i].Trim().Length > 0)
                    {
                        asParameters = asParameterSets[i].Split('|');

                        spPramName = asParameters[0].Replace("[", "");
                        spPramValue = asParameters[1];
                        spPramDataType = asParameters[2].Replace("]", "");
                        if (asParameters.Length > 3)
                            spPramDirection = asParameters[3].Replace("]", "");
                    }

                    if (spPramName.Length == 0) break;

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

                        case "DOUBLE":
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

                        case "STRING":
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
                    if (spPramDirection.ToLower().Contains("out"))
                    {
                        pram.Direction = ParameterDirection.Output;
                        if (pram.SqlDbType.ToString().ToUpper() == "NVARCHAR"
                            || pram.SqlDbType.ToString().ToUpper() == "VARCHAR"
                            || pram.SqlDbType.ToString().ToUpper() == "STRING")
                            pram.Size = 4000;
                        //else
                        //    throw new Exception("Output parameters will support only following data types: NVARCHAR, VARCHAR, STRING");
                    }
                    else
                        pram.Direction = ParameterDirection.Input;
                }
            return cmd;
        }

        private static string ReadOutputPrarams(ref SqlCommand cmd)
        {
            string outputParamsValueList = string.Empty;
            foreach (SqlParameter param in cmd.Parameters)
            {
                if (param.Direction == ParameterDirection.Output)
                    outputParamsValueList += spArgumentsCollection(param.ParameterName, param.Value.ToString());
            }
            return outputParamsValueList;
        }

        public static string spArgumentsCollection(string spParmName, string spParmValue)
        {
            return "[" + spParmName + "|" + spParmValue + "]";
        }
    }
}