using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DocumentSearchDAL : BaseDAL
    {


        public DataTable GetTempcolumns(List<string> SearchData, string strColumns)
        {
            SqlDataAdapter dbAdr = new SqlDataAdapter();
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            DataSet dsDomain = new DataSet();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                //dbCmd.CommandText = "Ax_GetDocumentSearch";
                dbCmd.CommandText = "GetDocumentSearchwithTemplatecol";
                dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
                dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
                dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
                dbCmd.Parameters.Add("@SP_SearchField", SqlDbType.VarChar, 1000).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
                dbCmd.Parameters.Add("@SP_Columns", SqlDbType.VarChar, 1000).Value = strColumns == "" ? (object)DBNull.Value : strColumns;
                
                dbAdr.SelectCommand = dbCmd;
                dbAdr.Fill(dsDomain);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbAdr != null)
                {
                    dbAdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }

            }
            return dsDomain.Tables[0];
        }
        public DataTable GetSearcData(List<string> SearchData)
        {
            SqlDataAdapter dbAdr = new SqlDataAdapter();
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            DataSet dsDomain = new DataSet();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                //dbCmd.CommandText = "Ax_GetDocumentSearch";
                dbCmd.CommandText = "GetDocumentSearchNew";
                dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
                dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
                dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
                dbCmd.Parameters.Add("@SP_SearchField", SqlDbType.VarChar, 1000).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
              //  dbCmd.Parameters.Add("@SP_Columns", SqlDbType.VarChar, 1000).Value = strColumns == "" ? (object)DBNull.Value : strColumns;

                dbAdr.SelectCommand = dbCmd;
                dbAdr.Fill(dsDomain);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbAdr != null)
                {
                    dbAdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
               
            }
            return dsDomain.Tables[0];
        }
        public DataTable GetColumn(int UserId,int OrgId,string DocuType, int DeptId)
        {
            SqlDataAdapter dbAdr = new SqlDataAdapter();
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            DataSet dsDomain = new DataSet();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;
                dbCmd.CommandText = "GetColumns";
                dbCmd.Parameters.Add("@USERID", SqlDbType.Int).Value = UserId == 0 ? (object)DBNull.Value : UserId;
                dbCmd.Parameters.Add("@ORGID", SqlDbType.Int).Value = OrgId == 0 ? (object)DBNull.Value : OrgId;
                dbCmd.Parameters.Add("@DOCUTYPE", SqlDbType.VarChar, 100).Value = DocuType == "" ? (object)DBNull.Value : DocuType;
                dbCmd.Parameters.Add("@DEPARTMENTID", SqlDbType.Int).Value = DeptId == 0 ? (object)DBNull.Value : DeptId;
                dbAdr.SelectCommand = dbCmd;
                dbAdr.Fill(dsDomain);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbAdr != null)
                {
                    dbAdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
                
            }
            return dsDomain.Tables[0];
        }
        public string GetStartPage(List<string> SearchData)
        {
            SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            string TagPages = string.Empty;
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "GetSingleTagPages";
                dbCmd.Parameters.Add("@SP_DOCPROCESSID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
                dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
                dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
                dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
                dbCmd.Parameters.Add(new SqlParameter("@SP_TagPages", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                dbRdr = dbCmd.ExecuteReader();
                TagPages = dbCmd.Parameters["@SP_TagPages"].Value.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (dbRdr != null)
                {
                    dbRdr.Dispose();
                }
                if (dbCmd != null)
                {
                    dbCmd.Dispose();
                }
            }
            return TagPages;
        }
    }
}
