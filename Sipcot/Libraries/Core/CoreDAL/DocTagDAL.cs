using Lotex.EnterpriseSolutions.CoreBE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Lotex.EnterpriseSolutions.CoreDAL
{
    public class DocTagDAL : BaseDAL
    {
        //public Results SaveDetails(List<string> SearchData, string loginOrgId, string loginToken)
        //{
        //    SqlDataReader dbRdr = null;
        //    SqlCommand dbCmd = null;
        //    SqlConnection dbCon = null;
        //    Results results = new Results();
        //    results.ActionStatus = "SUCCESS";
        //    try
        //    {
        //        string Msg = string.Empty;
        //        string[] SinglePage = SearchData[5].Split(',');

        //        foreach (string page in SinglePage)
        //        {
        //            if (page.Contains("-"))
        //            {
        //                string[] MultiPage = page.Split('-');
        //                if (MultiPage[0] != "" && MultiPage[1] != "")
        //                {
        //                    int start = Convert.ToInt32(MultiPage[0]);
        //                    int End = Convert.ToInt32(MultiPage[1]);
        //                    for (int i = start; i <= End; i++)
        //                    {
        //                        dbCon = OpenConnection();
        //                        dbCmd = new SqlCommand();
        //                        dbCmd.Connection = dbCon;
        //                        dbCmd.CommandTimeout = 320;
        //                        dbCmd.CommandType = CommandType.StoredProcedure;

        //                        dbCmd.CommandText = "UpdateSingleDoc_TagPages";
        //                        dbCmd.Parameters.Add("@SP_DOCPROCESSID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
        //                        dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
        //                        dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
        //                        dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
        //                        dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
        //                        dbCmd.Parameters.Add("@SP_Pages", SqlDbType.VarChar, 100).Value = SearchData[5] == "" ? (object)DBNull.Value : SearchData[5];
        //                        dbCmd.Parameters.Add("@SP_Page", SqlDbType.Int).Value = i == 0 ? (object)DBNull.Value : i;
        //                        dbCmd.Parameters.Add("@SP_PageCount", SqlDbType.Int).Value = SearchData[6] == "" ? (object)DBNull.Value : SearchData[6];

        //                        //sesstion
        //                        //dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
        //                        dbCmd.Parameters.Add("@SP_LoginID", SqlDbType.Int).Value = loginOrgId;
        //                        dbCmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
        //                        dbCmd.Parameters.Add(new SqlParameter("@SP_RETURNVALUE", SqlDbType.SmallInt, 0)).Direction = ParameterDirection.Output;
        //                        dbRdr = dbCmd.ExecuteReader();


        //                    }

        //                }

        //            }
        //            else
        //            {
        //                if (page != "")
        //                {
        //                    dbCon = OpenConnection();
        //                    dbCmd = new SqlCommand();
        //                    dbCmd.Connection = dbCon;
        //                    dbCmd.CommandTimeout = 320;
        //                    dbCmd.CommandType = CommandType.StoredProcedure;

        //                    dbCmd.CommandText = "UpdateSingleDoc_TagPages";
        //                    dbCmd.Parameters.Add("@SP_DOCPROCESSID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
        //                    dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
        //                    dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
        //                    dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
        //                    dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
        //                    dbCmd.Parameters.Add("@SP_Pages", SqlDbType.VarChar, 100).Value = SearchData[5] == "" ? (object)DBNull.Value : SearchData[5];
        //                    dbCmd.Parameters.Add("@SP_Page", SqlDbType.Int).Value = page == "" ? (object)DBNull.Value : page;
        //                    dbCmd.Parameters.Add("@SP_PageCount", SqlDbType.Int).Value = SearchData[6] == "" ? (object)DBNull.Value : SearchData[6];

        //                    //sesstion
        //                    //dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
        //                    dbCmd.Parameters.Add("@SP_LoginID", SqlDbType.Int).Value = loginOrgId;
        //                    dbCmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
        //                    dbCmd.Parameters.Add(new SqlParameter("@SP_RETURNVALUE", SqlDbType.SmallInt, 0)).Direction = ParameterDirection.Output;
        //                    dbRdr = dbCmd.ExecuteReader();

        //                }
        //            }

        //        }


        //        //dbCmd.CommandText = "Ax_UpdateDoc_TagPages";
        //        //dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
        //        //dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
        //        //dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
        //        //dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
        //        //dbCmd.Parameters.Add("@SP_Pages", SqlDbType.VarChar, 100).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4;
        //        //dbCmd.Parameters.Add("@SP_Page", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
        //        //dbCmd.Parameters.Add("@@SP_PageCount", SqlDbType.Int).Value = SearchData[5] == "" ? (object)DBNull.Value : SearchData[5];

        //        ////sesstion
        //        ////dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
        //        //dbCmd.Parameters.Add("@SP_LoginID", SqlDbType.Int).Value = loginOrgId;
        //        //dbCmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
        //        //dbCmd.Parameters.Add(new SqlParameter("@SP_RETURNVALUE", SqlDbType.SmallInt, 0)).Direction = ParameterDirection.Output;
        //        //dbRdr = dbCmd.ExecuteReader();

        //        results.Message = dbCmd.Parameters["@SP_MESSAGE"].Value.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (dbRdr != null)
        //        {
        //            dbRdr.Dispose();
        //        }
        //        if (dbCmd != null)
        //        {
        //            dbCmd.Dispose();
        //        }

        //    }
        //    return results;
        //}

        public Results SaveDetails(List<string> SearchData, string loginOrgId, string loginToken)
        {
            SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            try
            {
                // string Msg = string.Empty;
                //string[] SinglePage = SearchData[5].Split(',');

                //foreach (string page in SinglePage)
                //{
                //if (page.Contains("-"))
                //{
                //    string[] MultiPage = page.Split('-');
                //    if (MultiPage[0] != "" && MultiPage[1] != "")
                //    {
                //        int start = Convert.ToInt32(MultiPage[0]);
                //        int End = Convert.ToInt32(MultiPage[1]);
                //        for (int i = start; i <= End; i++)
                //        {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandTimeout = 320;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "Document_Tagging";

                dbCmd.Parameters.Add("@SP_DocID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
                dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
                dbCmd.Parameters.Add("@PageNumbers", SqlDbType.VarChar, 5000).Value = SearchData[5] == "" ? (object)DBNull.Value : SearchData[5];
                dbCmd.Parameters.Add("@SP_PageCount", SqlDbType.Int).Value = SearchData[6] == "" ? (object)DBNull.Value : SearchData[6];

                dbCmd.Parameters.Add("@SP_LoginID", SqlDbType.Int).Value = loginOrgId;
                dbCmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                dbCmd.Parameters.Add(new SqlParameter("@SP_RETURNVALUE", SqlDbType.SmallInt, 0)).Direction = ParameterDirection.Output;
                dbRdr = dbCmd.ExecuteReader();


                //    }

                //} 

                //}
                //else
                //{
                //    if (page != "")
                //    {
                //        dbCon = OpenConnection();
                //        dbCmd = new SqlCommand();
                //        dbCmd.Connection = dbCon;
                //        dbCmd.CommandTimeout = 320;
                //        dbCmd.CommandType = CommandType.StoredProcedure;

                //        dbCmd.CommandText = "UpdateSingleDoc_TagPages";
                //        dbCmd.Parameters.Add("@SP_DOCPROCESSID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                //        dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
                //        dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
                //        dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
                //        dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
                //        dbCmd.Parameters.Add("@SP_Pages", SqlDbType.VarChar, 100).Value = SearchData[5] == "" ? (object)DBNull.Value : SearchData[5];
                //        dbCmd.Parameters.Add("@SP_Page", SqlDbType.Int).Value = page == "" ? (object)DBNull.Value : page;
                //        dbCmd.Parameters.Add("@SP_PageCount", SqlDbType.Int).Value = SearchData[6] == "" ? (object)DBNull.Value : SearchData[6];

                //        //sesstion
                //        //dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                //        dbCmd.Parameters.Add("@SP_LoginID", SqlDbType.Int).Value = loginOrgId;
                //        dbCmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                //        dbCmd.Parameters.Add(new SqlParameter("@SP_RETURNVALUE", SqlDbType.SmallInt, 0)).Direction = ParameterDirection.Output;
                //        dbRdr = dbCmd.ExecuteReader();

                //    }
                //}

                //  }


                //dbCmd.CommandText = "Ax_UpdateDoc_TagPages";
                //dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                //dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
                //dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
                //dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
                //dbCmd.Parameters.Add("@SP_Pages", SqlDbType.VarChar, 100).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4;
                //dbCmd.Parameters.Add("@SP_Page", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                //dbCmd.Parameters.Add("@@SP_PageCount", SqlDbType.Int).Value = SearchData[5] == "" ? (object)DBNull.Value : SearchData[5];

                ////sesstion
                ////dbCmd.Parameters.Add("@in_vLoginToken", SqlDbType.VarChar, 100).Value = loginToken;
                //dbCmd.Parameters.Add("@SP_LoginID", SqlDbType.Int).Value = loginOrgId;
                //dbCmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                //dbCmd.Parameters.Add(new SqlParameter("@SP_RETURNVALUE", SqlDbType.SmallInt, 0)).Direction = ParameterDirection.Output;
                //dbRdr = dbCmd.ExecuteReader();

                results.Message = dbCmd.Parameters["@SP_MESSAGE"].Value.ToString();
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
            return results;
        }

        //Sachin
        public Results DeleteData(List<string> SearchData, string loginOrgId, string loginToken)
        {
            SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            Results results = new Results();
            results.ActionStatus = "SUCCESS";
            try
            {

                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandTimeout = 120;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "DeleteSingleTagData";
                dbCmd.Parameters.Add("@SP_DOCPROCESSID", SqlDbType.Int).Value = SearchData[0] == "" ? (object)DBNull.Value : SearchData[0];
                dbCmd.Parameters.Add("@SP_DocTypeID", SqlDbType.Int).Value = SearchData[1] == "" ? (object)DBNull.Value : SearchData[1];
                dbCmd.Parameters.Add("@SP_DeptID", SqlDbType.Int).Value = SearchData[2] == "" ? (object)DBNull.Value : SearchData[2];
                dbCmd.Parameters.Add("@SP_MainTagID", SqlDbType.Int).Value = SearchData[3] == "" ? (object)DBNull.Value : SearchData[3];
                dbCmd.Parameters.Add("@SP_SubTagID", SqlDbType.Int).Value = SearchData[4] == "" ? (object)DBNull.Value : SearchData[4];
                dbCmd.Parameters.Add("@SP_Pagenumbers", SqlDbType.VarChar, 5000).Value = SearchData[5] == "" ? (object)DBNull.Value : SearchData[5];
                dbCmd.Parameters.Add(new SqlParameter("@SP_MESSAGE", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                dbCmd.Parameters.Add(new SqlParameter("@SP_RETURNVALUE", SqlDbType.SmallInt, 0)).Direction = ParameterDirection.Output;
                dbRdr = dbCmd.ExecuteReader();

                results.Message = dbCmd.Parameters["@SP_MESSAGE"].Value.ToString();
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
            return results;
        }

        public string GetFilePath(int Id, int DocTypeId, int DeptId)
        {
            SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            string FilePath = string.Empty;
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;

                dbCmd.CommandText = "GetFilePath";
                dbCmd.Parameters.Add("@SP_DocId", SqlDbType.Int).Value = DocTypeId == 0 ? (object)DBNull.Value : Id;
                dbCmd.Parameters.Add("@SP_DocTypeId", SqlDbType.Int).Value = DocTypeId == 0 ? (object)DBNull.Value : DocTypeId;
                dbCmd.Parameters.Add("@SP_DeptId", SqlDbType.Int).Value = DeptId == 0 ? (object)DBNull.Value : DeptId;
                dbCmd.Parameters.Add(new SqlParameter("@SP_FilePath", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                dbRdr = dbCmd.ExecuteReader();

                FilePath = dbCmd.Parameters["@SP_FilePath"].Value.ToString();

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
            return FilePath;
        }

        public string GetTagPages(List<string> SearchData)
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
                dbCmd.Parameters.Add(new SqlParameter("@SP_TagPages", SqlDbType.VarChar, -1)).Direction = ParameterDirection.Output;
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

        public string GetDownloadRights(List<string> SearchData)
        {
            SqlDataReader dbRdr = null;
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            String RCOUNT = string.Empty;
            try
            {

                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;


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
            return RCOUNT;
        }
        public DataTable Gettotaltagpages(int DocId, int PageNo, int iMaintag, int iSubtag)
        {
            SqlDataAdapter dbAdr = new SqlDataAdapter();
            SqlCommand dbCmd = null;
            SqlConnection dbCon = null;
            DataSet dsAcknow = new DataSet();
            try
            {
                dbCon = OpenConnection();
                dbCmd = new SqlCommand();
                dbCmd.Connection = dbCon;
                dbCmd.CommandType = CommandType.StoredProcedure;
                dbCmd.CommandText = "Gettotaltagpages";

                dbCmd.Parameters.Add("@SP_DocId", SqlDbType.Int).Value = DocId == 0 ? (object)DBNull.Value : DocId;
                dbCmd.Parameters.Add("@SP_PagNo", SqlDbType.Int).Value = PageNo == 0 ? (object)DBNull.Value : PageNo;
                dbCmd.Parameters.Add("@in_iMainTagID", SqlDbType.Int).Value = iMaintag == 0 ? (object)DBNull.Value : iMaintag;
                dbCmd.Parameters.Add("@in_iSubTagID", SqlDbType.Int).Value = iSubtag == 0 ? (object)DBNull.Value : iSubtag;

                dbAdr.SelectCommand = dbCmd;
                dbAdr.Fill(dsAcknow);

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
            return dsAcknow.Tables[0];
        }
    }
}
