//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;

//namespace WorkflowNotifications
//{
//    public class DBHelper
//    {
//        private DataSet _objDataSet;
//        public SqlParameter _objReturnParameter;
//        private SqlCommand _objSQLCommand;
//        private SqlConnection _objSQLConnection = new SqlConnection();
//        private SqlDataAdapter _objSqlDataAdapter;
//        public SqlParameter _objSQLParameter;
//        private SqlTransaction _objSQLTrans;
//        private string _strErrorLogPath = string.Empty;
//        private string _strSqlConnection = string.Empty;
//        private string _strUserID = string.Empty;
//        private SqlParameter[] Input_parameters = new SqlParameter[1];
//        public SqlParameter[] Output_parameters = new SqlParameter[1];

//        public DBHelper()
//        {
//            try
//            {
//                string strTemp = string.Empty;
//                AppSettingsReader appSettings= new AppSettingsReader();
//                this._strSqlConnection = appSettings.GetValue("SqlServerConnString", strTemp.GetType()).ToString();
//                this._strErrorLogPath = "";
//            }
//            catch (Exception)
//            {
//            }
//        }

//        private bool CloseConnection()
//        {
//            try
//            {
//                if (this._objSQLConnection.State != ConnectionState.Closed)
//                {
//                    this._objSQLConnection.Close();
//                    return true;
//                }
//                return false;
//            }
//            catch (SqlException)
//            {
//                return false;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        public DBResult ExecuteDataset(string strStoredProcedureName, params SqlParameter[] Parameters)
//        {
//            DBResult objDBResult = new DBResult();
//            try
//            {
//                if (this.OpenConnection())
//                {
//                    this._objSqlDataAdapter = new SqlDataAdapter();
//                    this._objSQLCommand = new SqlCommand();
//                    this._objSQLCommand.Connection = this._objSQLConnection;
//                    this._objSQLCommand.CommandType = CommandType.StoredProcedure;
//                    this._objSQLCommand.CommandText = strStoredProcedureName;
//                    this._objSQLCommand.Parameters.Clear();

//                    this._objSQLCommand.Parameters.AddRange(Parameters);
//                    this._objSqlDataAdapter.SelectCommand = this._objSQLCommand;
//                    this._objSqlDataAdapter.Fill(objDBResult.dsResult);

//                    // Handle output parameters
//                    var ErrorState = Parameters.SingleOrDefault(p => p.ParameterName.ToUpper() == "@OUT_IERRORSTATE");
//                    if (ErrorState != null) { objDBResult.ErrorState = (ErrorState.Value.ToString() != string.Empty ? Convert.ToInt32(ErrorState.Value) : 0); }

//                    var ErrorSeverity = Parameters.SingleOrDefault(p => p.ParameterName.ToUpper() == "@OUT_IERRORSEVERITY");
//                    if (ErrorSeverity != null) { objDBResult.ErrorSeverity = (ErrorSeverity.Value.ToString() != string.Empty ? Convert.ToInt32(ErrorSeverity.Value) : 0); }

//                    var Message = Parameters.SingleOrDefault(p => p.ParameterName.ToUpper() == "@OUT_VMESSAGE");
//                    if (Message != null) { objDBResult.Message = Message.Value.ToString(); }
//                }
//            }
//            catch (Exception ex)
//            {
//                throw ex;
//            }
//            finally
//            {
//                this._objSqlDataAdapter = null;
//                this._objSQLCommand = null;
//                this.CloseConnection();
//            }
//            return objDBResult;
//        }

//        public bool ExecuteStoredProcedure(string strStoredProcedureName, params SqlParameter[] Parameters)
//        {
//            bool flag; int ErrorState = 0, ErrorSeverity = 0;
//            string ErrorMessage = string.Empty;

//            try
//            {
//                if (this.OpenConnection())
//                {
//                    this._objDataSet = new DataSet();
//                    this._objSqlDataAdapter = new SqlDataAdapter();
//                    this._objSQLCommand = new SqlCommand();
//                    this._objSQLCommand.Connection = this._objSQLConnection;
//                    this._objSQLCommand.CommandType = CommandType.StoredProcedure;
//                    this._objSQLCommand.CommandText = strStoredProcedureName;
//                    foreach (SqlParameter parameter in Parameters)
//                    {

//                        this._objSQLCommand.Parameters.Add(parameter);
//                    }
//                    this._objSQLCommand.ExecuteNonQuery();
//                    ErrorState = Convert.ToInt32(this._objSQLCommand.Parameters["@out_iErrorState"].Value);
//                    ErrorSeverity = Convert.ToInt32(this._objSQLCommand.Parameters["@out_iErrorSeverity"].Value);
//                    ErrorMessage = this._objSQLCommand.Parameters["@out_vMessage"].Value.ToString();
//                    flag = true;

//                }
//                else
//                {
//                    ErrorState = -1;
//                    ErrorSeverity = 0;
//                    ErrorMessage = "Failed to open connection to the database.";
//                    flag = false;
//                }

//            }

//            catch (Exception ex)
//            {
//                ErrorState = 1;
//                ErrorSeverity = 0;
//                ErrorMessage = ex.Message;

//                flag = false;
//            }
//            finally
//            {
//                this._objDataSet = null;
//                this._objSqlDataAdapter = null;
//                this._objSQLCommand = null;
//                this.CloseConnection();
//            }
//            return flag;
//        }

//        public DataSet ExecuteDataSet(string strTableName, string strColumns, string strWhereCondition = "", string strOrderBY = "")
//        {
//            DataSet set;
//            try
//            {
//                string str = null;
//                if (this.OpenConnection())
//                {
//                    this._objDataSet = new DataSet();
//                    this._objSQLCommand = new SqlCommand();
//                    this._objSqlDataAdapter = new SqlDataAdapter();
//                    if (!string.IsNullOrEmpty(strWhereCondition) & !string.IsNullOrEmpty(strOrderBY))
//                    {
//                        str = "SELECT " + strColumns + " FROM " + strTableName + " WHERE " + strWhereCondition + " ORDER BY " + strOrderBY;
//                    }
//                    else if (!string.IsNullOrEmpty(strWhereCondition) & string.IsNullOrEmpty(strOrderBY))
//                    {
//                        str = "SELECT " + strColumns + " FROM " + strTableName + " WHERE " + strWhereCondition;
//                    }
//                    else if (string.IsNullOrEmpty(strWhereCondition) & !string.IsNullOrEmpty(strOrderBY))
//                    {
//                        str = "SELECT " + strColumns + " FROM " + strTableName + " ORDER BY " + strOrderBY;
//                    }
//                    else
//                    {
//                        str = "SELECT " + strColumns + " FROM " + strTableName;
//                    }
//                    this._objSQLCommand.Connection = this._objSQLConnection;
//                    this._objSQLCommand.CommandType = CommandType.Text;
//                    this._objSQLCommand.CommandText = str;
//                    this._objSqlDataAdapter.SelectCommand = this._objSQLCommand;
//                    this._objSqlDataAdapter.Fill(this._objDataSet);
//                    if (this._objDataSet.Tables.Count > 0)
//                    {
//                        return this._objDataSet;
//                    }
//                    return null;
//                }
//                set = null;
//            }
//            catch (SqlException)
//            {
//                set = null;
//            }
//            catch (Exception)
//            {
//                set = null;
//            }
//            finally
//            {
//                this._objDataSet = null;
//                this._objSQLCommand = null;
//                this._objSqlDataAdapter = null;
//                this.CloseConnection();
//            }
//            return set;
//        }

//        private bool OpenConnection()
//        {
//            try
//            {
//                if (this._objSQLConnection.State != ConnectionState.Open)
//                {
//                    this._objSQLConnection.ConnectionString = this._strSqlConnection;
//                    this._objSQLConnection.Open();
//                    return true;
//                }
//                return false;
//            }
//            catch (SqlException)
//            {
//                return false;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }
//        /*
//        public void LogError(string ScreenName, string MethodName, DateTime Time, string ex)
//        {
//            try
//            {
//                new ClsBAL { ScreenName = ScreenName.ToString(), MethodName = MethodName.ToString(), Time = Convert.ToDateTime(Time), Exception = ex.ToString() }.fninsertLog();
//            }
//            catch (System.Exception exception)
//            {
//                FileStream stream = new FileStream(System.Web.HttpContext.Current.Server.MapPath("~/logexception.txt"), FileMode.OpenOrCreate, FileAccess.Write);
//                StreamWriter writer = new StreamWriter(stream);
//                writer.BaseStream.Seek(0L, SeekOrigin.End);
//                writer.WriteLine(exception.ToString());
//                writer.Flush();
//                writer.Close();
//            }
//        }*/
//    }
//}
